using System.Windows.Forms;
using OpenGL;

namespace CogiEngine
{
    public class Camera
    {
        Vertex3f _position = new Vertex3f(0, 0, 0);
        public Vertex3f Position { get { return _position; } }

        float _pitch;
        public float Pitch { get { return _pitch; } }

        float _yaw;
        public float Yaw { get { return _yaw; } }

        float _roll;
        public float Roll { get { return _roll; } }

        public Camera()
        {
        }

        public void UpdateMoveByInput(InputManager input)
        {
            if (input.IsKeyStatus(KeyStatus.KeyDown,Keys.W))
            {
                _position.z -= 0.02f;
            }  
            
            if (input.IsKeyStatus(KeyStatus.KeyDown,Keys.S))
            {
                _position.z += 0.02f;
            }  
            
            if (input.IsKeyStatus(KeyStatus.KeyDown,Keys.D))
            {
                _position.x += 0.02f;
            }  
            
            if (input.IsKeyStatus(KeyStatus.KeyDown,Keys.A))
            {
                _position.x -= 0.02f;
            }
        }
    }
}