namespace  M_yoko
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    public class MeasurementCPU
    {
        private ConcurrentDictionary<string, ProcessCounterWithCPUUses> _cpuUses;
        System.Diagnostics.Stopwatch sw;
        public MeasurementCPU()
        {
            var psList = new List<System.Diagnostics.Process>();
            psList.AddRange(System.Diagnostics.Process.GetProcesses());
            _cpuUses = new ConcurrentDictionary<string, ProcessCounterWithCPUUses>();
            sw = new System.Diagnostics.Stopwatch();

            psList.AsParallel().ForAll(
                ps =>
                {
                    try
                    {
                        var counter = new PerformanceCounter("Process", "% Processor Time", ps.ProcessName, true);
                        counter.NextValue();
                        _cpuUses.TryAdd(ps.ProcessName, new ProcessCounterWithCPUUses(counter));
                    }
                    catch (Exception)
                    {
                    }
                }
            );
            sw.Start();
        }

        public void StopStopWatch()
        {
            sw.Stop();
        }

        public void Measurement()
        {
            _cpuUses.AsParallel().ForAll(
                processCounterWithCpuUses =>
                {
                    processCounterWithCpuUses.Value.StorePerformanceCounter();
                }
                );
        }

        public void Print()
        {
            var result = new Dictionary<string, float>();

            foreach (var cpuUse in this._cpuUses)
            {
                var length = cpuUse.Value.GetPerfomanceCounterLength();
                var skip = length / 4;
                var take = length / 4 * 2;
                result.Add(cpuUse.Key, cpuUse.Value.GetPerformanceCounterAverage(skip, take));
                //Console.WriteLine($"{cpuUse.Key}:{cpuUse.Value.GetPerformanceCounterAverage(skip, take):N2}");
            }
            foreach (var cpuUse in result.OrderByDescending(val => val.Value))
            {
                Console.WriteLine($"{cpuUse.Key}:{cpuUse.Value:N2}");
            }
        }

        public void PrintToFile(string path,string secondComment ="seconds")
        {
            var result = new Dictionary<string, float>();

            foreach (var cpuUse in this._cpuUses)
            {
                var length = cpuUse.Value.GetPerfomanceCounterLength();
                var skip = length / 4;
                var take = length / 4 * 2;
                result.Add(cpuUse.Key, cpuUse.Value.GetPerformanceCounterAverage(skip, take));
                //Console.WriteLine($"{cpuUse.Key}:{cpuUse.Value.GetPerformanceCounterAverage(skip, take):N2}");
            }
            foreach (var cpuUse in result.OrderByDescending(val => val.Value))
            {
                File.AppendAllText(path, $"{cpuUse.Key}:{cpuUse.Value:N2}\n");
            }
            File.AppendAllText(path, $"{secondComment}:{sw.Elapsed.TotalSeconds:N2}\n");

        }

    }
    public class ProcessCounterWithCPUUses
    {
        private PerformanceCounter _performanceCounter;
        private List<float> _cpuUses;

        public ProcessCounterWithCPUUses(PerformanceCounter performanceCounter)
        {
            this._performanceCounter = performanceCounter;
            this._cpuUses = new List<float>();
        }

        public void StorePerformanceCounter()
        {
            try
            {
                _cpuUses.Add(_performanceCounter.NextValue() / Environment.ProcessorCount);

            }
            catch (Exception)
            {

            }
        }

        public int GetPerfomanceCounterLength()
        {
            return this._cpuUses.Count;
        }

        public float GetPerformanceCounterAverage(int skip, int take)
        {
            return this._cpuUses.Skip(skip).Take(take).Average();
        }

    }



}
