using Autrage.LEX.NET.Extensions;
using Autrage.RNN.NET;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ConsoleApp
{
    internal class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            TestSimulation();

            Console.ReadLine();
        }

        private static void TestSimulation()
        {
            Simulation simulation = new Simulation()
            {
                Order = 10,
                Complexity = 100,
            };

            Stopwatch stopwatch = Stopwatch.StartNew();
            simulation.Populate();
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
        }

        #endregion Methods
    }
}