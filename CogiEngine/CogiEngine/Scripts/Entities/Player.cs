using System;
using System.Diagnostics;
using System.Windows.Forms;
using OpenGL;

namespace CogiEngine
{
    public class Player : Entity
    {
        private const float SPEED = 20f;
        private const float TURN_SPPED = 160f;
        private const float GRAVITY = -50f;
        private const float JUMP_POWER = 30f;
        
        private float currentSpeed = 0f;
        private float currentTurnSpeed = 0f;
        private float upwardsSpeed = 0f;
        private bool isInAir = false;
        
        public Player(TextureModel textureModel, Vertex3f position, float rotationX, float rotationY, float rotationZ, float scale)
            : base(textureModel, position, rotationX, rotationY, rotationZ, scale)
        {
            
        }
        
        private void Move(Terrain terrain, float frameTimeSec)
        {
            base.IncreaseRotation(0f, this.currentTurnSpeed * frameTimeSec, 0f);
            float distance = currentSpeed * frameTimeSec;
            double radian  = Math.PI * rotationY / 180.0f;
            float dx = distance * (float)Math.Sin(radian);
            float dz = distance * (float) Math.Cos(radian);
            base.IncreasePosition(dx,0,dz);
            this.upwardsSpeed += GRAVITY * frameTimeSec;
            base.IncreasePosition(0f, upwardsSpeed * frameTimeSec, 0);
            float terrainHeight = terrain.GetHeightOfTerrain(base.position.x, base.position.z);
            if (base.position.y < terrainHeight)
            {
                upwardsSpeed = 0;
                base.position.y = terrainHeight;
                isInAir = false;
            }
        }

        private void Jump()
        {
            if (isInAir == false)
            {
                this.upwardsSpeed = JUMP_POWER;
                isInAir = true;
            }
        }
        
        public void UpdateMove(InputManager input, Terrain terrain, float frameTimeSec)
        {
            if (input.IsKeyStatus(KeyStatus.KeyDown, Keys.W))
            {
                this.currentSpeed = SPEED;
            } 
            else if (input.IsKeyStatus(KeyStatus.KeyDown, Keys.S))
            {
                this.currentSpeed = -SPEED;
            }
            else {
                this.currentSpeed = 0f;
            }

            if (input.IsKeyStatus(KeyStatus.KeyDown, Keys.D))
            {
                this.currentTurnSpeed = -TURN_SPPED;
            }
            else if (input.IsKeyStatus(KeyStatus.KeyDown, Keys.A))
            {
                this.currentTurnSpeed = TURN_SPPED;
            }
            else
            {
                this.currentTurnSpeed =  0f;
            }

            if (input.IsKeyStatus(KeyStatus.KeyDown, Keys.Space))
            {
                Jump();
            }
            Move(terrain, frameTimeSec);
        }
    }
}