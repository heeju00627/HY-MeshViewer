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


/*
 * <TextBlock Text="X Position:"/>
                        <TextBox Name="positionXTextBox" HorizontalAlignment="Stretch" TextAlignment="Center" Text="{Binding Path=TranslationX}"/>
                        <TextBlock Text="Y Position:"/>
                        <TextBox Name="positionYTextBox" HorizontalAlignment="Stretch" TextAlignment="Center" Text="{Binding Path=TranslationY}"/>
                        <TextBlock Text="Z Position:"/>
                        <TextBox Name="positionZTextBox" HorizontalAlignment="Stretch" TextAlignment="Center" Text="{Binding Path=TranslationZ}"/>
                        <Separator/>
                        <TextBlock Text="Rotation Angle:"/>
                        <TextBox Name="rotationAngleTextBox" HorizontalAlignment="Stretch" TextAlignment="Center" Text="{Binding Path=RotationAngle}"/>
                        <TextBlock Text="Rotation X:"/>
                        <TextBox Name="rotationXTextBox" HorizontalAlignment="Stretch" TextAlignment="Center" Text="{Binding Path=RotationAxis.X}"/>
                        <TextBlock Text="Rotation Y:"/>
                        <TextBox Name="rotationYTextBox" HorizontalAlignment="Stretch" TextAlignment="Center" Text="{Binding Path=RotationAxis.Y}"/>
                        <TextBlock Text="Rotation Z:"/>
                        <TextBox Name="rotationZTextBox" HorizontalAlignment="Stretch" TextAlignment="Center" Text="{Binding Path=RotationAxis.Z}"/>
                        <TextBlock Text="Scale:"/>
                        <TextBox Name="scaleTextBox" HorizontalAlignment="Stretch" TextAlignment="Center" Text="{Binding Path=Scale}"/>
 * 
 * <CheckBox Name="coloringCheckBox" Content="Coloring"></CheckBox>
                        <ComboBox x:Name="propertyColorComboBox" Width="100" ItemsSource="{Binding Path=Properties}" DisplayMemberPath="name" SelectedValuePath="value"></ComboBox>
                        <Border Background="{Binding ElementName=CB, Path=Brush}"/>
                        <ncore:ColorBox x:Name="colorBox" Grid.Column="1" Margin="10, 0, 0, 0" VerticalAlignment="Top"></ncore:ColorBox>
                        <Separator /> */

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
            if (this.openGlControl1 == null || this.openGlControl1.OpenGL == null)
                return;

            switch (polygonModeComboBox.SelectedIndex)
            {
                case 0:
                    this.openGlControl1.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Points);
                    this.openGlControl2.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Points);
                    break;
                case 1:
                    this.openGlControl1.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);
                    this.openGlControl2.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);
                    break;
                case 2:
                    this.openGlControl1.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled);
                    this.openGlControl2.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled);
                    break;
            }
        }

        private void comboBoxNormalMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.openGlControl1 == null || this.openGlControl1.OpenGL == null)
                return;

            switch (normalModeComboBox.SelectedIndex)
            {
                case 0:
                    this.MainWindowViewModel.Mode_normal = 0;
                    break;
                case 1:
                    this.MainWindowViewModel.Mode_normal = 1;
                    break;
            }
        }

        private void ElectrodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (ElectrodeControl.Visibility == Visibility.Visible)
            {
                electrodeButton.Content = "Electrode>>";
                ContentGrid.Children.Clear();
                ContentGrid.RowDefinitions.Clear();
                ElectrodeControl.Visibility = Visibility.Collapsed;
            }
            else if (ElectrodeControl.Visibility == Visibility.Collapsed)
            {
                if (MainWindowViewModel.N_electrode == 0)
                {
                    return;
                }
                else
                {
                    electrodeButton.Content = "<<Electrode";
                    SetElectrodeControl();
                    ElectrodeControl.Visibility = Visibility.Visible;
                }
            }
        }

        /* ------------------------------------------------------------------------------------------------ */
        /* electrode control */

        public void SetElectrodeControl()
        {
            for (int i = 0; i < MainWindowViewModel.N_electrode; i++)
            {
                RowDefinition r = new RowDefinition();
                ContentGrid.RowDefinitions.Add(r);
            }

            for (int i = 0; i < MainWindowViewModel.N_electrode; i++)
            {
                TextBlock t = new TextBlock();
                TextBox tbox = new TextBox();
                
                t.Text = (i + 1).ToString();
                t.VerticalAlignment = VerticalAlignment.Center;
                t.HorizontalAlignment = HorizontalAlignment.Left;

                //tbox.Margin = new Thickness(5);
                tbox.MaxHeight = 20;
                tbox.MinHeight = 20;
                tbox.MinWidth = 40;
                tbox.MaxWidth = 40;
                tbox.Text = MainWindowViewModel.Values_electrode[i].ToString();
                tbox.HorizontalAlignment = HorizontalAlignment.Center;

                System.Windows.Controls.Grid.SetRow(t, i);
                System.Windows.Controls.Grid.SetColumn(t, 0);
                System.Windows.Controls.Grid.SetRow(tbox, i);
                System.Windows.Controls.Grid.SetColumn(tbox, 1);

                ContentGrid.Children.Add(t);
                ContentGrid.Children.Add(tbox);
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            double sum = 0.0;
            for (int i = 0; i < MainWindowViewModel.N_electrode; i++)
            {
                TextBox tb = (TextBox)ContentGrid.Children[i * 2 + 1];
                sum += Double.Parse(tb.Text);
            }

            if (sum == 0.0)
            {
                for (int i = 0; i < MainWindowViewModel.N_electrode; i++)
                {
                    TextBox tb = (TextBox)ContentGrid.Children[i * 2 + 1];
                    MainWindowViewModel.Values_electrode[i] = Double.Parse(tb.Text);
                }
                return;
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("합이 0이 되야 합니다.", "Try Again", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ResetValuesButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < MainWindowViewModel.N_electrode; i++)
            {
                MainWindowViewModel.Values_electrode[i] = 0.0;
                TextBox tb = (TextBox)ContentGrid.Children[i * 2 + 1];
                tb.Text = "0";
            }
        }

        /* ------------------------------------------------------------------------------------------------ */
        /* viewer control */

        private void OpenGLControl_OpenGLInitialized1(object sender, OpenGLEventArgs args)
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
            float[][] lightOpos = new float[][] { new float[] { 0f, 0f, 10000f, 1.0f }, new float[] { 0f, -10000f, 0f, 1.0f }, new float[] { 10000f, 0f, 0f, 1.0f }, new float[] { -10000f, 0f, 0f, 1.0f } };

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
        }

        private void SetLighting(OpenGL gl)
        {
            uint[] lights = new uint[] { OpenGL.GL_LIGHT0, OpenGL.GL_LIGHT1, OpenGL.GL_LIGHT2, OpenGL.GL_LIGHT3 };
            
            // 조명 위치
            float[][] lightOpos = new float[][] { new float[] { 0f, 0f, 10000f, 1.0f }, new float[] { 0f, -10000f, 0f, 1.0f }, new float[] { 10000f, 0f, 0f, 1.0f }, new float[] { -4000f, 0f, 0f, 1.0f } };
            
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

        private void OpenGLControl_OpenGLInitialized2(object sender, OpenGLEventArgs args)
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
            float[][] lightOpos = new float[][] { new float[] { 0f, 0f, 10000f, 1.0f }, new float[] { 0f, -10000f, 0f, 1.0f }, new float[] { 10000f, 0f, 0f, 1.0f }, new float[] { -10000f, 0f, 0f, 1.0f } };

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

        private void DrawLabel(OpenGL gl)
        {
            int hitface = MainWindowViewModel.MouseRay.getHitFace();

            Triangle t;
            int[] indices;

            if (hitface != -1)
            {
                t = MainWindowViewModel.Elements_cortex[hitface];
                indices = t.getIndices();

                for (int i = 0; i < MainWindowViewModel.N_label; i++)
                {
                    if (MainWindowViewModel.Labels[i].isLabel(indices))
                    {
                        MainWindowViewModel.Hit_label = i;

                        string[] s = MainWindowViewModel.Labels[i].getInfo();

                        gl.DrawText((int)this.openGlControl2.RenderSize.Width - 300, (int)this.openGlControl2.RenderSize.Height - 20, 0, 0, 0, "Courier New", 13, "# name : " + s[0]);
                        gl.DrawText((int)this.openGlControl2.RenderSize.Width - 300, (int)this.openGlControl2.RenderSize.Height - 35, 0, 0, 0, "Courier New", 13, "# part : " + s[1]);
                        gl.DrawText((int)this.openGlControl2.RenderSize.Width - 300, (int)this.openGlControl2.RenderSize.Height - 50, 0, 0, 0, "Courier New", 13, "# n_node : " + s[2]);
                        return;
                    }
                }
            }
            else
            {
                gl.DrawText((int)this.openGlControl2.RenderSize.Width - 100, (int)this.openGlControl2.RenderSize.Height - 20, 0, 0, 0, "Courier New", 13, "????");
            }
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

        private void DrawElectrodeLabel()
        {
            if (MainWindowViewModel.FileName != "no file")
            {
                OpenGL gl = this.openGlControl1.OpenGL;

                int[] viewport = new int[4];
                double[] modelMatrix = new double[16];
                double[] projectionMatrix = new double[16];

                gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);
                gl.GetDouble(OpenGL.GL_MODELVIEW_MATRIX, modelMatrix);
                gl.GetDouble(OpenGL.GL_PROJECTION_MATRIX, projectionMatrix);

                for (int i = 0; i < MainWindowViewModel.N_electrode; i++)
                {
                    int n = MainWindowViewModel.Electrodes[i].getNode(0);
                    Vertex v = MainWindowViewModel.Nodes_head[n].getPosition();

                    double[] x, y, z;
                    x = y = z = new double[1];
                    
                    gl.Project(v.X, v.Y, v.Z, modelMatrix, projectionMatrix, viewport, x, y, z);
                    

                    gl.DrawText((int)(v.X * x[0]), (int)(v.Y * y[0]), 0, 0, 0, "Courier New", 25, "# elec : " + i);
                }
            }

            return;

            /*mat4 mvp = g_proj_mat * g_view_mat * g_model_mat;

            //billboard 같은 느낌으로 글자 쓰기
            //기울어지는거 없이 항상 글자가 뜨도록 적절히 만들기
            // rendering pipeline통과 후 좌표값
            vec4 cliping_pos = mvp * vec4(x, y, z, 1);
            cliping_pos /= cliping_pos.w;
            cliping_pos.z = -cliping_pos.z; //보정된 좌표계는 z방향 다르다

            // -1~+1로 보정된 좌표를 윈도우좌표로 변환
            vec3 win_coord(
              (cliping_pos.x + 1) * kWidth / 2.0f,
              (cliping_pos.y + 1) * kHeight / 2.0f,
              cliping_pos.z
    );

            glMatrixMode(GL_PROJECTION);
            glLoadIdentity();
            glOrtho(0, kWidth, 0, kHeight, -1, 1);

            glMatrixMode(GL_MODELVIEW);
            glLoadIdentity();
            glTranslatef(win_coord.x, win_coord.y, win_coord.z);

            glVertexPointer(2, GL_FLOAT, sizeof(FontVertex), &label.vertex_list()[0].p);
            glTexCoordPointer(2, GL_FLOAT, sizeof(FontVertex), &label.vertex_list()[0].uv);
            glDrawElements(GL_TRIANGLES, label.index_count(), GL_UNSIGNED_SHORT, label.index_data());

            {
                // restore 3d matrix
                glMatrixMode(GL_PROJECTION);
                glLoadIdentity();
                glLoadMatrixf(glm::value_ptr(g_proj_mat));

                glMatrixMode(GL_MODELVIEW);
                glLoadIdentity();
                glm::mat4 modelview = g_view_mat * g_model_mat;
                glLoadMatrixf(glm::value_ptr(modelview));
            }*/
        }

        /// <summary>
        /// Handles the OpenGLDraw event of the OpenGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void OpenGLControl_OpenGLDraw1(object sender, OpenGLEventArgs args)
        {
            Point pos = Mouse.GetPosition(this);

            Point actualPos = new Point(
                    pos.X - this.ActualWidth / 2,
                    pos.Y - this.ActualHeight / 2);

            MainWindowViewModel.MousePosition = pos;

            OpenGL gl = args.OpenGL;

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


            gl.LookAt(-6.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f);

            if (System.IO.Path.GetExtension(MainWindowViewModel.FileName) == ".txt")
            {
                if (MainWindowViewModel.Triangles != null && MainWindowViewModel.Nodes != null)
                {
                    gl.InitNames();
                    gl.PushName(0);

                    gl.Begin(OpenGL.GL_TRIANGLES);                  // Start Drawing

                    // Color Mode
                    if (coloringCheckBox.IsChecked == true)
                    {
                        // Vertex normal
                        if (this.MainWindowViewModel.Mode_normal == 1)
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
                        if (this.MainWindowViewModel.Mode_normal == 1)
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
            }

            else if (System.IO.Path.GetExtension(MainWindowViewModel.FileName) == ".bin")
            {
                if (MainWindowViewModel.Flag_surface == 1)
                {
                    gl.InitNames();
                    gl.PushName(0);

                    gl.Begin(OpenGL.GL_TRIANGLES);                  // Start Drawing

                    // Color Mode
                    if (coloringCheckBox.IsChecked == true)
                    {
                        // Vertex normal
                        if (this.MainWindowViewModel.Mode_normal == 1)
                        {
                            int count = MainWindowViewModel.Nele_scalp;
                            for (int i = 0; i < count; i++)
                            {
                                Triangle t = MainWindowViewModel.Elements_scalp[i];
                                int[] indices = t.getIndices();

                                gl.LoadName((uint)i);

                                foreach (int ind in indices)
                                {
                                    Node n = MainWindowViewModel.Nodes_scalp[ind];

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
                            int count = MainWindowViewModel.Nele_scalp;
                            for (int i = 0; i < count; i++)
                            {
                                Triangle t = MainWindowViewModel.Elements_scalp[i];
                                int[] indices = t.getIndices();

                                Vector3D normal = t.getNormal();

                                gl.LoadName((uint)i);
                                gl.Normal(normal.X, normal.Y, normal.Z);

                                foreach (int ind in indices)
                                {
                                    Node n = MainWindowViewModel.Nodes_scalp[ind];

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
                        if (this.MainWindowViewModel.Mode_normal == 1)
                        {
                            int count = MainWindowViewModel.Nele_scalp;
                            for (int i = 0; i < count; i++)
                            {
                                Triangle t = MainWindowViewModel.Elements_scalp[i];
                                int[] indices = t.getIndices();

                                gl.LoadName((uint)i);

                                foreach (int ind in indices)
                                {
                                    Node n = MainWindowViewModel.Nodes_scalp[ind];

                                    Vector3D normal = n.getNormal();

                                    gl.Normal(normal.X, normal.Y, normal.Z);
                                    gl.Vertex(n.getPosition());
                                }
                            }
                        }

                        // Face normal
                        else
                        {
                            int count = MainWindowViewModel.Nele_scalp;
                            for (int i = 0; i < count; i++)
                            {
                                Triangle t = MainWindowViewModel.Elements_scalp[i];
                                int[] indices = t.getIndices();
                                Vector3D normal = t.getNormal();

                                gl.LoadName((uint)i);
                                gl.Normal(normal.X, normal.Y, normal.Z);

                                foreach (int ind in indices)
                                {
                                    Node n = MainWindowViewModel.Nodes_scalp[ind];

                                    gl.Vertex(n.getPosition());
                                }
                            }
                        }
                    }

                    gl.End();
                }
            }

            DrawAxis(gl);

            DrawElectrodeLabel();
        }

        /// <summary>
        /// Handles the OpenGLDraw event of the OpenGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void OpenGLControl_OpenGLDraw2(object sender, OpenGLEventArgs args)
        {
            if (MainWindowViewModel.FileName == "no file" && ElectrodeControl.Visibility == Visibility.Visible)
            {
                electrodeButton.Content = "Electrode>>";
                ContentGrid.Children.Clear();
                ContentGrid.RowDefinitions.Clear();
                ElectrodeControl.Visibility = Visibility.Collapsed;
            }

            Point pos = Mouse.GetPosition(this);

            Point actualPos = new Point(
                    pos.X - this.ActualWidth / 2,
                    pos.Y - this.ActualHeight / 2);

            MainWindowViewModel.MousePosition = pos;

            OpenGL gl = args.OpenGL;

            /* 후면 제거
            if (cullfaceCheckBox.IsChecked == true)
            {
                gl.Enable(OpenGL.GL_CULL_FACE);
            }
            else
            {
                gl.Disable(OpenGL.GL_CULL_FACE);
            }*/

            // Clear The Color And The Depth Buffer, Current Matrix
            gl.ClearColor(255f, 255f, 255f, 0.0f);
            //gl.ClearDepth(1.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();

            SetLighting(gl);

            DrawLabel(gl);



            // Move Left And Into The Screen(object)
            gl.Translate(MainWindowViewModel.TranslationX, MainWindowViewModel.TranslationY, MainWindowViewModel.TranslationZ);

            double scale = MainWindowViewModel.Scale;
            gl.Scale(scale, scale, scale);

            gl.Rotate(MainWindowViewModel.RotationAngle, MainWindowViewModel.RotationAxis.X, MainWindowViewModel.RotationAxis.Y, MainWindowViewModel.RotationAxis.Z);


            gl.LookAt(-6.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f);

            if (System.IO.Path.GetExtension(MainWindowViewModel.FileName) == ".txt")
            {
                if (MainWindowViewModel.Triangles != null && MainWindowViewModel.Nodes != null)
                {
                    gl.InitNames();
                    gl.PushName(0);

                    gl.Begin(OpenGL.GL_TRIANGLES);                  // Start Drawing

                    // Color Mode
                    if (coloringCheckBox.IsChecked == true)
                    {
                        // Vertex normal
                        if (this.MainWindowViewModel.Mode_normal == 1)
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
                        if (this.MainWindowViewModel.Mode_normal == 1)
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
            }

            else if (System.IO.Path.GetExtension(MainWindowViewModel.FileName) == ".bin")
            {
                if (MainWindowViewModel.Flag_surface == 1)
                {
                    gl.InitNames();
                    gl.PushName(0);

                    gl.Begin(OpenGL.GL_TRIANGLES);                  // Start Drawing

                    // Color Mode
                    if (coloringCheckBox.IsChecked == true)
                    {
                        gl.Color(1.0f, 0.0f, 0.0f);
                        // Vertex normal
                        if (this.MainWindowViewModel.Mode_normal == 1)
                        {
                            int count = MainWindowViewModel.Nele_cortex;
                            for (int i = 0; i < count; i++)
                            {
                                Triangle t = MainWindowViewModel.Elements_cortex[i];
                                int[] indices = t.getIndices();

                                gl.LoadName((uint)i);

                                foreach (int ind in indices)
                                {
                                    Node n = MainWindowViewModel.Nodes_cortex[ind];

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
                            int count = MainWindowViewModel.Nele_cortex;
                            for (int i = 0; i < count; i++)
                            {
                                Triangle t = MainWindowViewModel.Elements_cortex[i];
                                int[] indices = t.getIndices();

                                Vector3D normal = t.getNormal();

                                gl.LoadName((uint)i);
                                gl.Normal(normal.X, normal.Y, normal.Z);

                                foreach (int ind in indices)
                                {
                                    Node n = MainWindowViewModel.Nodes_cortex[ind];

                                    gl.Color(n.getProperties());
                                    gl.Vertex(n.getPosition());
                                }
                            }
                        }
                    }
                    // NonColor Mode
                    else
                    {
                        int hitlabel = MainWindowViewModel.Hit_label;

                        gl.Color(0.5f, 0.5f, 0.5f);
                        // Vertex normal
                        if (this.MainWindowViewModel.Mode_normal == 1)
                        {
                            int count = MainWindowViewModel.Nele_cortex;
                            for (int i = 0; i < count; i++)
                            {
                                Triangle t = MainWindowViewModel.Elements_cortex[i];
                                int[] indices = t.getIndices();

                                gl.LoadName((uint)i);

                                if (hitlabel != -1 && MainWindowViewModel.Labels[hitlabel].isLabel(indices))
                                {
                                    gl.Color(0.0f, 0.0f, 0.0f);
                                }
                                else
                                {
                                    gl.Color(0.5f, 0.5f, 0.5f);
                                }

                                foreach (int ind in indices)
                                {
                                    Node n = MainWindowViewModel.Nodes_cortex[ind];

                                    Vector3D normal = n.getNormal();

                                    gl.Normal(normal.X, normal.Y, normal.Z);
                                    gl.Vertex(n.getPosition());
                                }
                            }
                        }

                        // Face normal
                        else
                        {
                            int count = MainWindowViewModel.Nele_cortex;
                            for (int i = 0; i < count; i++)
                            {
                                Triangle t = MainWindowViewModel.Elements_cortex[i];
                                int[] indices = t.getIndices();
                                Vector3D normal = t.getNormal();

                                gl.LoadName((uint)i);
                                gl.Normal(normal.X, normal.Y, normal.Z);

                                if (hitlabel != -1 && MainWindowViewModel.Labels[hitlabel].isLabel(indices))
                                {
                                    gl.Color(0.0f, 0.0f, 0.0f);
                                }
                                else
                                {
                                    gl.Color(0.5f, 0.5f, 0.5f);
                                }

                                foreach (int ind in indices)
                                {
                                    Node n = MainWindowViewModel.Nodes_cortex[ind];

                                    gl.Vertex(n.getPosition());
                                }
                            }
                        }
                    }

                    gl.End();
                }
            }

            DrawAxis(gl);
        }

        // 키보드 입력 정보
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A)
            {
                moveRadioButton.IsChecked = true;
            }
            else if (e.Key == Key.S)
            {
                rotateRadioButton.IsChecked = true;
            }
            else if (e.Key == Key.D)
            {
                zoomRadioButton.IsChecked = true;
            }
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
            

            if (mLeftDown && rotateRadioButton.IsChecked == true)
            {
                MainWindowViewModel.RotationAxis = axis;
                MainWindowViewModel.RotationAngle += (float)(rotation * 180 / Math.PI);
            }

            else if (mLeftDown && moveRadioButton.IsChecked == true)
            {
                MainWindowViewModel.TranslationX += (float)dx / 100;
                MainWindowViewModel.TranslationY += (float)dy / 100;
            }

            else if (mLeftDown && zoomRadioButton.IsChecked == true)
            {
                MainWindowViewModel.TranslationZ += (float)dy / 100;
            }

            mLastPos = actualPos;
        }

        private void OpenGlControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            //if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            //{
            //    MainWindowViewModel.TranslationZ += 0.5f;
            //}

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

                Ray ray = new Ray(near, far, direction);
                float r;
                float minR = ray.getMinR();
                Vector3D point;
                int curFace = -1;
                Vector3D hitPoint = ray.getHitPos();
                int[] indices = new int[3];

                if (System.IO.Path.GetExtension(MainWindowViewModel.FileName) == ".txt")
                {
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
                }
                else if (System.IO.Path.GetExtension(MainWindowViewModel.FileName) == ".bin")
                {
                    for (int i = 0; i < MainWindowViewModel.Nele_cortex; i++)
                    {
                        Triangle t = MainWindowViewModel.Elements_cortex[i];

                        bool isIntersect = IsRayTriIntersect(t.getIndices(), t.getNormal(), ray.getNear(), ray.getDirection(), out point, out r);

                        if (isIntersect & (r < minR))
                        {
                            minR = r;
                            curFace = i;
                            hitPoint = point;
                            indices = t.getIndices();
                        }
                    }
                }

                if (curFace != -1)
                {
                    ray.setMinR(minR);
                    ray.setHitPos(hitPoint);
                    ray.setHitFace(curFace);

                    //MessageBox.Show(near.ToString() + " " + far.ToString() + "\n" + curFace.ToString() + " " + indices[0] + "/" + indices[1] + "/" + indices[2] + "\n" + Math.Round(hitPoint.X, 2) + " " + Math.Round(hitPoint.Y, 2) + " " + Math.Round(hitPoint.Z, 2) + "\n", "pos", MessageBoxButton.OK);
                }
                else
                {
                    MainWindowViewModel.Hit_label = -1;
                }
                MainWindowViewModel.MouseRay = ray;

                mLeftDown = false;
            }
        }

        private void OpenGlControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed) return;

            //if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            //{  
            //    MainWindowViewModel.TranslationZ -= 0.5f;
            //    return;
            //}

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
            OpenGL gl = this.openGlControl2.OpenGL;

            Point position = Mouse.GetPosition(this.content2);
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

            Vertex p0 = new Vertex();
            Vertex p1 = new Vertex();
            Vertex p2 = new Vertex();

            if (System.IO.Path.GetExtension(MainWindowViewModel.FileName) == ".txt")
            {
                p0 = MainWindowViewModel.Nodes[indices[0]].getPosition();
                p1 = MainWindowViewModel.Nodes[indices[1]].getPosition();
                p2 = MainWindowViewModel.Nodes[indices[2]].getPosition();
            }
            else if (System.IO.Path.GetExtension(MainWindowViewModel.FileName) == ".bin")
            {
                p0 = MainWindowViewModel.Nodes_cortex[indices[0]].getPosition();
                p1 = MainWindowViewModel.Nodes_cortex[indices[1]].getPosition();
                p2 = MainWindowViewModel.Nodes_cortex[indices[2]].getPosition();
            }

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
