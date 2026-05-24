using System;

namespace NockChat.Services.Common.Exceptions
{
    public class ServerException(string message, int statusCode) : Exception(message)
    {
        public int StatusCode { get; } = statusCode;
    }
}