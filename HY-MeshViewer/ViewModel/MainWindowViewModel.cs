using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;
using Microsoft.Win32;
using System.IO;
using HY_MeshViewer.Model;
using System.Windows;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.Enumerations;

namespace HY_MeshViewer.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private object _propertyView;
        private object _viewerView;

        private Mesh _mesh;

        public MainWindowViewModel()
        {
            PropertyView = new PropertyControlViewModel();
            ViewerView = new ViewerControlViewModel();

            _mesh = new Mesh
            {
                FileName = "no file",

                N_index = 0,
                N_node = 0,
                N_property = 0,
                N_triangle = 0,

                RotationX = 0,
                RotationY = 0,
                RotationZ = 0
            };

            OpenCommand = new RelayCommand(OpenFile);
            CloseCommand = new RelayCommand(CloseFile);
        }

        public object PropertyView
        {
            get { return this._propertyView; }
            set
            {
                _propertyView = value;
                NotifyPropertyChanged("PropertyView");
            }
        }

        public object ViewerView
        {
            get { return this._viewerView; }
            set
            {
                _viewerView = value;
                NotifyPropertyChanged("ViewerView");
            }
        }

        public Mesh Mesh
        {
            get { return this._mesh; }
            set
            {
                _mesh = value;
                NotifyPropertyChanged("Mesh");
            }
        }
        
        public string FileName
        {
            get { return this._mesh.FileName; }
            set
            {
                this._mesh.FileName = value;
                NotifyPropertyChanged("FileName");
            }
        }

        public Dictionary<int, Node> Nodes
        {
            get { return this._mesh.Nodes; }
            set
            {
                this._mesh.Nodes = value;
                NotifyPropertyChanged("Nodes");
            }
        }
        public List<Triangle> Triangles
        {
            get { return this._mesh.Triangles; }
            set
            {
                this._mesh.Triangles = value;
                NotifyPropertyChanged("Triangles");
            }
        }

        public int N_node
        {
            get { return this._mesh.N_node; }
            set
            {
                this._mesh.N_node = value;
                NotifyPropertyChanged("N_node");
            }
        }
        public int N_triangle
        {
            get { return this._mesh.N_triangle; }
            set
            {
                this._mesh.N_triangle = value;
                NotifyPropertyChanged("N_triangle");
            }
        }
        public int N_property
        {
            get { return this._mesh.N_property; }
            set
            {
                this._mesh.N_property = value;
                NotifyPropertyChanged("N_property");
            }
        }
        public int N_index
        {
            get { return this._mesh.N_index; }
            set
            {
                this._mesh.N_index = value;
                NotifyPropertyChanged("N_index");
            }
        }

        #region Command
        public ICommand OpenCommand
        {
            get;
            set;
        }

        public ICommand CloseCommand
        {
            get;
            set;
        }
        
        public sealed class RelayCommand : ICommand
        {
            private Action function;
            public RelayCommand(Action function)
            {
                this.function = function;
            }
            public bool CanExecute(object parameter)
            {
                return this.function != null;
            }
            public void Execute(object parameter)
            {
                if (this.function != null)
                {
                    this.function();
                }
            }
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }
        }

        /*private ICommand mUpdater;
        public ICommand UpdateCommand
        {
            get
            {
                if (mUpdater == null)
                    mUpdater = new Updater();
                return mUpdater;
            }
            set
            {
                mUpdater = value;
            }
        }

        public class Updater : ICommand
        {
            #region ICommand Members
            public bool CanExecute(object parameter)
            {
                return true;
            }
            public event EventHandler CanExecuteChanged;
            public void Execute(object parameter)
            {

            }
            #endregion
        }*/

        #endregion

        private void OpenFile()
        {
            if (FileName != "no file")
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
                FileName = openDlg.FileName;

                String[] data = File.ReadAllLines(openDlg.FileName);

                // 첫째줄 데이터 정보
                String[] tmp = data[0].Split(' ');

                // 올바른 file 아님
                if (tmp.Length != 4)
                {
                    MessageBox.Show("wrong file", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                N_node = Int32.Parse(tmp[0]);
                N_triangle = Int32.Parse(tmp[1]);
                N_property = Int32.Parse(tmp[2]);
                N_index = Int32.Parse(tmp[3]);

                Nodes = new Dictionary<int, Node>();
                Triangles = new List<Triangle>();

                for (int i = 0; i < N_node; i++)
                {
                    tmp = data[i + 1].Split('\t');

                    Node n = new Node(tmp, N_property);

                    Nodes.Add(i, n);
                }

                for (int i = 0; i < N_triangle; i++)
                {
                    tmp = data[i + 1 + N_node].Split('\t');

                    Triangle t = new Triangle(tmp, N_index);

                    Triangles.Add(t);
                }
            }
        }

        private void CloseFile()
        {
            FileName = "no file";

            N_node = 0;
            N_triangle = 0;
            N_property = 0;
            N_index = 0;

            Nodes = null;
            Triangles = null;
        }
    }
}
