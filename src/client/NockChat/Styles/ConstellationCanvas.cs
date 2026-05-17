using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Threading;
using Avalonia.VisualTree;
using NockChat.Services.Structures.Animations;

namespace NockChat.Styles
{
    /// <summary>
    /// Высокопроизводительный контрол для отображения анимированного звёздного неба с эффектом констелляции.
    /// Оптимизирован для работы на слабых ПК, автоматически останавливает анимацию при неактивном/свёрнутом окне.
    /// </summary>
    public sealed class ConstellationCanvas : Control, IDisposable
    {
        #region Константы конфигурации
        /// <summary>
        /// Количество звёзд в созвездии
        /// </summary>
        private const int STAR_COUNT = 50;

        /// <summary>
        /// Максимальное расстояние между звёздами для отрисовки соединительных линий (в квадрате для оптимизации)
        /// </summary>
        private const double MAX_DISTANCE_SQ = 18000;

        /// <summary>
        /// Скорость плавного появления/исчезновения звёзд
        /// </summary>
        private const double FADE_SPEED = 0.05;

        /// <summary>
        /// Скорость появления соединительных линий<
        /// /summary>
        private const double LINE_SPEED = 0.02;

        /// <summary>
        /// Множитель прозрачности для линий (делает линии более тонкими)
        /// </summary>
        private const double LINE_ALPHA_MULTIPLIER = 0.6;

        /// <summary>
        /// Интервал обновления анимации в миллисекундах (~60 FPS)
        /// </summary>
        private const int TIMER_INTERVAL_MS = 16;

        /// <summary>
        /// Количество кадров для пропуска при стабильной анимации (экономим cpu)
        /// </summary>
        private const int FRAME_SKIP_THRESHOLD = 2;

        /// <summary>
        /// Размер таблицы предвычисленных значений синуса для мерцания
        /// </summary>
        private const int FLICKER_TABLE_SIZE = 512;

        /// <summary>
        /// Битовая маска для быстрого модуло через AND операцию
        /// </summary>
        private const int FLICKER_TABLE_MASK = FLICKER_TABLE_SIZE - 1;

        /// <summary>
        /// Задержка для debounce при изменении размеров окна
        /// </summary>
        private const int RESIZE_DEBOUNCE_MS = 50;

        /// <summary>
        /// Порог прозрачности для отсечения невидимых элементов
        /// </summary>
        private const double VISIBILITY_THRESHOLD = 0.01;
        #endregion

        #region Поля данных
        /// <summary>
        /// Массив всех звёзд в созвездии. Содержит относительные позиции, размеры и параметры мерцания
        /// </summary>
        private readonly Star[] _stars = new Star[STAR_COUNT];

        /// <summary>
        /// Предвычисленные офсеты для таблицы мерцания для каждой звезды.
        /// Используется для избежания вычисления модуло в горячем пути рендеринга
        /// </summary>
        private readonly int[] _pulseOffsets = new int[STAR_COUNT];

        /// <summary>
        /// Кэш абсолютных позиций звёзд в пикселях. Обновляется только при изменении размера контрола.
        /// Предотвращает пересчёт позиций на каждом кадре
        /// </summary>
        private readonly Point[] _cachedPositions = new Point[STAR_COUNT];

        /// <summary>
        /// Последняя известная ширина контрола. Используется для определения необходимости обновления позиций
        /// </summary>
        private double _lastWidth;

        /// <summary>
        /// Последняя известная высота контрола. Используется для определения необходимости обновления позиций
        /// </summary>
        private double _lastHeight;

        /// <summary>
        /// Флаг указывающий, что кэш позиций устарел и требует обновления.
        /// Устанавливается в true при изменении размера или генерации новых звёзд
        /// </summary>
        private bool _positionsDirty = true;

        /// <summary>
        /// Кэш ручек (Pen) с предвычисленной прозрачностью от 0% до 100%.
        /// Индекс массива соответствует проценту прозрачности. Используется для отрисовки линий
        /// </summary>
        private readonly Pen[] _cachedPens = new Pen[101];

        /// <summary>
        /// Кэш кистей (SolidColorBrush) с предвычисленной прозрачностью от 0% до 100%.
        /// Индекс массива соответствует проценту прозрачности. Используется для отрисовки звёзд
        /// </summary>
        private readonly SolidColorBrush[] _cachedBrushes = new SolidColorBrush[101];

