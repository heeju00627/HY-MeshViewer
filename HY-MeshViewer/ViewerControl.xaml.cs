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

namespace HY_MeshViewer
{
    /* node 정보 */
    public struct Vertex
    {
        // id는 dictionary로 관리
        Point3D position;
        double[] properties;

        /** 생성자 */
        public Vertex(String[] tmp, int n_property)
        {
            position = new Point3D(Double.Parse(tmp[0]), Double.Parse(tmp[1]), Double.Parse(tmp[2]));
      
            properties = new double[n_property];
            for (int i = 0; i < n_property; i++)
            {
                properties[i] = Double.Parse(tmp[3 + i]);
            }
        }

        public Point3D getPosition()
        {
            return position;
        }

        public double[] getProperties()
        {
            return properties;
        }
    }

    /* triangle 정보 */
    public struct Triangle
    {
        int[] indices;
        // geometry + material로 구성
        GeometryModel3D geoModel;

        public Triangle(String[] tmp, int n_index)
        {
            indices = new int[n_index];
            for (int i = 0; i < n_index; i++)
            {
                indices[i] = Int32.Parse(tmp[i]);
            }

            geoModel = new GeometryModel3D();
        }

        public int[] getIndices()
        {
            return indices;
        }

        public GeometryModel3D getGeomodel()
        {
            return geoModel;
        }

        public void setGeomodel(MeshGeometry3D mesh, Material material)
        {
            geoModel.Geometry = mesh;
            geoModel.Material = material;
        }

        public void setMaterial(Material material)
        {
            geoModel.Material = material;
        }
    }


    /// <summary>
    /// ViewerControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewerControl : UserControl
    {
        // 데이터 파일
        String fileName;

        // 데이터 관리
        Dictionary<int, Vertex> vertices;
        List<Triangle> triangles;

        int n_node;
        int n_triangle;
        int n_property;
        // triangle mesh : n_index == 3
        int n_index;

        // Viewport 관리(단일 모델)
        ModelVisual3D model;
        Transform3DGroup transformGroup;

        // mouse 입력 정보
        private bool mDown;
        private Point mLastPos;


        /** 생성자 */
        public ViewerControl()
        {
            InitializeComponent();

            // model 생성 + 속성
            model = new ModelVisual3D();
            transformGroup = new Transform3DGroup();
            this.mainViewport.Children.Add(model);
            
            // camera 속성
            PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;
            camera.Transform = new Transform3DGroup();
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
            AxisAngleRotation3D rotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 5);

            RotateTransform3D rt = new RotateTransform3D(rotation);

            transformGroup.Children.Add(rt);

            model.Transform = transformGroup;
        }

