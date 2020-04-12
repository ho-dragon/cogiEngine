using System.Windows.Forms;
using OpenGL;

namespace CogiEngine
{
    public class Camera
    {
        private Vertex3f position = Vertex3f.Zero;
        private const float MOVE_ROTATION_PER_FRAME = 1f;
        private const float MOVE_POSITION_PER_FRAME = 0.02f;
        private bool isRotation = false;
        private bool isSpeedUp = false;

        public Vertex3f Position { get { return position; } }
        
        float pitch;
        public float Pitch { get { return pitch; } }

        float yaw;
        public float Yaw { get { return yaw; } }

        float roll;
        public float Roll { get { return roll; } }

        public Camera()
        {
            position = new Vertex3f(0, 10, 0);
            pitch = 20f;
        }

        public void OnEventKeyDown(Keys inputKey)
        {
            switch (inputKey)
            {
                case Keys.R:
                    isRotation = !isRotation;
                    break;
                case Keys.F:
                    isSpeedUp = !isSpeedUp;
                    break;
            }
        }
        
        public void UpdateMove(InputManager input)
        {
            float speed = isSpeedUp ? 100f : 1f;
            if (input.IsKeyStatus(KeyStatus.KeyDown,Keys.W))
            {
                if (isRotation)
                {
                    pitch -= MOVE_ROTATION_PER_FRAME;
                }
                else
                {
                    position.z -= MOVE_POSITION_PER_FRAME * speed;    
                }
            }  
            
            if (input.IsKeyStatus(KeyStatus.KeyDown,Keys.S))
            {
                if (isRotation)
                {
                    pitch +=  MOVE_ROTATION_PER_FRAME;;
                }
                else
                {
                    position.z += MOVE_POSITION_PER_FRAME * speed;    
                }
            }  
            
            if (input.IsKeyStatus(KeyStatus.KeyDown,Keys.D))
            {
                if (isRotation)
                {
                    yaw +=  MOVE_ROTATION_PER_FRAME;;
                }
                else
                {
                    position.x += MOVE_POSITION_PER_FRAME * speed;    
                }
            }  
            
            if (input.IsKeyStatus(KeyStatus.KeyDown,Keys.A))
            {
                if (isRotation)
                {
                    yaw -=  MOVE_ROTATION_PER_FRAME;;
                }
                else
                {
                    position.x -= MOVE_POSITION_PER_FRAME * speed;    
                }
            }
            
            if (input.IsKeyStatus(KeyStatus.KeyDown,Keys.Q))
            {
                position.y -= MOVE_POSITION_PER_FRAME * speed;
            }
            
            if (input.IsKeyStatus(KeyStatus.KeyDown,Keys.E))
            {
                position.y += MOVE_POSITION_PER_FRAME * speed;
            }
        }
    }
}