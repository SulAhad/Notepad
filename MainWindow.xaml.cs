using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace Notepad
{
    public partial class MainWindow : Window
    {
        public string id;
        public string text;
        public string date;

        public MainWindow()
        {
            InitializeComponent();
        }
        public void TimerForEmpty()
        {
            DispatcherTimer dispatcherTimer = new();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 4);
            dispatcherTimer.Start();
        }///Диспетчер времени для отсчета времени для очистки уведмлений
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            DownTray.Content = "";
            DownTray.Background = Brushes.AliceBlue;
        }///Исходное положение после отсчета таймера
        public void ClearText()
        {
            idEdit.Content = string.Empty;
            TextEdit.Text = string.Empty;
            DateEdit.Content = string.Empty;
        }
        private void AddSomeText(object sender, MouseButtonEventArgs e)
        {
            if (TextEdit.Text == "")
            {
                DownTray.Content = "Не ввели текст!";
                DownTray.Background = Brushes.LightCoral;
                TimerForEmpty();
            }
            else
            {
                TextEdit.Text = TextEdit.Text.Trim(' ');
                DataBaseContext db = new();
                DataBase tim = new DataBase
                {
                    SomeText = TextEdit.Text,
                    Date = DateTime.Now.ToString()
                };
                db.DataBases.Add(tim);
                db.SaveChanges();
                Update();
                ClearText();
                DownTray.Content = "Добавлена запись!";
                DownTray.Background = Brushes.LightGreen;
                TimerForEmpty();
            }
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
        private void UpdateSomeText(object sender, MouseButtonEventArgs e)
        {
            Update();
            DownTray.Content = "Обновлено!";
            DownTray.Background = Brushes.LightGreen;
            ClearText();
            TimerForEmpty();
        }
        private void grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DataBase path = ListSomeText.SelectedItem as DataBase;
            if (path != null)
            {
                id = Convert.ToString(path.Id);
                idEdit.Content = Convert.ToString(path.Id);
                TextEdit.Text = Convert.ToString(path.SomeText);
                DateEdit.Content = Convert.ToString(path.Date);
            }
        }
        private void DeleteSomeText(object sender, MouseButtonEventArgs e)
        {
            DataBaseContext db = new();
            if (id == "")
            {
                DownTray.Content = "Не выбрана строка для удаления!";
                DownTray.Background = Brushes.LightCoral;
                TimerForEmpty();
            }
            else
            {
                int key = Convert.ToInt32(id);
                var item = db.DataBases.Find(key);
                if (item != null)
                {

                    db.DataBases.Remove(item);
                    db.SaveChanges();
                    Update();
                    DownTray.Background = Brushes.LightGreen;
                    DownTray.Content = "Удалена запись --" + id;
                    id = null;
                    ClearText();
                    TimerForEmpty();
                }
                else
                {
                    DownTray.Content = "Не выбрана строка для удаления!";
                    DownTray.Background = Brushes.LightCoral;
                    TimerForEmpty();
                }
            }
        }
        private void ChangeSomeText(object sender, MouseButtonEventArgs e)
        {
            DataBaseContext db = new();
            if (id == "")
            {
                DownTray.Content = "Не выбрана строка для изменения!";
                DownTray.Background = Brushes.LightCoral;
                TimerForEmpty();
            }
            else
            {
                int key = Convert.ToInt32(id);
                var item = db.DataBases.Find(key);
                if (item != null)
                { 
                    item.SomeText = TextEdit.Text;
                    item.Date = ("изменен:" + " " + DateTime.Now.ToString());
                    db.DataBases.Update(item);
                    db.SaveChanges();
                    Update();
                    DownTray.Background = Brushes.LightGreen;
                    DownTray.Content = "Изменена запись --" + id;
                    id = null;
                    ClearText();
                    TimerForEmpty();
                }
                else
                {
                    DownTray.Content = "Не выбрана строка для изменения!";
                    DownTray.Background = Brushes.LightCoral;
                    TimerForEmpty();
                }
            }
        }
        private void FindSomeText(object sender, MouseButtonEventArgs e)
        {
            DataBaseContext db = new();
            if (TextEdit.Text == "")
            {
                DownTray.Content = "Нет данных для поиска!";
                DownTray.Background = Brushes.LightCoral;
                TimerForEmpty();
            }
            else
            {
                string find = TextEdit.Text;
                ListSomeText.ItemsSource = db.DataBases.Where(p => p.SomeText == find).ToList();
                DownTray.Content = "Выбран фильтр по наименовавнию:" + " " + find;
                DownTray.Background = Brushes.LightBlue;
                TimerForEmpty();
            }
        }
    }
}
