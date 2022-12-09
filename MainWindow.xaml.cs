using Microsoft.Win32;
using Notepad.Model;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;

namespace Notepad
{
    public partial class MainWindow : Window
    {
        public string? id;
        public string? text;
        public string? date;
        public byte[] image;
        public MainWindow()
        {
            InitializeComponent();
            AddHandler(Keyboard.KeyDownEvent, (KeyEventHandler)HandlerKeyDownEvent);
        }
        private void HandlerKeyDownEvent(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    SendMessageTapEnter();
                    break;
                case Key.Escape:
                    CloseNotepad();
                    break;
                default:
                    break;
            }
        }//Клавиатура
        public void LabelImageAdd(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog ofdPicture = new OpenFileDialog();
            ofdPicture.Filter = "Image files|*.bmp;*.jpg;*.gif;*.png;*.tif|All files|*.*";
            if (ofdPicture.ShowDialog() == true)
                myImage.Source = new BitmapImage(new Uri(ofdPicture.FileName));
        }
        public static BitmapImage ConvertByteArrayToBitmapImage(Byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            stream.Seek(0, SeekOrigin.Begin);
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            return image;
        }
        public void SendMessageTapEnter()
        {

            if (TextEdit.Text == "")
            {
                DownTray.Content = "Не ввели текст!";
                DownTray.Background = Brushes.LightCoral;
                TimerForEmpty();
            }
            else
            {
                if (myImage.Source == null)
                {
                    myImage.Source = BitmapFrame.Create(new Uri(@"E:\Мои проекты\C#\Notepad\Notepad\images\Нет_изображения.jpg"));
                }
                TextEdit.Text = TextEdit.Text.Trim(' ');
                DataBaseContext db = new();
                DataBase tim = new DataBase
                {
                    SomeText = TextEdit.Text,
                    Date = DateTime.Now.ToString(),
                    Image = ConvertBitmapSourceToByteArray(myImage)
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
        private byte[] ConvertBitmapSourceToByteArray(Image myImage)
        {
            byte[] data;
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)myImage.Source));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }
        private void CloseNotepad()
        {
            if (TextEdit.Text.Length > 0)
            {
                MessageBoxResult result = MessageBox.Show(
            "Вы действительно хотите выйти, данные будет потеряны?",
            "Сообщение",
            MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
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
            SendMessageTapEnter();
        }
        public void Update()
        {
            DataBaseContext dataBaseContext = new();
            ListSomeText.ItemsSource = dataBaseContext.DataBases.ToList();
            myImage.Source = null;
        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
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
        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DataBase path = ListSomeText.SelectedItem as DataBase;
            if (path != null)
            {
                id = Convert.ToString(path.Id);
                idEdit.Content = Convert.ToString(path.Id);
                TextEdit.Text = Convert.ToString(path.SomeText);
                DateEdit.Content = Convert.ToString(path.Date);
                myImage.Source = ConvertByteArrayToBitmapImage(path.Image);
                BorderLabelChange.Visibility = Visibility.Visible;
                BorderLabelDelete.Visibility = Visibility.Visible;
            }
            else
            {
                BorderLabelChange.Visibility = Visibility.Hidden;
                BorderLabelDelete.Visibility = Visibility.Hidden;
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
                    BorderLabelChange.Visibility = Visibility.Hidden;
                    BorderLabelDelete.Visibility = Visibility.Hidden;
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
        private void LabelChange_MouseEnter(object sender, MouseEventArgs e)
        {
            labelChange.Background = Brushes.DimGray;
        }
        private void LabelChange_MouseLeave(object sender, MouseEventArgs e)
        {
            labelChange.Background = Brushes.Gray;
        }
        private void LabelAdd_MouseEnter(object sender, MouseEventArgs e)
        {
            labelAdd.Background = Brushes.DimGray;
        }
        private void LabelAdd_MouseLeave(object sender, MouseEventArgs e)
        {
            labelAdd.Background = Brushes.Gray;
        }
        private void LabelDelete_MouseEnter(object sender, MouseEventArgs e)
        {
            labelDelete.Background = Brushes.DimGray;
        }
        private void LabelDelete_MouseLeave(object sender, MouseEventArgs e)
        {
            labelDelete.Background = Brushes.Gray;
        }
        private void LabelUpdate_MouseEnter(object sender, MouseEventArgs e)
        {
            labelUpdate.Background = Brushes.DimGray;
        }
        private void LabelUpdate_MouseLeave(object sender, MouseEventArgs e)
        {
            labelUpdate.Background = Brushes.Gray;
        }
        private void LabelFind_MouseEnter(object sender, MouseEventArgs e)
        {
            labelFind.Background = Brushes.DimGray;
        }
        private void LabelFind_MouseLeave(object sender, MouseEventArgs e)
        {
            labelFind.Background = Brushes.Gray;
        }
        private void LabelEmpty_MouseEnter(object sender, MouseEventArgs e)
        {
            labelEmpty.Background = Brushes.DimGray;
        }
        private void LabelEmpty_MouseLeave(object sender, MouseEventArgs e)
        {
            labelEmpty.Background = Brushes.Gray;
        }
        private void EmptySomeText(object sender, MouseButtonEventArgs e)
        {
            Update();
            idEdit.Content = string.Empty;
            TextEdit.Text = string.Empty;
            DateEdit.Content = string.Empty;
            myImage.Source = null;
            BorderLabelChange.Visibility = Visibility.Hidden;
            BorderLabelDelete.Visibility = Visibility.Hidden;
        }
        private void LabelImageAdd_MouseEnter(object sender, MouseEventArgs e)
        {
            labelImageAdd.Background = Brushes.DimGray;
        }
        private void LabelImageAdd_MouseLeave(object sender, MouseEventArgs e)
        {
            labelImageAdd.Background = Brushes.Gray;
        }
        private void ImageEdit_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ImageWindow imageWindow = new();
            imageWindow.ImageSourceWindow.Source = myImage.Source;
            imageWindow.ShowDialog();
        }
    }
}
