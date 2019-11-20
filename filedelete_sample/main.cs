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
        static void Main()
        {
            var measure = new MeasurementCPU();
            var task = Task.Run(() =>
            {
                for (int i = 0; i < ConstParams.NumberOfFiles; i++)
                {
                    File.Delete($@"{ConstParams.DirName}\{i}.txt");
                }
            }
);
            while (task.Status != TaskStatus.RanToCompletion)
            {
                measure.Measurement();
            }
            measure.StopStopWatch();
            measure.PrintToFile(@".\delete.txt", "delete seconds");
        }

    }
}
