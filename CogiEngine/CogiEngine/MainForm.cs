using System;
using System.Drawing;
using System.Windows.Forms;
using OpenGL;

namespace CogiEngine
{
    public class MainForm : Form
    {
        private GlControl glControl;
        private DisplayManager displayManager;
        private InputManager inputManager;
        private RawModel rowModel;
        private Loader loader;
        private Renderer renderer;
        private Camera camera;
        private Light lgiht;
        private StaticShader shader;
        private ModelTexture modelTexture;
        private TextureModel textureModel;
        private Entity entity;
        
        public MainForm()
        {
            InitializeComponent();
            KeyPreview = true;

        }

        public void InitializeComponent()
        {
            this.glControl = new GlControl();
            this.displayManager = new DisplayManager();
            this.inputManager = new InputManager();
            
            KeyDown += this.inputManager.OnKeyDown;
            KeyUp += this.inputManager.OnKeyUp;
            
            this.SuspendLayout();

            //glControl
            this.glControl.Animation = true;
            this.glControl.AnimationTimer = false;
            this.glControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.glControl.ColorBits = 24u;//Gl.COLOR_BUFFER_BIT;
            this.glControl.DepthBits = 24u;//Gl.DEPTH_BUFFER_BIT;
            this.glControl.Dock = DockStyle.Fill;
            this.glControl.Location = new Point(0, 0);
            this.glControl.MultisampleBits = 0u;
            this.glControl.Name = "glControl";
            this.glControl.Size = new System.Drawing.Size(1280, 720);
            this.glControl.StencilBits = 0u;//Gl.MULTISAMPLE_BIT;
            this.glControl.TabIndex = 0;

            this.glControl.ContextCreated += new EventHandler<GlControlEventArgs>(this.OnCreated_GlControl);
            this.glControl.ContextDestroying += new EventHandler<GlControlEventArgs>(this.OnDestroying_GlControl);
            this.glControl.Render += new EventHandler<GlControlEventArgs>(this.OnRender_GlControl);
            this.glControl.ContextUpdate += new EventHandler<GlControlEventArgs>(this.OnUpdate_GlControl);
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.Controls.Add(this.glControl);
            this.Name = "CogiEngine";
            this.Text = "[CogiEngine] Window";
            this.ResumeLayout(false);
        }

        private void OnCreated_GlControl(object sender, GlControlEventArgs e)
        {
            GlControl glControl = (GlControl)sender;
            this.displayManager.CreateDisplay(glControl);
            
            bool result = Soil.NET.WrapSOIL.Initialize();
            if (result == false)
            {
                MessageBox.Show("SOIL: Failed initialize : " + Soil.NET.WrapSOIL.GetSoilLastError());
                return;
            }
            
            this.loader = new Loader();
            this.shader = new StaticShader();
            this.renderer = new Renderer(this.shader, glControl.ClientSize.Width, glControl.ClientSize.Height);
            
            //Model
            //this.rowModel = OBJLoader.LoadObjModel("dragon", this.loader);
            this.rowModel = OBJLoader.LoadObjModelFromAssimp("dragon", this.loader);
            this.modelTexture = new ModelTexture(this.loader.LoadTexture("white2"));
            this.modelTexture.ShineDamper = 10f;
            this.modelTexture.Reflectivity = 1f;
            
            //Remove texture outline
            int value = (int)TextureMagFilter.Linear;
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, value);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, value);
            value = Gl.REPEAT;
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, value);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, value);
            
            this.textureModel = new TextureModel(this.rowModel, this.modelTexture);
            
            //Camera 
            this.camera = new Camera();
            this.lgiht = new Light(new Vertex3f(0,150,-50), new Vertex3f(1,1,1));
            this.inputManager.OnEventKeyDown += this.camera.OnEventKeyDown;
            
            //Entity
            this.entity = new Entity(textureModel, new Vertex3f(0, -3, -8), 0, 180, 0, 1);
                    
        
        }

        private void OnDestroying_GlControl(object sender, GlControlEventArgs e)
        {
            this.displayManager.CloseDisplay();
            this.loader.CleanUp();
            this.shader.CleanUp();
        }
        
        private void OnUpdate_GlControl(object sender, GlControlEventArgs e)
        {   
            this.entity.IncreaseRotation(0f,0.5f,0f);
            this.renderer.SetViewRect(glControl.ClientSize.Width, glControl.ClientSize.Height);
            this.camera.UpdateMove(this.inputManager);
        }
        
        private void OnRender_GlControl(object sender, GlControlEventArgs e)
        {
            Control senderControl = (Control) sender;
            Gl.Viewport(0, 0, senderControl.ClientSize.Width, senderControl.ClientSize.Height);
            this.renderer.Prepare();
            this.shader.Start();
            this.shader.LoadLight(this.lgiht);
            this.shader.LoadViewMatrix(camera);
            this.renderer.Render(this.entity, this.shader);
            this.shader.Stop();
            this.displayManager.UpdateDisplay();
        }
    }
}