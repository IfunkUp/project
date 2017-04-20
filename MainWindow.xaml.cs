using SyncWPF.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZendeskApi_v2.Models.Organizations;
using ZendeskApi_v2.Models.Tickets;
using ZendeskApi_v2.Models.Users;
using SatisfactionRating = ZendeskApi_v2.Models.Satisfaction.SatisfactionRating;
using Category = ZendeskApi_v2.Models.Categories.Category;
using SyncWPF.workers;
using System.Threading;
using System.Windows.Threading;

namespace SyncWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
      //  string m_UserName = "info@sms-timing.com";
      //  string m_PassWord = "smssms123";
      //  string m_Url = "http://smstiming.zendesk.com";
        DateTimeOffset m_date = DateTimeOffset.Now;
        private ProgressBar bar = null;
        



        public MainWindow()
        {
           
            InitializeComponent();
        }


        private async void DownloadAndSaveAll()
        {

            Thread T = new Thread(
                new ThreadStart(
                    delegate ()
                    {
                        DispatcherOperation dispOp = pbUpdate.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(
                             async delegate ()
                            {
                                var organizations = await ZendeskHelper.GetOrganisations();
                                foreach (var item in organizations)
                                {
                                    Firebirdhelper.SaveOrganization(item);
                                }
                            }));
                        dispOp.Completed += new EventHandler(DispOp_Completed);

                    }));
            T.Start();
                 
            
            var users = await ZendeskHelper.GetUsers();

            foreach (var item in users)
            {
                Firebirdhelper.SaveUser(item);
            }

            var tickets = await ZendeskHelper.GetTickets();
            foreach (var item in tickets)
            {
                Firebirdhelper.SaveTicket(item);
            }

        }

        private void DispOp_Completed(object sender, EventArgs e)
        {
            // nothing
            return;
        }

        private async void DownloadAndSaveIncremental(DateTimeOffset givenDate)
        {
            var tickets = await ZendeskHelper.GetLastTickets(givenDate);
            var commentList = new List<Comment>();
            foreach (var item in tickets)
            {
                commentList.AddRange(ZendeskHelper.GetComment(DataHelper.ConvertToPrimitive(item.Id)));
                foreach (var comment in commentList)
                {
                    Firebirdhelper.SaveComment(comment, DataHelper.ConvertToPrimitive(item.Id));
                }
                Firebirdhelper.SaveTicket(item);
               
            }
        }

        private async void DownloadAndSaveSatisfaction()
        {
            var satisfactions = await ZendeskHelper.GetSatisfaction();
            var satisfactionList = new List<SatisfactionRating>();

            foreach (var item in satisfactions)
            {
                Firebirdhelper.SaveSatisfaction(item);
            }


        }

        private void dpStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            m_date = dpStart.SelectedDate.Value.Date;
        }

        private void grdMain_Loaded(object sender, RoutedEventArgs e)
        {
            dpStart.SelectedDate = m_date.Date;
        }

        private void IncrementalSync_Click(object sender, RoutedEventArgs e)
        {
            m_date = dpStart.SelectedDate.Value;
            DownloadAndSaveIncremental(m_date);
        }

        private void FullSync_Click(object sender, RoutedEventArgs e)
        {
              DownloadAndSaveAll();
            
        }
    }
}
/*
    automatisch de incremental doen en de volledige enkel manueel
    

     
     
     */