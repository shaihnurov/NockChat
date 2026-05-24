using System;

namespace NockChat.Services.Common.Exceptions
{
    public class SignalRException(string message, Exception? inner = null) : Exception(message, inner);
}