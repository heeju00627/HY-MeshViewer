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

namespace HY_MeshViewer
{
    /// <summary>
    /// ViewerControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewerControl : UserControl
    {
        String fileName;

        int n_node;
        int n_face;
        int n_property;
        int n_index;

        Point3D[] nodes;
        int[][] faces;
        double[][] properties;

        ModelVisual3D model;
        Transform3DGroup transformGroup;

        private bool mDown;
        private Point mLastPos;

        public ViewerControl()
        {
            InitializeComponent();

            model = new ModelVisual3D();
            transformGroup = new Transform3DGroup();

            //model.Transform = new Transform3DGroup();
            this.mainViewport.Children.Add(model);

            PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;
            camera.Transform = new Transform3DGroup();
        }

        /* ------------------------------------------------------------------------------------------------ */
        /* viewport slider, mouse event */

        private void ZoomSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        private void XSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AxisAngleRotation3D rotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 5);

            RotateTransform3D rt = new RotateTransform3D(rotation);

            transformGroup.Children.Add(rt);

            model.Transform = transformGroup;
        }

        private void YSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AxisAngleRotation3D rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 5);

            RotateTransform3D rt = new RotateTransform3D(rotation);

            transformGroup.Children.Add(rt);

            model.Transform = transformGroup;
        }

        private void ZSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AxisAngleRotation3D rotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 5);

            RotateTransform3D rt = new RotateTransform3D(rotation);

            transformGroup.Children.Add(rt);

            model.Transform = transformGroup;
        }

        private void OnGridMouseWheel(object sender, MouseWheelEventArgs e)
        {
            PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;

            camera.Position = new Point3D(
                camera.Position.X - e.Delta / 20D, camera.Position.Y - e.Delta / 20D, camera.Position.Z - e.Delta / 20D);
        }

        private void OnGridMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            mDown = true;
            Point pos = Mouse.GetPosition(this);
            mLastPos = new Point(
                    pos.X - this.ActualWidth / 2,
                    this.ActualHeight / 2 - pos.Y);

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
            }

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

            //PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;
            //Transform3DGroup group = camera.Transform as Transform3DGroup;

            QuaternionRotation3D r =
                 new QuaternionRotation3D(
                 new Quaternion(axis, rotation * 180 / Math.PI));
            
            transformGroup.Children.Add(new RotateTransform3D(r));
            model.Transform = transformGroup;

            //group.Children.Add(new RotateTransform3D(r));

            mLastPos = actualPos;
        }


        /* ------------------------------------------------------------------------------------------------ */
        /*  */

        public void OpenFile()
        {
            // TODO
            // open 되어 있는 파일이 있을 때 close할지 여부 물어보기! 혹은 새창 띄우기
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

                String[] tmp = data[0].Split(' ');

                // 올바른 file 아님
                if (tmp.Length != 4)
                {
                    MessageBox.Show("wrong file", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

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

                DrawModel();
            }
        }

        public void CloseFile()
        {
            n_node = 0;
            n_face = 0;
            n_property = 0;
            n_index = 0;

            nodes = null;
            faces = null;
            properties = null;

            fileName = null;

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

            PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;
            camera.Transform = new Transform3DGroup();
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

            mesh.TextureCoordinates.Add(new Point(0, 0));
            mesh.TextureCoordinates.Add(new Point(0, 1));
            mesh.TextureCoordinates.Add(new Point(1, 1));
            mesh.TextureCoordinates.Add(new Point(1, 0));

            LinearGradientBrush brush = new LinearGradientBrush();

            brush.StartPoint = new Point(0, 0);
            brush.EndPoint = new Point(1, 1);
            brush.GradientStops.Add(new GradientStop(Colors.Yellow, 0.0));
            brush.GradientStops.Add(new GradientStop(Colors.Red, 0.25));
            brush.GradientStops.Add(new GradientStop(Colors.Blue, 0.75));
            brush.GradientStops.Add(new GradientStop(Colors.LimeGreen, 1.0));

            //Material material = new DiffuseMaterial(new SolidColorBrush(Colors.AliceBlue));
            Material material = new DiffuseMaterial(brush);
            GeometryModel3D model = new GeometryModel3D(mesh, material);

            Model3DGroup group = new Model3DGroup();
            group.Children.Add(model);

            return group;
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
            Model3DCollection a = new Model3DCollection();

            if (nodes != null)
            {
                for (int i = 0; i < n_face; i++)
                {
                    /* 일단 triangle mesh만 사용 */
                    Model3DGroup t = CreateTriangleModel(nodes[faces[i][0]], nodes[faces[i][1]], nodes[faces[i][2]]);

                    a.Add(t);
                }

                group.Children = a;
                model.Content = group;
            }

            else
            {
                MessageBox.Show("no file", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void Coloring()
        {


        }
    }
}
