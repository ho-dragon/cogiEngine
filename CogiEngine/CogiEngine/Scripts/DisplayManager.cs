using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Khronos;
using OpenGL;

namespace CogiEngine
{
    public class DisplayManager
    {
        public const int WIDTH = 1280;
        public const int HEIGHT = 720;

        private long lastFrameTime;
        private float deltaTime;
        
        Gl.DebugProc _debugProc;
        
        public void CreateDisplay(GlControl glControl)
        {
            _debugProc = GLDebugMessageCallbackProc;
            
            if (Gl.CurrentExtensions != null && Gl.CurrentExtensions.DebugOutput_ARB)
            {
                Gl.DebugMessageCallback(_debugProc, IntPtr.Zero);
                Gl.DebugMessageControl(DebugSource.DontCare, DebugType.DontCare, DebugSeverity.DontCare, 0, null, true);
            }

            if (Gl.CurrentVersion != null && Gl.CurrentVersion.Api == KhronosVersion.ApiGl && glControl.MultisampleBits > 0)
            {
                Gl.Enable(EnableCap.Multisample);
            }

            this.lastFrameTime = GetCurrentTime();
        }
        
        private void GLDebugMessageCallbackProc(DebugSource source, DebugType type, uint id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string debug_string = Marshal.PtrToStringAnsi(message, length);
            Debug.WriteLine($"{source}, {type}, {severity}: {debug_string}");
        }


        public void UpdateDisplay()
        {
            long currentFrameTime = GetCurrentTime(); 
            this.deltaTime = (currentFrameTime - lastFrameTime) / 10000000f;
            this.lastFrameTime = currentFrameTime;
        }

        public float GetFrameTimeSeconds()
        {
            return this.deltaTime;
        }

        public void CloseDisplay()
        {
            
        }

        private long GetCurrentTime()
        {
            return DateTime.Now.Ticks;
        }
        
    }
}