using System;

namespace NockChat.Services.Common.Exceptions
{
    public class StorageException(string message, Exception? inner = null) : Exception(message, inner);
}