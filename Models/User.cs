using System.Collections.Generic;
using System;

namespace HotelApp.Models
{
    public class User
    {
        public enum UserType : int
        {
            Administrator = 0,
            Worker = 1,
            Guest = 2,
        }

        public enum WorkForWorkerType : int
        {
            None = 0,
            Сleaner = 1,
            Engineer = 2,
            Concierge = 3,
            Accounting = 4,
        }

        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Cookie { get; set; }
        public byte[] Salt { get; set; }
        public UserType TypeUser { get; set; }
        public WorkForWorkerType TypeWorkForWorker { get; set; } = WorkForWorkerType.None;
    }
}