        /// <summary>
        /// Основной таймер анимации. Срабатывает каждые ~16мс для обновления состояния и перерисовки.
        /// Автоматически останавливается когда окно неактивно или свёрнуто
        /// </summary>
        private readonly DispatcherTimer _timer;

        /// <summary>
        /// Секундомер для отслеживания времени анимации. Используется для вычисления фазы мерцания звёзд.
        /// Останавливается вместе с таймером для экономии ресурсов
        /// </summary>
        private readonly Stopwatch _stopwatch = new();

        /// <summary>
        /// Debounce таймер для обработки событий изменения размера.
        /// Предотвращает избыточные пересчёты при быстром изменении размера окна
        /// </summary>
        private DispatcherTimer? _resizeDebounceTimer;

        /// <summary>
        /// Флаг активности анимации. Управляется через свойство IsActive.
        /// При true - анимация плавно появляется, при false - плавно исчезает.
        /// Volatile для потокобезопасного доступа
        /// </summary>
        private volatile bool _isActive = false;

        /// <summary>
        /// Флаг освобождения ресурсов. Устанавливается в true при вызове Dispose().
        /// Используется для предотвращения работы с освобождёнными ресурсами.
        /// Volatile для потокобезопасного доступа
        /// </summary>
        private volatile bool _isDisposed = false;

        /// <summary>
        /// Текущая глобальная прозрачность всех элементов (0.0 - 1.0).
        /// Плавно изменяется при появлении/исчезновении анимации
        /// </summary>
        private double _globalAlpha = 0;

        /// <summary>
        /// Прогресс появления соединительных линий (0.0 - 1.0).
        /// Линии начинают появляться когда globalAlpha > 0.5
        /// </summary>
        private double _lineProgress = 0;

        /// <summary>
        /// Счётчик для пропуска кадров в стабильном состоянии.
        /// Используется для снижения частоты перерисовки с 60 FPS до ~20 FPS для экономии CPU
        /// </summary>
        private int _frameSkipCounter = 0;

        /// <summary>
        /// Флаг видимости окна. False когда окно свёрнуто (WindowState.Minimized).
        /// Используется для автоматической остановки анимации
        /// </summary>
        private bool _isWindowVisible = true;

        /// <summary>
        /// Флаг активности окна. False когда окно потеряло фокус (Deactivated).
        /// Может использоваться для строгого режима энергосбережения
        /// </summary>
        private bool _isWindowActive = true;

        /// <summary>
        /// Ссылка на родительское окно. Используется для подписки на события активации и изменения состояния.
        /// Null до момента присоединения к визуальному дереву
        /// </summary>
        private Window? _parentWindow;
        #endregion

        #region Статические данные
        /// <summary>
        /// Thread-safe генератор случайных чисел для каждого потока
        /// </summary>
        [ThreadStatic]
        private static Random? _threadRng;
        private static Random ThreadRng => _threadRng ??=
            new Random(Environment.TickCount + Environment.CurrentManagedThreadId);

        /// <summary>
        /// Предвычисленная таблица синусов для оптимизации мерцания звёзд
        /// </summary>
        private static readonly double[] _flickerTable;
        #endregion

        #region Конструкторы
        /// <summary>
        /// Инициализирует таблицу предвычисленных значений синуса
        /// </summary>
        static ConstellationCanvas()
        {
            _flickerTable = new double[FLICKER_TABLE_SIZE];
            for (int i = 0; i < FLICKER_TABLE_SIZE; i++)
            {
                double angle = (i / (double)FLICKER_TABLE_SIZE) * Math.PI * 2;
                // Синус от 0.7 до 1.0 для естественного мерцания
                _flickerTable[i] = 0.7 + Math.Sin(angle) * 0.3;
            }
        }

        public ConstellationCanvas()
        {
            _timer = new DispatcherTimer(DispatcherPriority.Render)
            {
                Interval = TimeSpan.FromMilliseconds(TIMER_INTERVAL_MS)
            };
            _timer.Tick += OnAnimationTick;
        }
        #endregion

        #region Avalonia Properties
        /// <summary>
        /// Определяет свойство для цвета звёзд
        /// </summary>
        public static readonly StyledProperty<IBrush?> StarBrushProperty =
            AvaloniaProperty.Register<ConstellationCanvas, IBrush?>(nameof(StarBrush), defaultValue: Brushes.White);

