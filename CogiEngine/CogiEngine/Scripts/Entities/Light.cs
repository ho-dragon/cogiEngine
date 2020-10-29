using OpenGL;

namespace CogiEngine
{

    public class DirectionalLight : Light
    {
        public Vertex3f Direction { get; }
        public DirectionalLight(Vertex3f direction, Vertex3f position, Vertex3f color, Vertex3f attenuation) : base(position, color, attenuation)
        {
            Direction = direction;
        }
    }
    
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

        public void SetPosition(float x, float y, float z)
        {
            this.position.x = x;
            this.position.y = y;
            this.position.z = z;
        }
    }
}