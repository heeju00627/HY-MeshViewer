using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Class1
    {
        

            /* electrodes */
            if (Flag_electrode == 1)
            {
                writer.WriteLine("* electrodes - Nodes_*");
                Nnode_electrode = reader.ReadInt32();
                writer.WriteLine(Nnode_electrode);

                Nodes_electrode = new int[Nnode_electrode];
                for (int i = 0; i<Nnode_electrode; i++)
                {
                    Nodes_electrode[i] = reader.ReadInt32();
                    writer.WriteLine(Nodes_electrode[i]);
                }

                writer.WriteLine("* electrodes - num*");
                Nelectrode = reader.ReadInt32();
                writer.WriteLine(Nelectrode);

                writer.WriteLine("* electrodes - Faces_ *");
                Nface_electrode = new int[Nelectrode];
                for (int i = 0; i<Nelectrode; i++)
                {
                    Nface_electrode[i] = reader.ReadInt32();
                    writer.WriteLine(Nface_electrode[i]);
                }

                Faces_electrode = new int[Nelectrode][,];
                for (int k = 0; k<Nelectrode; k++)
                {
                    Faces_electrode[k] = new int[Nface_electrode[k], 4];
                    for (int i = 0; i< 4; i++)
                    {
                        for (int j = 0; j<Nface_electrode[k]; j++)
                        {
                            Faces_electrode[k][j, i] = reader.ReadInt32();
                        }
                    }

                    writer.WriteLine("* electrodes - Faces_[" + k + "] *");
                    for (int i = 0; i<Nface_electrode[k]; i++)
                    {
                        writer.WriteLine(Faces_electrode[k][i, 0] + " " + Faces_electrode[k][i, 1] + " " + Faces_electrode[k][i, 2] + " " + Faces_electrode[k][i, 3]);
                    }
                }
            }
    }
}
