using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZendeskApi_v2;

namespace SyncWPF.workers
{
    class Backgrndworker
    {








        public static void UpdateInc(DateTime m_date)
        {
            BackgroundWorker worker = new BackgroundWorker();
            ZendeskApi s_client = ZendeskHelper.LoadClient();
            
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync(s_client.Tickets.GetAllTickets().Count);
            





        }

        private static void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int max = (int)e.Argument;
            int res = 0;
            for (int i = 0; i < max; i++)
            {
                int procesPrecentage = Convert.ToInt32(((double)i / max) * 100);
                
                



            }
        }
    }
}
