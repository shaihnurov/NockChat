namespace NockChat.Application.Common.Exceptions
{
    public class NotFoundException(string message) : Exception(message) { }
}