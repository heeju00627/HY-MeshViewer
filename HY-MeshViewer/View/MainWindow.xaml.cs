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
        /* menu click event */
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
    }
}
