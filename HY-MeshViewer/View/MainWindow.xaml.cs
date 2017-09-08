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
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using Microsoft.Win32;
using System.IO;
using WPF.MDI;
using _3DTools;
using HY_MeshViewer.ViewModel;
using HY_MeshViewer.Model;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Shaders;
namespace HY_MeshViewer.View
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        /* ------------------------------------------------------------------------------------------------ */
        /* menu event */
        private void SubmenuExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void SubmenuToolbar_Click(object sender, RoutedEventArgs e)
        {
            if (Toolbar.Visibility == Visibility.Visible)
            {
                Toolbar.Visibility = Visibility.Collapsed;
                submenuToolbar.IsChecked = false;
            }
            else if (Toolbar.Visibility == Visibility.Collapsed)
            {
                Toolbar.Visibility = Visibility.Visible;
                submenuToolbar.IsChecked = true;
            }
        }

        private void SubmenuStatusbar_Click(object sender, RoutedEventArgs e)
        {
            if (Statusbar.Visibility == Visibility.Visible)
            {
                Statusbar.Visibility = Visibility.Collapsed;
                secondRow.Height = new GridLength(0);
                submenuStatusbar.IsChecked = false;
            }
            else if (Statusbar.Visibility == Visibility.Collapsed)
            {
                Statusbar.Visibility = Visibility.Visible;
                secondRow.Height = new GridLength(25);
                submenuStatusbar.IsChecked = true;
            }
        }

        private void SubmenuProperty_Click(object sender, RoutedEventArgs e)
        {
            if (PropertyControl.Visibility == Visibility.Visible)
            {
                PropertyControl.Visibility = Visibility.Collapsed;
                submenuPropertybox.IsChecked = false;
            }
            else if (PropertyControl.Visibility == Visibility.Collapsed)
            {
                PropertyControl.Visibility = Visibility.Visible;
                submenuPropertybox.IsChecked = true;
            }
        }

        /* ------------------------------------------------------------------------------------------------ */
        /* property control */

        private void comboBoxPolygonMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.openGlControl == null || this.openGlControl.OpenGL == null)
                return;

            switch (polygonModeComboBox.SelectedIndex)
            {
                case 0:
                    this.openGlControl.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Points);
                    break;
                case 1:
                    this.openGlControl.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);
                    break;
                case 2:
                    this.openGlControl.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled);
                    break;
            }
        }
        
        /* ------------------------------------------------------------------------------------------------ */
        /* viewer control */

        private void OpenGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;

            // 가려진 면 제거
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.DepthFunc(OpenGL.GL_LEQUAL);
            gl.Enable(OpenGL.GL_NORMALIZE);

            uint[] lights = new uint[] { OpenGL.GL_LIGHT0, OpenGL.GL_LIGHT1, OpenGL.GL_LIGHT2, OpenGL.GL_LIGHT3 };
            
            // 주변반사에 대한 물체색(흡수율)
            float[] materialAmbient = new float[] { 0.5f, 0.5f, 0.5f, 1.0f };
            // 확산반사에 대한 물체색(반사율)
            float[] materialDiffuse = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };
            // 경면반사에 대한 물체색(흡수율)
            float[] materialSpecular = new float[] { 0.3f, 0.3f, 0.3f, 1.0f };

            // 전역 주변광
            float[] global_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };

            // 조명 위치
            float[][] lightOpos = new float[][] { new float[] { 0f, 0f, 300f, 1.0f }, new float[] { 0f, -300f, 0f, 1.0f }, new float[] { 300f, 0f, 0f, 1.0f }, new float[] { -300f, 0f, 0f, 1.0f } };
            
            // 주변광
            float[] light0ambient = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };
            // 분산광
            float[] light0diffuse = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            // 반사광
            float[] light0specular = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };

            // 전체적인 밝기 분포
            float lightConstAttenuation = 2.0f;
            // 1차 계수(거리에 따른 밝기)
            float lightLinearAttenuation = 0.0f;
            // 2차 계수
            float lightQuadraticAttenuation = 0.0f;
            
            //gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);

            for (int i = 0; i < 4; i++)
            {
                gl.Light(lights[i], OpenGL.GL_POSITION, lightOpos[i]);
                gl.Light(lights[i], OpenGL.GL_AMBIENT, light0ambient);
                gl.Light(lights[i], OpenGL.GL_DIFFUSE, light0diffuse);
                gl.Light(lights[i], OpenGL.GL_SPECULAR, light0specular);
                gl.Light(lights[i], OpenGL.GL_CONSTANT_ATTENUATION, lightConstAttenuation);
                gl.Light(lights[i], OpenGL.GL_LINEAR_ATTENUATION, lightLinearAttenuation);
                gl.Light(lights[i], OpenGL.GL_QUADRATIC_ATTENUATION, lightQuadraticAttenuation);
            }
            
            // 조명 활성화
            gl.Enable(OpenGL.GL_LIGHTING);
            // 조명 ON
            for (int i = 0; i < 4; i++)
            {
                if (i != 0) continue;
                gl.Enable(lights[i]);
            }

            // Enable color tracking
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT_AND_DIFFUSE);

            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT_AND_DIFFUSE, materialDiffuse);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT, materialAmbient);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_DIFFUSE, materialDiffuse);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SPECULAR, materialSpecular);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SHININESS, 100.0f);

            // 매끄러운 세이딩 사용
            gl.ShadeModel(OpenGL.GL_SMOOTH);

            /*OpenGL gl = args.OpenGL;

            gl.Enable(OpenGL.GL_NORMALIZE);
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

            gl.ShadeModel(OpenGL.GL_SMOOTH);*/
        }

        private void SetLighting(OpenGL gl)
        {
            uint[] lights = new uint[] { OpenGL.GL_LIGHT0, OpenGL.GL_LIGHT1, OpenGL.GL_LIGHT2, OpenGL.GL_LIGHT3 };
            
            // 조명 위치
            float[][] lightOpos = new float[][] { new float[] { 0f, 0f, 400f, 1.0f }, new float[] { 0f, -400f, 0f, 1.0f }, new float[] { 4000f, 0f, 0f, 1.0f }, new float[] { -4000f, 0f, 0f, 1.0f } };
            
            for (int i = 0; i < 4; i++)
            {
                gl.Light(lights[i], OpenGL.GL_POSITION, lightOpos[i]);
            }

            // 조명 활성화
            gl.Enable(OpenGL.GL_LIGHTING);
            // 조명 ON
            for (int i = 0; i < 4; i++)
            {
                if (i != 0) continue;
                gl.Enable(lights[i]);
            }
            
        }

        private void DrawAxis(OpenGL gl)
        {
            gl.LineWidth(2);
            gl.Begin(OpenGL.GL_LINES);

            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 0.0f, 0.0f);
            gl.Vertex(1000.0f, 0.0f, 0.0f);

            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(0.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 1000.0f, 0.0f);

            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(0.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 0.0f, 1000.0f);

            Vector3D near = MainWindowViewModel.MouseRay.getNear();
            Vector3D far = MainWindowViewModel.MouseRay.getFar();
            Vector3D hitPos = MainWindowViewModel.MouseRay.getHitPos();

            gl.Color(0.0f, 0.0f, 0.0f);
            gl.Vertex(near.X, near.Y, near.Z);
            gl.Vertex(far.X, far.Y, far.Z);

            gl.End();

            /*int hitFace = MainWindowViewModel.MouseRay.getHitFace();

            if (hitFace != -1)
            {
                Triangle t = MainWindowViewModel.Triangles[hitFace];
                int[] indices = t.getIndices();

                gl.LineWidth(2);
                gl.Begin(OpenGL.GL_LINES);
                gl.Color(1.0f, 0.0f, 0.0f);

                foreach (int ind in indices)
                {
                    Node n = MainWindowViewModel.Nodes[ind];

                    gl.Vertex(n.getPosition());
                }
                gl.End();
            }*/
        }

        private void DrawClicked(OpenGL gl)
        {
            int hitFace = MainWindowViewModel.MouseRay.getHitFace();

            if (hitFace != -1)
            {
                Triangle t = MainWindowViewModel.Triangles[hitFace];
                int[] indices = t.getIndices();

                gl.LineWidth(10);
                gl.Begin(OpenGL.GL_LINES);
                gl.Color(1.0f, 0.0f, 0.0f);

                foreach (int ind in indices)
                {
                    Node n = MainWindowViewModel.Nodes[ind];

                    gl.Vertex(n.getPosition());
                }
                gl.End();
            }
        }

        /// <summary>
        /// Handles the OpenGLDraw event of the OpenGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void OpenGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            Point pos = Mouse.GetPosition(this);

            Point actualPos = new Point(
                    pos.X - this.ActualWidth / 2,
                    pos.Y - this.ActualHeight / 2);

            MainWindowViewModel.MousePosition = pos;

            OpenGL gl = args.OpenGL;

            // 후면 제거
            if (cullfaceCheckBox.IsChecked == true)
            {
                gl.Enable(OpenGL.GL_CULL_FACE);
            }
            else
            {
                gl.Disable(OpenGL.GL_CULL_FACE);
            }

            // Clear The Color And The Depth Buffer, Current Matrix
            gl.ClearColor(255f, 255f, 255f, 0.0f);
            //gl.ClearDepth(1.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();

            SetLighting(gl);

            // Move Left And Into The Screen(object)
            gl.Translate(MainWindowViewModel.TranslationX, MainWindowViewModel.TranslationY, MainWindowViewModel.TranslationZ);

            double scale = MainWindowViewModel.Scale;
            gl.Scale(scale, scale, scale);
            
            gl.Rotate(MainWindowViewModel.RotationAngle, MainWindowViewModel.RotationAxis.X, MainWindowViewModel.RotationAxis.Y, MainWindowViewModel.RotationAxis.Z);

            

            if (MainWindowViewModel.Triangles != null && MainWindowViewModel.Nodes != null)
            {
                gl.InitNames();
                gl.PushName(0);

                gl.Begin(OpenGL.GL_TRIANGLES);                  // Start Drawing

                // Color Mode
                if (coloringCheckBox.IsChecked == true)
                {
                    // Vertex normal
                    if (vertexNormalRadioButton.IsChecked == true)
                    {
                        int count = MainWindowViewModel.N_triangle;
                        for (int i = 0; i < count; i++)
                        {
                            Triangle t = MainWindowViewModel.Triangles[i];
                            int[] indices = t.getIndices();

                            gl.LoadName((uint)i);

                            foreach (int ind in indices)
                            {
                                Node n = MainWindowViewModel.Nodes[ind];

                                Vector3D normal = n.getNormal();

                                gl.Normal(normal.X, normal.Y, normal.Z);
                                gl.Color(n.getProperties());
                                gl.Vertex(n.getPosition());
                            }
                        }
                    }
                    // Face normal
                    else
                    {
                        int count = MainWindowViewModel.N_triangle;
                        for (int i = 0; i < count; i++)
                        {
                            Triangle t = MainWindowViewModel.Triangles[i];
                            int[] indices = t.getIndices();

                            Vector3D normal = t.getNormal();

                            gl.LoadName((uint)i);
                            gl.Normal(normal.X, normal.Y, normal.Z);

                            foreach (int ind in indices)
                            {
                                Node n = MainWindowViewModel.Nodes[ind];
                                
                                gl.Color(n.getProperties());
                                gl.Vertex(n.getPosition());
                            }
                        }
                    }
                }
                // NonColor Mode
                else
                {
                    gl.Color(0.5f, 0.5f, 0.5f);
                    // Vertex normal
                    if (vertexNormalRadioButton.IsChecked == true)
                    {
                        int count = MainWindowViewModel.N_triangle;
                        for (int i = 0; i < count; i++)
                        {
                            Triangle t = MainWindowViewModel.Triangles[i];
                            int[] indices = t.getIndices();
                            
                            gl.LoadName((uint)i);

                            foreach (int ind in indices)
                            {
                                Node n = MainWindowViewModel.Nodes[ind];

                                Vector3D normal = n.getNormal();
                                
                                gl.Normal(normal.X, normal.Y, normal.Z);
                                gl.Vertex(n.getPosition());
                            }
                        }
                    }

                    // Face normal
                    else
                    {
                        int count = MainWindowViewModel.N_triangle;
                        for (int i = 0; i < count; i++)
                        {
                            Triangle t = MainWindowViewModel.Triangles[i];
                            int[] indices = t.getIndices();
                            Vector3D normal = t.getNormal();

                            gl.LoadName((uint)i);
                            gl.Normal(normal.X, normal.Y, normal.Z);

                            foreach (int ind in indices)
                            {
                                Node n = MainWindowViewModel.Nodes[ind];
                                
                                gl.Vertex(n.getPosition());
                            }
                        }
                    }
                }
                
                gl.End();
            }

            DrawAxis(gl);
        }

        // mouse 입력 정보
        private bool mLeftDown;
        private bool mRightDown;
        private Point mLastPos;   

        private void OpenGlControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mLeftDown && !mRightDown) return;

            Point pos = Mouse.GetPosition(this);

            Point actualPos = new Point(
                    pos.X - this.ActualWidth / 2,
                    pos.Y - this.ActualHeight / 2);

            double dx = actualPos.X - mLastPos.X;
            double dy = mLastPos.Y - actualPos.Y;

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
            

            if (mLeftDown && !mRightDown)
            {
                MainWindowViewModel.RotationAxis = axis;
                MainWindowViewModel.RotationAngle += (float)(rotation * 180 / Math.PI);
            }

            else if (!mLeftDown && mRightDown)
            {
                MainWindowViewModel.TranslationX += (float)dx / 100;
                MainWindowViewModel.TranslationY += (float)dy / 100;
            }

            mLastPos = actualPos;
        }

        private void OpenGlControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                MainWindowViewModel.TranslationZ += 0.5f;
            }

            mLeftDown = true;

            Point pos = Mouse.GetPosition(this);

            mLastPos = new Point(
                    pos.X - this.ActualWidth / 2,
                    pos.Y - this.ActualHeight / 2);

            // 더블클릭 했을 때 클릭 위치 메세지 팝업
            if (e.ClickCount == 2)
            {
                Vector3D near = ConvertMouseToCoordinate(0.0f);
                Vector3D far = ConvertMouseToCoordinate(1.0f);
                Vector3D direction = far - near;
                direction.Normalize();

                Ray ray = MainWindowViewModel.MouseRay = new Ray(near, far, direction);
                float r;
                float minR = ray.getMinR();
                Vector3D point;
                int curFace = -1;
                Vector3D hitPoint = ray.getHitPos();
                int[] indices = new int[3];

                for (int i = 0; i < MainWindowViewModel.N_triangle; i++)
                {
                    Triangle t = MainWindowViewModel.Triangles[i];

                    bool isIntersect = IsRayTriIntersect(t.getIndices(), t.getNormal(), ray.getNear(), ray.getDirection(), out point, out r);

                    if (isIntersect & (r < minR))
                    {
                        minR = r;
                        curFace = i;
                        hitPoint = point;
                        indices = t.getIndices();
                    }
                }

                if (curFace != -1)
                {
                    ray.setMinR(minR);
                    ray.setHitPos(hitPoint);
                    ray.setHitFace(curFace);

                    MessageBox.Show(near.ToString() + " " + far.ToString() + "\n" + curFace.ToString() + " " + indices[0] + "/" + indices[1] + "/" + indices[2] + "\n" + Math.Round(hitPoint.X, 2) + " " + Math.Round(hitPoint.Y, 2) + " " + Math.Round(hitPoint.Z, 2) + "\n", "pos", MessageBoxButton.OK);

                    OpenGL gl = this.openGlControl.OpenGL;
                    //DrawClicked(gl);
                }

                mLeftDown = false;
            }
        }

        private void OpenGlControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed) return;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                //if (MainWindowViewModel.TranslationZ > 0.02f)
                    MainWindowViewModel.TranslationZ -= 0.5f;
                return;
            }

            mRightDown = true;

            Point pos = Mouse.GetPosition(this);
            mLastPos = new Point(
                    pos.X - this.ActualWidth / 2,
                    pos.Y - this.ActualHeight / 2);
        }

        private void OpenGlControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mLeftDown = false;
            
        }

        private void OpenGlControl_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            mRightDown = false;
        }

        private Vector3D ConvertMouseToCoordinate(float mouseZ)
        {
            OpenGL gl = this.openGlControl.OpenGL;

            Point position = Mouse.GetPosition(this.openGlControl);
            //Point position = Mouse.GetPosition(this);

            int[] viewport = new int[4];
            double[] modelMatrix = new double[16];
            double[] projectionMatrix = new double[16];

            gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);
            gl.GetDouble(OpenGL.GL_MODELVIEW_MATRIX, modelMatrix);
            gl.GetDouble(OpenGL.GL_PROJECTION_MATRIX, projectionMatrix);

            float winX = (float)position.X;
            float winY = (float)(viewport[3] - position.Y);

            double x, y, z;
            x = y = z = 0;
            gl.UnProject(winX, winY, mouseZ, modelMatrix, projectionMatrix, viewport, ref x, ref y, ref z);

            return new Vector3D(x, y, z);
        }
        
        private static float SMALL_NUM = 0.00000001f;

        private bool IsRayTriIntersect(int[] indices, Vector3D normal, Vector3D nearPoint, Vector3D direction, out Vector3D hitPoint, out float r)
        {
            hitPoint = new Vector3D(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
            r = float.NegativeInfinity;

            // check if ray and triangle are parallel
            float dot = (float)Vector3D.DotProduct(normal, direction);
            if (dot > -SMALL_NUM && dot < SMALL_NUM)
            {
                return false;
            }

            Vertex p0 = MainWindowViewModel.Nodes[indices[0]].getPosition();
            Vertex p1 = MainWindowViewModel.Nodes[indices[1]].getPosition();
            Vertex p2 = MainWindowViewModel.Nodes[indices[2]].getPosition();

            Vector3D v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            Vector3D v1 = new Vector3D(p2.X - p0.X, p2.Y - p0.Y, p2.Z - p0.Z);

            // r = (normal`PA) / (normal`direction)
            Vector3D q = new Vector3D(p0.X - nearPoint.X, p0.Y - nearPoint.Y, p0.Z - nearPoint.Z);
            r = (float)Vector3D.DotProduct(normal, q) / dot;

            // check if triangle is at the back
            if (r < 0)
            {
                r = float.NegativeInfinity;
                return false;
            }

            hitPoint = nearPoint + r * direction;

            Vector3D v2 = new Vector3D(hitPoint.X - p0.X, hitPoint.Y - p0.Y, hitPoint.Z - p0.Z);

            Vector3D u = Vector3D.CrossProduct(v1, v2);
            if (Vector3D.DotProduct(-normal, u) < 0)
            {
                hitPoint = new Vector3D(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
                r = float.NegativeInfinity;
                return false;
            }

            Vector3D v = Vector3D.CrossProduct(v0, v2);
            if (Vector3D.DotProduct(normal, v) < 0)
            {
                hitPoint = new Vector3D(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
                r = float.NegativeInfinity;
                return false;
            }

            float denom = (float)normal.Length;
            float ul = (float)u.Length / denom;
            float vl = (float)v.Length / denom;

            if (ul + vl > 1)
            {
                hitPoint = new Vector3D(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
                r = float.NegativeInfinity;
                return false;
            }

            return true;
            
            /*Vector3D p = Vector3D.CrossProduct(direction, v1);
            float a = (float)Vector3D.DotProduct(v0, p);
            if (a > -SMALL_NUM && a < SMALL_NUM)
            { 
                return false;
            }

            float f = 1 / a;
            Vector3D s = nearPoint - new Vector3D(p0.X, p1.X, p2.X);
            float u = f * (float)Vector3D.DotProduct(s, p);
            if (u < 0 || u > 1)
            {
                return false;
            }

            Vector3D q = Vector3D.CrossProduct(s, v0);
            float v = f * (float)Vector3D.DotProduct(direction, q);
            if (v < 0 || u + v > 1)
            {
                return false;
            }

            r = f * (float)Vector3D.DotProduct(v1, q);
            hitPoint = nearPoint + r * direction;
            return true;*/
        }
    }
}
