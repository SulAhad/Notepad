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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Notepad
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void openAddSomeText(object sender, MouseButtonEventArgs e)
        {
            AddSomeText addSomeText = new();
            addSomeText.ShowDialog();
        }
        public void Update()
        {
            DataBaseContext dataBaseContext = new();
            ListSomeText.ItemsSource = dataBaseContext.DataBases.ToList();
        }
        private void grid_Loaded(object sender, RoutedEventArgs e)
        {
            Update();
        }
    }
}
