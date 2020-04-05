using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace CogiEngine
{
    public static class CogiLogger
    {
        public static void Log(string title, string desc)
        {
            Debug.WriteLine(string.Format("[{0}] {1}", title, desc));
        }
        public static void LogConsole(string title, string desc)
        {
            Console.WriteLine(title, desc);
        }
    }
}