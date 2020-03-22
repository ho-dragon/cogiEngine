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
        private StaticShader shader;
        private ModelTexture modelTexture;
        private TextureModel textureModel;
        private Entity entity;
        
        private float[] vertices =
        {
            -0.5f, 0.5f, 0f,   // V0
            -0.5f, -0.5f, 0f,  // V1
            0.5f, -0.5f, 0f,   // V2
            0.5f, 0.5f, 0f     // V3
        };
        
        private int[] indices =
        {
            0, 1, 3,  // Top left triangle (V0, V1, V3)
            3, 1, 2   // Bottom right triangle (V3, V1, V2)
        };
        
        float[] textureCoords =
        {
            0,0, // V0
            0,1, // V1
            1,1, // V2
            1,0, // V3
        };
        
        public MainForm()
        {
            InitializeComponent();
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
            this.glControl.DepthBits = 0u;//Gl.DEPTH_BUFFER_BIT;
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
            this.renderer = new Renderer();
            this.rowModel = loader.LoadToVAO(vertices,textureCoords, indices);
            this.modelTexture = new ModelTexture(this.loader.LoadTexture("image_2"));
            this.textureModel = new TextureModel(this.rowModel, this.modelTexture);
            this.shader = new StaticShader();
            this.entity = new Entity(textureModel, new Vertex3f(-1, 0, 0), 0, 0, 0, 1);
        }
        
        private void OnDestroying_GlControl(object sender, GlControlEventArgs e)
        {
            this.displayManager.CloseDisplay();
            this.loader.CleanUp();
            this.shader.CleanUp();
        }
        
        private void OnUpdate_GlControl(object sender, GlControlEventArgs e)
        {   
            this.entity.IncreasePosition(0.002f, 0f, 0f);
            this.entity.IncreaseRotation(0, 1, 0);
        }
        
        private void OnRender_GlControl(object sender, GlControlEventArgs e)
        {
            Control senderControl = (Control) sender;
            Gl.Viewport(0, 0, senderControl.ClientSize.Width, senderControl.ClientSize.Height);
            this.renderer.Prepare();
            this.shader.Start();
            this.renderer.Render(this.entity, this.shader);
            this.shader.Stop();
            this.displayManager.UpdateDisplay();
        }
    }
}