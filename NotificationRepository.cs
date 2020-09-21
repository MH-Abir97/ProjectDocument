using System;
using System.Collections.Generic;
using System.Text;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Core;

namespace Pronali.Data.Repositories.Core
{
    public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
    {
        private ApplicationDbContext _db;
        public NotificationRepository(ApplicationDbContext _context) : base(_context)
        {

        }

        public void sendNotification(string sendFrom, string userId, string msg)
        {
            throw new NotImplementedException();
        }
    }
}
