namespace CogiEngine
{
    public class RawModel
    {
        private uint vaoID;
        private int vertexCount;
        
        public uint VaoID { get { return vaoID; } }
        public int VertexCount { get { return vertexCount; } }

        public RawModel(uint vaoID, int vertexCount)
        {
            this.vaoID = vaoID;
            this.vertexCount = vertexCount;
        }
    }
}