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

namespace HY_MeshViewer
{
    /// <summary>
    /// ViewerControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewerControl_GL : UserControl
    {
        // 데이터 파일
        String fileName;

        // 데이터 관리
        Dictionary<int, Node> nodes;
        List<Triangle> triangles;

        int n_node;
        int n_triangle;
        int n_property;
        // triangle mesh : n_index == 3
        int n_index;
        
        // mouse 입력 정보
        private bool mDown;
        private Point mLastPos;


        ShaderProgram program = new ShaderProgram();

        float rotationX = 0;
        float rotationY = 0;
        float rotationZ = 0;

        /** 생성자 */
        public ViewerControl_GL()
        {
            InitializeComponent();
        }

        /* ------------------------------------------------------------------------------------------------ */
        /* viewport slider */

        /** 확대축소 슬라이더 */
        private void ZoomSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        /** x축 회전 슬라이더 */
        private void XSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            rotationX = (float)e.NewValue;
        }

        /** y축 회전 슬라이더 */
        private void YSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            rotationY = (float)e.NewValue;
        }

        /** z축 회전 슬라이더 */
        private void ZSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            rotationZ= (float)e.NewValue;
        }

        /* ------------------------------------------------------------------------------------------------ */
        /* mouse event */

        /** 마우스 휠 -> 확대축소 (ctrl + 클릭 으로 변경 요망) */
        private void OnGridMouseWheel(object sender, MouseWheelEventArgs e)
        {
        }

        /** 마우스 왼쪽버튼 눌렀을 때 */
        private void OnGridMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        /** 마우스 왼쪽버튼 뗐을 때 */
        private void OnGridMouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        /** 마우스 왼쪽버튼 누른 뒤 움직일 때 */
        private void OnGridMouseMove(object sender, MouseEventArgs e)
        {
        }


        /* ------------------------------------------------------------------------------------------------ */
        /** 파일 관리 */

        /* 파일 열기 */
        public void OpenFile()
        {
            if (fileName != null)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to close file?", "There is already...", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    CloseFile();
                }

                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            OpenFileDialog openDlg = new OpenFileDialog();

            openDlg.Filter = "TXT Files (*.txt)|*.*";
            
            if (openDlg.ShowDialog() == true)
            {
                // get the path
                fileName = openDlg.FileName;

                String[] data = File.ReadAllLines(openDlg.FileName);

                // 첫째줄 데이터 정보
                String[] tmp = data[0].Split(' ');

                // 올바른 file 아님
                if (tmp.Length != 4)
                {
                    MessageBox.Show("wrong file", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                n_node = Int32.Parse(tmp[0]);
                n_triangle = Int32.Parse(tmp[1]);
                n_property = Int32.Parse(tmp[2]);
                n_index = Int32.Parse(tmp[3]);

                nodes = new Dictionary<int, Node>();
                triangles = new List<Triangle>();

                for (int i = 0; i < n_node; i++)
                {
                    tmp = data[i + 1].Split('\t');

                    Node n = new Node(tmp, n_property);

                    nodes.Add(i, n);
                }

                for (int i = 0; i < n_triangle; i++)
                {
                    tmp = data[i + 1 + n_node].Split('\t');

                    Triangle t = new Triangle(tmp, n_index);

                    triangles.Add(t);
                }
            }
        }

        /* 파일 닫기 */
        public void CloseFile()
        {
            n_node = 0;
            n_triangle = 0;
            n_property = 0;
            n_index = 0;

            fileName = null;

            nodes = null;
            triangles = null;
            
        }

        /* ------------------------------------------------------------------------------------------------ */
        /** Model3DGroup : GeometryModel3D 집합 or Model3DGroup의 집합... 작은 compopnent로서 기능 */

        private void CreateTriangle(Triangle t)
        {
        }

        /** normal vector 계산 */
        private Vector3D CalculateNormal(int[] indices)
        {
            Vertex p0 = nodes[indices[0]].getPosition();
            Vertex p1 = nodes[indices[1]].getPosition();
            Vertex p2 = nodes[indices[2]].getPosition();

            Vector3D v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            Vector3D v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3D.CrossProduct(v0, v1);
        }

        private void DrawModel()
        {
            if (triangles != null)
            {
            }

            else
            {
                MessageBox.Show("no file", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        public void Coloring()
        {
        }


        /* ------------------------------------------------------------------------------------------------ */
        /**  */

        private void OpenGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            // Get the OpenGL instance
            OpenGL gl = args.OpenGL;

            // Clear The Color And The Depth Buffer
            gl.ClearColor(0f, 0f, 0f, 0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
             
            // Move Left And Into The Screen
            gl.LoadIdentity();
            gl.Translate(0.0f, 0.0f, -6.0f);
            gl.Scale(0.02f, 0.02f, 0.02f);
            //gl.Scale(0.5f, 0.5f, 0.5f);

            program.Push(gl, null);
            gl.Rotate(rotationX, 0.1f, 0.0f, 0.0f);
            gl.Rotate(rotationY, 0.0f, 0.1f, 0.0f);
            gl.Rotate(rotationZ, 0.0f, 0.1f, 0.1f);


            /* mesh 형태! */
            //gl.PushAttrib(OpenGL.GL_POLYGON_BIT);
            //gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);

            //Teapot tp = new Teapot();
            //tp.Draw(gl, 14, 1, OpenGL.GL_FILL);

            if (triangles != null)
            {

                gl.Begin(OpenGL.GL_TRIANGLES);                  // Start Drawing

                foreach (Triangle t in triangles)
                {
                    int[] indices = t.getIndices();

                    Vector3D normal = CalculateNormal(indices);

                    gl.Normal(normal.X, normal.Y, normal.Z);

                    foreach (int i in indices)
                    {
                        gl.Color(nodes[i].getProperties());
                        gl.Vertex(nodes[i].getPosition());
                    }
                }

                gl.End();

            }

            program.Pop(gl, null);
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

            //  Create a vertex shader.
            /*VertexShader vertexShader = new VertexShader();
            vertexShader.CreateInContext(gl);
            vertexShader.SetSource(
                "void main()" + Environment.NewLine +
                "{" + Environment.NewLine +
                "gl_Position = ftransform();" + Environment.NewLine +
                "}" + Environment.NewLine);

            //  Create a fragment shader.
            FragmentShader fragmentShader = new FragmentShader();
            fragmentShader.CreateInContext(gl);
            fragmentShader.SetSource(
                "void main()" + Environment.NewLine +
                "{" + Environment.NewLine +
                "gl_FragColor = vec4(0.4,0.4,0.8,1.0);" + Environment.NewLine +
                "}" + Environment.NewLine);

            //  Compile them both.
            vertexShader.Compile();
            fragmentShader.Compile();

            //  Build a program.
            program.CreateInContext(gl);

            //  Attach the shaders.
            program.AttachShader(vertexShader);
            program.AttachShader(fragmentShader);
            program.Link();*/
        }

        private void OpenGLControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
