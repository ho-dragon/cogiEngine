namespace CogiEngine
{
    public class TextureModel
    {
        RawModel _rawModel;
        public RawModel RawModel
        {
            get { return _rawModel; }
        }

        ModelTexture _texture;
        public ModelTexture Texture
        {
            get { return _texture; }
        }

        public TextureModel(RawModel model, ModelTexture texture)
        {
            this._rawModel = model;
            this._texture = texture;
        }
    }
}