﻿using System;
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

namespace HY_MeshViewer
{
    /// <summary>
    /// PropertyControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PropertyControl : UserControl
    {
        public PropertyControl()
        {
            InitializeComponent();
        }

        private void ResetButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void ColorButtonClick(object sender, RoutedEventArgs e)
        {
        }

        private void comboBoxPolygonMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*if (openGlCtrl == null || openGlCtrl.OpenGL == null)
                return;

            switch (polygonModeComboBox.SelectedIndex)
            {
                case 0:
                    openGlCtrl.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Points);
                    break;
                case 1:
                    openGlCtrl.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);
                    break;
                case 2:
                    openGlCtrl.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled);
                    break;
            }*/
        }
    }
}
