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

namespace HY_MeshViewer.Model
{
    /* node 정보 */
    public struct Node
    {
        // id는 dictionary로 관리
        Vertex position;
        float[] properties;

        /** 생성자 */
        public Node(String[] tmp, int n_property)
        {
            position = new Vertex(float.Parse(tmp[0]), float.Parse(tmp[1]), float.Parse(tmp[2]));

            properties = new float[n_property];
            for (int i = 0; i < n_property; i++)
            {
                properties[i] = float.Parse(tmp[3 + i]);
            }
        }

        public Vertex getPosition()
        {
            return position;
        }

        public float[] getProperties()
        {
            return properties;
        }
    }

    /* triangle 정보 */
    public struct Triangle
    {
        int[] indices;

        public Triangle(String[] tmp, int n_index)
        {
            indices = new int[n_index];
            for (int i = 0; i < n_index; i++)
            {
                indices[i] = Int32.Parse(tmp[i]);
            }
        }

        public int[] getIndices()
        {
            return indices;
        }
    }

    public class Mesh
    {
        // 데이터 파일
        public string FileName { get; set; }

        // 데이터 관리
        public Dictionary<int, Node> Nodes
        {
            get;
            set; }

        public List<Triangle> Triangles
        {
            get;
            set; }

        // 마우스 정보
        public Point MousePosition { get; set; }

        // mesh 정보
        public int N_node { get; set; }
        public int N_triangle { get; set; }
        public int N_property { get; set; }
        public int N_index { get; set; }

        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }
        public float Scale { get; set; }
    }
}
