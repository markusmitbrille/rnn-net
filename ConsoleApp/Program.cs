using Autrage.LEX.NET.Extensions;
using Autrage.LEX.NET.Serialization;
using Autrage.RNN.NET;
using System;
using System.Diagnostics;
using System.IO;
using static System.Console;

namespace ConsoleApp
{
    internal class Program
    {
        delegate void Move(double stimulus);
        private static void Main(string[] args)
        {
            WriteLine(typeof(Program).GetMethod(nameof(P_Died)).ReturnParameter.ParameterType);
            ReadLine();
        }

        public static void Do(double p)
        {
            WriteLine(p);
        }

        public static void P_Died(object sender, EventArgs e) => WriteLine("method");

        private static void TestSimulation()
        {
            Simulation simulation = new Simulation()
            {
                Order = 10,
                GenomeComplexity = 20,
                MaxChromosomeSize = 100,
                MaxChromosomeSensors = 10,
                MaxChromosomeMuscles = 10,
                MaxChromosomeOrder = 500,
                MaxChromosomeConnectivity = 200,
                MaxChromosomeSensitivity = 20,
                MaxChromosomeProactivity = 20,
            };

            Stopwatch stopwatch = Stopwatch.StartNew();
            simulation.Populate();
            stopwatch.Stop();
            WriteLine($"Genesis: {stopwatch.ElapsedMilliseconds}ms");

            using (MemoryStream stream = new MemoryStream())
            {
                stopwatch.Restart();
                simulation.Serialize(stream);
                stopwatch.Stop();
                WriteLine($"Serialization: {stopwatch.ElapsedMilliseconds}ms");

                stream.Reset();

                stopwatch.Restart();
                simulation = Simulation.Deserialize(stream);
                stopwatch.Stop();
                WriteLine($"Deserialization: {stopwatch.ElapsedMilliseconds}ms");
            }
        }
    }

    internal class Person
    {
        public event EventHandler Died;

        public void Die()
        {
            Died?.Invoke(this, EventArgs.Empty);
        }
    }
}