        /// <summary>
        /// Получает или задаёт цвет звёзд и соединительных линий
        /// </summary>
        public IBrush? StarBrush
        {
            get => GetValue(StarBrushProperty);
            set => SetValue(StarBrushProperty, value);
        }

        /// <summary>
        /// Определяет свойство активности анимации
        /// </summary>
        public static readonly StyledProperty<bool> IsActiveProperty =
            AvaloniaProperty.Register<ConstellationCanvas, bool>(nameof(IsActive), defaultValue: false);

        /// <summary>
        /// Получает или задаёт значение, указывающее, активна ли анимация.
        /// При установке в true запускается плавное появление звёзд.
        /// При установке в false запускается плавное исчезновение
        /// </summary>
        public bool IsActive
        {
            get => GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }
        #endregion

        #region Lifecycle методы Avalonia
        /// <summary>
        /// Вызывается при присоединении контрола к визуальному дереву
        /// </summary>
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            UpdateBrushesCache(StarBrush);
            _positionsDirty = true;

            SubscribeToWindowEvents();

            if (_isActive && !_isDisposed && ShouldAnimate())
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (!_isDisposed && _isActive && ShouldAnimate())
                    {
                        _timer.Start();
                    }
                }, DispatcherPriority.Background);
            }
        }

        /// <summary>
        /// Вызывается при отсоединении контрола от визуального дерева
        /// </summary>
        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            StopAnimation();
            DisposeResizeTimer();
            UnsubscribeFromWindowEvents();
            base.OnDetachedFromVisualTree(e);
        }

        /// <summary>
        /// Вызывается при изменении свойств контрола
        /// </summary>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == IsActiveProperty)
                HandleIsActiveChanged(change.GetNewValue<bool>());
            else if (change.Property == StarBrushProperty)
                HandleStarBrushChanged(change.GetNewValue<IBrush?>());
            else if (change.Property == BoundsProperty)
                DebounceResize();
        }
        #endregion

        #region Управление событиями окна
        /// <summary>
        /// Подписывается на события родительского окна для отслеживания видимости
        /// </summary>
        private void SubscribeToWindowEvents()
        {
            if (_parentWindow != null) return;

            _parentWindow = this.FindAncestorOfType<Window>();
            if (_parentWindow == null) return;

            _parentWindow.Activated += OnWindowActivated;
            _parentWindow.Deactivated += OnWindowDeactivated;
            _parentWindow.PropertyChanged += OnWindowPropertyChanged;

            // Инициализируем текущее состояние
            _isWindowActive = _parentWindow.IsActive;
            _isWindowVisible = _parentWindow.WindowState != WindowState.Minimized;
        }

        /// <summary>
        /// Отписывается от событий родительского окна
        /// </summary>
        private void UnsubscribeFromWindowEvents()
        {
            if (_parentWindow == null) return;

            _parentWindow.Activated -= OnWindowActivated;
            _parentWindow.Deactivated -= OnWindowDeactivated;
            _parentWindow.PropertyChanged -= OnWindowPropertyChanged;
            _parentWindow = null;
        }

        /// <summary>
        /// Обработчик активации окна (получение фокуса)
        /// </summary>
        private void OnWindowActivated(object? sender, EventArgs e)
        {
            _isWindowActive = true;
            UpdateAnimationState();
        }

        /// <summary>
        /// Обработчик деактивации окна (потеря фокуса)
        /// </summary>
        private void OnWindowDeactivated(object? sender, EventArgs e)
        {
            _isWindowActive = false;
            UpdateAnimationState();
        }

        /// <summary>
        /// Обработчик изменения свойств окна (отслеживание сворачивания)
        /// </summary>
        private void OnWindowPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property != Window.WindowStateProperty) return;

            var newState = (WindowState)e.NewValue!;
            bool wasVisible = _isWindowVisible;
            _isWindowVisible = newState != WindowState.Minimized;

            if (wasVisible != _isWindowVisible)
                UpdateAnimationState();
        }

        /// <summary>
        /// Проверяет, должна ли анимация выполняться в текущий момент
        /// </summary>
        /// <returns>true если окно видимо и анимацию нужно продолжать, иначе false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ShouldAnimate()
        {
            // return _isWindowVisible && _isWindowActive;
            return _isWindowVisible;
        }

        /// <summary>
        /// Обновляет состояние анимации в зависимости от видимости окна.
        /// Запускает или останавливает таймер для экономии ресурсов
        /// </summary>
        private void UpdateAnimationState()
        {
            if (!_isActive || _isDisposed)
                return;

            bool shouldAnimate = ShouldAnimate();

            if (shouldAnimate && !_timer.IsEnabled)
            {
                // Окно стало видимым - возобновляем анимацию
                _stopwatch.Start();
                _timer.Start();
            }
            else if (!shouldAnimate && _timer.IsEnabled)
            {
                // Окно скрыто/свёрнуто - останавливаем для экономии ресурсов (cpu в частности)
                _timer.Stop();
                _stopwatch.Stop();
            }
        }
        #endregion

        #region Обработчики изменения свойств
        /// <summary>
        /// Обрабатывает изменение свойства IsActive
        /// </summary>
        private void HandleIsActiveChanged(bool newValue)
        {
            if (_isActive == newValue)
                return;

            _isActive = newValue;

            if (_isActive)
                StartAnimation();
            // При _isActive = false анимация плавно затухнет через OnAnimationTick
        }

        /// <summary>
        /// Обрабатывает изменение цвета звёзд
        /// </summary>
        private void HandleStarBrushChanged(IBrush? newBrush)
        {
            UpdateBrushesCache(newBrush);
            InvalidateVisual();
        }
        #endregion

        #region Управление размерами
        /// <summary>
        /// Запускает debounce таймер для отложенного обновления позиций при изменении размера.
        /// Предотвращает избыточные пересчёты при быстром изменении размера окна
        /// </summary>
        private void DebounceResize()
        {
            if (_resizeDebounceTimer == null)
            {
                _resizeDebounceTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(RESIZE_DEBOUNCE_MS)
                };
                _resizeDebounceTimer.Tick += OnResizeDebounceTimerTick;
            }

            _resizeDebounceTimer.Stop();
            _resizeDebounceTimer.Start();
        }

        /// <summary>
        /// Обработчик debounce таймера. Помечает позиции звёзд как устаревшие
        /// </summary>
        private void OnResizeDebounceTimerTick(object? sender, EventArgs e)
        {
            _positionsDirty = true;
            _resizeDebounceTimer?.Stop();
        }

        /// <summary>
        /// Освобождает ресурсы debounce таймера
        /// </summary>
        private void DisposeResizeTimer()
        {
            if (_resizeDebounceTimer == null) return;

            _resizeDebounceTimer.Stop();
            _resizeDebounceTimer.Tick -= OnResizeDebounceTimerTick;
            _resizeDebounceTimer = null;
        }
        #endregion

        #region Кэширование графических ресурсов
        /// <summary>
        /// Обновляет кэш кистей и ручек для заданного цвета.
        /// Создаёт 101 вариант с разной прозрачностью (0%-100%) для быстрого доступа
        /// </summary>
        /// <param name="baseBrush">Базовый цвет для звёзд и линий</param>
        private void UpdateBrushesCache(IBrush? baseBrush)
        {
            baseBrush ??= Brushes.White;

            Color baseColor = ExtractColorFromBrush(baseBrush);

            for (int i = 0; i <= 100; i++)
            {
                double opacity = i / 100.0;
                var brush = new SolidColorBrush(baseColor, opacity);

                _cachedBrushes[i] = brush;
                _cachedPens[i] = new Pen(brush, 0.4);
            }
        }

        /// <summary>
        /// Извлекает цвет из кисти. Поддерживает SolidColorBrush и градиенты
        /// </summary>
        /// <param name="brush">Кисть для извлечения цвета</param>
        /// <returns>Цвет из кисти, или белый если извлечь не удалось</returns>
        private static Color ExtractColorFromBrush(IBrush brush)
        {
            return brush switch
            {
                SolidColorBrush solid => solid.Color,
                GradientBrush gradient when gradient.GradientStops.Count > 0 => gradient.GradientStops[0].Color,
                ImmutableSolidColorBrush immutable => immutable.Color,
                _ => Colors.White
            };
        }
        #endregion

        #region Рендеринг
        /// <summary>
        /// Выполняет рендеринг звёздного неба
        /// </summary>
        public override void Render(DrawingContext context)
        {
            if (_isDisposed)
                return;
            if (_globalAlpha <= 0 && !_isActive)
                return;

            double width = Bounds.Width;
            double height = Bounds.Height;

            if (width <= 0 || height <= 0)
                return;

            if (ShouldUpdateCachedPositions(width, height))
                UpdateCachedPositions(width, height);

            double timeSeconds = _stopwatch.Elapsed.TotalSeconds;
            RenderStarsAndLines(context, timeSeconds);
        }

        /// <summary>
        /// Проверяет, нужно ли обновить кэшированные позиции звёзд
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ShouldUpdateCachedPositions(double width, double height)
        {
            return _positionsDirty ||
                   Math.Abs(_lastWidth - width) > VISIBILITY_THRESHOLD ||
                   Math.Abs(_lastHeight - height) > VISIBILITY_THRESHOLD;
        }

        /// <summary>
        /// Обновляет кэшированные абсолютные позиции звёзд на основе текущих размеров контрола
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateCachedPositions(double width, double height)
        {
            for (int i = 0; i < STAR_COUNT; i++)
            {
                ref var star = ref _stars[i];
                _cachedPositions[i] = new Point(
                    star.RelativePosition.X * width,
                    star.RelativePosition.Y * height
                );
            }

            _lastWidth = width;
            _lastHeight = height;
            _positionsDirty = false;
        }

        /// <summary>
        /// Отрисовывает звёзды и соединительные линии
        /// </summary>
        private void RenderStarsAndLines(DrawingContext context, double timeSeconds)
        {
            double globalAlpha = _globalAlpha;
            double lineProgress = _lineProgress;

            if (globalAlpha <= VISIBILITY_THRESHOLD)
                return;

            bool shouldDrawLines = lineProgress > VISIBILITY_THRESHOLD;

            // Вычисляем базовый индекс для таблицы мерцания
            // Используем битовую маску вместо модуло для максимальной производительности
            double rawTimeIndex = timeSeconds * 5 * FLICKER_TABLE_SIZE / (Math.PI * 2);
            int baseFlickerIndex = (int)rawTimeIndex & FLICKER_TABLE_MASK;

            for (int i = 0; i < STAR_COUNT; i++)
            {
                ref var star = ref _stars[i];
                Point position = _cachedPositions[i];

                int flickerIndex = (baseFlickerIndex + _pulseOffsets[i]) & FLICKER_TABLE_MASK;
                double flicker = _flickerTable[flickerIndex];
                double currentOpacity = star.Opacity * globalAlpha * flicker;

                if (currentOpacity <= VISIBILITY_THRESHOLD) continue;

                if (shouldDrawLines)
                    DrawConnectionLines(context, i, position, globalAlpha, lineProgress);

                DrawStar(context, position, star.Size, currentOpacity);
            }
        }

        /// <summary>
        /// Отрисовывает соединительные линии между близкими звёздами
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawConnectionLines(DrawingContext context, int starIndex, Point positionA, double globalAlpha, double lineProgress)
        {
            for (int j = starIndex + 1; j < STAR_COUNT; j++)
            {
                Point positionB = _cachedPositions[j];

                double dx = positionA.X - positionB.X;
                double dy = positionA.Y - positionB.Y;
                double distanceSq = dx * dx + dy * dy;

                if (distanceSq < MAX_DISTANCE_SQ)
                {
                    double normalizedDistance = distanceSq / MAX_DISTANCE_SQ;

                    double lineAlpha = (1 - normalizedDistance) * lineProgress * globalAlpha * LINE_ALPHA_MULTIPLIER;

                    if (lineAlpha > VISIBILITY_THRESHOLD)
                    {
                        int penIndex = Math.Clamp((int)(lineAlpha * 100), 0, 100);
                        context.DrawLine(_cachedPens[penIndex], positionA, positionB);
                    }
                }
            }
        }

        /// <summary>
        /// Отрисовывает одну звезду
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawStar(DrawingContext context, Point position, double size, double opacity)
        {
            int brushIndex = Math.Clamp((int)(opacity * 100), 0, 100);
            context.DrawEllipse(_cachedBrushes[brushIndex], null, position, size, size);
        }
        #endregion

        #region Управление анимацией
        /// <summary>
        /// Запускает анимацию звёздного неба с плавным появлением
        /// </summary>
        private void StartAnimation()
        {
            if (_isDisposed)
                return;

            EnsureStarsGenerated();

            _globalAlpha = 0;
            _lineProgress = 0;
            _frameSkipCounter = 0;
            _positionsDirty = true;

            if (ShouldAnimate())
            {
                _stopwatch.Restart();

                if (!_timer.IsEnabled)
                    _timer.Start();
            }
        }

        /// <summary>
        /// Полностью останавливает анимацию и все таймеры
        /// </summary>
        private void StopAnimation()
        {
            if (_timer.IsEnabled)
                _timer.Stop();

            if (_stopwatch.IsRunning)
                _stopwatch.Stop();
        }

        /// <summary>
        /// Обработчик тика таймера анимации. Обновляет состояние и запускает перерисовку
        /// </summary>
        private void OnAnimationTick(object? sender, EventArgs e)
        {
            if (_isDisposed)
                return;

            if (!ShouldAnimate() && _isActive)
            {
                _timer.Stop();
                _stopwatch.Stop();
                return;
            }

            bool needsRender;
            if (_isActive)
                needsRender = UpdateFadeInAnimation();
            else
                needsRender = UpdateFadeOutAnimation();

            if (needsRender)
                InvalidateVisual();
        }

        /// <summary>
        /// Обновляет анимацию появления (fade-in) звёзд и линий
        /// </summary>
        /// <returns>true если требуется перерисовка, иначе false</returns>
        private bool UpdateFadeInAnimation()
        {
            bool needsRender = false;

            if (_globalAlpha < 1.0)
            {
                _globalAlpha = Math.Min(1.0, _globalAlpha + FADE_SPEED);
                needsRender = true;
            }

            if (_globalAlpha > 0.5 && _lineProgress < 1.0)
            {
                _lineProgress = Math.Min(1.0, _lineProgress + LINE_SPEED);
                needsRender = true;
            }

            if (_globalAlpha >= 1.0 && _lineProgress >= 1.0)
            {
                _frameSkipCounter++;

                if (_frameSkipCounter >= FRAME_SKIP_THRESHOLD)
                {
                    _frameSkipCounter = 0;
                    needsRender = true;
                }
            }

            return needsRender;
        }

        /// <summary>
        /// Обновляет анимацию исчезновения (fade-out) линий и звёзд
        /// </summary>
        /// <returns>true если требуется перерисовка, иначе false</returns>
        private bool UpdateFadeOutAnimation()
        {
            bool needsRender = false;

            if (_lineProgress > 0)
            {
                _lineProgress = Math.Max(0, _lineProgress - FADE_SPEED);
                needsRender = true;
            }

            else if (_globalAlpha > 0)
            {
                _globalAlpha = Math.Max(0, _globalAlpha - FADE_SPEED);
                needsRender = true;
            }

            if (_globalAlpha <= 0 && _lineProgress <= 0)
            {
                StopAnimation();
                InvalidateVisual();
            }

            return needsRender;
        }
        #endregion

        #region Генерация звёзд
        /// <summary>
        /// Проверяет, что звёзды инициализированы, и генерирует их при необходимости
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureStarsGenerated()
        {
            if (_stars[0].Size == 0)
                GenerateConstellation();
        }

        /// <summary>
        /// Генерирует случайное расположение звёзд в созвездии
        /// </summary>
        private void GenerateConstellation()
        {
            var rng = ThreadRng;

            for (int i = 0; i < STAR_COUNT; i++)
            {
                double pulse = rng.NextDouble() * Math.PI;

                _stars[i] = new Star
                {
                    RelativePosition = new Point(rng.NextDouble(), rng.NextDouble()),

                    Size = rng.NextDouble() * 2 + 1,

                    Opacity = rng.NextDouble() * 0.4 + 0.6,

                    Pulse = pulse
                };

                _pulseOffsets[i] = (int)(pulse * FLICKER_TABLE_SIZE / (Math.PI * 2));
            }

            _positionsDirty = true;
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// Освобождает все ресурсы, используемые контролом
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            if (Dispatcher.UIThread.CheckAccess())
                DisposeCore();
            else
                Dispatcher.UIThread.Post(DisposeCore, DispatcherPriority.Send);
        }

        /// <summary>
        /// Внутренняя логика освобождения ресурсов
        /// </summary>
        private void DisposeCore()
        {
            try
            {
                _timer.Stop();
                _timer.Tick -= OnAnimationTick;
            }
            catch (InvalidOperationException)
            {
                // Таймер уже disposed - игнорируем
            }

            DisposeResizeTimer();
            UnsubscribeFromWindowEvents();

            if (_stopwatch.IsRunning)
                _stopwatch.Stop();
        }
        #endregion
    }
}