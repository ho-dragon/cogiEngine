using OpenGL;

namespace CogiEngine
{
    public class Light
    {
        Vertex3f position;
        Vertex3f color;
        Vertex3f attenuation = new Vertex3f(1, 0, 0);
        
        public Vertex3f Position => position;

        public Vertex3f Color => color;

        public Vertex3f Attenuation => attenuation;
        
        public Light(Vertex3f position, Vertex3f color, Vertex3f attenuation)
        {
            this.position = position;
            this.color = color;
            this.attenuation = attenuation;
        }
    }
}