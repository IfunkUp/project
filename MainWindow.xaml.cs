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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SyncWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //  string m_UserName = "info@sms-timing.com";
        //  string m_PassWord = "smssms123";
        //  string m_Url = "http://smstiming.zendesk.com";


        private DateTime? m_SelectedDate = DateTime.Now;

        public DateTime? SelectedDate
        {
            get => m_SelectedDate;
            set
            {
                m_SelectedDate = value;
                OnPropertyChanged();
            }
        }

        public DateTimeOffset SelectedDateOffset => GetDateTimeOffset(SelectedDate);

        DateTimeOffset GetDateTimeOffset(DateTime? aDateTime)
        {
            if (aDateTime == null)
            {
                return (DateTimeOffset)DateTime.Now;
            }

            return (DateTimeOffset)aDateTime;
        }

        private string m_Comment;
        public string Comment
        {
            get => m_Comment;
            set
            {
                m_Comment = value;
                OnPropertyChanged();
            }
        }

        private double m_Maximum;

        public double Maximum
        {
            get => m_Maximum;
            set
            {
                m_Maximum = value;
                OnPropertyChanged();
            }
        }

        private double m_ProgressValue;

        public double ProgressValue
        {
            get => m_ProgressValue;
            set
            {
                m_ProgressValue = value;
                OnPropertyChanged();
            }
        }

        #region propertyChanged     
        public event PropertyChangedEventHandler PropertyChanged;
        // Check the attribute in the following line :
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }


        private async void DownloadAndSaveAll()
        {
            GetOrganizations();

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


        private async void DownloadAndSaveIncremental()
        {
            var tickets = await ZendeskHelper.GetLastTickets(SelectedDateOffset);
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

        private void IncrementalSync_Click(object sender, RoutedEventArgs e)
        {
            DownloadAndSaveIncremental();
        }

        private void FullSync_Click(object sender, RoutedEventArgs e)
        {
            //  DownloadAndSaveAll();
            Task.Factory.StartNew(() => GetOrganizations());
        }

        public async void GetOrganizations()
        {
            var organizations = await ZendeskHelper.GetOrganisations();
            Maximum = organizations.Count();
            ProgressValue = 1;
            foreach (var item in organizations)
            {
                Firebirdhelper.SaveOrganization(item);
                Comment = "importing  " + item.Name + " to the database (" + ProgressValue.ToString() + "/" + Maximum.ToString() + ")";
                ProgressValue++;
            }
        }
    }
}