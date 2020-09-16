using OpenGL;

namespace CogiEngine
{
    public class ShadowDepthFrameBuffer
    {
        public const int SHADOW_WIDTH = 1280;
        public const int SHADOW_HEIGHT = 720;
        
        private uint depthTexture;
        public uint DepthMap => depthTexture;

        private uint frameBuffer;
        
        public ShadowDepthFrameBuffer()
        {
            this.frameBuffer = CreateFrameBuffer();
            this.depthTexture = CreateDepthTextureAttachment(SHADOW_WIDTH, SHADOW_HEIGHT);
            UnbindCurrentFrameBuffer();
        }
        public void UnbindCurrentFrameBuffer()
        {
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
        
        public void BindFrameBuffer()
        {
            //Gl.BindTexture(TextureTarget.Texture2d, 0); //To make sure the texture isn't bound
            Gl.Viewport(0, 0, SHADOW_WIDTH, SHADOW_HEIGHT);
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, this.frameBuffer);
            Gl.Clear(ClearBufferMask.DepthBufferBit);
        }
        
        private uint CreateFrameBuffer()
        {
            uint frameBuffer = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            Gl.DrawBuffer(DrawBufferMode.None);
            Gl.ReadBuffer(0);
            return frameBuffer;
        }
        
        private uint CreateDepthTextureAttachment(int width, int height)
        {
            uint texture = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, texture);
            Gl.TexImage2D(TextureTarget.Texture2d
                , 0
                , InternalFormat.DepthComponent32
                , width
                , height
                , 0
                , PixelFormat.DepthComponent
                , PixelType.Float
                , null);

            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, Gl.LINEAR);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, Gl.LINEAR);
            Gl.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, texture, 0);
            return texture;
        }

        public void CleanUp()
        {
            Gl.DeleteFramebuffers(this.frameBuffer);
            Gl.DeleteTextures(this.depthTexture);
        }
    }   
}