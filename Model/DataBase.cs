using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Notepad.Model
{
    class DataBase : INotifyPropertyChanged
    {
        private int id;
        private string someText;
        private string date;
        private byte[] image;

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }
        public string SomeText
        {
            get { return someText; }
            set
            {
                someText = value;
                OnPropertyChanged("SomeText");
            }
        }
        public string Date
        {
            get { return date; }
            set
            {
                date = value;
                OnPropertyChanged("Date");
            }
        }
        public byte[] Image
        {
            get { return image; }
            set
            {
                image = value;
                OnPropertyChanged("Date");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
