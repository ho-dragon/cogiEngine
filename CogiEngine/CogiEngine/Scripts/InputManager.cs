using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices; 

namespace CogiEngine
{
    public enum KeyStatus
    {
        None = 0,
        KeyDown = 1,
        KeyUp = 2,
    }
    public class InputManager
    {
        private bool isMouseLeftDown;
        private bool isMouseRightDown;
        private float mousePosX;
        private float mousePosY;
        private float mouseDeltaX;
        private float mouseDeltaY;
        private float lastMousePosX;
        private float lastMousePosY;

        private Dictionary<Keys, KeyStatus> inputStatus = new Dictionary<Keys, KeyStatus>();
        public delegate void OnEventKeyDownHandler(Keys key);
        public event OnEventKeyDownHandler OnEventKeyDown;
        
        public delegate void OnEventWheelHandler(int deltaWheel);
        public event OnEventWheelHandler OnEventMouseWheel;
        
        public bool IsMouseLeftDown => isMouseLeftDown;
        public bool IsMouseRightDown => isMouseRightDown;
        public float MousePosX => mousePosX;
        public float MousePosY => mousePosY;
        public float MouseDeltaX => mouseDeltaX;
        public float MouseDeltaY => mouseDeltaY;
        
        public KeyStatus GetKeyStatus(Keys keyCode)
        {
            if (inputStatus.ContainsKey(keyCode) == false)
            {
                return KeyStatus.None;
            }
            return inputStatus[keyCode];
        }
        
        public bool IsKeyStatus(KeyStatus keyStatus, Keys keyCode)
        {
            if (inputStatus.ContainsKey(keyCode) == false)
            {
                return false;
            }
            return inputStatus[keyCode] == keyStatus;
        }
        
        public void OnKeyDown(object sender, KeyEventArgs args)
        {
            UpdateKeyStatus(args.KeyCode, KeyStatus.KeyDown);
            if (OnEventKeyDown != null)
            {
                OnEventKeyDown(args.KeyCode);    
            }
        }

        public void OnKeyUp(object sender, KeyEventArgs args)
        {
            UpdateKeyStatus(args.KeyCode, KeyStatus.KeyUp);
        }

        public void OnMouseDown(object sender, MouseEventArgs args)
        {
            if (args.Button == MouseButtons.Left)
            {
                this.isMouseLeftDown = true;
            }
            
            if (args.Button == MouseButtons.Right)
            {
                this.isMouseRightDown = true;
            }
        }

        public void OnMouseUp(object sender, MouseEventArgs args)
        {
            if (args.Button == MouseButtons.Left)
            {
                this.isMouseLeftDown = false;
            }

            if (args.Button == MouseButtons.Right)
            {
                this.isMouseRightDown = false;
            }
        }

        public void OnMoueWheel(object sender, MouseEventArgs args)
        {
            OnEventMouseWheel(args.Delta);
        }

        public void OnMouseMove(object sender, MouseEventArgs args)
        {
            this.mousePosX = args.X;
            this.mousePosY = args.Y;
        }

        public void UpdateMousePosition()
        {
            this.mouseDeltaX = this.lastMousePosX - this.mousePosX;
            this.mouseDeltaY = this.lastMousePosY - this.mousePosY;
            
            this.lastMousePosX = mousePosX;
            this.lastMousePosY = mousePosY;
        }

        private void UpdateKeyStatus(Keys keyCode, KeyStatus keyStatus)
        {
            if (inputStatus.ContainsKey(keyCode) == false)
            {
                inputStatus.Add(keyCode, keyStatus);
                return;
            }
            inputStatus[keyCode] = keyStatus;
        }
    }
}