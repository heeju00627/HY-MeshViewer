using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        /* model */
        string subname;
        int flag_surface;
        int flag_electrode;

        int nnode_head;
        int nele_head;
        double[,] nodes_head;
        int[,] elements_head;
        int[] regions_head;

        int nnode_scalp;
        int nele_scalp;
        double[,] nodes_scalp;
        int[,] elements_scalp;
        int[] traces_scalp;

        int nnode_cortex;
        int nele_cortex;
        double[,] nodes_cortex;
        int[,] elements_cortex;
        int[] traces_cortex;

        int nnode_electrode;
        int[] nodes_electrode;
        int n_electrode;
        int[] nface_electrode;
        int[][,] faces_electrode;

        /* label */
        int n_label;

        string[] name_label;
        string[] part_label;
        int[] nnode_label;
        int[][] nodes_label;

        void LoadModel()
        {
            FileStream stream = new FileStream("D:\\HY-MeshViewer\\ConsoleApp1\\defaultmodel.bin", FileMode.Open);

            StreamWriter writer = new StreamWriter(new FileStream("D:\\HY-MeshViewer\\ConsoleApp1\\defaultmodel.txt", FileMode.Create));
            BinaryReader reader = new BinaryReader(stream);

            writer.WriteLine(reader.BaseStream.Length);

            subname = "";
            for (int i = 0; i < 100; i++)
            {
                subname += reader.ReadChar();
            }
            flag_surface = reader.ReadInt32();
            flag_electrode = reader.ReadInt32();

            writer.WriteLine(subname);
            writer.WriteLine(flag_surface + " " + flag_electrode);

            /* volume data */
            nnode_head = reader.ReadInt32();
            nele_head = reader.ReadInt32();

            writer.WriteLine("* volume data *");
            writer.WriteLine(nnode_head + " " + nele_head);

            writer.WriteLine("nodes_head");
            nodes_head = new double[nnode_head, 3];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < nnode_head; j++)
                {
                    nodes_head[j, i] = reader.ReadDouble();
                }
            }

            for (int i = 0; i < nnode_head; i++)
            {
                writer.WriteLine(nodes_head[i, 0] + " " + nodes_head[i, 1] + " " + nodes_head[i, 2]);
            }

            writer.WriteLine("elements_head");
            elements_head = new int[nele_head, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < nele_head; j++)
                {
                    elements_head[j, i] = reader.ReadInt32();
                }
            }

            for (int i = 0; i < nele_head; i++)
            {
                writer.WriteLine(elements_head[i, 0] + " " + elements_head[i, 1] + " " + elements_head[i, 2] + " " + elements_head[i, 3]);
            }

            writer.WriteLine("regions_head");
            regions_head = new int[nele_head];
            for (int i = 0; i < nele_head; i++)
            {
                regions_head[i] = reader.ReadInt32();
                writer.WriteLine(regions_head[i]);
            }

            if (flag_surface == 1)
            {
                /* surface data - scalp*/
                nnode_scalp = reader.ReadInt32();
                nele_scalp = reader.ReadInt32();

                writer.WriteLine("* surface data - scalp *");
                writer.WriteLine(nnode_scalp + " " + nele_scalp);

                writer.WriteLine("nodes_scalp");
                nodes_scalp = new double[nnode_scalp, 3];
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < nnode_scalp; j++)
                    {
                        nodes_scalp[j, i] = reader.ReadDouble();
                    }
                }

                for (int i = 0; i < nnode_scalp; i++)
                {
                    writer.WriteLine(nodes_scalp[i, 0] + " " + nodes_scalp[i, 1] + " " + nodes_scalp[i, 2]);
                }

                writer.WriteLine("elements_scalp");
                elements_scalp = new int[nele_scalp, 3];
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < nele_scalp; j++)
                    {
                        elements_scalp[j, i] = reader.ReadInt32();
                    }
                }

                for (int i = 0; i < nele_scalp; i++)
                {
                    writer.WriteLine(elements_scalp[i, 0] + " " + elements_scalp[i, 1] + " " + elements_scalp[i, 2]);
                }

                writer.WriteLine("traces_scalp");
                traces_scalp = new int[nnode_scalp];
                for (int i = 0; i < nnode_scalp; i++)
                {
                    traces_scalp[i] = reader.ReadInt32();
                    writer.WriteLine(traces_scalp[i]);
                }

                /* surface data - cortex*/
                nnode_cortex = reader.ReadInt32();
                nele_cortex = reader.ReadInt32();

                writer.WriteLine("* surface data - cortex *");
                writer.WriteLine(nnode_cortex + " " + nele_cortex);

                writer.WriteLine("nodes_cortex");
                nodes_cortex = new double[nnode_cortex, 3];
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < nnode_cortex; j++)
                    {
                        nodes_cortex[j, i] = reader.ReadDouble();
                    }
                }

                for (int i = 0; i < nnode_cortex; i++)
                {
                    writer.WriteLine(nodes_cortex[i, 0] + " " + nodes_cortex[i, 1] + " " + nodes_cortex[i, 2]);
                }

                writer.WriteLine("elements_cortex");
                elements_cortex = new int[nele_cortex, 3];
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < nele_cortex; j++)
                    {
                        elements_cortex[j, i] = reader.ReadInt32();
                    }
                }

                for (int i = 0; i < nele_cortex; i++)
                {
                    writer.WriteLine(elements_cortex[i, 0] + " " + elements_cortex[i, 1] + " " + elements_cortex[i, 2]);
                }

                writer.WriteLine("traces_cortex");
                traces_cortex = new int[nnode_cortex];
                for (int i = 0; i < nnode_cortex; i++)
                {
                    traces_cortex[i] = reader.ReadInt32();
                    writer.WriteLine(traces_cortex[i]);
                }
            }

            /* electrodes */
            if (flag_electrode == 1)
            {
                writer.WriteLine("* electrodes - nodes*");
                nnode_electrode = reader.ReadInt32();
                writer.WriteLine(nnode_electrode);

                nodes_electrode = new int[nnode_electrode];
                for (int i = 0; i < nnode_electrode; i++)
                {
                    nodes_electrode[i] = reader.ReadInt32();
                    writer.WriteLine(nodes_electrode[i]);
                }

                writer.WriteLine("* electrodes - num*");
                n_electrode = reader.ReadInt32();
                writer.WriteLine(n_electrode);

                writer.WriteLine("* electrodes - faces *");
                nface_electrode = new int[n_electrode];
                for (int i = 0; i < n_electrode; i++)
                {
                    nface_electrode[i] = reader.ReadInt32();
                    writer.WriteLine(nface_electrode[i]);
                }

                faces_electrode = new int[n_electrode][,];
                for (int k = 0; k < n_electrode; k++)
                {
                    faces_electrode[k] = new int[nface_electrode[k], 4];
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < nface_electrode[k]; j++)
                        {
                            faces_electrode[k][j, i] = reader.ReadInt32();
                        }
                    }

                    writer.WriteLine("* electrodes - faces[" + k + "] *");
                    for (int i = 0; i < nface_electrode[k]; i++)
                    {
                        writer.WriteLine(faces_electrode[k][i, 0] + " " + faces_electrode[k][i, 1] + " " + faces_electrode[k][i, 2] + " " + faces_electrode[k][i, 3]);
                    }
                }
            }

            writer.Close();
            reader.Close();
            stream.Close();
        }

        void LoadLabel()
        {
            FileStream stream = new FileStream("D:\\HY-MeshViewer\\ConsoleApp1\\labels.bin", FileMode.Open);

            StreamWriter writer = new StreamWriter(new FileStream("D:\\HY-MeshViewer\\ConsoleApp1\\labels.txt", FileMode.Create));
            BinaryReader reader = new BinaryReader(stream);

            writer.WriteLine(reader.BaseStream.Length);

            n_label = reader.ReadInt32();
            writer.WriteLine(n_label);

            name_label = new string[n_label];
            part_label = new string[n_label];
            nnode_label = new int[n_label];
            nodes_label = new int[n_label][];

            for (int i = 0; i < n_label; i++)
            {
                writer.WriteLine("* labels[" + i + "] *");

                name_label[i] = "";
                for (int j = 0; j < 100; j++)
                {
                    name_label[i] += reader.ReadChar();
                }
                writer.WriteLine(name_label[i]);

                part_label[i] = "";
                for (int j = 0; j < 10; j++)
                {
                    part_label[i] += reader.ReadChar();
                }
                writer.WriteLine(part_label[i]);

                nnode_label[i] = reader.ReadInt32();
                writer.WriteLine(nnode_label[i]);

                nodes_label[i] = new int[nnode_label[i]];
                for (int j = 0; j < nnode_label[i]; j++)
                {
                    nodes_label[i][j] = reader.ReadInt32();
                    writer.WriteLine(nodes_label[i][j]);
                }
            }

            writer.Close();
            reader.Close();
            stream.Close();
        }

            static void Main(string[] args)
        {
            Program p= new Program();

            p.LoadModel();
            p.LoadLabel();
        }    
    }
}
