namespace CogiEngine
{
    public class ModelTexture
    {
        uint _textureID;
        public uint ID
        {
            get { return _textureID; }
        }

        public ModelTexture(uint id)
        {
            this._textureID = id;
        }
    }
}