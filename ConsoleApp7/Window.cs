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
        int Nums = 2;
        static int Segments = 3;
        static float Radius = 0.35f;
        uint offset = 0;
               
        List<float> _verticies = new List<float>();
        
        List<uint> _indicies = new List<uint>();
       
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _elementBufferObject;

        private double _time;
        float Velocity;
        float Y;
        float k = 1.05f;
                
        private Camera _camera;

        private Matrix4 projection;
        private Matrix4 view;

        private Stopwatch _timer;
        private Shader _shader;
        public Window(GameWindowSettings Settings, NativeWindowSettings nativeWindowSettings)
            : base(Settings, nativeWindowSettings)
        {
            /*foreach (var v in _verticies)
            {
                Console.Write(v + "  ");
            }*/
            for(int i = 0; i < Nums; i++)
            {
                Vector3 pos = (0f, i * 2f, 0f); 
                float[] _verts = CreateN.SphereCreator(Segments, Radius, pos);
                for(int j = 0; j < _verts.Length; j++)
                {
                    _verticies.Add(_verts[j]);
                }
                uint[] _inds = CreateN.SpherePolygonsCreator(Segments, offset);
                for(int j =0; j < _inds.Length; j++)
                {
                    Console.WriteLine(_inds[j] + "  " + i);    
                    _indicies.Add(_inds[j]);
                    offset = Convert.ToUInt16(_verts.Length / 3);
                }
            }
            
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0f, 0f, 0.3f, 1f);
            GL.Enable(EnableCap.DepthTest);

            float[] verts = new float[_verticies.Count];
            for (int i = 0; i < _verticies.Count; i++)
            {
                verts[i] = _verticies[i];
            }
            uint[] inds = new uint[_indicies.Count];
            for (int i = 0; i < _indicies.Count; i++)
            {
                inds[i] = _indicies[i];
            }

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, verts.Length * sizeof(float), verts, BufferUsageHint.StaticDraw);
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);            

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);            
            GL.BufferData(BufferTarget.ElementArrayBuffer, inds.Length * sizeof(uint), inds, BufferUsageHint.StaticDraw);

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

            uint[] inds = new uint[_indicies.Count];
            for (int i = 0; i < _indicies.Count; i++)       //нужен фикс памяти 
            {
                inds[i] = _indicies[i];
            }

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
            GL.DrawElements(PrimitiveType.Triangles, inds.Length, DrawElementsType.UnsignedInt, 0);            
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
