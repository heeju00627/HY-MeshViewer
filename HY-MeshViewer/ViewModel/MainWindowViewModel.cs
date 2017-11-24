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
using System.Windows.Controls;

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
                SubName = "",
                Flag_surface = 0,
                Flag_electrode = 0,

                N_index = 0,
                N_node = 0,
                N_property = 0,
                N_triangle = 0,

                Nnode_head = 0,
                Nnode_scalp = 0,
                Nnode_cortex = 0,
                Nnode_electrode = 0,
                N_electrode = 0,

                Nele_head = 0,
                Nele_scalp = 0,
                Nele_cortex = 0,

                RotationAxis = new Vector3D(0, 0, 0),
                RotationAngle = 0,

                TranslationX = 0.0f,
                TranslationY = 0.0f,
                TranslationZ = -6.0f,

                Scale = 0.02f,

                MouseRay = new Ray(-1),
                Properties = new List<ComboBoxItem>(),
            };

            OpenCommand = new RelayCommand(OpenFile);
            CloseCommand = new RelayCommand(CloseFile);
            ResetCommand = new RelayCommand(ResetProperty);
        }

        #region getset
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
        public string SubName
        {
            get { return this._mesh.SubName; }
            set
            {
                this._mesh.SubName = value;
                NotifyPropertyChanged("SubName");
            }
        }
        public int Flag_surface
        {
            get { return this._mesh.Flag_surface; }
            set
            {
                this._mesh.Flag_surface = value;
                NotifyPropertyChanged("Flag_surface");
            }
        }
        public int Flag_electrode
        {
            get { return this._mesh.Flag_electrode; }
            set
            {
                this._mesh.Flag_electrode = value;
                NotifyPropertyChanged("Flag_electrode");
            }
        }

        #region before..
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
        #endregion

        public int Mode_normal
        {
            get { return this._mesh.Mode_normal; }
            set
            {
                this._mesh.Mode_normal = value;
                NotifyPropertyChanged("Mode_normal");
            }
        }

        // VOLUME - HEAD
        public int Nnode_head
        {
            get { return this._mesh.Nnode_head; }
            set
            {
                this._mesh.Nnode_head = value;
                NotifyPropertyChanged("Nnode_head");
            }
        }
        public int Nele_head
        {
            get { return this._mesh.Nele_head; }
            set
            {
                this._mesh.Nele_head = value;
                NotifyPropertyChanged("Nele_head");
            }
        }
        public Dictionary<int, Node> Nodes_head
        {
            get { return this._mesh.Nodes_head; }
            set
            {
                this._mesh.Nodes_head = value;
                NotifyPropertyChanged("Nodes_head");
            }
        }
        public Dictionary<int, Tetrahedron> Elements_head
        {
            get { return this._mesh.Elements_head; }
            set
            {
                this._mesh.Elements_head = value;
                NotifyPropertyChanged("Elements_head");
            }
        }
        public int[] Regions_head
        {
            get { return this._mesh.Regions_head; }
            set
            {
                this._mesh.Regions_head = value;
                NotifyPropertyChanged("Regions_head");
            }
        }

        // SCALP SURFACE
        public int Nnode_scalp
        {
            get { return this._mesh.Nnode_scalp; }
            set
            {
                this._mesh.Nnode_scalp = value;
                NotifyPropertyChanged("Nnode_scalp");
            }
        }
        public int Nele_scalp
        {
            get { return this._mesh.Nele_scalp; }
            set
            {
                this._mesh.Nele_scalp = value;
                NotifyPropertyChanged("Nele_scalp");
            }
        }
        public Dictionary<int, Node> Nodes_scalp
        {
            get { return this._mesh.Nodes_scalp; }
            set
            {
                this._mesh.Nodes_scalp = value;
                NotifyPropertyChanged("Nodes_scalp");
            }
        }
        public Dictionary<int, Triangle> Elements_scalp
        {
            get { return this._mesh.Elements_scalp; }
            set
            {
                this._mesh.Elements_scalp = value;
                NotifyPropertyChanged("Elements_scalp");
            }
        }
        public int[] Traces_scalp
        {
            get { return this._mesh.Traces_scalp; }
            set
            {
                this._mesh.Traces_scalp = value;
                NotifyPropertyChanged("Traces_scalp");
            }
        }

        // CORTICAL SURFACE
        public int Nnode_cortex
        {
            get { return this._mesh.Nnode_cortex; }
            set
            {
                this._mesh.Nnode_cortex = value;
                NotifyPropertyChanged("Nnode_cortex");
            }
        }
        public int Nele_cortex
        {
            get { return this._mesh.Nele_cortex; }
            set
            {
                this._mesh.Nele_cortex = value;
                NotifyPropertyChanged("Nele_cortex");
            }
        }
        public Dictionary<int, Node> Nodes_cortex
        {
            get { return this._mesh.Nodes_cortex; }
            set
            {
                this._mesh.Nodes_cortex = value;
                NotifyPropertyChanged("Nodes_cortex");
            }
        }
        public Dictionary<int, Triangle> Elements_cortex
        {
            get { return this._mesh.Elements_cortex; }
            set
            {
                this._mesh.Elements_cortex = value;
                NotifyPropertyChanged("Elements_cortex");
            }
        }
        public int[] Traces_cortex
        {
            get { return this._mesh.Traces_cortex; }
            set
            {
                this._mesh.Traces_cortex = value;
                NotifyPropertyChanged("Traces_cortex");
            }
        }

        // ELECTRODE
        public int Nnode_electrode
        {
            get { return this._mesh.Nnode_electrode; }
            set
            {
                this._mesh.Nnode_electrode = value;
                NotifyPropertyChanged("Nnode_electrode");
            }
        }

        public int[] Nodes_electrode
        {
            get { return this._mesh.Nodes_electrode; }
            set
            {
                this._mesh.Nodes_electrode = value;
                NotifyPropertyChanged("Nodes_electrode");
            }
        }

        public int N_electrode
        {
            get { return this._mesh.N_electrode; }
            set
            {
                this._mesh.N_electrode = value;
                NotifyPropertyChanged("N_electrode");
            }
        }

        public Dictionary<int, Electrode> Electrodes
        {
            get { return this._mesh.Electrodes; }
            set
            {
                this._mesh.Electrodes = value;
                NotifyPropertyChanged("Electrodes");
            }
        }

        public double[] Values_electrode
        {
            get { return this._mesh.Values_electrode; }
            set
            {
                this._mesh.Values_electrode = value;
                NotifyPropertyChanged("Values_electrode");
            }
        }

        // LABEL
        public int N_label
        {
            get { return this._mesh.N_label; }
            set
            {
                this._mesh.N_label = value;
                NotifyPropertyChanged("N_label");
            }
        }
        
        public Dictionary<int, HY_MeshViewer.Model.Label> Labels
        {
            get { return this._mesh.Labels; }
            set
            {
                this._mesh.Labels = value;
                NotifyPropertyChanged("Labels");
            }
        }

        public int Hit_label
        {
            get { return this._mesh.Hit_label; }
            set
            {
                this._mesh.Hit_label = value;
                NotifyPropertyChanged("Hit_label");
            }
        }

        // 마우스 정보
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

        public List<ComboBoxItem> Properties
        {
            get { return this._mesh.Properties; }
            set
            {
                this._mesh.Properties = value;
                NotifyPropertyChanged("Properties");
            }
        }
        #endregion


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

        public ICommand ElectrodeCommand
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

        #region File
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

            openDlg.Filter = "BIN Files (*.bin)|*.bin|TXT Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (openDlg.ShowDialog() == true)
            {
                // get the path
                FileName = openDlg.FileName;

                String extension = Path.GetExtension(FileName);

                #region txt
                // text file
                if (extension == ".txt")
                {
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

                        Triangle t = new Triangle(tmp);

                        int[] indices = t.getIndices();

                        foreach (int ind in indices)
                        {
                            Nodes[ind].setConfaces(i);
                        }

                        Vector3D normal = CalculateFaceNormal(indices, Nodes);
                        t.setNormal(normal);

                        Triangles.Add(i, t);
                    }

                    Properties = new List<ComboBoxItem>()
                    {
                        new ComboBoxItem() { Name = "RGB" },
                    };

                    CalculateVertexNormal();

                    ResetProperty();
                }
                #endregion

                else if (extension == ".bin")
                {
                    LoadModel(FileName);
                    LoadLabel("D:\\HY-MeshViewer\\labels.bin");
                    ResetProperty();
                }
            }
        }

        private void LoadModel(String FileName)
        {
            //try
            //{
                using (FileStream stream = new FileStream(FileName, FileMode.Open))

            using (BinaryReader reader = new BinaryReader(stream))
                {
                    SubName = "";
                    for (int i = 0; i < 100; i++)
                    {
                        SubName += reader.ReadChar();
                    }
                    Flag_surface = reader.ReadInt32();
                    Flag_electrode = reader.ReadInt32();

                    #region volume data
                    Nnode_head = reader.ReadInt32();
                    Nele_head = reader.ReadInt32();

                    Nodes_head = new Dictionary<int, Node>();

                    double[,] tmpd = new double[Nnode_head, 3];
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < Nnode_head; j++)
                        {
                            tmpd[j, i] = reader.ReadDouble();
                        }
                    }

                    float[] indf = new float[3];
                    for (int i = 0; i < Nnode_head; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            indf[j] = (float)tmpd[i, j];
                        }
                        Node n = new Node(indf, N_property);
                        Nodes_head.Add(i + 1, n);
                    }

                    Elements_head = new Dictionary<int, Tetrahedron>();

                    int[,] tmpi = new int[Nele_head, 4];
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < Nele_head; j++)
                        {
                            tmpi[j, i] = reader.ReadInt32();
                        }
                    }

                    int[] indi = new int[4];
                    for (int i = 0; i < Nele_head; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            indi[j] = tmpi[i, j];
                        }
                        Tetrahedron t = new Tetrahedron(indi);

                        foreach (int ind in indi)
                        {
                            Nodes_head[ind].setConfaces(i);
                        }

                        Elements_head.Add(i, t);
                    }

                    Regions_head = new int[Nele_head];
                    for (int i = 0; i < Nele_head; i++)
                    {
                        Regions_head[i] = reader.ReadInt32();
                    }
                    #endregion

                    #region surface
                    if (Flag_surface == 1)
                    {
                        // surface data - scalp
                        Nnode_scalp = reader.ReadInt32();
                        Nele_scalp = reader.ReadInt32();

                        Nodes_scalp = new Dictionary<int, Node>();

                        tmpd = new double[Nnode_scalp, 3];
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < Nnode_scalp; j++)
                            {
                                tmpd[j, i] = reader.ReadDouble();
                            }
                        }

                        indf = new float[3];
                        for (int i = 0; i < Nnode_scalp; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                indf[j] = (float)tmpd[i, j];
                            }
                            Node n = new Node(indf, N_property);
                            Nodes_scalp.Add(i + 1, n);
                        }

                        Elements_scalp = new Dictionary<int, Triangle>();
                        tmpi = new int[Nele_scalp, 3];
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < Nele_scalp; j++)
                            {
                                tmpi[j, i] = reader.ReadInt32();
                            }
                        }

                        indi = new int[3];
                        for (int i = 0; i < Nele_scalp; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                indi[j] = tmpi[i, 2 - j];
                            }

                            Triangle t = new Triangle(indi);

                            foreach (int ind in indi)
                            {
                                Nodes_scalp[ind].setConfaces(i);
                            }

                            Vector3D normal = CalculateFaceNormal(indi, Nodes_scalp);
                            t.setNormal(normal);

                            Elements_scalp.Add(i, t);
                        }
                        
                        Traces_scalp = new int[Nnode_scalp];
                        for (int i = 0; i < Nnode_scalp; i++)
                        {
                            Traces_scalp[i] = reader.ReadInt32();
                        }

                        CalculateVertexNormal(Nnode_scalp, Nodes_scalp, Elements_scalp);

                        // surface data - cortex
                        Nnode_cortex = reader.ReadInt32();
                        Nele_cortex = reader.ReadInt32();

                        Nodes_cortex = new Dictionary<int, Node>();

                        tmpd = new double[Nnode_cortex, 3];
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < Nnode_cortex; j++)
                            {
                                tmpd[j, i] = reader.ReadDouble();
                            }
                        }

                        indf = new float[3];
                        for (int i = 0; i < Nnode_cortex; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                indf[j] = (float)tmpd[i, j];
                            }
                            Node n = new Node(indf, N_property);
                            Nodes_cortex.Add(i + 1, n);
                        }

                        Elements_cortex = new Dictionary<int, Triangle>();
                        tmpi = new int[Nele_cortex, 3];
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < Nele_cortex; j++)
                            {
                                tmpi[j, i] = reader.ReadInt32();
                            }
                        }

                        indi = new int[3];
                        for (int i = 0; i < Nele_cortex; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                indi[j] = tmpi[i, 2 - j];
                            }
                            Triangle t = new Triangle(indi);

                            foreach (int ind in indi)
                            {
                                Nodes_cortex[ind].setConfaces(i);
                            }

                            Vector3D normal = CalculateFaceNormal(indi, Nodes_cortex);
                            t.setNormal(normal);

                            Elements_cortex.Add(i, t);
                        }

                        Traces_cortex = new int[Nnode_cortex];
                        for (int i = 0; i < Nnode_cortex; i++)
                        {
                            Traces_cortex[i] = reader.ReadInt32();
                        }

                        CalculateVertexNormal(Nnode_cortex, Nodes_cortex, Elements_cortex);
                    }
                #endregion

                #region electrode
                if (Flag_electrode == 1)
                {
                    Nnode_electrode = reader.ReadInt32();

                    Nodes_electrode = new int[Nnode_electrode];
                    for (int i = 0; i < Nnode_electrode; i++)
                    {
                        Nodes_electrode[i] = reader.ReadInt32();
                    }

                    N_electrode = reader.ReadInt32();

                    Values_electrode = new double[N_electrode];
                    
                    int[] n_faces = new int[N_electrode];
                    int sum = 0;

                    for (int i = 0; i < N_electrode; i++)
                    {
                        n_faces[i] = reader.ReadInt32();
                        sum += n_faces[i];
                    }

                    Electrodes = new Dictionary<int, Electrode>();

                    for (int k = 0; k < N_electrode; k++)
                    {
                        tmpi = new int[n_faces[k], 4];
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < n_faces[k]; j++)
                            {
                                tmpi[j, i] = reader.ReadInt32();
                            }
                        }

                        Electrode e = new Electrode(n_faces[k], tmpi);
                        Electrodes.Add(k, e);
                    }
                }
                #endregion

                reader.Close();
                stream.Close();
            }
            //}
            /*catch
            {
                CloseFile();
                return;
            }*/
        }

        private void LoadLabel(String FileName)
        {
            using (FileStream stream = new FileStream(FileName, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                N_label = reader.ReadInt32();

                Labels = new Dictionary<int, Model.Label>();
                
                for (int i = 0; i < N_label; i++)
                {
                    string name = "";
                    for (int j = 0; j < 100; j++)
                    {
                        name += reader.ReadChar();
                    }

                    string part = "";
                    for (int j = 0; j < 10; j++)
                    {
                        part += reader.ReadChar();
                    }

                    int n_node = reader.ReadInt32();

                    int[] nodes = new int[n_node];
                    for (int j = 0; j < n_node; j++)
                    {
                        nodes[j] = reader.ReadInt32();
                    }

                    Model.Label l = new Model.Label(name, part, n_node, nodes);

                    Labels.Add(i, l);
                }

                reader.Close();
                stream.Close();
            }
        }

        private void CloseFile()
        {
            FileName = "no file";

            N_node = 0;
            N_triangle = 0;
            N_property = 0;
            N_index = 0;
            N_electrode = 0;

            Nodes = null;
            Triangles = null;

            ResetProperty();

            MouseRay = new Ray(-1);
            Values_electrode = new double[0];

            Properties.Clear();
        }


        #endregion

        private void ResetProperty()
        {
            TranslationX = 0.0f;
            TranslationY = 0.0f;
            TranslationZ = -6.0f;

            RotationAxis = new Vector3D(0, 0, 0);
            RotationAngle = 0;

            Scale = 0.02f;
        }

        private Vector3D CalculateFaceNormal(int[] indices, Dictionary<int, Node> Nodes)
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

        private void CalculateVertexNormal(int N_node, Dictionary<int, Node> Nodes, Dictionary<int, Triangle> Triangles)
        {
            for (int i = 1; i <= N_node; i++)
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