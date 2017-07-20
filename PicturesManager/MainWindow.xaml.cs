using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using Domain.Repositories;
using Domain.Services;
using Domain;

namespace PicturesManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitWebData();
            DataContext = PicturesRepository;
        }



        public PicturesRepository PicturesRepository = new PicturesRepository();



        private async void btnFirstImage_Click(object sender, RoutedEventArgs e)
        {
            await WebTools.ServicePictures[0].DownLoadImage(txtbFirstImage.Text);
        }

        private async void btnSecondImage_Click(object sender, RoutedEventArgs e)
        {
            await WebTools.ServicePictures[1].DownLoadImage(txtbSecondImage.Text);
        }

        private async void btnThirdImage_Click(object sender, RoutedEventArgs e)
        {
            await WebTools.ServicePictures[2].DownLoadImage(txtbThirdImage.Text);
        }

        private void btnCancelFirstImage_Click(object sender, RoutedEventArgs e)
        {
            WebTools.Tokens[0].Cancel();
        }

        private void btnCancelSecondImage_Click(object sender, RoutedEventArgs e)
        {
            WebTools.Tokens[1].Cancel();
        }

        private void btnCancelThirdImage_Click(object sender, RoutedEventArgs e)
        {
            WebTools.Tokens[2].Cancel();
        }

        private async void btnDownloadAll_Click(object sender, RoutedEventArgs e)
        {
            Task[] tasks = new Task[3];
            string[] urls = new string[] { txtbFirstImage.Text, txtbSecondImage.Text, txtbThirdImage.Text };

            for (int i = 0; i < 3; i++)
            {
                tasks[i] = WebTools.ServicePictures[i].DownLoadImage(urls[i]);
            }
            await Task.WhenAll(tasks);
   
        }

        private void InitWebData()
        {
            for(int i=0;i<3;i++)
            {
                WebTools.ServicePictures[i] = new ServicePicture(new Progress<double>(value => prgbImageDownload.Value = value),
                        PicturesRepository);

                WebTools.ServicePictures[i].AddPicture(i+1);
            }
        }
    }
}
