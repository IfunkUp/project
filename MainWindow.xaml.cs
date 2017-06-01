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

        private double m_OrgMaximum;
        private double m_UserMax;
        private double m_TicketsMax;
        private double m_SatMax;
        private double m_CatMax;
        private double m_SecMax;
        private double m_ArtMax;

        private double m_OrgProgressValue;
        private double m_UserProgressValue;
        private double m_TicketsProgressValue;
        private double m_SatisfactionProgressValue;
        private double m_CategoriesProgressValue;
        private double m_SectionsProgressValue;
        private double m_ArticlesProgressValue;


        public double OrganizationMaximum
        {
            get => m_OrgMaximum;
            set
            {
                m_OrgMaximum = value;
                OnPropertyChanged();
            }
        }
        public double OrganizationProgressValue
        {
            get => m_OrgProgressValue;
            set
            {
                m_OrgProgressValue = value;
                OnPropertyChanged();
            }
        }
        public double UserMax
        {
            get => m_UserMax;
            set { m_UserMax = value;
                OnPropertyChanged();
            }
        }
        public double UserProgressValue
        {
            get => m_UserProgressValue;
            set
            {
                m_UserProgressValue = value;
                OnPropertyChanged();
            }
        }

        public double TicketsMax
        {
            get => m_TicketsMax;
            set
            {
                m_TicketsMax = value;
                OnPropertyChanged();
            }
        }
        public double SatMax
        {
            get => m_SatMax; set
            {
                m_SatMax = value;
                OnPropertyChanged();
            }
        }
        public double CatMax
        {
            get => m_CatMax;
            set
            {
                m_CatMax = value;
                OnPropertyChanged();
            }
        }
        public double SecMax
        {
            get => m_SecMax;
            set
            {
                m_SecMax = value;
                OnPropertyChanged();
            }
        }
        public double ArtMax
        {
            get => m_ArtMax;
            set
            {
                m_ArtMax = value;
                OnPropertyChanged();
            }
        }

        public double TicketsProgressValue { get => m_TicketsProgressValue;
            set
            {
                m_TicketsProgressValue = value;
                OnPropertyChanged();
            }
                }
        public double SatisfactionProgressValue { get => m_SatisfactionProgressValue;
            set
            {
                m_SatisfactionProgressValue = value;
                OnPropertyChanged();
            }
        }
        public double CategoriesProgressValue { get => m_CategoriesProgressValue;
            set
            {
                m_CategoriesProgressValue = value;
                OnPropertyChanged();
            }
        }
        public double SectionsProgressValue { get => m_SectionsProgressValue;
            set
            {
                m_SectionsProgressValue = value;
                OnPropertyChanged();
            }
        }
        public double ArticlesProgressValue { get => m_ArticlesProgressValue;
            set
            {
                m_ArticlesProgressValue = value;
                OnPropertyChanged();
            }
        }

        private string m_OrgLabel;
        private string m_UserLabel;
        private string m_TicketLabel;
        private string m_RatingLabel;
        private string m_CategoryLabel;
        private string m_SectionLabel;
        private string m_ArticleLabel;

        public string OrgLabel { get => m_OrgLabel;
            set
            {
                m_OrgLabel = value;
                OnPropertyChanged();
            }
        }
        public string UserLabel { get => m_UserLabel;
            set
            {
                m_UserLabel = value;
                OnPropertyChanged();
            }
        }
        public string TicketLabel
        {
            get => m_TicketLabel;
            set
            {
                m_TicketLabel = value;
                OnPropertyChanged();
            }
        }
        public string RatingLabel { get => m_RatingLabel;
            set
            {
                m_RatingLabel = value;
                OnPropertyChanged();
            }
        }
        public string CategoryLabel { get => m_CategoryLabel;
            set
            {
                m_CategoryLabel = value;
                OnPropertyChanged();
            }
        }
        public string SectionLabel { get => m_SectionLabel;
            set
            {
                m_SectionLabel = value;
                OnPropertyChanged();
            }
        }
        public string ArticleLabel { get => m_ArticleLabel;
            set
            {
                m_ArticleLabel = value;
                OnPropertyChanged();
            }
        }

        public int LabelWidth
        {
            get => m_labelWidth;
            set
            {
                m_labelWidth = value;
                OnPropertyChanged();
            }
        }

        private int m_labelWidth;


        string fetch = "Fetching from Zendesk";
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
            int proginit = 1;
            int maxinit = 10000;
            LabelWidth = 300;


            OrganizationProgressValue = proginit;
            UserProgressValue = proginit;
            TicketsProgressValue = proginit;
            SatisfactionProgressValue = proginit;
            CategoriesProgressValue = proginit;
            SectionsProgressValue = proginit;
            ArticlesProgressValue = proginit;

            OrganizationMaximum = maxinit;
            UserMax = maxinit;
            TicketsMax = maxinit;
            SatMax = maxinit;
            CatMax = maxinit;
            SecMax = maxinit;
            ArtMax = maxinit;

            OrgLabel = "organisations";
            UserLabel = "Users";
            TicketLabel = "Tickets";
            RatingLabel = "Satisfaction Ratings";
            CategoryLabel = "knowledgebase categories";
            SectionLabel = "knowledgebase sections";
            ArticleLabel = "knowledgebase articles";



        }


        private void IncrementalSync_Click(object sender, RoutedEventArgs e)
        {

            Task.Factory.StartNew(() => GetTickets(SelectedDateOffset));

        }

        private void FullSync_Click(object sender, RoutedEventArgs e)
        {

            SyncUAndO();
            SyncTickets();
            SyncKB();
        }
        private void SyncUAndO()
        {
            Task.Factory.StartNew(() => GetOrganizations());
            Task.Factory.StartNew(() => GetUsers());
        }
        private void SyncTickets()
        {
            Task.Factory.StartNew(() => GetTickets());
            Task.Factory.StartNew(() => GetSatisfaction());
        }
        private void SyncKB()
        {
            Task.Factory.StartNew(() => GetKbCategories());
            Task.Factory.StartNew(() => GetKbSections());
            Task.Factory.StartNew(() => GetKbArticles());
        }

        #region myTasks

        #region Support Tickets
 
        public async void GetOrganizations()
        {
            OrgLabel = fetch;
            UserLabel = "waiting on Organisations";
            var organizations = await ZendeskHelper.GetOrganisations();
            OrganizationMaximum = organizations.Count();
            OrganizationProgressValue = 1;
            foreach (var item in organizations)
            {
                Firebirdhelper.SaveOrganization(item);
                OrgLabel = "importing organization (" + OrganizationProgressValue.ToString() + "/" + OrganizationMaximum.ToString() + ")  to the database";
                OrganizationProgressValue++;
            }
            OrgLabel = "organizations up to date!";
            UserLabel = fetch;
        }
        public async void GetUsers()
        {
           
            var users = await ZendeskHelper.GetUsers();
            UserMax = users.Count();           
            foreach (var item in users)
            {
                Firebirdhelper.SaveUser(item);
                UserLabel = "importing user (" + UserProgressValue.ToString() + "/" + UserMax.ToString() + ") to the database";
                UserProgressValue++;
            }
            UserLabel = "Users up to date!";
        }
        public async void GetTickets()
        {
            TicketLabel = fetch;
            var tickets = await ZendeskHelper.GetTickets();
            InsertTicket(tickets);
        }

        public async void GetTickets(DateTimeOffset SelectedDateTimeOffset)
        {
            TicketLabel = fetch;
            var tickets = await ZendeskHelper.GetLastTickets(SelectedDateTimeOffset);
            InsertTicket(tickets);
        }

        public void InsertTicket(List<Ticket> tickets)
        {
            TicketsMax = tickets.Count();
            TicketLabel = "importing to database";
            foreach (var item in tickets)
            {
                
                Firebirdhelper.SaveTicket(item);
                TicketLabel = "importing ticket " + TicketsProgressValue.ToString() + "/" + TicketsMax.ToString() + " to the database ";
                //GetAudit(item.Id);
                TicketsProgressValue++;
            }
            TicketLabel = "Tickets up to date!";
        }

        //public void GetAudit(Int64? ticketID)
        //{
        //    var audits = ZendeskHelper.GetAudit(ticketID);

        //    foreach (Audit item in audits)
        //    {
        //        Firebirdhelper.SaveAudit(item);
        //    }


            
        //}





        public async void GetSatisfaction()
        {
            RatingLabel = fetch;
            var satisfactions = ZendeskHelper.GetSatisfaction();
            SatMax = satisfactions.Count();

            foreach (var item in satisfactions)
            {
                Firebirdhelper.SaveSatisfaction(item);
                RatingLabel = "importing Satisfaction (" + SatisfactionProgressValue.ToString() + "/" + SatMax.ToString() + " to the database ";
                SatisfactionProgressValue++;
            }
            RatingLabel = "Satisfaction ratings up to date!";
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
            ArticleLabel = fetch;
            var articles = await KnowledgeBaseHelper.GetAllKBArticles();
            ArtMax = articles.Count();

            foreach (var item in articles)
            {
                Firebirdhelper.SaveArticles(item);
                ArticleLabel = "importing article " + ArticlesProgressValue.ToString() + "/" + ArtMax.ToString() + " to the database";
                ArticlesProgressValue++;
            }
            ArticleLabel = "knowledgebase articles up to date!";
        }


        public async void GetKbSections()
        {
            SectionLabel = fetch;
            var sections = await KnowledgeBaseHelper.GetAllKBSections();
            SecMax = sections.Count(); ;

            foreach (var item in sections)
            {
                Firebirdhelper.SaveSections(item);
                SectionLabel = "importing section " + SectionsProgressValue.ToString() + "/" + SecMax.ToString() + " to the database ";
                SectionsProgressValue++;
            }
            SectionLabel = "knowledgebase sections up to date!";
        }

        public async void GetKbCategories()
        {
            CategoryLabel = fetch;
            var categories = await KnowledgeBaseHelper.GetAllKBCategories();
            CatMax = categories.Count();

            foreach (var item in categories)
            {
                Firebirdhelper.SaveCategories(item);
                CategoryLabel = "importing category " + CategoriesProgressValue.ToString() + "/" + CatMax.ToString() + " to the database ";
                CategoriesProgressValue++;
            }
            CategoryLabel = "knowledgebase categories up to date!";

        }






        #endregion
        #endregion
    }
}