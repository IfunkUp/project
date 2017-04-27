using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncWPF.Classes
{
    public class SatisFactionRating
    {


        public long Id { get; set; }
        public string Url { get; set; }
        public long? Assignee_id  { get; set; }
        public long? Group_id { get; set; }
        public long? Requester_id { get; set; }
        public long? Ticket_id { get; set; }
        public string Score { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
        public string Comment { get; set; }






    }
}
