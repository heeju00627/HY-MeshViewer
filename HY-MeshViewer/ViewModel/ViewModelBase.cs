using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using HY_MeshViewer.Model;
using System.Windows;

namespace HY_MeshViewer.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {

        public ViewModelBase()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
