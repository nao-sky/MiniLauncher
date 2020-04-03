using System;
using System.Diagnostics;

namespace CmdExecuter
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Process process = new Process();
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = args[0];
                processStartInfo.Arguments = string.Join(' ', args[1..]);
                processStartInfo.UseShellExecute = true;
                processStartInfo.CreateNoWindow = false;

                process.StartInfo = processStartInfo;
                process.Start();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine();
            Console.WriteLine("終了するにはエンターキーを押してください。");
            Console.ReadLine();

        }
    }
}
