using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static double[] SphereCreator(int n, double Radius)
        {
            double[] verticies = new double[n * n * 3];
            int num = 3;    
            double Rad = 2 * Math.PI / n;
            verticies[0] = 0;
            verticies[1] = 0;
            verticies[2] = Radius;
            for (int i = 1; i < n-1; i++)
            {
                double RotateAngle = Rad * i;
                double _Radius = Radius * Math.Abs(Math.Sin(RotateAngle));
                double z = Radius * Math.Cos(RotateAngle);
                double[] Circle = CreateCircleSphere(n, _Radius, z);                

                for(int j = 0; j < Circle.Length; j++)
                {
                    verticies[num] = Circle[j];
                    num++;
                }
            }
            verticies[num] = 0;
            num++;
            verticies[num] = 0;
            num++;
            verticies[num] = Radius;
            num++;
            return verticies;
        }

        public static uint[] SpherePolygonsCreator(int n)
        {
            uint[] Poly = new uint[6 * n * n];
            int num = 0;
            for(int i = 0; i < n; i++)
            {
                if(i == 0)
                {
                    for(int j = 0; j < n; j++)
                    {
                        Poly[num] = 0;
                        num++;
                        Poly[num] = Convert.ToUInt32(j+1);  
                        num++;
                        Poly[num] = Convert.ToUInt32(j + 2);
                        num++;
                        if (j == n-1)
                        {
                            Poly[num] = 0;                               
                            num++;
                            Poly[num] = Convert.ToUInt32(n);
                            num++;
                            Poly[num] = 1;
                            num++;
                        }
                    }
                }
                if( i < n-1 && i > 0)
                {
                    for(int j = 0;j < n; j++)
                    {
                        Poly[num] = Convert.ToUInt32(1 + n * (i-1)+j);
                        num++;
                        Poly[num] = Convert.ToUInt32(1 + n * i + j);
                        num++;
                        Poly[num] = Convert.ToUInt32(1 + n * (i - 1) + j + 1);
                        num++;
                        Poly[num] = Convert.ToUInt32(1 + n * (i - 1) + j + 1);
                        num++;
                        Poly[num] = Convert.ToUInt32(1 + n * i + j + 1);
                        num++;
                        Poly[num] = Convert.ToUInt32(1 + n * i + j);
                        num++;

                        if (j == n)
                        {
                            Poly[num] = Convert.ToUInt32(1 + n * (i - 1));
                            num++;
                            Poly[num] = Convert.ToUInt32(1 + n * i);
                            num++;
                            Poly[num] = Convert.ToUInt32(1 + n * (i - 1) - 1);
                            num++;
                            Poly[num] = Convert.ToUInt32(1 + n * (i - 1) - 1);
                            num++;
                            Poly[num] = Convert.ToUInt32(1 + n * i - 1);
                            num++;
                            Poly[num] = Convert.ToUInt32(1 + n * i);
                            num++;
                        }
                    }                    
                }

                if ( i == n - 1 )
                {
                    for (int j = 0; j < n; j++)
                    {
                        Poly[num] = Convert.ToUInt32(1 + (n-2) * i);
                        num++;
                        Poly[num] = Convert.ToUInt32(1 + (n-2) * i);
                        num++;
                        Poly[num] = Convert.ToUInt32((i - 1) * n + j + 2);
                        num++;
                        if (j == n - 1)
                        {
                            Poly[num] = Convert.ToUInt32(1 + (n - 2) * i);
                            num++;
                            Poly[num] = Convert.ToUInt32((n - 2) * i);
                            num++;
                            Poly[num] = Convert.ToUInt32((i - 1) * n + j -2);
                            num++;
                        }
                    }
                }
            }
            return Poly;
        }

        public static double[] CreateCircleSphere(int n, double Radius, double z)
        {
            int num = 0;
            double[] verticies = new double[n * 3];
            double Rad = 2 * Math.PI / Convert.ToDouble(n);            
            for (int i = 0; i < n; i++)
            {
                double Param = Rad * i;
                double x = Radius * Math.Cos(Param);
                double y = Radius * Math.Sin(Param);
                verticies[num] = x;
                num++;
                verticies[num] = y;
                num++;
                verticies[num] = z;
                num++;
            }

            return verticies;
        }
    }
}
