using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            progressBar.Visibility = Visibility.Collapsed;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                     if (Uri.TryCreate(addresTextBox.Text, UriKind.Absolute, out Uri? uri))
                      {
                        progressBar.Visibility = Visibility.Visible;
                        using HttpResponseMessage response = await client.GetAsync(uri);
                        response.EnsureSuccessStatusCode();
                        responceTextBox.Text = await response.Content.ReadAsStringAsync();
                        statusCodeTextBox.Text = (int)response.StatusCode + " " + response.StatusCode.ToString();
                        progressBar.Visibility=Visibility.Collapsed;
                     }
                     else
                     {
                         MessageBox.Show("Проверьте адрес интересующего ресурса!");
                         return;
                     }
                }
            }
            catch (HttpRequestException ex) when (ex is {StatusCode:System.Net.HttpStatusCode.NotFound})
            {
                progressBar.Visibility = Visibility.Collapsed;
                MessageBox.Show($"{ex.Message}");
            }
            catch (HttpRequestException ex) when(ex is { StatusCode:System.Net.HttpStatusCode.BadRequest}) 
            {
                progressBar.Visibility = Visibility.Collapsed;
                MessageBox.Show($"{ex.Message}");
            }
            
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            if (responceTextBox.Text == "")
            {
                MessageBox.Show("Проверьте, посмотрели ли Вы ресурс!");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.ShowDialog(this);
            saveFileDialog.DefaultExt = ".html";
            saveFileDialog.AddExtension = true;
            fileNameLabel.Content = saveFileDialog.FileName;
            File.WriteAllText((string)fileNameLabel.Content, responceTextBox.Text);
            MessageBox.Show("Страница успешно сохранена!");
        }
    }
}

