using System;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Reflection;
using System.Threading;
using System.Runtime.Serialization;


namespace ConsoleApp7
{
    internal class Window : GameWindow
    {
        //public static Semaphore Sync = new Semaphore(3, 3);
        public Thread[] thread = new Thread[Nums];
        public Thread thread2;
        public Thread thread3;

        public static int AccuracyLim = 1;
        public static double EnergyLoss = 0.895d;
        public float k = (float)Math.Pow(EnergyLoss, (1 / (double)AccuracyLim));
        static float BoundingBox_Y = -6.5f;
        static int resolution = 10;
        static int Nums = resolution * resolution * resolution;
        uint Segments = 30;
        static float Radius = 0.3f;
        static float grid = Radius + 0.050f;
        Vector3[] Velocities = new Vector3[Nums];
        Vector3 Coordinates = (-2 * grid * resolution / 2, 0.8f * grid * resolution + 5f, -2 * grid * resolution / 2);
        Vector3 CoordinatesC = (0f, 12f, 0f);
        static float RadiusC = 0.75f;
        Sphere[] sphere = new Sphere[Nums];
        Sphere Collider;
        int offset = 0;
        Task[] task = new Task[Nums];

        private double _time;

        float[] Verticies;
        public uint[] Indicies;
        public int VBO;
        public int VAO;
        public int EBO;

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

            /* Verticies = Sphere.CreateSphere(Radius, Segments, (0f, 0f, 0f));
             Indicies = Sphere.CreatIndicies(Segments);*/

            for (uint i = 0; i < Nums; i++)
            {
                if (i % resolution == 0)
                {
                    Coordinates = (Coordinates.X + 2 * grid, Coordinates.Y - 2 * grid * resolution, Coordinates.Z);
                }
                if (i % (resolution * resolution) == 0 && i > 0)
                {
                    Coordinates = (Coordinates.X - 2 * grid * resolution, Coordinates.Y, Coordinates.Z + 2 * grid);
                }
                sphere[i] = new Sphere(Radius, Segments, Coordinates, offset);

                Coordinates = (Coordinates.X, Coordinates.Y + 2 * grid, Coordinates.Z);

                offset++;
            }
            Sphere Collider = new Sphere(RadiusC, Segments, CoordinatesC, offset);

            _timer = new Stopwatch();
            _timer.Start();

            _camera = new Camera(Vector3.UnitZ * 22, Size.X / (float)Size.Y);
            //_camera.Yaw = -90f;           

        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            _time = e.Time;
            //_time = (1.0d / 75.0d);
            double TimeValue = _timer.Elapsed.TotalSeconds;
            var model = Matrix4.Identity;

            view = _camera.GetViewMatrix();
            projection = _camera.GetProjectionMatrix();

            for (int i = 0; i < sphere.Length; i++)
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

            _time = e.Time;
            //_time = (1.0d / 75.0d);
            double FrameRate = 75;

            float FPS = 1.0f / (float)_time;
            _time = Math.Min(_time, 1.0 / FrameRate);
            Console.WriteLine(FPS);
            double dt = _time / AccuracyLim;          

            
            ProcessPhysics(dt);
            
            //Task.Run(()=>ProcessPhysicsAsync(dt));
            //Task.WaitAll();

