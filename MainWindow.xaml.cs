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
using ZendeskApi_v2.Models.Articles;
using SyncWPF.Classes;

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

        #region declarations

        private DateTime? m_SelectedDate = DateTime.Now;
        
        public DateTimeOffset SelectedDateOffset => DataHelper.GetDateTimeOffset(SelectedDate);
        public DateTime? SelectedDate
        {
            get => m_SelectedDate;
            set
            {
                m_SelectedDate = value;
                OnPropertyChanged();
            }
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
        #endregion
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
            ProgressValue = 1;
        }


        private void IncrementalSync_Click(object sender, RoutedEventArgs e)
        {

            Task.Factory.StartNew(() => GetTickets(SelectedDateOffset));

        }

        private void FullSync_Click(object sender, RoutedEventArgs e)
        {
            Firebirdhelper.SaveUser(
            ZendeskHelper.GetUser(16553321147));

            //Task.Factory.StartNew(() => GetOrganizations());
            //Task.Factory.StartNew(() => GetUsers());
           // Task.Factory.StartNew(() => GetTickets());
            //Task.Factory.StartNew(() => GetSatisfaction());

            //Task.Factory.StartNew(() => GetKbCategories());
            //Task.Factory.StartNew(() => GetKbSections());
           // Task.Factory.StartNew(() => GetKbArticles());

        }

        #region myTasks

        #region Support Tickets
 
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
        public async void GetUsers()
        {
            var users = await ZendeskHelper.GetUsers();
            Maximum = users.Count();
            
            foreach (var item in users)
            {
                Firebirdhelper.SaveUser(item);
                Comment = "importing  " + item.Name + " to the database (" + ProgressValue.ToString() + "/" + Maximum.ToString() + ")";
                ProgressValue++;
            }
        }
        public async void GetTickets()
        {
            var tickets = await ZendeskHelper.GetTickets();
            InsertTicket(tickets);
        }

        public async void GetTickets(DateTimeOffset SelectedDateTimeOffset)
        {
            var tickets = await ZendeskHelper.GetLastTickets(SelectedDateTimeOffset);
            InsertTicket(tickets);
        }

        public void InsertTicket(List<Ticket> tickets)
        {
            Maximum = tickets.Count();

            foreach (var item in tickets)
            {
                
                Firebirdhelper.SaveTicket(item);
                Comment = "importing ticket " + ProgressValue.ToString() + " of " + Maximum.ToString() + " to the database ";
                GetAudit(item.Id);
                ProgressValue++;
            }
        }

        public void GetAudit(Int64? ticketID)
        {
            var audits = ZendeskHelper.GetAudit(ticketID);

            foreach (Audit item in audits)
            {
                Firebirdhelper.SaveAudit(item);
            }


            
        }





        public async void GetSatisfaction()
        {
            var satisfactions = ZendeskHelper.GetSatisfaction();
            Maximum = satisfactions.Count();

            foreach (var item in satisfactions)
            {
                Firebirdhelper.SaveSatisfaction(item);
                Comment = "importing ticket " + ProgressValue.ToString() + " of " + Maximum.ToString() + " to the database ";
                ProgressValue++;
            }
        }
        public async void GetComments(Ticket item )
        {
             var commentList = new List<Comment>();
                commentList.AddRange(ZendeskHelper.GetComment(DataHelper.ConvertToPrimitive(item.Id)));
                foreach (var comment in commentList)
                {
                    Firebirdhelper.SaveComment(comment, DataHelper.ConvertToPrimitive(item.Id));
                }
        }
        #endregion

        #region KnowledgeBaseTasks


        public async void GetKbArticles()
        {
            var articles = await KnowledgeBaseHelper.GetAllKBArticles();
            Maximum = articles.Count();

            foreach (var item in articles)
            {
                Firebirdhelper.SaveArticles(item);
                Comment = "importing article " + ProgressValue.ToString() + " of " + Maximum.ToString() + " to the database ";
                ProgressValue++;
            }
           
        }


        public async void GetKbSections()
        {
            var sections = await KnowledgeBaseHelper.GetAllKBSections();
            Maximum = sections.Count(); ;

            foreach (var item in sections)
            {
                Firebirdhelper.SaveSections(item);
                Comment = "importing section " + ProgressValue.ToString() + " of " + Maximum.ToString() + " to the database ";
                ProgressValue++;
            }
        }

        public async void GetKbCategories()
        {
            var categories = await KnowledgeBaseHelper.GetAllKBCategories();
            Maximum = categories.Count();

            foreach (var item in categories)
            {
                Firebirdhelper.SaveCategories(item);
                Comment = "importing category " + ProgressValue.ToString() + " of " + Maximum.ToString() + " to the database ";
                ProgressValue++;
            }


        }






        #endregion
        #endregion
    }
}