using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.ComponentModel;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Shaders;
using System.Windows.Media.Media3D;
using System.Windows.Controls;

namespace HY_MeshViewer.Model
{
    /* node 정보 */
    public struct Node
    {
        // id는 dictionary로 관리
        Vertex position;
        List<int> confaces;
        float[] properties;
        Vector3D normal;

        /** 생성자 */
        public Node(String[] tmp, int n_property)
        {
            position = new Vertex(float.Parse(tmp[0]), float.Parse(tmp[1]), float.Parse(tmp[2]));

            confaces = new List<int>();

            properties = new float[n_property];
            for (int i = 0; i < n_property; i++)
            {
                properties[i] = float.Parse(tmp[3 + i]) / (float)255.0;
            }

            normal = new Vector3D(0, 0, 0);
        }

        public Vertex getPosition()
        {
            return position;
        }

        public List<int> getConfaces()
        {
            return confaces;
        }

        public void setConfaces(int t)
        {
            confaces.Add(t);
        }

        public float[] getProperties()
        {
            return properties;
        }

        public Vector3D getNormal()
        {
            return normal;
        }

        public void setNormal(Vector3D n)
        {
            normal = n;
        }

        public double Normalize(Vector3D normal)
        {
            double norm = Math.Sqrt(Math.Pow(normal.X, 2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.Z, 2));
            
            if (norm != 0)
            {
                normal.X /= norm;
                normal.Y /= norm;
                normal.Z /= norm;
            }

            this.normal = normal;

            return norm;
        }
    }

    /* triangle 정보 */
    public struct Triangle
    {
        int[] indices;
        Vector3D normal;
        double area;

        public Triangle(String[] tmp, int n_index)
        {
            indices = new int[n_index];
            for (int i = 0; i < n_index; i++)
            {
                indices[i] = Int32.Parse(tmp[n_index - i -1]);
            }

            normal = new Vector3D(0, 0, 0);
            area = 0;
        }

        public int[] getIndices()
        {
            return indices;
        }

        public double getArea()
        {
            return area;
        }

        public Vector3D getNormal()
        {
            return normal;
        }

        public void setNormal(Vector3D n)
        {
            normal = n;
            area = 0.5 * Normalize(n);
        }

        public double Normalize(Vector3D normal)
        {
            double norm = Math.Sqrt(Math.Pow(normal.X, 2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.Z, 2));
            if (norm != 0)
            {
                normal.X /= norm;
                normal.Y /= norm;
                normal.Z /= norm;
            }

            //this.normal = normal;

            return norm;
        }
    }

    public struct Pyramid
    {
        int[] indices;
        Vector3D normal;
        double area;

        public Pyramid(String[] tmp, int n_index)
        {
            indices = new int[n_index];
            for (int i = 0; i < n_index; i++)
            {
                indices[i] = Int32.Parse(tmp[n_index - i - 1]);
            }

            normal = new Vector3D(0, 0, 0);
            area = 0;
        }
    }

    public struct Ray
    {
        Vector3D near;
        Vector3D far;
        Vector3D direction;

        float minR;
        Vector3D hitPos;
        int hitFace;

        public Ray(int hitFace)
        {
            this.near = new Vector3D(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity); ;
            this.far = new Vector3D(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity); ;
            this.direction = new Vector3D(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity); ;

            this.minR = float.MaxValue;
            this.hitPos = new Vector3D(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
            this.hitFace = -1;
        }

        public Ray(Vector3D near, Vector3D far, Vector3D direction)
        {
            this.near = near;
            this.far = far;
            this.direction = direction;

            this.minR = float.MaxValue;
            this.hitPos = new Vector3D(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
            this.hitFace = -1;
        }

        public Vector3D getNear()
        {
            return near;
        }

        public Vector3D getFar()
        {
            return far;
        }

        public Vector3D getDirection()
        {
            return direction;
        }

        public float getMinR()
        {
            return minR;
        }

        public void setMinR(float f)
        {
            minR = f;
        }

        public Vector3D getHitPos()
        {
            return hitPos;
        }

        public void setHitPos(Vector3D v)
        {
            hitPos = v;
        }

        public int getHitFace()
        {
            return hitFace;
        }

        public void setHitFace(int f)
        {
            hitFace = f;
        }
    }

    /* mesh 정보 */
    public class Mesh
    {
        // 데이터 파일
        public string FileName { get; set; }

        // 데이터 관리
        public Dictionary<int, Node> Nodes
        {
            get;
            set; }

        public Dictionary<int, Triangle> Triangles
        {
            get;
            set; }

        // 마우스 정보
        public Point MousePosition { get; set; }
        public Ray MouseRay { get; set; }

        // mesh 정보
        public int N_node { get; set; }
        public int N_triangle { get; set; }
        public int N_property { get; set; }
        public int N_index { get; set; }

        public float RotationAngle { get; set; }
        public Vector3D RotationAxis { get; set; }

        public float TranslationX { get; set; }
        public float TranslationY { get; set; }
        public float TranslationZ { get; set; }

        public double Scale { get; set; }

        public List<ComboBoxItem> Properties { get; set; }
    }
}
