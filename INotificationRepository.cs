using System;
using System.Collections.Generic;
using System.Text;
using Pronali.Data.Models.Entity.Hr;

namespace Pronali.Data.Repositories.Interfaces.Core
{
    public interface INotificationRepository : IBaseRepository<Notification>
    {
        void sendNotification(string sendFrom, string userId, string msg);
    }
}
