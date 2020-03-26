using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace CogiEngine
{
    public enum KeyStatus
    {
        None = 0,
        KeyDown = 1,
        KeyUp =2,
    }
    public class InputManager
    {
        private bool isKeyDown;
        private Keys lastKeyDown;
        private Dictionary<Keys, KeyStatus> inputStatus = new Dictionary<Keys, KeyStatus>();

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
        
        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            UpdateKeyStatus(e.KeyCode, KeyStatus.KeyDown);
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            UpdateKeyStatus(e.KeyCode, KeyStatus.KeyUp);
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