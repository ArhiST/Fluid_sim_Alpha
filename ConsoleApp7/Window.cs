using System;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace ConsoleApp7
{
    internal class Window : GameWindow
    {
        static int Segments = 3;
        private double[] _verticies = CreateN.SphereCreator(Segments, 0.5d);        

        private readonly uint[] _indices = CreateN.SpherePolygonsCreator(Segments);      
       
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _elementBufferObject;
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
            GL.ClearColor(0f, 0f, 0.2f, 0f);
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _verticies.Length * sizeof(double), _verticies, BufferUsageHint.StaticDraw);
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Double, false, 3 * sizeof(double), 0);
            GL.EnableVertexAttribArray(0);            

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);            
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            _shader = new Shader("C:\\Users\\Stepan\\source\\repos\\ConsoleApp7\\shader.vert", "C:\\Users\\Stepan\\source\\repos\\ConsoleApp7\\shader.frag");
            _shader.Use();

            _timer = new Stopwatch();
            _timer.Start();
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            var transform = Matrix4.Identity;

            //transform = transform * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(70f));

            _shader.SetMatrix4("transform", transform);

            _shader.Use();
            double TimeValue = _timer.Elapsed.TotalSeconds;            
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
