using System.Diagnostics;

namespace CogiEngine
{
    public static class CogiLogger
    {
        public static void Log(string title, string desc)
        {
            Debug.WriteLine(string.Format("[{0}] {1}", title, desc));
        }
    }
}