
namespace M_yoko
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    class Program
    {
        private static readonly string passwordChars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";


        public static string GeneratePassword(int length)
        {
            StringBuilder sb = new StringBuilder(length);
            Random r = new Random();

            for (int i = 0; i < length; i++)
            {
                //文字の位置をランダムに選択
                int pos = r.Next(passwordChars.Length);
                //選択された位置の文字を取得
                char c = passwordChars[pos];
                //パスワードに追加
                sb.Append(c);
            }

            return sb.ToString();
        }

        static void Main()
        {

            var measure = new MeasurementCPU();

            if (!Directory.Exists(ConstParams.DirName))
            {
                Directory.CreateDirectory(ConstParams.DirName);
            }

            var task = Task.Run(() =>
            {
                for (int i = 0; i < ConstParams.NumberOfFiles; i++)
                {
                    using (StreamWriter sw = new StreamWriter($@"{ConstParams.DirName}\{i}.txt"))
                    {
                        sw.WriteLine(GeneratePassword(ConstParams.StringLength));
                    }
                }
            }
            );
            while (task.Status != TaskStatus.RanToCompletion)
            {
                measure.Measurement();
            }
            measure.StopStopWatch();
            measure.PrintToFile(@".\create.txt","create seconds");
        }
    }
}