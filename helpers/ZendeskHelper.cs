using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.Organizations;
using ZendeskApi_v2.Models.Tickets;
using ZendeskApi_v2.Models.Users;
using ZendeskApi_v2.Models.Satisfaction;
using SyncWPF.workers;
using SyncWPF.helpers;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json.Linq;
using satisfaction = SyncWPF.Classes.SatisFactionRating;
using SyncWPF.Classes;
using System.Web.Script.Serialization;
using System.Windows;

namespace SyncWPF
{
    public static class ZendeskHelper
    {
        private static string s_UserName = "info@sms-timing.com";
        private static string s_PassWord = "smssms123";
        private static string s_Url = "https://smstiming.zendesk.com";

        private static ZendeskApi s_Client = new ZendeskApi(s_Url, s_UserName, s_PassWord);
        



        #region GetAllAndEverything

        public static ZendeskApi LoadClient()
        {
            return s_Client;
        }
             
        public static async Task<List<User>> GetUsers()
        {
            var users = new List<User>();

            var response = await s_Client.Users.GetAllUsersAsync();
            do
            {
                users.AddRange(response.Users);
                if (!string.IsNullOrWhiteSpace(response.NextPage))
                {
                    response = await s_Client.Users.GetByPageUrlAsync<GroupUserResponse>(response.NextPage);
                }

            } while (users.Count != response.Count);
            return users;
        }

        public static async Task<List<Organization>> GetOrganisations()
        {
            var organizations = new List<Organization>();
            var organizationResponse = await s_Client.Organizations.GetOrganizationsAsync();
            
            do
            {
                organizations.AddRange(organizationResponse.Organizations);
                if (!string.IsNullOrWhiteSpace(organizationResponse.NextPage))
                {
                    organizationResponse =
                        await s_Client.Organizations.GetByPageUrlAsync<GroupOrganizationResponse>(
                            organizationResponse.NextPage);
                }

            } while (organizations.Count != organizationResponse.Count);
            return organizations;
        }

        public static async Task<List<Ticket>> GetTickets()
        {
            var ticketResponse = await s_Client.Tickets.GetAllTicketsAsync();
            var tickets = new List<Ticket>();

            do
            {
                tickets.AddRange(ticketResponse.Tickets);
                if (!string.IsNullOrWhiteSpace(ticketResponse.NextPage))
                    ticketResponse = await s_Client.Tickets.GetByPageUrlAsync<GroupTicketResponse>(
                        ticketResponse.NextPage);
            } while (tickets.Count != ticketResponse.Count);

            return tickets;
        }


       
        public static List<satisfaction> GetSatisfaction()
        {
            int teller = 0;
            var page = "";
            var listSat = new List<satisfaction>();
            var s_Client = new RestClient(s_Url + "/");
            s_Client.Authenticator = new HttpBasicAuthenticator(s_UserName, s_PassWord);
 
            do
            {
                teller++;
                page = "api/v2/satisfaction_ratings/received.json?page=" + teller;
                var request = new RestRequest(page, Method.GET);
                s_Client.AddDefaultHeader("Accept", "application/json");
                IRestResponse response = s_Client.Execute(request);
                var content = response.Content;
                JObject Satisfaction = JObject.Parse(content);


                foreach (JObject rating in Satisfaction.SelectToken("satisfaction_ratings"))
                {
                    listSat.Add(new SatisFactionRating()
                    {
                        Id = (long)rating.SelectToken("id"),
                        Url = (string)rating.SelectToken("url"),
                        Assignee_id = (long?)rating.SelectToken("assignee_id"),
                        Group_id = (long?)rating.SelectToken("group_id"),
                        Requester_id = (long?)rating.SelectToken("requester_id"),
                        Ticket_id = (long?)rating.SelectToken("ticket_id"),
                        Score = (string)rating.SelectToken("score"),
                        Created_at = (DateTime)rating.SelectToken("created_at"),
                        Updated_at = (DateTime)rating.SelectToken("updated_at"),
                        Comment = (string)rating.SelectToken("comment")
                    });
                }
                page = (string) Satisfaction.SelectToken("next_page");
            } while (!string.IsNullOrEmpty((string) page));
            
             

            return listSat;
        }

    









        






        #endregion

        #region GetIncremental

        public static async Task<List<Ticket>> GetLastTickets(DateTimeOffset start)
        {

            var ticketResponse = await s_Client.Tickets.GetInrementalTicketExportAsync(start);
            var tickets = new List<Ticket>();

            do
            {
                tickets.AddRange(ticketResponse.Tickets);
                
                
            } while (tickets.Count != ticketResponse.Count);
            return tickets;
            
        }

        public static async Task<List<User>> GetLastUsers(DateTimeOffset start)
        {
            
            return null;
        }


        #endregion

        #region add & delete


        public static void AddTicket(Ticket newticket)
        {
            s_Client.Tickets.CreateTicket(newticket);
        }

        public static void DeleteTicket(Ticket oldticket)
        {
            s_Client.Tickets.Delete(long.Parse(oldticket.Id.ToString()));
        }


        #endregion






        //region for other usefull apicalls
        #region RestOfRest

        public static List<Comment> GetComment(long ticketid)
        {

            var response = s_Client.Tickets.GetTicketComments(ticketid);
            var list = new List<Comment>();

            list.AddRange(response.Comments);


            return list;
        }

       


        #endregion
    }
}
