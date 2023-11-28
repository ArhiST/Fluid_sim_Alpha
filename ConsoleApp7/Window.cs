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
        static float BoundingBox_Y = -6.5f;
        static int resolution = 13;
        static int Nums = resolution * resolution * resolution;
        uint Segments = 20;
        static float Radius = 0.3f;
        static float grid = Radius + 0.025f;
        Vector3 Coordinates = (-2 * grid * resolution / 2, 0.8f * grid * resolution + 4f, -2 * grid * resolution / 2);

        Sphere[] sphere = new Sphere[Nums];
        int offset = 0;           
        
        private double _time;
        
                
        private Camera _camera;

        private Matrix4 projection;
        private Matrix4 view;

        private Stopwatch _timer;
        private Shader _shader;

        public Window(GameWindowSettings Settings, NativeWindowSettings nativeWindowSettings)
            : base(Settings, nativeWindowSettings)
        {
            VSync = VSyncMode.On;          
            
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0f, 0f, 0.3f, 1f);
            GL.Enable(EnableCap.DepthTest);



            for (uint i = 0; i < Nums; i++)
            {
                if (i % resolution == 0)
                {
                    Coordinates = (Coordinates.X + 2 * grid, Coordinates.Y - 2 * grid * resolution, Coordinates.Z);
                }
                if(i % (resolution * resolution) == 0 && i > 0)
                {
                    Coordinates = (Coordinates.X - 2 * grid * resolution, Coordinates.Y , Coordinates.Z + 2 * grid);
                }
                sphere[i] = new Sphere(Radius, Segments, Coordinates, offset);
                
                Coordinates = (Coordinates.X, Coordinates.Y + 2 * grid, Coordinates.Z);
                
                offset++;
            }           

            

            _timer = new Stopwatch();
            _timer.Start();

            _camera = new Camera(Vector3.UnitZ * 20, Size.X / (float)Size.Y);
            //_camera.Yaw = -90f;           
            
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);                      

            _time = e.Time;
            double TimeValue = _timer.Elapsed.TotalSeconds;
            var model = Matrix4.Identity;                       
            
            view = _camera.GetViewMatrix();
            projection = _camera.GetProjectionMatrix();
                       
            for(int i = 0; i < sphere.Length; i++)
            {
                int vertexColorLocation = GL.GetUniformLocation(sphere[i]._shader.Handle, "ourColor");
                GL.Uniform4(vertexColorLocation, 1f, 0f, 0f, 1.0f);
                GL.BindVertexArray(sphere[i].VAO);
                GL.DrawElements(PrimitiveType.Triangles, sphere[i].Indicies.Length, DrawElementsType.UnsignedInt, 0);
                sphere[i]._shader.SetMatrix4("model", model);
                sphere[i]._shader.SetMatrix4("view", view);
                sphere[i]._shader.SetMatrix4("projection", projection);

                sphere[i]._shader.Use();
                
            }
            SwapBuffers();


        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            //double time;
            _time = e.Time;            
            double FrameRate = 60.0;
            /*if (_time < (1/ 50))
            {
                _time = (1 / 60);
            }*/
            float FPS = 1.0f / (float)_time;
            _time = Math.Min(_time, 1.0 / FrameRate);            
            Console.WriteLine(FPS);
            
            for(int i = 0; i < Nums; i++)
            {

                sphere[i].Velocity_Y = sphere[i].Velocity_Y - 9.81f * (float)_time;
                float Delta_Y = sphere[i].Velocity_Y * (float)_time;
                if (sphere[i].Position.Y < BoundingBox_Y)
                {   
                    Delta_Y = -Delta_Y;
                    sphere[i].Velocity_Y = -sphere[i].Velocity_Y * sphere[i].k;
                    if(sphere[i].Velocity_Y < 0.25f)
                    {
                        sphere[i].Velocity_Y = 0;
                    }
                }
                
                float Delta_X = sphere[i].Velocity_X * (float)_time;
                float Delta_Z = sphere[i].Velocity_Z * (float)_time;
                sphere[i].Position.X += Delta_X;
                sphere[i].Position.Y += Delta_Y;
                sphere[i].Position.Z += Delta_Z;

                //Vector3 Translation = (Delta_X, Delta_Y, Delta_Z);
                Matrix4 Translation = Matrix4.CreateTranslation(sphere[i].Position.X, sphere[i].Position.Y, sphere[i].Position.Z);

                sphere[i].Move(Translation);
            }
            

            if (!IsFocused) // Check to see if the window is focused
            {
                return;
            }
            var input = KeyboardState;
            /*float cameraSpeed = 3f;             
            if (input.IsKeyPressed(Keys.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time;
            }*/
            
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
            for(int i = 0; i < 2; i++)
            {
                GL.DeleteBuffer(sphere[i].VBO);
                GL.DeleteVertexArray(sphere[i].VAO);
                GL.DeleteProgram(sphere[i]._shader.Handle);
            }
            
            
            base.OnUnload();
        }
    }
}
