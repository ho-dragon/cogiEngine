﻿using System;
using System.Diagnostics;
using System.Windows.Forms;
using Khronos;

namespace CogiEngine
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            CogiLogger.Log("Main", "=========START=======");
            string envDebug = Environment.GetEnvironmentVariable("DEBUG");
            if (envDebug == "GL")
            {
                KhronosApi.Log += delegate(object sender, KhronosLogEventArgs e)
                {
                    CogiLogger.Log("KhronosApi", e.ToString());
                };
                KhronosApi.LogEnabled = true;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}