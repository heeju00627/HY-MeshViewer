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
using WPF.MDI;

namespace HY_MeshViewer.View
{
    /// <summary>
    /// MainedWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainedWindow : Window
    {
        //ModelVisual3D model = new ModelVisual3D();

        int n_node;
        int n_face;
        int n_property;
        int n_index;

        Point3D[] nodes;
        int[][] faces;
        double[][] properties;
        
        private bool mDown;
        private Point mLastPos;

        public MainedWindow()
        {
            InitializeComponent();

            /** mainViewport에 한 가지 model만 추가(동시에 여러 model 불가) */
            //this.mainViewport.Children.Add(model);
            PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;
            camera.Transform = new Transform3DGroup();

            CreateAxis(new Point3D(0, 0, 0));
        }

        /* ------------------------------------------------------------------------------------------------ */
        /* Model3DGroup : GeometryModel3D 집합 or Model3DGroup의 집합... 작은 compopnent로서 기능 */

        /** 단일 삼각형 mesh를 포함하는 Model3DGroup 생성 */
        private Model3DGroup CreateTriangleModel(Point3D p0, Point3D p1, Point3D p2)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);

            Vector3D normal = CalculateNormal(p0, p1, p2);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);

            Material material = new DiffuseMaterial(new SolidColorBrush(Colors.AliceBlue));
            GeometryModel3D model = new GeometryModel3D(mesh, material);

            Model3DGroup group = new Model3DGroup();
            group.Children.Add(model);

            return group;
        }

        /** center를 중심으로 축 그리게 */
        //TODO
        //center 계산
        private Model3DGroup CreateAxis(Point3D center)
        {
            MeshGeometry3D axis = new MeshGeometry3D();

            Point3D origin = new Point3D(0, 0, 0);
            Point3D x = new Point3D(1000 + origin.X, 0 + origin.Y, 0 + origin.Z);
            Point3D y = new Point3D(0 + origin.X, 1000 + origin.Y, 0 + origin.Z);
            Point3D z = new Point3D(0 + origin.X, 0 + origin.Y, 1000 + origin.Z);

            ScreenSpaceLines3D xAxis = new ScreenSpaceLines3D();
            ScreenSpaceLines3D yAxis = new ScreenSpaceLines3D();
            ScreenSpaceLines3D zAxis = new ScreenSpaceLines3D();

            Color c = Colors.Red;
            int width = 1;

            xAxis.Color = Colors.Red;
            xAxis.Thickness = width;
            yAxis.Color = Colors.Blue;
            yAxis.Thickness = width;
            zAxis.Color = Colors.Green;
            zAxis.Thickness = width;

            xAxis.Points.Add(origin);
            xAxis.Points.Add(x);
            yAxis.Points.Add(origin);
            yAxis.Points.Add(y);
            zAxis.Points.Add(origin);
            zAxis.Points.Add(z);

            Material material = new DiffuseMaterial(Brushes.Red);
            GeometryModel3D model = new GeometryModel3D(axis, material);

            Model3DGroup group = new Model3DGroup();
            group.Children.Add(model);

            this.mainViewport.Children.Add(xAxis);
            this.mainViewport.Children.Add(yAxis);
            this.mainViewport.Children.Add(zAxis);

            return group;
        }

        /** normal vector 계산 */
        private Vector3D CalculateNormal(Point3D p0, Point3D p1, Point3D p2)
        {
            Vector3D v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            Vector3D v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3D.CrossProduct(v0, v1);
        }

        /* center point 계산 */
        public Point3D GetCenter(ModelVisual3D model)
        {
            var rect3D = Rect3D.Empty;
            UnionRect(model, ref rect3D);

            Point3D center = new Point3D((rect3D.X + rect3D.SizeX / 2), (rect3D.Y + rect3D.SizeY / 2), (rect3D.Z + rect3D.SizeZ / 2));

            SetCamera(rect3D, center);
            return center;
        }

        /* model 합쳐나감 */
        private void UnionRect(ModelVisual3D model, ref Rect3D rect3D)
        {
            for (int i = 0; i < model.Children.Count; i++)
            {
                var child = model.Children[i] as ModelVisual3D;
                UnionRect(child, ref rect3D);
            }
            if (model.Content != null)
                rect3D.Union(model.Content.Bounds);
        }

        /* ------------------------------------------------------------------------------------------------ */
        /** Viewport에 보여지는 모든 ModelVisual3D model 삭제 -> 화면 clear */
        private void ClearViewport()
        {
            ModelVisual3D m;

            for (int i = this.mainViewport.Children.Count - 1; i > 3; i--)
            {
                m = (ModelVisual3D)mainViewport.Children[i];
                if (m.Content is DirectionalLight == false)
                    mainViewport.Children.Remove(m);
            }
        }

        /** camera control(center를 바라보는 방향) */
        private void SetCamera(Rect3D rect3D, Point3D center)
        {
            PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;

            double size = Math.Max(Math.Max(rect3D.SizeX, rect3D.SizeY), rect3D.SizeZ);

            Point3D offset = new Point3D(3 * size, 3 * size, 3 * size);
            Point3D position = new Point3D(center.X + offset.X, center.Y + offset.Y, center.Z + offset.Z);

            Vector3D lookDirection = new Vector3D(-offset.X, -offset.Y, -offset.Z);

            camera.Position = position;
            camera.LookDirection = lookDirection;

            SetTextBox();
        }

        /** textbox control */
        private void SetTextBox()
        {
            PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;

            Point3D position = camera.Position;
            Vector3D lookDirection = camera.LookDirection;

            cameraPositionXTextBox.Text = Convert.ToString(position.X);
            cameraPositionYTextBox.Text = Convert.ToString(position.Y);
            cameraPositionZTextBox.Text = Convert.ToString(position.Z);

            lookAtXTextBox.Text = Convert.ToString(lookDirection.X);
            lookAtYTextBox.Text = Convert.ToString(lookDirection.Y);
            lookAtZTextBox.Text = Convert.ToString(lookDirection.Z);
        }

        /** 카메라 원래대로 */
        private void ResetButtonClick(object sender, RoutedEventArgs e)
        {
            PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;

            Point3D position
                = new Point3D(Convert.ToDouble(cameraPositionXTextBox.Text), Convert.ToDouble(cameraPositionYTextBox.Text), Convert.ToDouble(cameraPositionZTextBox.Text));

            Vector3D lookDirection
                = new Vector3D(Convert.ToDouble(lookAtXTextBox.Text), Convert.ToDouble(lookAtYTextBox.Text), Convert.ToDouble(lookAtZTextBox.Text));

            camera.Position = position;
            camera.LookDirection = lookDirection;
        }

        private void ZoomSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            /* PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;

            camera.Position = new Point3D(
                camera.Position.X - e.NewValue / 20D, camera.Position.Y - e.NewValue / 20D, camera.Position.Z - e.NewValue / 20D);*/
        }

        private void XSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            /* PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;

            camera.Position = new Point3D(
                camera.Position.X - e.NewValue / 20D, camera.Position.Y - e.NewValue / 20D, camera.Position.Z - e.NewValue / 20D);*/
        }

        private void YSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            /* PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;

            camera.Position = new Point3D(
                camera.Position.X - e.NewValue / 20D, camera.Position.Y - e.NewValue / 20D, camera.Position.Z - e.NewValue / 20D);*/
        }

        private void ZSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            /* PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;

            camera.Position = new Point3D(
                camera.Position.X - e.NewValue / 20D, camera.Position.Y - e.NewValue / 20D, camera.Position.Z - e.NewValue / 20D);*/
        }

        private void OnGridMouseWheel(object sender, MouseWheelEventArgs e)
        {
            PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;

            camera.Position = new Point3D(
                camera.Position.X - e.Delta / 20D, camera.Position.Y - e.Delta / 20D, camera.Position.Z - e.Delta / 20D);

            SetTextBox();
        }

        private void OnGridMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            mDown = true;
            Point pos = Mouse.GetPosition(this);
            mLastPos = new Point(
                    pos.X - this.ActualWidth / 2,
                    this.ActualHeight / 2 - pos.Y);
            label.Content = pos;

            SetTextBox();
        }

        private void OnGridMouseUp(object sender, MouseButtonEventArgs e)
        {
            mDown = false;
        }

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
            
            PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;
            Transform3DGroup group = camera.Transform as Transform3DGroup;
            
            QuaternionRotation3D r =
                 new QuaternionRotation3D(
                 new Quaternion(axis, rotation * 180 / Math.PI));
            
            group.Children.Add(new RotateTransform3D(r));

            mLastPos = actualPos;

            SetTextBox();
        }

        /* ------------------------------------------------------------------------------------------------ */
        /** random topography */
        private Point3D[] GetRandomTopographyPoints()
        {
            Point3D[] points = new Point3D[100];

            Random r = new Random();
            double y;
            double denom = 1000;
            int count = 0;

            for (int z = 0; z < 10; z++)
            {
                for (int x = 0; x < 10; x++)
                {
                    System.Threading.Thread.Sleep(1);
                    y = Convert.ToDouble(r.Next(1, 999)) / denom;
                    points[count] = new Point3D(x, y, z);
                    count += 1;
                }
            }

            return points;
        }

        /* ------------------------------------------------------------------------------------------------ */
        /** open dialog + load data */
        private void OpenButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();

            //openDlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            // TODO
            // type이 맞는지 체크하는 부분 추가
            if (openDlg.ShowDialog() == true)
            {
                String[] data = File.ReadAllLines(openDlg.FileName);

                txtBox.Text = openDlg.FileName + "\n" + data.Length + "\n" + data[0];

                String[] tmp = data[0].Split(' ');
                txtBox.Text = tmp[0];
                n_node = Int32.Parse(tmp[0]);
                n_face = Int32.Parse(tmp[1]);
                n_property = Int32.Parse(tmp[2]);
                n_index = Int32.Parse(tmp[3]);

                nodes = new Point3D[n_node];
                faces = new int[n_face][];
                properties = new double[n_node][];

                for (int i = 0; i < n_node; i++)
                {
                    tmp = data[i + 1].Split('\t');
                    nodes[i] = new Point3D(Double.Parse(tmp[0]), Double.Parse(tmp[1]), Double.Parse(tmp[2]));

                    properties[i] = new double[n_property];
                    for (int j = 0; j < n_property; j++)
                    {
                        properties[i][j] = Double.Parse(tmp[3 + j]);
                    }
                }
                for (int i = 0; i < n_face; i++)
                {
                    tmp = data[i + 1 + n_node].Split('\t');

                    faces[i] = new int[n_index];
                    for (int j = 0; j < n_index; j++)
                    {
                        faces[i][j] = Int32.Parse(tmp[j]);
                    }
                }
            }
        }
        
        /* ------------------------------------------------------------------------------------------------ */
        /** triangle model 생성 */
        private void TriangleButtonClick(object sender, RoutedEventArgs e)
        {
            ClearViewport();

            Model3DGroup triangle = new Model3DGroup();

            Point3D point0 = new Point3D(0, 0, 0);
            Point3D point1 = new Point3D(5, 0, 0);
            Point3D point2 = new Point3D(0, 0, 5);

            triangle.Children.Add(CreateTriangleModel(point0, point2, point1));

            ModelVisual3D model = new ModelVisual3D();
            model.Content = triangle;
            Point3D center = GetCenter(model);
            txtBox.Text = center.ToString();

            //model.Transform = new Transform3DGroup();
            
            this.mainViewport.Children.Add(model);
        }


        /** cube model 생성(6면, 12개 triangle) */
        private void CubeButtonClick(object sender, RoutedEventArgs e)
        {
            ClearViewport();

            Model3DGroup cube = new Model3DGroup();

            Point3D p0 = new Point3D(-5, -5, -5);
            Point3D p1 = new Point3D(5, -5, -5);
            Point3D p2 = new Point3D(5, -5, 5);
            Point3D p3 = new Point3D(-5, -5, 5);
            Point3D p4 = new Point3D(-5, 5, -5);
            Point3D p5 = new Point3D(5, 5, -5);
            Point3D p6 = new Point3D(5, 5, 5);
            Point3D p7 = new Point3D(-5, 5, 5);

            //front side traingles
            cube.Children.Add(CreateTriangleModel(p3, p2, p6));
            cube.Children.Add(CreateTriangleModel(p3, p6, p7));
            //right side traingles
            cube.Children.Add(CreateTriangleModel(p2, p1, p5));
            cube.Children.Add(CreateTriangleModel(p2, p5, p6));
            //back side traingles
            cube.Children.Add(CreateTriangleModel(p1, p0, p4));
            cube.Children.Add(CreateTriangleModel(p1, p4, p5));
            //left side traingles
            cube.Children.Add(CreateTriangleModel(p0, p3, p7));
            cube.Children.Add(CreateTriangleModel(p0, p7, p4));
            //top side traingles
            cube.Children.Add(CreateTriangleModel(p7, p6, p5));
            cube.Children.Add(CreateTriangleModel(p7, p5, p4));
            //bottom side traingles
            cube.Children.Add(CreateTriangleModel(p2, p3, p0));
            cube.Children.Add(CreateTriangleModel(p2, p0, p1));

            ModelVisual3D model = new ModelVisual3D();
            model.Content = cube;
            Point3D center = GetCenter(model);
            txtBox.Text = center.ToString();

            //model.Transform = new Transform3DGroup();
            this.mainViewport.Children.Add(model);

            
        }

        /** topography model 생성 */
        private void TopographyButtonClick(object sender, RoutedEventArgs e)
        {
            ClearViewport();

            Model3DGroup topography = new Model3DGroup();

            Point3D[] points = GetRandomTopographyPoints();

            for (int z = 0; z <= 80; z = z + 10)
            {
                for (int x = 0; x < 9; x++)
                {
                    topography.Children.Add(CreateTriangleModel(points[x + z], points[x + z + 10], points[x + z + 1]));
                    topography.Children.Add(CreateTriangleModel(points[x + z + 1], points[x + z + 10], points[x + z + 11]));
                }
            }
            
            ModelVisual3D model = new ModelVisual3D();
            model.Content = topography;
            Point3D center = GetCenter(model);
            txtBox.Text = center.ToString();

            //model.Transform = new Transform3DGroup();
            this.mainViewport.Children.Add(model);

            
        }

        /** brain model 생성 */
        private void BrainButtonClick(object sender, RoutedEventArgs e)
        {
            //ClearViewport();
            //SetCamera();

            Model3DGroup brain = new Model3DGroup();

            if (nodes != null)
            {
                for (int i = 0; i < n_face; i++)
                {
                    /* 일단 triangle mesh만 사용 */
                    brain.Children.Add(CreateTriangleModel(nodes[faces[i][0]], nodes[faces[i][1]], nodes[faces[i][2]]));
                }

                ModelVisual3D model = new ModelVisual3D();
                model.Content = brain;
                Point3D center = GetCenter(model);
                txtBox.Text = center.ToString();

                //model.Transform = new Transform3DGroup();
                this.mainViewport.Children.Add(model);
            }

            else
            {
                txtBox.Text = "No File";
            }
        }
    }
}
