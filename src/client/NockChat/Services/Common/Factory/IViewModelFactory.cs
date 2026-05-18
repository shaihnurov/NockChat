namespace NockChat.Services.Common.Factory
{
    /// <summary>
    /// Интерфейс фабрики ViewModel, позволяющий создавать экземпляры ViewModel
    /// с возможностью передачи параметров
    /// </summary>
    /// <typeparam name="T">Тип создаваемого ViewModel</typeparam>
    public interface IViewModelFactory<T>
    {
        /// <summary>
        /// Создает экземпляр ViewModel с указанными параметрами
        /// </summary>
        /// <param name="parameters">Параметры, передаваемые в конструктор ViewModel.</param>
        /// <returns>Созданный экземпляр ViewModel</returns>
        T Create(params object[] parameters);
    }
}