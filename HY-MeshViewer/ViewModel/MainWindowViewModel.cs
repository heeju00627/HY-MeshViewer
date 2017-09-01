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
using HY_MeshViewer.View;

namespace HY_MeshViewer.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Mesh _mesh;

        public MainWindowViewModel()
        {
            _mesh = new Mesh
            {
                FileName = "no file",

                N_index = 0,
                N_node = 0,
                N_property = 0,
                N_triangle = 0,

                RotationAxis = new Vector3D(0, 0, 0),
                RotationAngle = 0,

                TranslationX = 0,
                TranslationY = 0,
                TranslationZ = -6.0f,

                Scale = 0.02f,
            };

            OpenCommand = new RelayCommand(OpenFile);
            CloseCommand = new RelayCommand(CloseFile);
            ResetCommand = new RelayCommand(ResetProperty);
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
        public Dictionary<int, Triangle> Triangles
        {
            get { return this._mesh.Triangles; }
            set
            {
                this._mesh.Triangles = value;
                NotifyPropertyChanged("Triangles");
            }
        }

        public Point MousePosition
        {
            get { return this._mesh.MousePosition; }
            set
            {
                this._mesh.MousePosition = value;
                NotifyPropertyChanged("MousePosition");
            }
        }

        public Ray MouseRay
        {
            get { return this._mesh.MouseRay; }
            set
            {
                this._mesh.MouseRay = value;
                NotifyPropertyChanged("MouseRay");
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

        public Vector3D RotationAxis
        {
            get { return this._mesh.RotationAxis; }
            set
            {
                this._mesh.RotationAxis = value;
                NotifyPropertyChanged("RotationAxis");
            }
        }

        public float RotationAngle
        {
            get { return this._mesh.RotationAngle; }
            set
            {
                this._mesh.RotationAngle = value;
                NotifyPropertyChanged("RotationAngle");
            }
        }

        public float TranslationX
        {
            get { return this._mesh.TranslationX; }
            set
            {
                this._mesh.TranslationX = value;
                NotifyPropertyChanged("TranslationX");
            }
        }

        public float TranslationY
        {
            get { return this._mesh.TranslationY; }
            set
            {
                this._mesh.TranslationY = value;
                NotifyPropertyChanged("TranslationY");
            }
        }

        public float TranslationZ
        {
            get { return this._mesh.TranslationZ; }
            set
            {
                this._mesh.TranslationZ = value;
                NotifyPropertyChanged("TranslationZ");
            }
        }

        public double Scale
        {
            get { return this._mesh.Scale; }
            set
            {
                this._mesh.Scale = value;
                NotifyPropertyChanged("Scale");
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

        public ICommand ResetCommand
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
                    CloseFile();
                    return;
                }

                N_node = Int32.Parse(tmp[0]);
                N_triangle = Int32.Parse(tmp[1]);
                N_property = Int32.Parse(tmp[2]);
                N_index = Int32.Parse(tmp[3]);

                Nodes = new Dictionary<int, Node>();
                Triangles = new Dictionary<int, Triangle>();

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

                    int[] indices = t.getIndices();

                    foreach (int ind in indices)
                    {
                        Nodes[ind].setConfaces(i);
                    }

                    Vector3D normal = CalculateFaceNormal(indices);
                    t.setNormal(normal);

                    Triangles.Add(i, t);
                }

                CalculateVertexNormal();

                ResetProperty();
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

            TranslationX = 0;
            TranslationY = 0;
            TranslationZ = -6.0f;

            RotationAxis = new Vector3D(0, 0, 0);
            RotationAngle = 0;

            Scale = 0.02f;

            MouseRay = new Ray();
        }

        private void ResetProperty()
        {
            TranslationX = 0;
            TranslationY = 0;
            TranslationZ = -6.0f;

            RotationAxis = new Vector3D(0, 0, 0);
            RotationAngle = 0;

            Scale = 0.02f;
        }

        private Vector3D CalculateFaceNormal(int[] indices)
        {
            Vector3D normal;

            Vertex p0 = Nodes[indices[0]].getPosition();
            Vertex p1 = Nodes[indices[1]].getPosition();
            Vertex p2 = Nodes[indices[2]].getPosition();

            Vector3D v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            Vector3D v1 = new Vector3D(p2.X - p0.X, p2.Y - p0.Y, p2.Z - p0.Z);

            normal = Vector3D.CrossProduct(v0, v1);

            return normal;
        }

        private void CalculateVertexNormal()
        {
            for (int i = 0; i < N_node; i++)
            {
                Node n = Nodes[i];

                List<int> cons = n.getConfaces();
                if (cons.Count() == 1)
                {
                    int t = cons.First();
                    n.setNormal(Triangles[t].getNormal());
                }
                else
                {
                    Vector3D normal = new Vector3D(0, 0, 0);
                    foreach (int c in cons)
                    {
                        normal += Triangles[c].getArea() * Triangles[c].getNormal();
                    }

                    n.Normalize(normal);
                }
                Nodes[i] = n;
            }
        }
    }
}