﻿using OpenGL;
//using SFML.Window;

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

        public void Move()
        {
            /*
            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                _position.z -= 0.02f;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                _position.z += 0.02f;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                _position.x += 0.02f;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                _position.x -= 0.02f;
            }*/
        }
    }
}