        /** y축 회전 슬라이더 */
        private void YSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AxisAngleRotation3D rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 5);

            RotateTransform3D rt = new RotateTransform3D(rotation);

            transformGroup.Children.Add(rt);

            model.Transform = transformGroup;
        }

        /** z축 회전 슬라이더 */
        private void ZSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AxisAngleRotation3D rotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 5);

            RotateTransform3D rt = new RotateTransform3D(rotation);

            transformGroup.Children.Add(rt);

            model.Transform = transformGroup;
        }

        /* ------------------------------------------------------------------------------------------------ */
        /* mouse event */

        /** 마우스 휠 -> 확대축소 (ctrl + 클릭 으로 변경 요망) */
        private void OnGridMouseWheel(object sender, MouseWheelEventArgs e)
        {
            PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;

            camera.Position = new Point3D(
                camera.Position.X - e.Delta / 20D, camera.Position.Y - e.Delta / 20D, camera.Position.Z - e.Delta / 20D);
        }

        /** 마우스 왼쪽버튼 눌렀을 때 */
        private void OnGridMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            mDown = true;
            Point pos = Mouse.GetPosition(this);
            mLastPos = new Point(
                    pos.X - this.ActualWidth / 2,
                    this.ActualHeight / 2 - pos.Y);

            // 더블클릭 했을 때 클릭 위치 메세지 팝업
            if (e.ClickCount == 2)
            {
                PointHitTestParameters hitParams = new PointHitTestParameters(pos);
                VisualTreeHelper.HitTest(this, null, delegate (HitTestResult hr)
                {
                    RayMeshGeometry3DHitTestResult rayHit = hr as
                    RayMeshGeometry3DHitTestResult;
                    if (rayHit != null)
                    {
                        MessageBox.Show(rayHit.PointHit.ToString(), "pos", MessageBoxButton.OK);
                    }
                    return HitTestResultBehavior.Continue;
                }, hitParams);

                mDown = false;

                Coloring();
            }

        }

        /** 마우스 왼쪽버튼 뗐을 때 */
        private void OnGridMouseUp(object sender, MouseButtonEventArgs e)
        {
            mDown = false;
        }

        /** 마우스 왼쪽버튼 누른 뒤 움직일 때 */
        private void OnGridMouseMove(object sender, MouseEventArgs e)
        {
            if (!mDown) return;

            Point pos = Mouse.GetPosition(this);

            Point actualPos = new Point(
                    pos.X - this.ActualWidth / 2,
                    this.ActualHeight / 2 - pos.Y);

            double dx = actualPos.X - mLastPos.X;
            double dy = actualPos.Y - mLastPos.Y;
            double mouseAngle = 0;

            if (dx != 0 && dy != 0)
            {
                mouseAngle = Math.Asin(Math.Abs(dy) /
                    Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
                if (dx < 0 && dy > 0) mouseAngle += Math.PI / 2;
                else if (dx < 0 && dy < 0) mouseAngle += Math.PI;
                else if (dx > 0 && dy < 0) mouseAngle += Math.PI * 1.5;
            }
            else if (dx == 0 && dy != 0)
            {
                mouseAngle = Math.Sign(dy) > 0 ? Math.PI / 2 : Math.PI * 1.5;
            }
            else if (dx != 0 && dy == 0)
            {
                mouseAngle = Math.Sign(dx) > 0 ? 0 : Math.PI;
            }

            double axisAngle = mouseAngle + Math.PI / 2;

            Vector3D axis = new Vector3D(
                    Math.Cos(axisAngle) * 4,
                    Math.Sin(axisAngle) * 4, 0);

            double rotation = 0.02 *
                    Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

            QuaternionRotation3D r =
                 new QuaternionRotation3D(
                 new Quaternion(axis, rotation * 180 / Math.PI));
            
            transformGroup.Children.Add(new RotateTransform3D(r));
            model.Transform = transformGroup;

            //group.Children.Add(new RotateTransform3D(r));

            mLastPos = actualPos;
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

            //openDlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            // TODO
            // type이 맞는지 체크하는 부분 추가
            if (openDlg.ShowDialog() == true)
            {
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

                vertices = new Dictionary<int, Vertex>();
                triangles = new List<Triangle>();
                
                for (int i = 0; i < n_node; i++)
                {
                    tmp = data[i + 1].Split('\t');

                    Vertex v = new Vertex(tmp, n_property);

                    vertices.Add(i, v);
                }

                for (int i = 0; i < n_triangle; i++)
                {
                    tmp = data[i + 1 + n_node].Split('\t');

                    Triangle t = new Triangle(tmp, n_index);

                    triangles.Add(t);
                }

                DrawModel();
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

            vertices = null;
            triangles = null;

            ModelVisual3D m;

            for (int i = this.mainViewport.Children.Count - 1; i > 0; i--)
            {
                m = (ModelVisual3D)mainViewport.Children[i];
                //if (m.Content is DirectionalLight == false)
                if (m.Content is Model3DGroup == true)
                    mainViewport.Children.Remove(m);
            }
            model = new ModelVisual3D();
            this.mainViewport.Children.Add(model);

            // 카메라 속성 초기화
            PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;
            camera.Transform = new Transform3DGroup();
        }

        /* ------------------------------------------------------------------------------------------------ */
        /** Model3DGroup : GeometryModel3D 집합 or Model3DGroup의 집합... 작은 compopnent로서 기능 */

        private void CreateTriangle(Triangle t)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            Material material = new DiffuseMaterial(new SolidColorBrush(Colors.AliceBlue));

            foreach (int i in t.getIndices())
            {
                mesh.Positions.Add(vertices[i].getPosition());
            }

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);

            //Vector3D normal = CalculateNormal(p0, p1, p2);
            //mesh.Normals.Add(normal);
            //mesh.Normals.Add(normal);
            //mesh.Normals.Add(normal);

            mesh.TextureCoordinates.Add(new Point(0, 0));
            mesh.TextureCoordinates.Add(new Point(1, 0));
            mesh.TextureCoordinates.Add(new Point(0, 1));

            t.setGeomodel(mesh, material);
        }
        
        /** normal vector 계산 */
        private Vector3D CalculateNormal(Point3D p0, Point3D p1, Point3D p2)
        {
            Vector3D v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            Vector3D v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3D.CrossProduct(v0, v1);
        }

        private void DrawModel()
        {
            Model3DGroup group = new Model3DGroup();

            if (triangles != null)
            {
                foreach (Triangle t in triangles)
                {
                    CreateTriangle(t);
                    group.Children.Add(t.getGeomodel());
                }

                model.Content = group;
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
    }
}
