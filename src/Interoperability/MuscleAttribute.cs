using System;

namespace Autrage.RNN.NET
{
    [AttributeUsage(System.AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class MuscleAttribute : Attribute
    {
    }
}