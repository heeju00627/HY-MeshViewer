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
        int n_node;
        int n_face;
        int n_property;
        int n_index;

        Point3D[] nodes;
        int[][] faces;
        double[][] properties;

        private bool mDown;
        private Point mLastPos;

        public ViewerControl()
        {
            InitializeComponent();
        }

        private void ZoomSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        private void XSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        private void YSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        private void ZSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        private void OnGridMouseWheel(object sender, MouseWheelEventArgs e)
        {
        }

        private void OnGridMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void OnGridMouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void OnGridMouseMove(object sender, MouseEventArgs e)
        {
        }
    }
}
