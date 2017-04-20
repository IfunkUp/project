using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using ZendeskApi_v2.Models.Tickets;
using satisfaction = ZendeskApi_v2.Models.Satisfaction.SatisfactionRating;
using SyncWPF.helpers;
using ZendeskApi_v2.Models.Sections;
using ZendeskApi_v2.Models.Categories;
using ZendeskApi_v2.Models.HelpCenter.Categories;
using Category =  ZendeskApi_v2.Models.HelpCenter.Categories.Category;
using ZendeskApi_v2.Models.Articles;
using ZendeskApi_v2.Models.Organizations;
using ZendeskApi_v2.Models.Users;

namespace SyncWPF
{
    class Firebirdhelper
    {
        #region Connection
        // credentials of the database
        public static string Connectionstring()
        {
            FbConnectionStringBuilder csb = new FbConnectionStringBuilder();
            csb.Database = "localhost";
            csb.Port = 3050;
            //csb.Database = @"C:\Users\trainee\OneDrive\dag\stage\stageproject\ZENDESK.FDB"; //SMS
            csb.Database = @"X:\OneDrive\dag\stage\stageproject\ZENDESK.FDB"; //thuis
            //csb.Database = @"C:\Users\steve\OneDrive\dag\stage\stageproject\ZENDESK.FDB"; //book
            csb.UserID = "SYSDBA";
            csb.Password = "masterkey";
            csb.ServerType = FbServerType.Default;
            return csb.ToString();
        }

        //establish connection with the database
        public static FbConnection Openconnection(string connectionstring)
        {
            FbConnection con = null;
            try
            {
                con = new FbConnection(connectionstring);
                con.Open();
                return con;
            }
            catch (Exception e)
            {
                if (con != null)
                {
                    Console.WriteLine(e);
                    con.Dispose();
                }
                throw;
            }
        }




        #endregion

        #region Tickets


