using System;
using System.ComponentModel;
using System.IO;

namespace Domain.Entities
{
    public class Picture : INotifyPropertyChanged
    {
        protected internal Picture(int id)
        {
            if (!(id >= 1 && id <= 3))
                throw new InvalidOperationException("Неверный ID");
            Id = id;
        }



        public int Id { get; protected internal set; }

        public string Url { get; protected internal set; }

        public int Size { get; protected internal set; }

        private string adress;

        public string Adress
        {
            get
            {
                return adress;
            }
            set
            {
                adress = value;
                NotifyPropertyChanged("Adress");
            }
        }

        public double ReceiveBytes { get; protected internal set; }

        private bool isdownloded = false;

        public bool IsDownloaded
        {
            get
            {
                return isdownloded;
            }
            protected internal set
            {
                isdownloded = value;
                NotifyPropertyChanged("isDownloaded");
            }
        }

        private bool begunDownload = false;

        public bool BegunDownload
        {
            get { return begunDownload; }
            set
            {
                begunDownload = value;
                NotifyPropertyChanged("BegunDownload");
            }
        }

        private bool readyDownload = true;

        public bool ReadyDownload
        {
            get { return readyDownload; }
            set
            {
                readyDownload = value;
                NotifyPropertyChanged("ReadyDownload");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;



        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        protected internal void SetPictureAdress()
        {
            string pictureName = "";

            for (int i = Url.Length - 1; i >= 0 && Url[i] != '/'; i--)
            {
                pictureName = Url[i] + pictureName;
            }

            string prefix = "I" + Id + "_";
            Adress = Path.Combine(Environment.CurrentDirectory, prefix + pictureName);

            for (int i = 0; File.Exists(Adress); i++)
            {
                prefix = "I" + Id + "_(" + i.ToString() + ")";
                Adress = Path.Combine(Environment.CurrentDirectory, prefix + pictureName);
            }
        }
    }
}
