using System;
using System.Collections.Generic;
using Domain.Entities;
using System.ComponentModel;

namespace Domain.Repositories
{
    public class PicturesRepository : IRepository, INotifyPropertyChanged
    {
        private List<Picture> Pictures = new List<Picture>();

        private Picture first;
        public Picture First
        {
            get
            {
                return first;
            }
            private set
            {
                first = value;
                NotifyPropertyChanged("First");
            }
        }

        private Picture second;
        public Picture Second
        {
            get
            {
                return second;
            }
            private set
            {
                second = value;
                NotifyPropertyChanged("Second");
            }
        }

        private Picture third;
        public Picture Third
        {
            get
            {
                return third;
            }
            private set
            {
                third = value;
                NotifyPropertyChanged("Third");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;



        public void Add(Picture picture)
        {
            Pictures.Add(picture);

            switch (picture.Id)
            {
                case 1:
                    First = picture;
                    break;
                case 2:
                    Second = picture;
                    break;
                case 3:
                    Third = picture;
                    break;
                default:
                    throw new InvalidOperationException("Id is wrong");
            }
        }

        public IEnumerable<Picture> GetPictures()
        {
            return Pictures;
        }

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
