using System;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Reflection;


namespace ConsoleApp7
{
    internal class Window : GameWindow
    {
        static int Segments = 32;
        private float[] _verticies = CreateN.SphereCreator(Segments, 0.45f);        

        private readonly uint[] _indices = CreateN.SpherePolygonsCreator(Segments);      
       
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _elementBufferObject;

        private double _time;

        private Matrix4 _projection; // 
        private Matrix4 _view; //

        private Stopwatch _timer;
        private Shader _shader;
        public Window(GameWindowSettings Settings, NativeWindowSettings nativeWindowSettings)
            : base(Settings, nativeWindowSettings)
        {
            /*foreach (var v in _indices)
            {
                Console.Write(v + "  ");
            }*/
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0f, 0f, 0.3f, 1f);
            GL.Enable(EnableCap.DepthTest);
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _verticies.Length * sizeof(float), _verticies, BufferUsageHint.StaticDraw);
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);            

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);            
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            _shader = new Shader("C:\\Users\\Ultrabook-13\\source\\repos\\ArhiST\\Fluid_sim_Alpha\\shader.vert",
                "C:\\Users\\Ultrabook-13\\source\\repos\\ArhiST\\Fluid_sim_Alpha\\shader.frag");
            _shader.Use();

            _timer = new Stopwatch();
            _timer.Start();

            _view = Matrix4.CreateTranslation(0f, 0f, -3f);
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100f);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _time = e.Time;
            double TimeValue = _timer.Elapsed.TotalSeconds;
            var transform = Matrix4.Identity;
            transform = transform * Matrix4.CreateRotationX((float)_time);
            //Vector3 Velocity = (0f, -0.981f *  (float)TimeValue, 0f);

            var model = Matrix4.Identity * Matrix4.CreateTranslation(0.05f, 0.05f*(float)TimeValue, 0f);
            //transform = transform * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(70f));
            //transform = transform * Matrix4.CreateTranslation(Velocity);

            //_shader.SetMatrix4("transform", transform);
            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", _view);
            _shader.SetMatrix4("projection", _projection);

            _shader.Use();                       
            float ColorValue = (float)Math.Sin(TimeValue);            
            float ColorValue3 = -(float)Math.Sin(TimeValue);
            int vertexColorLocation = GL.GetUniformLocation(_shader.Handle, "ourColor");
            GL.Uniform4(vertexColorLocation,1f, 0f, 0f, 1.0f);
            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);            
            SwapBuffers();
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            var input = KeyboardState;
            
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }
        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
            GL.DeleteProgram(_shader.Handle);
            base.OnUnload();
        }
    }
}
