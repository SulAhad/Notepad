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

namespace Notepad
{
    public partial class AddSomeText : Window
    {
        public AddSomeText()
        {
            InitializeComponent();
        }

        public void ClearText()
        {
            textArea.Text = string.Empty;
        }
        private void AddText(object sender, MouseButtonEventArgs e)
        {
            if (textArea.Text == string.Empty)
            {
                DownTrayAddText.Content = "Не ввели текст!";
                DownTrayAddText.Background = Brushes.LightCoral;
            }
            else
            {
                textArea.Text = textArea.Text.Trim(' ');

                DataBaseContext db = new();
                DataBase tim = new DataBase
                {
                    SomeText = textArea.Text,
                    Date = DateTime.Now.ToString()
                };

                db.DataBases.Add(tim);
                db.SaveChanges();

               // Update();
                ClearText();
                this.Close();
            }
        }
        public void Update()
        {
            MainWindow mainWindow = new();
            DataBaseContext dataBaseContext = new();
            mainWindow.ListSomeText.ItemsSource = dataBaseContext.DataBases.ToList();
        }
        private void grid_Loaded(object sender, RoutedEventArgs e)
        {
            Update();
        }
    }
}
