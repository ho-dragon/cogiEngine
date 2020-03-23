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
        private RawModel rowModel;
        private Loader loader;
        private Renderer renderer;
        private Camera camera;
        private StaticShader shader;
        private ModelTexture modelTexture;
        private TextureModel textureModel;
        private Entity entity;
        
        float[] vertices = {			
            -0.5f,0.5f,-0.5f,	
            -0.5f,-0.5f,-0.5f,	
            0.5f,-0.5f,-0.5f,	
            0.5f,0.5f,-0.5f,		
				
            -0.5f,0.5f,0.5f,	
            -0.5f,-0.5f,0.5f,	
            0.5f,-0.5f,0.5f,	
            0.5f,0.5f,0.5f,
				
            0.5f,0.5f,-0.5f,	
            0.5f,-0.5f,-0.5f,	
            0.5f,-0.5f,0.5f,	
            0.5f,0.5f,0.5f,
				
            -0.5f,0.5f,-0.5f,	
            -0.5f,-0.5f,-0.5f,	
            -0.5f,-0.5f,0.5f,	
            -0.5f,0.5f,0.5f,
				
            -0.5f,0.5f,0.5f,
            -0.5f,0.5f,-0.5f,
            0.5f,0.5f,-0.5f,
            0.5f,0.5f,0.5f,
				
            -0.5f,-0.5f,0.5f,
            -0.5f,-0.5f,-0.5f,
            0.5f,-0.5f,-0.5f,
            0.5f,-0.5f,0.5f
        };

        float[] textureCoords = {
				
            0,0,
            0,1,
            1,1,
            1,0,			
            0,0,
            0,1,
            1,1,
            1,0,			
            0,0,
            0,1,
            1,1,
            1,0,
            0,0,
            0,1,
            1,1,
            1,0,
            0,0,
            0,1,
            1,1,
            1,0,
            0,0,
            0,1,
            1,1,
            1,0
        };


        int[] indices = {
            0,1,3,	
            3,1,2,	
            4,5,7,
            7,5,6,
            8,9,11,
            11,9,10,
            12,13,15,
            15,13,14,	
            16,17,19,
            19,17,18,
            20,21,23,
            23,21,22
        };
        
        public MainForm()
        {
            InitializeComponent();
            KeyPreview = true;
            KeyDown += MainForm_KeyDown;
        }

        public void InitializeComponent()
        {
            this.glControl = new GlControl();
            this.displayManager = new DisplayManager();
            
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

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.camera != null)
            {
                this.camera.Move(sender, e);
            }
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
            this.rowModel = loader.LoadToVAO(vertices,textureCoords, indices);
            this.modelTexture = new ModelTexture(this.loader.LoadTexture("image"));
            this.textureModel = new TextureModel(this.rowModel, this.modelTexture);
            
            //Camera 
            this.camera = new Camera();
            
            //Entity
            this.entity = new Entity(textureModel, new Vertex3f(0, 0, -5), 0, 0, 0, 1);
        }
        
        private void OnDestroying_GlControl(object sender, GlControlEventArgs e)
        {
            this.displayManager.CloseDisplay();
            this.loader.CleanUp();
            this.shader.CleanUp();
        }
        
        private void OnUpdate_GlControl(object sender, GlControlEventArgs e)
        {   
            this.entity.IncreaseRotation(1f,1f,0f);
            this.renderer.SetViewRect(glControl.ClientSize.Width, glControl.ClientSize.Height);
        }
        
        private void OnRender_GlControl(object sender, GlControlEventArgs e)
        {
            Control senderControl = (Control) sender;
            Gl.Viewport(0, 0, senderControl.ClientSize.Width, senderControl.ClientSize.Height);
            this.renderer.Prepare();
            this.shader.Start();
            this.shader.LoadViewMatrix(camera);
            this.renderer.Render(this.entity, this.shader);
            this.shader.Stop();
            this.displayManager.UpdateDisplay();
        }
    }
}