using Autrage.LEX.NET.Extensions;
using Autrage.RNN.NET;
using System;
using System.Diagnostics;
using System.IO;

namespace ConsoleApp
{
    internal class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            Simulation simulation = new Simulation()
            {
                Order = 100,
                Complexity = 1000,
            };

            Stopwatch stopwatch = Stopwatch.StartNew();
            simulation.Genesis();
            stopwatch.Stop();
            Console.WriteLine($"Genesis: {stopwatch.ElapsedMilliseconds}ms");

            using (MemoryStream stream = new MemoryStream())
            {
                stopwatch.Restart();
                simulation.Serialize(stream);
                stopwatch.Stop();
                Console.WriteLine($"Serialization: {stopwatch.ElapsedMilliseconds}ms");

                stream.Reset();

                stopwatch.Restart();
                simulation = Simulation.Deserialize(stream);
                stopwatch.Stop();
                Console.WriteLine($"Deserialization: {stopwatch.ElapsedMilliseconds}ms");

                stopwatch.Restart();
                simulation.Serialize(stream);
                stopwatch.Stop();
                Console.WriteLine($"Serialization: {stopwatch.ElapsedMilliseconds}ms");
            }

            Console.ReadLine();
        }

        #endregion Methods
    }
}