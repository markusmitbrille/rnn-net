using System;

namespace Autrage.RNN.NET
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class SensorAttribute : Attribute
    {
    }
}