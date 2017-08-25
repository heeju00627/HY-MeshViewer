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
                properties[i] = float.Parse(tmp[3 + i]);
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
                indices[i] = Int32.Parse(tmp[i]);
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

            this.normal = normal;

            return norm;
        }
    }

    public class PickingRay
    {
        private Vector3D clickPosInWorld = new Vector3D();
        private Vector3D direction = new Vector3D();

        /* Compute the intersection of this ray with the X-Y Plane (where Z = 0)
         * and writes it back to the provided vector.
         */
        public void intersectionWithXyPlane(float[] worldPos)
        {
            double s = -clickPosInWorld.Z / direction.Z;
            worldPos[0] = (float)(clickPosInWorld.X + direction.X * s);
            worldPos[1] = (float)(clickPosInWorld.Y + direction.Y * s);
            worldPos[2] = 0;
        }

        public Vector3D getClickPosInWorld()
        {
            return clickPosInWorld;
        }

        public void setClickPosInWorld(Vector3D v)
        {
            clickPosInWorld = v;
        }

        public void addClickPosInWorld(Vector3D v)
        {
            clickPosInWorld += v;
        }

        public Vector3D getDireciton()
        {
            return direction;
        }

        public void setDirection(Vector3D v)
        {
            direction = v;
        }

        public void addDirection(Vector3D v)
        {
            direction += v;
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
        public Vector3D MouseInScreenPosition { get; set; }

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

        public PickingRay PickingRay { get; set; }
    }
}
