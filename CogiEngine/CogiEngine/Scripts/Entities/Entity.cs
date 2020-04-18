using OpenGL;

namespace CogiEngine
{
    public class Entity
    {
        TextureModel _model;

        public TextureModel Model => _model;

        public Vertex3f Position => _position;

        public float RotX => _rotX;

        public float RotY => _rotY;

        public float RotZ => _rotZ;

        public float Scale => _scale;

        protected Vertex3f _position;
        protected float _rotX;
        protected float _rotY;
        protected float _rotZ;
        protected float _scale;
  
        public Entity(TextureModel model, Vertex3f position, float rotX, float rotY, float rotZ, float scale)
        {
            _model = model;
            _position = position;
            _rotX = rotX;
            _rotY = rotY;
            _rotZ = rotZ;
            _scale = scale;
        }


        public void IncreasePosition(float dx, float dy, float dz)
        {
            _position.x += dx;
            _position.y += dy;
            _position.z += dz;
        }

        public void IncreaseRotation(float dx, float dy, float dz)
        {
            _rotX += dx;
            _rotY += dy;
            _rotZ += dz;
        }
    }
}
