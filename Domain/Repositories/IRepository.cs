using System.Collections.Generic;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IRepository
    {
        void Add(Picture picture);

        IEnumerable<Picture> GetPictures();
    }
}
