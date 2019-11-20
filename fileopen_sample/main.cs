using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M_yoko
{
    class Program
    {
        public static volatile string str;
        static void Main()
        {
            var measure = new MeasurementCPU();
            var task = Task.Run(() =>
            {
                for (int i = 0; i < ConstParams.NumberOfFiles; i++)
                {
                    using (StreamReader sr = new StreamReader($@"{ConstParams.DirName}\{i}.txt"))
                    {
                        str = sr.ReadToEnd();
                    }
                }
            }
);
            while (task.Status != TaskStatus.RanToCompletion)
            {
                measure.Measurement();
            }
            measure.StopStopWatch();
            measure.PrintToFile(@".\openRead.txt", "openRead seconds");
        }

    }
}
