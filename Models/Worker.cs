using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Models
{
    public class Worker
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public WorkerType TypeWorker { get; set; }

        public enum WorkerType : int
        {
            Сleaner = 0,
            Engineer = 1,
            Concierge = 2,
            Accounting = 3,
        }
    }
}
