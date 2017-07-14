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

namespace HY_MeshViewer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {

        int n_node;
        int n_face;
        int n_property;
        int n_index;

        Point3D[] nodes;
        int[][] faces;
        double[][] properties;

        private bool mDown;
        private Point mLastPos;


        public MainWindow()
        {
            InitializeComponent();

            PropertyMdiContainer.Children.Add(new MdiChild()
            {
                Title = "Property Box",
                Name = "PropertyBox",
                //Here UserRegistration is the class that you have created for mainWindow.xaml user control. 
                //Content = new UserRegistration()
                Content = new PropertyControl(),
                MaxWidth = 200,
                WindowState = WindowState.Maximized,
            });
        }


        /* ------------------------------------------------------------------------------------------------ */
        /* menu click event */

        private void SubmenuOpen_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openDlg = new OpenFileDialog();

            //openDlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            // TODO
            // type이 맞는지 체크하는 부분 추가
            if (openDlg.ShowDialog() == true)
            {
                String[] data = File.ReadAllLines(openDlg.FileName);

                String[] tmp = data[0].Split(' ');

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

            //MainMdiContainer.Children.Clear();

            ViewerMdiContainer.Children.Add(new MdiChild()
            {
                Title = openDlg.FileName,
                //Here UserRegistration is the class that you have created for mainWindow.xaml user control. 
                Content = new ViewerControl(),
                WindowState = WindowState.Normal,
                Width = 1000,
                Height = 600,
            });

            /** show property bar */
            PropertyMdiContainer.Visibility = Visibility.Visible;
            submenuPropertybox.IsChecked = true;

        }

        private void SubmenuExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void SubmenuToolbar_Click(object sender, RoutedEventArgs e)
        {
            if (Toolbar.Visibility == Visibility.Visible)
            {
                Toolbar.Visibility = Visibility.Collapsed;
                zeroRow.Height = new GridLength(18);
                submenuToolbar.IsChecked = false;
            }
            else if (Toolbar.Visibility == Visibility.Collapsed)
            {
                Toolbar.Visibility = Visibility.Visible;
                zeroRow.Height = new GridLength(45);
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
            if (PropertyMdiContainer.Visibility == Visibility.Visible)
            {
                PropertyMdiContainer.Visibility = Visibility.Collapsed;
                submenuPropertybox.IsChecked = false;
            }
            else if (PropertyMdiContainer.Visibility == Visibility.Collapsed)
            {
                PropertyMdiContainer.Visibility = Visibility.Visible;
                submenuPropertybox.IsChecked = true;
            }
        }
    }
}
