using OpenGL;

namespace CogiEngine
{
    public class Entity
    {
        TextureModel textureModel;
        protected Vertex3f position;
        protected float rotationX;
        protected float rotationY;
        protected float rotationZ;
        protected float scale;
        private int textureIndex = 0;
        
        public TextureModel TextureModel => textureModel;

        public Vertex3f Position => position;

        public float RotationX => rotationX;

        public float RotationY => rotationY;

        public float RotationZ => rotationZ;

        public float Scale => scale;

        public Entity(TextureModel textureModel, Vertex3f position, float rotationX, float rotationY, float rotationZ, float scale)
        {
            this.textureModel = textureModel;
            this.position = position;
            this.rotationX = rotationX;
            this.rotationY = rotationY;
            this.rotationZ = rotationZ;
            this.scale = scale;
        }

        public Entity(TextureModel textureModel, int textureIndex, Vertex3f position, float rotationX, float rotationY, float rotationZ, float scale)
        {
            this.textureIndex = textureIndex;
            this.textureModel = textureModel;
            this.position = position;
            this.rotationX = rotationX;
            this.rotationY = rotationY;
            this.rotationZ = rotationZ;
            this.scale = scale;
        }

        public void IncreasePosition(float dx, float dy, float dz)
        {
            this.position.x += dx;
            this.position.y += dy;
            this.position.z += dz;
        }

        public void IncreaseRotation(float dx, float dy, float dz)
        {
            this.rotationX += dx;
            this.rotationY += dy;
            this.rotationZ += dz;
        }

        public float GetTextureOffsetX()
        {
            int column = this.textureIndex % this.textureModel.Texture.NumberOfRows;
            return (float) column / (float) this.textureModel.Texture.NumberOfRows;
        }
        
        public float GetTextureOffsetY()
        {
            int row = this.textureIndex / this.textureModel.Texture.NumberOfRows;
            return (float) row / (float) this.textureModel.Texture.NumberOfRows;
        }
    }
}
