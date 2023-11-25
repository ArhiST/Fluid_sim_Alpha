using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace ConsoleApp7
{
    public class CreateN
    {
        public static double[] CreateCircle(int n, double Radius)
        {
            int num = 3;
            double[] verticies = new double[(n+1)*3];
            double Rad = 2 * Math.PI / Convert.ToDouble(n);
            verticies[0] = 0d;
            verticies[1] = 0d;
            verticies[2] = 0d;            
            for (int i = 0; i < n; i++)
            {
                double Param = Rad * i;                
                double x = Radius * Math.Cos(Param);
                double y = Radius * Math.Sin(Param);
                verticies[num] = x;                
                num++;                
                verticies[num] = y;                
                num++;                
                verticies[num] = 0d;                               
                num++;                
            }

            return verticies;
        }

        public static uint[] CreatePoly(int n)
        {
            uint[] Polys = new uint[(n+1)*3];
            int num = 0;
            for(int i = 0; i < n; i++)
            {
                Polys[num] = 0;                
                num++;
                Polys[num] = Convert.ToUInt32(i + 1);                
                num++;
                Polys[num] = Convert.ToUInt32(i + 2);                
                num++;

            }
            Polys[num] = 0;
            num++;
            Polys[num] = Convert.ToUInt32(n);
            num++;
            Polys[num] = Convert.ToUInt32(1);
            num++;

            return Polys;
        }

        public static float[] SphereCreator(int n, float Radius, Vector3 Position)
        {
            float[] verticies = new float[6 + (n-2) * n * 3];
            int num = 3;    
            double Rad = 2 * Math.PI / n;
            verticies[0] = Position.X;
            verticies[1] = Position.Y;
            verticies[2] = Radius + Position.Z;
            for (int i = 1; i < n-1; i++)
            {
                double RotateAngle = Rad * i;
                double _Radius = Radius * Math.Abs(Math.Sin(RotateAngle));
                double z = Radius * Math.Cos(RotateAngle);
                float[] Circle = CreateCircleSphere(n, _Radius, z, Position);                

                for(int j = 0; j < Circle.Length; j++)
                {
                    verticies[num] = Circle[j];
                    num++;
                }
            }
            verticies[num] = Position.X;
            num++;
            verticies[num] = Position.Y;
            num++;
            verticies[num] = - Radius + Position.Z;
            num++;
            return verticies;
        }

        public static uint[] SpherePolygonsCreator(int n, uint offset)
        {
            List<uint> Polys = new List<uint>();
            //uint[] Poly = new uint[6 * (n-1) * n + 6];            
            for(int i = 0; i < n; i++)
            {
                if(i == 0)
                {
                    for(int j = 0; j < n; j++)
                    {
                        Polys.Add(offset);                        
                        Polys.Add(Convert.ToUInt32(j+1) + offset);
                        Polys.Add(Convert.ToUInt32(j + 2) + offset);                        
                        if (j == n-1)
                        {                            
                            Polys.Add(offset);                             
                            Polys.Add(Convert.ToUInt32(n) + offset);                            
                            Polys.Add(offset + 1);                            
                        }
                    }
                }
                if( i < n-1 && i > 0)
                {
                    for(int j = 0;j < n; j++)
                    {
                        Polys.Add(Convert.ToUInt32(1 + n * (i - 1) + j) + offset);                        
                        Polys.Add(Convert.ToUInt32(1 + n * i + j) + offset);                        
                        Polys.Add(Convert.ToUInt32(1 + n * (i - 1) + j + 1) + offset);                        
                        Polys.Add(Convert.ToUInt32(1 + n * (i - 1) + j + 1) + offset);                        
                        Polys.Add(Convert.ToUInt32(1 + n * i + j + 1) + offset);                        
                        Polys.Add(Convert.ToUInt32(1 + n * i + j) + offset);                        

                        if (j == n)
                        {
                            Polys.Add(Convert.ToUInt32(1 + n * (i - 1)) + offset);                            
                            Polys.Add(Convert.ToUInt32(1 + n * i) + offset);                            
                            Polys.Add(Convert.ToUInt32(1 + n * (i - 1) - 1) + offset);                            
                            Polys.Add(Convert.ToUInt32(1 + n * (i - 1) - 1) + offset);                            
                            Polys.Add(Convert.ToUInt32(1 + n * i - 1) + offset);                           
                            Polys.Add(Convert.ToUInt32(1 + n * i) + offset);                            
                        }
                    }                    
                }

                if ( i == n - 1 )
                {
                    for (int j = 0; j < n; j++)
                    {
                        Polys.Add(Convert.ToUInt32(1 + (n - 2) * i) + offset);                        
                        Polys.Add(Convert.ToUInt32(1 + (n - 2) * i) + offset);                        
                        Polys.Add(Convert.ToUInt32((i - 1) * n + j + 2) + offset);                        
                        if (j == n - 1)
                        {
                            Polys.Add(Convert.ToUInt32(1 + (n - 2) * i) + offset);                            
                            Polys.Add(Convert.ToUInt32((n - 2) * i) + offset);                            
                            Polys.Add(Convert.ToUInt32((i - 1) * n + j - 2) + offset);                            
                        }
                    }
                }
            }
            uint[] Poly = new uint[Polys.Count];
            for (int i = 0; i < Polys.Count; i++)
            {
                Poly[i] = Polys[i];
            }
            return Poly;
        }

        public static float[] CreateCircleSphere(int n, double  Radius, double z, Vector3 Position)
        {
            int num = 0;
            float[] verticies = new float[n * 3];
            double Rad = 2 * Math.PI / Convert.ToDouble(n);            
            for (int i = 0; i < n; i++)
            {
                double Param = Rad * i;
                double x = Radius * Math.Cos(Param);
                double y = Radius * Math.Sin(Param);
                verticies[num] = (float)x + Position.X;
                num++;
                verticies[num] = (float)y + Position.Y;
                num++;
                verticies[num] = (float)z + Position.Z;
                num++;
            }

            return verticies;
        }
    }
}
