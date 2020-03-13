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
        float[] testVertices =  {
            -0.5f, 0.5f, 0f,
            -0.5f, -0.5f, 0f,
            0.5f, -0.5f, 0f,

            0.5f, -0.5f, 0f,
            0.5f, 0.5f, 0f,
            -0.5f, 0.5f, 0f
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
            
            loader = new Loader();
            rowModel = loader.loadToVAO(testVertices);
            renderer = new Renderer();
        }
        
        private void OnDestroying_GlControl(object sender, GlControlEventArgs e)
        {
            this.displayManager.CloseDisplay();
            loader.CleanUp();
        }
        
        private void OnUpdate_GlControl(object sender, GlControlEventArgs e)
        {
        }
        
        private void OnRender_GlControl(object sender, GlControlEventArgs e)
        {
            Control senderControl = (Control) sender;
            Gl.Viewport(0, 0, senderControl.ClientSize.Width, senderControl.ClientSize.Height);
            //Gl.ClearColor(0.0f, 0.0f, 1.0f, 1.0f);
            //Gl.Clear(ClearBufferMask.ColorBufferBit);
            renderer.Prepare();
            renderer.Render(rowModel);
            this.displayManager.UpdateDisplay();
        }
    }
}