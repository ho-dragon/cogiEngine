using System;
using System.IO;
using System.Numerics;
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
        
        public static Matrix4x4f CreateTransformationMatrix(Vertex2f translation, Vertex2f scale)
        {
            Matrix4x4f matrix = new Matrix4x4f(IdentityMatrix4x4);
            matrix.Translate(translation.x, translation.y, 0);
            matrix.Scale(scale.x, scale.y, 1f);
            return matrix;
        }

        public static Matrix4x4f CreateProjectionMatrix(float fov, float aspectRatio, float nearPlane, float farPlane)
        {
            return Matrix4x4f.Perspective(fov, aspectRatio, nearPlane, farPlane);
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
        
        public static float BarryCentric(Vertex3f p1, Vertex3f p2, Vertex3f p3, Vertex2f pos) {
            float det = (p2.z - p3.z) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.z - p3.z);
            float l1 = ((p2.z - p3.z) * (pos.x - p3.x) + (p3.x - p2.x) * (pos.y - p3.z)) / det;
            float l2 = ((p3.z - p1.z) * (pos.x - p3.x) + (p1.x - p3.x) * (pos.y - p3.z)) / det;
            float l3 = 1.0f - l1 - l2;
            return l1 * p1.y + l2 * p2.y + l3 * p3.y;
        }
    }
}