        public static void SaveTicket(Ticket ticket)
        {
            
            using (var con = Openconnection(Connectionstring()))
            {
                using (var tran = con.BeginTransaction())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Transaction = tran;
                        cmd.CommandText = "INSERT_TICKET";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("IN_ID", long.Parse(ticket.Id.ToString()));
                        if (!string.IsNullOrEmpty(ticket.AssigneeId.ToString()))
                        {
                            cmd.Parameters.AddWithValue("IN_ASS_US_ID", long.Parse(ticket.AssigneeId.ToString()));
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("IN_ASS_US_ID", 0);
                        }
                       
                       // cmd.Parameters.AddWithValue("IN_SUBMITTER_ID", long.Parse(ticket.SubmitterId.ToString()));
                        cmd.Parameters.AddWithValue("IN_CREATED", ticket.CreatedAt);
                        cmd.Parameters.AddWithValue("IN_UPDATED", ticket.UpdatedAt);
                        cmd.Parameters.AddWithValue("IN_SUBJECT", DataHelper.NulltoString( ticket.Subject));
                        //cmd.Parameters.AddWithValue("IN_DESCRIPTION", DataHelper.NulltoString(ticket.Description));
                        cmd.Parameters.AddWithValue("IN_STATUS",DataHelper.Status(ticket.Status));
                        cmd.Parameters.AddWithValue("IN_TYPE", DataHelper.NulltoString( ticket.Type));
                        cmd.Parameters.AddWithValue("IN_TAGS", DataHelper.ListToString(ticket.Tags));
                        cmd.Parameters.AddWithValue("IN_URL", ticket.Url);
                        cmd.Parameters.AddWithValue("IN_VIA", DataHelper.NulltoString( ticket.Via.Channel));
                        cmd.Parameters.AddWithValue("IN_HAS_INCIDENTS",DataHelper.BoolToNum( ticket.HasIncidents.ToString()));
                        cmd.Parameters.AddWithValue("IN_RECEPIENT", DataHelper.NulltoString( ticket.Recipient));


                        cmd.ExecuteNonQuery();
                        tran.Commit();
                    }
                }
            }
        }

        public static void SaveComment(Comment comment,long fkId)
        {
            using (var con = Openconnection(Connectionstring()))
            {
                using (var tran = con.BeginTransaction())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Transaction = tran;
                        cmd.CommandText = "INSERT_COMMENT";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                   
                        cmd.Parameters.AddWithValue("IN_ID",long.Parse(comment.Id.ToString()));
                        cmd.Parameters.AddWithValue("IN_TK_ID", fkId);
                        cmd.Parameters.AddWithValue("IN_CREATED", comment.CreatedAt);
                        cmd.Parameters.AddWithValue("IN_US_ID",long.Parse( comment.AuthorId.ToString()));
                        cmd.Parameters.AddWithValue("IN_PUBLIC",DataHelper.BoolToNum( comment.Public.ToString()));
                        cmd.Parameters.AddWithValue("IN_MESSAGE", comment.Body);

                        cmd.ExecuteNonQuery();
                        tran.Commit();
                    }
                }
            }
        }


        public static void SaveSatisfaction(satisfaction satisfaction)
        {
            using (var con = Openconnection(Connectionstring()))
            {
                using (var tran = con.BeginTransaction())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Transaction = tran;
                        cmd.CommandText = "INSERT_SATISFACTION";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("IN_SAT_ID", satisfaction.Id);
                        cmd.Parameters.AddWithValue("IN_SCORE", DataHelper.Score(satisfaction.Score.ToString()));
                        cmd.Parameters.AddWithValue("IN_SAT_COMMENT", satisfaction.Comment);
                        cmd.Parameters.AddWithValue("IN_SAT_ASS_ID", satisfaction.AssigneeId);
                        if (satisfaction.CreatedAt != null)
                            cmd.Parameters.AddWithValue("IN_SAT_CREATED", satisfaction.CreatedAt.Value.LocalDateTime);
                        


                        cmd.ExecuteNonQuery();
                        tran.Commit();
                        //cmd.Parameters.AddWithValue("IN_SAT_CREATED",satisfaction.CreatedAt.Value.Date.ToString());
                        //cmd.Parameters.AddWithValue("IN_SAT_CREATED",Int16.Parse( DateTime.Now.ToString()));
                        //cmd.Parameters.AddWithValue("IN_TK_ID", satisfaction.TicketId);

                    }
                }
            }
        }


        public static void SaveOrganization(Organization organization)
        {
            using (var con = Openconnection(Connectionstring())) 
            {
                using (var tran = con.BeginTransaction())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Transaction = tran;
                        cmd.CommandText = "INSERT_ORGANIZATION";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("IN_ID", float.Parse(organization.Id.ToString()));
                        cmd.Parameters.AddWithValue("IN_NAME", organization.Name);
                        cmd.Parameters.AddWithValue("IN_REGION", organization.Tags);
                        cmd.Parameters.AddWithValue("IN_SALES_ID", 1);

                        cmd.ExecuteNonQuery();
                        tran.Commit();

                    }
                }
            }
        }

        public static void SaveUser(User user)
        {
            using (var con = Openconnection(Connectionstring()))
            {
                using (var tran = con.BeginTransaction())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        
                        cmd.Transaction = tran;
                        cmd.CommandText = "INSERT_USER";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;


                        cmd.Parameters.AddWithValue("IN_ID", float.Parse( user.Id.ToString()));
                        cmd.Parameters.AddWithValue("IN_NAME", user.Name);
                        cmd.Parameters.AddWithValue("IN_EMAIL", user.Email);
                        if (!string.IsNullOrEmpty(user.OrganizationId.ToString()))
                        {
                            cmd.Parameters.AddWithValue("IN_ORG_ID", float.Parse(user.OrganizationId.ToString()));
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("IN_ORG_ID", -1);
                        }

                        cmd.ExecuteNonQuery();
                        tran.Commit();




                    }
                }
            }





        }




        #endregion

        #region Knowledge base

        public static void SaveSections(Section section)
        {
            using (var con = Openconnection(Connectionstring()))
            {
                using (var tran = con.BeginTransaction())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Transaction = tran;
                        cmd.CommandText = "INSERT_SECTION";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("IN_ID",int.Parse( section.Id.ToString()));
                        cmd.Parameters.AddWithValue("IN_NAME", section.Name);
                        cmd.Parameters.AddWithValue("IN_DESCRIPTION", section.Description);
                        cmd.Parameters.AddWithValue("IN_URL", section.Url);
                        cmd.Parameters.AddWithValue("IN_FK_CAT_ID",int.Parse( section.CategoryId.ToString()));
                        cmd.Parameters.AddWithValue("IN_CREATED", section.CreatedAt);
                        cmd.Parameters.AddWithValue("IN_UPDATED", section.UpdatedAt);


                        cmd.ExecuteNonQuery();
                        tran.Commit();
                        

                    }
                }
            }
        }

        public static void SaveCategories(Category category)
        {
            using (var con = Openconnection(Connectionstring()))
            {
                using (var tran = con.BeginTransaction())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Transaction = tran;
                        cmd.CommandText = "INSERT_CATEGORY";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("IN_ID", int.Parse(category.Id.ToString()));
                        cmd.Parameters.AddWithValue("IN_NAME", category.Name.ToString());
                        cmd.Parameters.AddWithValue("IN_DESC", category.Description.ToString());
                        cmd.Parameters.AddWithValue("IN_CREATED", category.CreatedAt);
                        cmd.Parameters.AddWithValue("IN_UPDATED", category.UpdatedAt);



                        cmd.ExecuteNonQuery();
                        tran.Commit();

                    }

                }
            }
        }
        public static void SaveArticles(Article article)
        {
            using (var con = Openconnection(Connectionstring()))
            {
                using (var tran = con.BeginTransaction())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Transaction = tran;
                        cmd.CommandText = "INSERT_ARTICLES";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("IN_ID", float.Parse(article.Id.ToString()));
                        cmd.Parameters.AddWithValue("IN_URL", article.Url.ToString());
                        cmd.Parameters.AddWithValue("IN_TITLE", article.Title.ToString());
                       // cmd.Parameters.AddWithValue("IN_BODY", article.Body.ToString());
                        cmd.Parameters.AddWithValue("IN_AUTHOR_ID", float.Parse( article.AuthorId.ToString()));
                        cmd.Parameters.AddWithValue("IN_LABEL_NAMES", DataHelper.ArrToString( article.LabelNames));
                        //cmd.Parameters.AddRange("IN_LABEL_NAMES", article.LabelNames);
                        cmd.Parameters.AddWithValue("IN_DRAFT", DataHelper.BoolToNum(article.Draft.ToString()));
                        cmd.Parameters.AddWithValue("IN_PROMOTED", DataHelper.BoolToNum(article.Promoted.ToString()));
                        cmd.Parameters.AddWithValue("IN_VOTE_SUM", int.Parse(article.VoteSum.ToString()));
                        cmd.Parameters.AddWithValue("IN_VOTE_COUNT",int.Parse(article.VoteCount.ToString()));
                        cmd.Parameters.AddWithValue("IN_SEC_ID", float.Parse(article.SectionId.ToString()));
                        cmd.Parameters.AddWithValue("IN_CREATED", article.CreatedAt);
                        cmd.Parameters.AddWithValue("IN_UPDATED", article.UpdatedAt);

                       cmd.ExecuteNonQuery();
                        tran.Commit();

                    }
                }
            }
        }







        #endregion

    }
}
