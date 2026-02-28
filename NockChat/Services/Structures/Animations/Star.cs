using Avalonia;

namespace NockChat.Services.Structures.Animations;

/// <summary>
/// Структура, описывающая звезду для анимационных эффектов (фон, декоративные элементы, мерцание)
/// </summary>
public struct Star
{
    /// <summary>
    /// Относительная позиция звезды в пределах контейнера или экрана
    /// </summary>
    public Point RelativePosition;

    /// <summary>
    /// Прозрачность звезды
    /// (0 — полностью прозрачна, 1 — полностью видима)
    /// </summary>
    public double Opacity;

    /// <summary>
    /// Размер звезды
    /// </summary>
    public double Size;

    /// <summary>
    /// Коэффициент пульсации звезды, используемый для анимации мерцания
    /// </summary>
    public double Pulse;
}