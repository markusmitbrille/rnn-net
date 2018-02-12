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
        #region Methods

        private static void Main(string[] args)
        {
            Marshaller m = new Marshaller(new PrimitiveSerializer(), new StringSerializer(), new EnumSerializer(), new DelegateSerializer(), new ContractSerializer());

            Person p = new Person();
            p.Died += (sender, e) => WriteLine("LERP");
            p.Died += P_Died;

            using (MemoryStream stream = new MemoryStream())
            {
                m.Serialize(stream, p);

                stream.Reset();

                Person res = m.Deserialize<Person>(stream);
                res?.Die();
            }

            ReadLine();
        }

        private static void P_Died(object sender, EventArgs e) => WriteLine("method");

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

        #endregion Methods
    }

    internal class Person
    {
        #region Events

        public event EventHandler Died;

        #endregion Events

        #region Methods

        public void Die()
        {
            Died?.Invoke(this, EventArgs.Empty);
        }

        #endregion Methods
    }
}