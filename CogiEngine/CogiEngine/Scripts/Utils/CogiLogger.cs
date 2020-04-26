using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace CogiEngine
{
    public static class CogiLogger
    {
        public static void Debug(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
        }
        
        public static void Error(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
        }
    }
}