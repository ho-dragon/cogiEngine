using OpenGL;

namespace CogiEngine
{
    public class GUITexture
    {
        private uint textureID;
        private Vertex2f position;
        private Vertex2f scale;

        public uint TextureID => textureID;

        public Vertex2f Position => position;

        public Vertex2f Scale => scale;

        public GUITexture(uint textureId, Vertex2f position, Vertex2f scale)
        {
            textureID = textureId;
            this.position = position;
            this.scale = scale;
        }
    }
}