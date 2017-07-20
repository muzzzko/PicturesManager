using Domain.Services;
using System.Threading;

namespace Domain
{
    public static class WebTools
    {
        public static CancellationTokenSource[] Tokens = new CancellationTokenSource[3];

        public static ServicePicture[] ServicePictures = new ServicePicture[3];

        public static object LockDownload = new object();

        public static object LockCancel = new object();

    }
}