            for (int i = 0; i < Nums; i++)
            {
                Matrix4 Translation = Matrix4.CreateTranslation(sphere[i].Position);
                sphere[i].Move(Translation);
                //NullVelocity(sphere[i]);
                sphere[i].SphereCollisionCount = 0;
                sphere[i].SphereColliders.Clear();
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

        bool IsCollided(Sphere sphere1, Sphere sphere2)
        {
            Vector3 Distance = sphere1.Position - sphere2.Position;
            if (Distance.Length < (2 * Radius))
            {
                return true;
            }
            return false;
        }


        //private static readonly SemaphoreSlim syncObj = new(1, 1);



        public void ProcessPhysics(double dt)
        {
            for (int r = 0; r < AccuracyLim; r++)
            {                
                GravityAndBoundingBox(dt);
                
                //Sphere2SphereCollisionDetermine();
                //Sphere2SphereCollisionCalculate();                
            }
        }

        //private async Task<bool> ProcessPhysicsAsync(double dt)
        //{
        //    try
        //    {
        //        await syncObj.WaitAsync();
        //        ProcessPhysics(dt);
        //    }
        //    finally
        //    {
        //        syncObj.Release();
        //    }
        //    return true;
        //}

        public void GravityAndBoundingBox(double dt)
        {
            for (int i = 0; i < Nums; i++)
            {                
                sphere[i].Velocity.Y = sphere[i].Velocity.Y - 9.81f * (float)dt;
                sphere[i].Delta.Y = sphere[i].Velocity.Y * (float)dt;
                sphere[i].Delta.X = sphere[i].Velocity.X * (float)dt;
                sphere[i].Delta.Z = sphere[i].Velocity.Z * (float)dt;

                int j = -1;
                for (j = 0 ; j < Nums - 1 ; j++) 
                {
                    thread[j] = new Thread(() =>
                    {                        
                        if (sphere[j].Position.Y + sphere[j].Delta.Y <= BoundingBox_Y)
                        {
                        if (sphere[j].Velocity.Y < 0)
                            sphere[j].Velocity = -k * sphere[j].Velocity;
                        }
                        else
                        {
                            sphere[j].Position += sphere[j].Delta;
                        }
                        Velocities[j] = sphere[j].Velocity;                        
                    });
                    thread[j].Start();
                    
                        
                }              
                             
                                                
            }
        }

        public void Sphere2SphereCollisionDetermine()
        {
            
                thread2 = new Thread(() =>
                {                    
                    for (int i = 0; i < Nums; i++)
                    {
                        for (int j = 0; j < Nums; j++)
                        {
                            if (j > i && IsCollided(sphere[i], sphere[j]))
                            {
                                sphere[i].SphereCollisionCount++;
                                sphere[j].SphereCollisionCount++;
                                sphere[i].SphereColliders.Add(j);
                                sphere[j].SphereColliders.Add(i);
                            }
                        }
                    }                   
                });
                thread2.Start();
           
            
        }

        public void Sphere2SphereCollisionCalculate()
        {
            
                thread3 = new Thread(() =>
                {
                    
                    for (int i = 0; i < Nums; i++)
                    {
                        if (sphere[i].SphereCollisionCount != 0)
                        {
                            var sphere2 = sphere[sphere[i].SphereColliders[0]];
                            sphere[i].Velocity = k * (Velocities[sphere[i].SphereColliders[0]]);
                            Vector3 Distance = sphere[i].Position - sphere2.Position;
                            float cos_x = Distance.X / (float)Math.Sqrt(Distance.X * Distance.X + Distance.Y * Distance.Y);
                            float sin_y = (float)Math.Sqrt(1 - cos_x * cos_x);
                            float cos_z = Distance.Z / (float)Math.Sqrt(Distance.Z * Distance.Z + Distance.Y * Distance.Y);
                            if (Distance.X > 0)
                            {
                                sphere[i].Position.X = sphere2.Position.X + 2 * Radius * cos_x;
                            }
                            else
                                sphere[i].Position.X = sphere2.Position.X - 2 * Radius * cos_x;
                            if (Distance.Y > 0)
                            {
                                sphere[i].Position.Y = sphere2.Position.Y + 2 * Radius * sin_y;
                            }
                            else
                                sphere[i].Position.Z = sphere2.Position.Y - 2 * Radius * sin_y;
                            if (Distance.Z > 0)
                            {
                                sphere[i].Position.Z = sphere2.Position.Z + 2 * Radius * cos_z;
                            }
                            else
                                sphere[i].Position.Z = sphere2.Position.Z - 2 * Radius * cos_z;
                        }
                        if (sphere[i].SphereCollisionCount == 2)
                        {
                            sphere[i].Velocity = Vector3.Zero;
                            sphere[i].Position -= sphere[i].Delta;
                        }
                    }
                    
                });
                thread3.Start();
            
                
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
            for (int i = 0; i < Nums; i++)
            {
                GL.DeleteBuffer(sphere[i].VBO);
                GL.DeleteVertexArray(sphere[i].VAO);
                GL.DeleteProgram(sphere[i]._shader.Handle);
            }


            base.OnUnload();
        }


    }
}
