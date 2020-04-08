using OpenGL;

namespace CogiEngine
{
    public class Light
    {
        Vertex3f _position;
        public Vertex3f Position
        {
            get { return _position; }
            set { _position = value; }
        }

        Vertex3f _colour;
        public Vertex3f Colour
        {
            get { return _colour; }
            set { _colour = value; }
        }

        public Light(Vertex3f position, Vertex3f colour)
        {
            this._position = position;
            this._colour = colour;
        }
    }
}