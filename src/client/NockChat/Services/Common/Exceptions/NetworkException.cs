using System;

namespace NockChat.Services.Common.Exceptions
{
    public class NetworkException(string message) : Exception(message);
}