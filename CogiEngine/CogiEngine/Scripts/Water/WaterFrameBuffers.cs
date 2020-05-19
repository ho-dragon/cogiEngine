using System;
using System.Net.Mail;
using OpenGL;

namespace CogiEngine.Water
{
    public class WaterFrameBuffers
    {
        public const int REFLECTION_WIDTH = 320;
        public const int REFLECTION_HEIGHT = 180;
        
        /*private const int REFLECTION_WIDTH = 1280;
        private const int REFLECTION_HEIGHT = 720;*/

        public const int REFRACTION_WIDTH = 1280;
        public const int REFRACTION_HEIGHT = 720;

        private uint reflectionFrameBuffer;
        private uint reflectionTexture;
        private uint reflectionDepthBuffer;

        private uint refractionFrameBuffer;
        private uint refractionTexture;
        private uint refractionDepthTexture;

        public uint ReflectionTexture => reflectionTexture;

        public uint RefractionTexture => refractionTexture;

        public uint RefractionDepthTexture => refractionDepthTexture;

        public WaterFrameBuffers(int width, int height)
        {
            //call when loading the game
            InitialiseReflectionFrameBuffer(width, height);
            InitialiseRefractionFrameBuffer(width, height);
        }
        
        private void InitialiseReflectionFrameBuffer(int width, int height)
        {
            this.reflectionFrameBuffer = CreateFrameBuffer();
            this.reflectionTexture = CreateTextureAttachment(REFLECTION_WIDTH, REFLECTION_HEIGHT);
            this.reflectionDepthBuffer = CreateDepthBufferAttachment(REFLECTION_WIDTH, REFLECTION_HEIGHT);
            UnbindCurrentFrameBuffer(width, height);
        }

        private void InitialiseRefractionFrameBuffer(int width, int height)
        {
            this.refractionFrameBuffer = CreateFrameBuffer();
            this.refractionTexture = CreateTextureAttachment(REFRACTION_WIDTH, REFRACTION_HEIGHT);
            this.refractionDepthTexture = CreateDepthTextureAttachment(REFRACTION_WIDTH, REFRACTION_HEIGHT);
            UnbindCurrentFrameBuffer(width, height);
        }

        public void CleanUp()
        {
            //call when closing the game
            //Reflection
            Gl.DeleteFramebuffers(this.reflectionFrameBuffer);
            Gl.DeleteTextures(this.reflectionTexture);
            Gl.DeleteRenderbuffers(this.reflectionDepthBuffer);
            
            //Refraction
            Gl.DeleteFramebuffers(this.refractionFrameBuffer);
            Gl.DeleteTextures(this.refractionTexture);
            Gl.DeleteTextures(this.refractionDepthTexture);
        }

        public void BindReflectionFrameBuffer()
        {
            //call before rendering to this FBO
            BindFrameBuffer(this.reflectionFrameBuffer, REFLECTION_WIDTH, REFLECTION_HEIGHT);
        }

        public void BindRefractionFrameBuffer()
        {
            //call before rendering to this FBO
            BindFrameBuffer(this.refractionFrameBuffer, REFRACTION_WIDTH, REFRACTION_HEIGHT);
        }

        public void UnbindCurrentFrameBuffer(int width, int height)
        {
            //call to switch to default frame buffer
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            Gl.Viewport(0, 0, width, height);
        }
        
        private void BindFrameBuffer(uint frameBuffer, int width, int height)
        {
            Gl.BindTexture(TextureTarget.Texture2d, 0); //To make sure the texture isn't bound
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            Gl.Viewport(0, 0, width, height);
        }

        private uint CreateFrameBuffer()
        {
            uint frameBuffer = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            Gl.DrawBuffer(DrawBufferMode.Aux0); //Gl.GL_COLOR_ATTACHMENT0
            //indicate that we will always render to color attachment 0
            return frameBuffer;
        }

        private uint CreateTextureAttachment(int width, int height)
        {
            uint texture = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, texture);
            Gl.TexImage2D(TextureTarget.Texture2d
                , 0
                , InternalFormat.Rgb
                , width
                , height
                , 0
                , PixelFormat.Rgb
                , PixelType.UnsignedByte
                , null);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, Gl.LINEAR);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, Gl.LINEAR);
            Gl.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, texture, 0);
            return texture;
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

        private uint CreateDepthBufferAttachment(int width, int height)
        {
            uint depthBuffer = Gl.GenRenderbuffer();
            Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
            Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.DepthComponent, width, height);
            Gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer
                , FramebufferAttachment.DepthAttachment
                , RenderbufferTarget.Renderbuffer
                , depthBuffer);
            return depthBuffer;
        }
    }
}