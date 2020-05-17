using System;
using System.Drawing.Printing;
using System.Windows.Forms;
using OpenGL;

namespace CogiEngine
{
    public class Camera
    {
        private float distanceFromPlayer = 50f;
        private float angleAroundPlayer = 0f;
        private Vertex3f position = Vertex3f.Zero;
        private float pitch = 0f;
        private float yaw = 0f;
        private float roll = 0f;
        
        public Vertex3f Position { get { return position; } }
        public float Pitch { get { return pitch; } }
        public float Yaw { get { return yaw; } }
        public float Roll { get { return roll; } }

        public Camera(Vertex3f position, float pitch)
        {
            this.position = position;
            this.pitch = pitch;
        }

        private void UpdateZoom(float deltaWeel)
        {
            this.distanceFromPlayer -= deltaWeel * 0.1f;
        }

        private void UpdatePitch(float mouseDeltaY)
        {
            this.pitch -= mouseDeltaY * 0.1f;
        }

        public void UpdatePosition(Vertex3f newPosition)
        {
            this.position = newPosition;
        }

        public void InvertPitch()
        {
            this.pitch = -this.pitch;
        }

        private void UpdateAngleAroundPlayer(float mouseDeltaX)
        {
            float angleChange = mouseDeltaX * 0.3f;
            this.angleAroundPlayer += angleChange;
        }

        private void UpdatePosition(Vertex3f targetPosition, float targetRotationY)
        {
            float theta = targetRotationY + angleAroundPlayer;
            float offsetX = GetHorizontalDisatnace() * (float)Math.Sin(Maths.DegreeToRadian(theta));
            float offsetZ = GetHorizontalDisatnace() * (float)Math.Cos(Maths.DegreeToRadian(theta));
            position.x = targetPosition.x - offsetX;
            position.z = targetPosition.z - offsetZ;
            position.y = targetPosition.y + GetVerticalDisatnace();
            this.yaw = 180 - (targetRotationY + angleAroundPlayer);
        }

        private float GetHorizontalDisatnace()
        {
            return this.distanceFromPlayer * (float)Math.Cos(Maths.DegreeToRadian(pitch));
        }
        
        private float GetVerticalDisatnace()
        {
            return this.distanceFromPlayer * (float)Math.Sin(Maths.DegreeToRadian(pitch));
        }

        public void OnEventWheel(int deltaWeel)
        {
            UpdateZoom(deltaWeel);
        }

        public void UpdateMove(Vertex3f targetPosition, float targetRotationY, InputManager input)
        {
            if (input.IsMouseLeftDown)
            {
                UpdatePitch(input.MouseDeltaY);
            }

            if (input.IsMouseRightDown)
            {
                UpdateAngleAroundPlayer(input.MouseDeltaX);
            }
            UpdatePosition(targetPosition, targetRotationY);
        }
    }
}