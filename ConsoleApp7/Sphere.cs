using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System.Reflection;

namespace ConsoleApp7
{
    public class Sphere
    {
        float[] Verticies;
        public uint[] Indicies;        
        public Vector3 Position;
        public Shader _shader;
        public Vector3 Velocity = (0f,0f,0f);
        //public bool SphereStop = false;

        public float Velocity_X;
        public float Velocity_Y;
        public float Velocity_Z;
        //public float k = 0.75f;        
        
        public int VBO;
        public int VAO;
        public int EBO;

        public Sphere(float Radius, uint Segments, Vector3 coordinates, int offset)
        {
            //Coordinates = coordinates;
            Position = coordinates;
            
            Verticies = CreateSphere(Radius, Segments, (0f, 0f, 0f));
            Indicies = CreatIndicies(Segments);           


            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            
            GL.BufferData(BufferTarget.ArrayBuffer, Verticies.Length * sizeof(float), Verticies, BufferUsageHint.DynamicDraw);

            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indicies.Length * sizeof(uint), Indicies, BufferUsageHint.DynamicDraw);

            _shader = new Shader("C:\\Users\\Stepan\\source\\repos\\ConsoleApp7\\shader.vert",
                "C:\\Users\\Stepan\\source\\repos\\ConsoleApp7\\shader.frag");
            _shader.Use();
        }

        public void Move(Matrix4 Translation)
        {                       
            _shader.SetMatrix4("translation", Translation);            
        }
                

        public float[] CreateSphere(float Radius, uint Segments, Vector3 Coordinates)
        {
            List<float> _verticies = new List<float>();
            double Rad = 2 * Math.PI / Segments;
            _verticies.Add(Coordinates.X);
            _verticies.Add(Coordinates.Y);
            _verticies.Add(Coordinates.Z + Radius);
            _verticies.Add(1.0f);
            for (int i = 1; i < Segments - 1; i++)
            {
                double RotateAngle = Rad * i;
                double _Radius = Radius * Math.Abs(Math.Sin(RotateAngle));
                double z = Radius * Math.Cos(RotateAngle);
                float[] Circle = CreateCircleSphere(Segments, _Radius, z, Coordinates);

                for (int j = 0; j < Circle.Length; j++)
                {
                    _verticies.Add(Circle[j]);
                }
            }
            _verticies.Add(Coordinates.X);
            _verticies.Add(Coordinates.Y);
            _verticies.Add(Coordinates.Z - Radius);
            _verticies.Add(1.0f);
            float[] verticies = _verticies.ToArray();
            return verticies;
        }

        public float[] CreateCircleSphere(uint Segments, double Radius, double z, Vector3 Position)
        {            
            List<float> _verticies = new List<float>();
            double Rad = 2 * Math.PI / Segments;
            for (int i = 0; i < Segments; i++)
            {
                double Param = Rad * i;
                double x = Radius * Math.Cos(Param);
                double y = Radius * Math.Sin(Param);
                _verticies.Add((float)x + Position.X);
                _verticies.Add((float)y + Position.Y);
                _verticies.Add((float)z + Position.Z);
                _verticies.Add(1.0f);
            }
            float[] verticies = _verticies.ToArray();

            return verticies;
        }

        public uint[] CreatIndicies(uint Segments)
        {
            List<uint> _indicies = new List<uint>();
            for (uint i = 0; i < Segments; i++)
            {
                if (i == 0)
                {
                    for(uint j = 0; j < Segments - 1; j++)
                    {
                        _indicies.Add(0);
                        _indicies.Add(j + 1);
                        _indicies.Add(j + 2);
                    }
                    _indicies.Add(0);
                    _indicies.Add(Segments);
                    _indicies.Add(Segments - 1);                    
                }
                if(i > 0 && i < Segments - 1)
                {
                    for (uint j = 0; j < Segments - 1; j++)
                    {
                        _indicies.Add(1 + j + (i-1) * Segments);
                        _indicies.Add(1 + j + 1 + (i - 1) * Segments);
                        _indicies.Add(1 + j + i * Segments);
                        _indicies.Add(1 + j + i * Segments);
                        _indicies.Add(1 + j + 1 + (i - 1) * Segments);
                        _indicies.Add(1 + 1 + j + i * Segments);
                    }
                    _indicies.Add(1 + (i - 1) * Segments);
                    _indicies.Add(Segments + (i - 1) * Segments);
                    _indicies.Add(1 + i * Segments);
                    _indicies.Add(1 + i * Segments);
                    _indicies.Add(Segments + (i - 1) * Segments);
                    _indicies.Add(Segments + i * Segments);

                }
                if(i == Segments - 1)
                {
                    uint num = Segments * (Segments - 2) + 2;
                    for (uint j = 0; j < Segments-1; j++)
                    {
                        _indicies.Add(num);
                        _indicies.Add(num - (j + 1));
                        _indicies.Add(num - (j + 2));
                    }
                    _indicies.Add(0);
                    _indicies.Add(Segments);
                    _indicies.Add(Segments - 1);
                }
            }
            uint[] indicies = _indicies.ToArray();
            return indicies;
        }
    }
}
