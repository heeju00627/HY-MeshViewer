using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using Microsoft.Win32;
using System.IO;
using _3DTools;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Shaders;
using HY_MeshViewer.ViewModel;
using HY_MeshViewer.Model;

namespace HY_MeshViewer.View
{
    /// <summary>
    /// ViewerControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewerControl_GL : UserControl
    {
        private ShaderProgram _program = new ShaderProgram();

        /** 생성자 */
        public ViewerControl_GL()
        {
            InitializeComponent();
        }

        private void OpenGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;

            //gl.Enable(OpenGL.GL_NORMALIZE);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.DepthFunc(OpenGL.GL_LESS);
            //gl.Enable(OpenGL.GL_CULL_FACE);

            float[] global_ambient = new float[] { 0.5f, 0.5f, 0.5f, 1.0f };
            float[] lmodel_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] light0pos = new float[] { 50.0f, 50.0f, 50.0f, 0.0f };
            float[] light0ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] light0diffuse = new float[] { 0.5f, 0.5f, 0.5f, 1.0f };
            float[] light0specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };

            // 전반사 반사율
            float[] lightOspecref = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };

            //gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, lmodel_ambient);
            //gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);

            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);

            // Enable color tracking
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);

            // 전반사 반사율 설정
            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SPECULAR, lightOspecref);
            // 재질의 밝기 설정..
            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 0);

            gl.ShadeModel(OpenGL.GL_SMOOTH);
        }

        /// <summary>
        /// Handles the OpenGLDraw event of the OpenGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void OpenGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;

            // Clear The Color And The Depth Buffer
            gl.ClearColor(0f, 0f, 0f, 0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Move Left And Into The Screen
            gl.LoadIdentity();
            gl.Translate(0.0f, 0.0f, -6.0f);
            gl.Scale(0.02f, 0.02f, 0.02f);
            //gl.Scale(0.5f, 0.5f, 0.5f);

            gl.Rotate(0, 0.1f, 0.0f, 0.0f);
            gl.Rotate(0, 0.0f, 0.1f, 0.0f);
            gl.Rotate(0, 0.0f, 0.0f, 0.1f);

            /* mesh 형태! */
            gl.PushAttrib(OpenGL.GL_POLYGON_BIT);
            gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);

            /*if (Triangles != null)
            {
                gl.Begin(OpenGL.GL_TRIANGLES);                  // Start Drawing

                foreach (Triangle t in ViewModel.Mesh.Triangles)
                {
                    int[] indices = t.getIndices();

                    Vector3D normal = CalculateNormal(indices);

                    gl.Normal(normal.X, normal.Y, normal.Z);

                    foreach (int i in indices)
                    {
                        gl.Color(ViewModel.Mesh.Nodes[i].getProperties());
                        gl.Vertex(ViewModel.Mesh.Nodes[i].getPosition());
                    }
                }

                gl.End();*/
            }
        }

    /* * normal vector 계산 */
    /*private Vector3D CalculateNormal(int[] indices)
    {
    /*Vertex p0 = ViewModel.Mesh.Nodes[indices[0]].getPosition();
    Vertex p1 = ViewModel.Mesh.Nodes[indices[1]].getPosition();
    Vertex p2 = ViewModel.Mesh.Nodes[indices[2]].getPosition();

    Vector3D v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
    Vector3D v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
    return Vector3D.CrossProduct(v0, v1);
    }

 <ContentControl x:Name="PropertyControl" Visibility="Visible" Content="{Binding PropertyView}"/>
        <ContentControl x:Name="ViewerControl_GL" Content="{Binding ViewerView}"/>
 */
}
