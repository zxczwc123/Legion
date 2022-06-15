// ========================================================
// 描 述：ShellUtility.cs 
// 作 者：郑贤春 
// 时 间：2017/02/23 09:53:15 
// 版 本：5.4.1f1 
// ========================================================
using System.Diagnostics;
using System.Threading;

namespace Framework.MEditor.Utility
{
    public class ShellUtility
    {
        public static void RunShell(string command)
        {
            RunCommand(command);
        }

        public static void RunShellInSilence(string command)
        {
            RunCommand(command,true);
        }

        public static void RunAsyncShell(string command)
        {
            // 这里不开线程的话，就会阻塞住unity的主线程，当然如果你需要阻塞的效果的话可以不开
            Thread newThread = new Thread(()=> {
                RunCommand(command);
            });
            newThread.Start();
        }

        private static void RunCommand(string command,bool createNoWindow = false)
        {
            Process process = new Process();
            process.StartInfo.FileName = "powershell";
            process.StartInfo.Arguments = command;

            process.StartInfo.CreateNoWindow = createNoWindow; // 获取或设置指示是否在新窗口中启动该进程的值（不想弹出powershell窗口看执行过程的话，就=true）
            process.StartInfo.ErrorDialog = true; // 该值指示不能启动进程时是否向用户显示错误对话框
            process.StartInfo.UseShellExecute = false;
            //process.StartInfo.RedirectStandardError = true;
            //process.StartInfo.RedirectStandardInput = true;
            //process.StartInfo.RedirectStandardOutput = true;

            process.Start();

            //process.StandardInput.WriteLine(@"explorer.exe D:\");
            //process.StandardInput.WriteLine("pause");

            process.WaitForExit();
            process.Close();
        }

        private static void RunProcessCommand(string command, string argument)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = command;
            start.Arguments = argument;

            start.CreateNoWindow = false;
            start.ErrorDialog = true;
            start.UseShellExecute = false;

            Process p = Process.Start(start);
            p.WaitForExit();
            p.Close();
        }
    }
}