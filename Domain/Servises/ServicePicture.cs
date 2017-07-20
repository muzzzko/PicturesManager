using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.ComponentModel;
using System.IO;
using Domain.Repositories;
using Domain.Entities;
using Domain;
using static System.Net.Mime.MediaTypeNames;
using System.Windows;

namespace Domain.Services
{
    public class ServicePicture : IServicePicture
    {
        private readonly IProgress<double> _progress;
        private readonly PicturesRepository _picturesRepository;



        public ServicePicture(Progress<double> progress,
            PicturesRepository picturesRepository)
        {
            _progress = progress;
            _picturesRepository = picturesRepository;
        }



        private Picture Picture { get; set; }

        private bool DownloadCompleted { get; set; }

        private bool Canceled { get; set; }



        public void SetPictureUrlAndAdress(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException();

            Picture.Url = url;
            Picture.SetPictureAdress();
            Picture.IsDownloaded = false;
        }

        public void AddPicture(int id)
        {
            Picture = new Picture(id);
            _picturesRepository.Add(Picture);
        }

        public async Task DownLoadImage(string url)
        {
            try
            {
                WebTools.ServicePictures[Picture.Id - 1].SetPictureUrlAndAdress(url);
                WebTools.Tokens[Picture.Id - 1] = new CancellationTokenSource();
                Picture.ReadyDownload = false;
                Picture.BegunDownload = true;

                DownloadCompleted = false;
                Canceled = false;

                HttpClient httpClient;

                using (httpClient = new HttpClient())
                {
                    byte[] data =
                     await httpClient.GetByteArrayAsync(Picture.Url).ConfigureAwait(false);
                    Picture.Size = data.Length;
                    MyProgressBar.Maximum += Picture.Size;
                }

                WebClient webClient;
                using (webClient = new WebClient())
                {
                    webClient.DownloadProgressChanged +=
                            new DownloadProgressChangedEventHandler(Picture_Download);
                    webClient.DownloadFileCompleted +=
                        new AsyncCompletedEventHandler(PictureDownload_Completed);
                    await webClient.DownloadFileTaskAsync(new Uri(Picture.Url), Picture.Adress).ConfigureAwait(false);

                };
            }
            catch (Exception ex)
            {
                Picture.ReadyDownload = true;
                Picture.BegunDownload = false;
                WebTools.Tokens[Picture.Id-1]?.Dispose();
                MessageBox.Show(ex.Message);
            }
        }

        private void Picture_Download(object sender, DownloadProgressChangedEventArgs e)
        {
            try
            {
                WebTools.Tokens[Picture.Id-1].Token.ThrowIfCancellationRequested();

                lock (WebTools.LockDownload)
                {
                    MyProgressBar.Value += e.ProgressPercentage * Picture.Size / 100.0 - Picture.ReceiveBytes;
                    Picture.ReceiveBytes = e.ProgressPercentage * Picture.Size / 100.0;
                }


                lock (WebTools.LockDownload)
                {
                    if (!DownloadCompleted)
                        _progress.Report(MyProgressBar.Value / MyProgressBar.Maximum * 100);
                    if (e.ProgressPercentage == 100)
                    {
                        DownloadCompleted = true;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                lock (WebTools.LockCancel)
                {
                    if (!Canceled)
                    {
                        DownloadCompleted = true;
                        WebClient webclient = (WebClient)sender;
                        webclient.CancelAsync();
                       // MessageBox.Show("Загрузка изображения " + Picture.Id + " была отменена");
                        Canceled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PictureDownload_Completed(object sender,
            AsyncCompletedEventArgs e)
        {
            while (!DownloadCompleted);
  
            if (e.Cancelled)
            {
                lock (WebTools.LockDownload)
                {
                    MyProgressBar.Value += (Picture.Size - Picture.ReceiveBytes);
                }
                File.Delete(Picture.Adress);
            }
            else
            {
                Picture.IsDownloaded = true;
                Picture.Adress = Picture.Adress;
            }

            if (_picturesRepository.GetPictures().Where(x=>x.BegunDownload).All(x => x.IsDownloaded))
            {
                lock (WebTools.LockDownload)
                {
                    _progress.Report(0);
                    MyProgressBar.Maximum = 0;
                    MyProgressBar.Value = 0;
                }
            }

            Picture.ReadyDownload = true;
            Picture.BegunDownload = false;
            WebTools.Tokens[Picture.Id - 1].Dispose();
            Picture.ReceiveBytes = 0;
            Picture.Size = 0;
        }
    }
}
