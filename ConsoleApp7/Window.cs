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
        static int Segments = 10;
        static Vector3 pos = (0f, 1f, 2f);
        private float[] _verticies = CreateN.SphereCreator(Segments, 0.5f, pos);       
        
        private readonly uint[] _indices = CreateN.SpherePolygonsCreator(Segments);      
       
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _elementBufferObject;

        private double _time;
        float Velocity;
        float Y;
        float k = 0.8f;
                
        private Camera _camera;

        private Matrix4 projection;
        private Matrix4 view;

        private Stopwatch _timer;
        private Shader _shader;
        public Window(GameWindowSettings Settings, NativeWindowSettings nativeWindowSettings)
            : base(Settings, nativeWindowSettings)
        {
            foreach (var v in _verticies)
            {
                Console.Write(v + "  ");
            }
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

            _shader = new Shader("C:\\Users\\Stepan\\source\\repos\\ConsoleApp7\\shader.vert",
                "C:\\Users\\Stepan\\source\\repos\\ConsoleApp7\\shader.frag");
            _shader.Use();

            _timer = new Stopwatch();
            _timer.Start();

            _camera = new Camera(Vector3.UnitZ * 6, Size.X / (float)Size.Y);
                       
            
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _time = e.Time;
            double TimeValue = _timer.Elapsed.TotalSeconds;
            var transform = Matrix4.Identity;
            transform = transform * Matrix4.CreateRotationX((float)_time);           
            Velocity = Velocity - 9.81f * (float)_time;            
            float Delta_Y = Velocity * (float)_time;                        
            Y += Delta_Y;            
            if(Y < -4.6f)                  //как фиксить лаг ебаный 
            {
                Velocity = -Velocity * k;
            }
            view = _camera.GetViewMatrix();
            projection = _camera.GetProjectionMatrix();
            var model = Matrix4.Identity * Matrix4.CreateTranslation(0f, Y, 0f);
            
            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);

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
            if (!IsFocused) // Check to see if the window is focused
            {
                return;
            }
            var input = KeyboardState;
            
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
            _camera.AspectRatio = Size.X / (float)Size.Y;
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
