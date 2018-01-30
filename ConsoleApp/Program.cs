using Autrage.LEX.NET;
using System;

namespace ConsoleApp
{
    internal class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(Singleton<Random>.Instance.NextDouble() * int.MaxValue - int.MaxValue);
            }

            Console.ReadLine();
        }

        #endregion Methods
    }
}