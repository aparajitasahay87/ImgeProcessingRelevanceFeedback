using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication4
{
    public class ImageDetails : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Uri image
        {
            get;
            set;
        }
        public string imageName
        {
            get;
            set;
        }
        public string RfMethodSelected
        {
            get;
            set;
        }
        public string otherMethodSelected
        {
            get;
            set;

        }

        public bool IsChecked
        {
            get;
            set;
        }

        protected void OnPropertyChanged(string RfMethodSelected)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(RfMethodSelected));
                handler(this, new PropertyChangedEventArgs(otherMethodSelected));
            }
        }
    }
}
