using System;

namespace DataAccess.Providers
{
    public interface ILogger
    {
        void Log(DateTime time, string message);
    }
}