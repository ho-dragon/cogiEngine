using System;
using OpenGL;

namespace CogiEngine
{
    public class Maths
    {
        static Matrix4x4f _identityMatrix4x4;
        public static Matrix4x4f IdentityMatrix4x4
        {
            get { return _identityMatrix4x4; }
        }

        static Maths()
        {
            _identityMatrix4x4 = new Matrix4x4f(Matrix4x4f.Identity);
        }

        public static Matrix4x4f CreateTransformationMatrix(Vertex3f translation, float rx, float ry, float rz, float scale)
        {
            Matrix4x4f matrix = new Matrix4x4f(IdentityMatrix4x4);
            matrix.Translate(translation.x, translation.y, translation.z);
            matrix.RotateX(rx);
            matrix.RotateY(ry);
            matrix.RotateY(rz);
            matrix.Scale(scale, scale, scale);
            return matrix;
        }

        public static Matrix4x4f CreateViewMatrix(Camera camera)
        {
            Matrix4x4f matrix = new Matrix4x4f(IdentityMatrix4x4);
            matrix.RotateX(camera.Pitch);
            matrix.RotateY(camera.Yaw);
            Vertex3f cameraPos = camera.Position;
            matrix.Translate(-cameraPos.x, -cameraPos.y, -cameraPos.z);
            return matrix;
        }

        public static float DegreeToRadian(float angle)
        {
            return (float)(Math.PI / 180) * angle;
        }

        public static float RadianToDegree(float angle)
        {
            return (float)(angle * (180.0 / Math.PI));
        }
    }
}