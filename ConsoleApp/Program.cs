using Autrage.LEX.NET.Extensions;
using Autrage.RNN.NET;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using static System.Console;

namespace ConsoleApp
{
    internal class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            TestSimulation();
        }

        private static void P_Died(object sender, EventArgs e) => WriteLine("method");

        private static void TestSimulation()
        {
            Simulation simulation = new Simulation()
            {
                Order = 100,
                Complexity = 1000,
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

    class Person
    {
        public event EventHandler Died;

        private void Die()
        {
            Died?.Invoke(this, EventArgs.Empty);
        }
    }
}