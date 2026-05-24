using System;

namespace NockChat.Services.Common.Exceptions
{
    public class DeserializationException(string message) : Exception(message);
}