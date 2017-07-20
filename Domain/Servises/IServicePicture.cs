using System.Threading.Tasks;

namespace Domain.Services
{
    interface IServicePicture
    {
        Task DownLoadImage(string url);

        void AddPicture(int id);
        
        void SetPictureUrlAndAdress(string url);
    }
}
