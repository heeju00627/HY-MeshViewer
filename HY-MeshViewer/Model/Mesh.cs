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
        List<int> coneles;
        float[] properties;
        Vector3D normal;

        // 생성자
        public Node(String[] tmp, int n_property)
        {
            position = new Vertex(float.Parse(tmp[0]), float.Parse(tmp[1]), float.Parse(tmp[2]));

            confaces = new List<int>();
            coneles = new List<int>();

            properties = new float[n_property];
            for (int i = 0; i < n_property; i++)
            {
                properties[i] = float.Parse(tmp[3 + i]) / (float)255.0;
            }

            normal = new Vector3D(0, 0, 0);
        }

        public Node(float[] tmp, int n_property)
        {
            position = new Vertex(tmp[0], tmp[1], tmp[2]);

            confaces = new List<int>();
            coneles = new List<int>();

            properties = new float[n_property];

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

        public List<int> getConeles()
        {
            return coneles;
        }

        public void setConeles(int t)
        {
            coneles.Add(t);
        }

        public float[] getProperties()
        {
            return properties;
        }

        public void setProperties(float a, float b, float c)
        {
            properties[0] = a;
            properties[1] = b;
            properties[2] = c;
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

        // 생성자
        public Triangle(String[] tmp)
        {
            indices = new int[3];
            for (int i = 0; i < 3; i++)
            {
                indices[i] = Int32.Parse(tmp[2 - i]);
            }

            normal = new Vector3D(0, 0, 0);
            area = 0;
        }

        public Triangle(int[] tmp)
        {
            indices = new int[3];
            for (int i = 0; i < 3; i++)
            {
                indices[i] = tmp[i];
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

    /* tetraheadron 정보 */
    public struct Tetrahedron
    {
        int[] indices;
        Triangle[] triangles;

        public Tetrahedron(String[] tmp)
        {
            indices = new int[4];
            for (int i = 0; i < 4; i++)
            {
                indices[i] = Int32.Parse(tmp[3 - i]);
            }

            triangles = null;
        }

        public Tetrahedron(int[] tmp)
        {
            indices = new int[4];

            for (int i = 0; i < 4; i++)
            {
                indices[i] = tmp[3 - i];
            }

            triangles = null;
        }

        public int[] getIndices()
        {
            return indices;
        }

        private void TetraToTriangle()
        {

        }
    }

    /* electrode 정보 */
    public struct Electrode
    {
        int n_face;
        int[,] faces; // 3 nodes + element

        // 생성자
        public Electrode(int n_face, int[,] tmp)
        {
            this.n_face = n_face;

            faces = new int[n_face, 4];
            for (int i = 0; i < n_face; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    faces[i, j] = tmp[i, j];
                }
            }
        }

        public int getNode(int i)
        {
            return faces[i, 0];
        }
    }

    /* label 정보 */
    public struct Label
    {
        string name;
        string part;
        int n_node;
        HashSet<int> nodes;

        public Label(string name, string part, int n_node, int[] tmp)
        {
            this.name = name;
            this.part = part;
            this.n_node = n_node;

            nodes = new HashSet<int>();
            for (int i = 0; i < n_node; i++)
            {
                nodes.Add(tmp[i]);
            }
        }

        public bool isLabel(int[] n)
        {
            if (nodes.Contains(n[0]) || nodes.Contains(n[1]) || nodes.Contains(n[2]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string[] getInfo()
        {
            string[] s = { name, part, n_node.ToString() };
            return s;
        }
    }

    /* ray 정보 */
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
        public string SubName { get; set; }

        public int Flag_surface { get; set; }
        public int Flag_electrode { get; set; }

        #region before..
        public Dictionary<int, Node> Nodes
        {
            get;
            set;
        }
        public Dictionary<int, Triangle> Triangles
        {
            get;
            set;
        }
        public int N_node { get; set; }
        public int N_triangle { get; set; }
        public int N_index { get; set; }
        #endregion

        public int N_property { get; set; }

        public int Mode_normal { get; set; }

        // VOLUME - HEAD
        public int Nnode_head { get; set; }
        public int Nele_head { get; set; }
        public Dictionary<int, Node> Nodes_head { get; set; }
        public Dictionary<int, Tetrahedron> Elements_head { get; set; }
        public int[] Regions_head { get; set; }

        // SCALP SURFACE
        public int Nnode_scalp { get; set; }
        public int Nele_scalp { get; set; }
        public Dictionary<int, Node> Nodes_scalp { get; set; }
        public Dictionary<int, Triangle> Elements_scalp { get; set; }
        public int[] Traces_scalp { get; set; }

        // CORTICAL SURFACE
        public int Nnode_cortex { get; set; }
        public int Nele_cortex { get; set; }
        public Dictionary<int, Node> Nodes_cortex { get; set; }
        public Dictionary<int, Triangle> Elements_cortex { get; set; }
        public int[] Traces_cortex { get; set; }

        // ELECTRODE
        public int Nnode_electrode { get; set; }
        public int[] Nodes_electrode { get; set; }
        public int N_electrode { get; set; }
        public Dictionary<int, Electrode> Electrodes { get; set; }

        public double[] Values_electrode { get; set; }
        public double[,,] Field_electric { get; set; }
        public double[,] Field_result { get; set; }

        // LABEL
        public int N_label { get; set; }
        public Dictionary<int, Label> Labels { get; set; }
        
        public int Hit_label { get; set; }

        // 마우스 정보
        public Point MousePosition { get; set; }
        public Ray MouseRay { get; set; }

        // 3D 정보
        public float RotationAngle { get; set; }
        public Vector3D RotationAxis { get; set; }

        public float TranslationX { get; set; }
        public float TranslationY { get; set; }
        public float TranslationZ { get; set; }

        public double Scale { get; set; }

        public List<ComboBoxItem> Properties { get; set; }
    }
}
