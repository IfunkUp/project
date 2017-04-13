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

namespace SyncWPF
{
    public static class ZendeskHelper
    {
        private static string s_UserName = "pieter.dewitte@sms-timing.com";
        private static string s_PassWord = "awesomeness";
        private static string s_Url = "http://smstiming.zendesk.com";

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
            //var ticketResponse = await s_Client.Tickets.GetAllTicketMetricsAsync();
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


        //gives a nullexeption on around the 1400th result
        public static async Task<List<ZendeskApi_v2.Models.Satisfaction.SatisfactionRating>> GetSatisfaction()
        {
            // var response = await s_Client.SatisfactionRatings.GetSatisfactionRatingsAsync();
            var response = await s_Client.SatisfactionRatings.GetReceivedSatisfactionRatingsAsync();
            var list = new List<ZendeskApi_v2.Models.Satisfaction.SatisfactionRating>();
            do
            {
                list.AddRange(response.SatisfactionRatings);
                if (!string.IsNullOrWhiteSpace(response.NextPage))
                {
                    response = await s_Client.SatisfactionRatings.GetByPageUrlAsync<GroupSatisfactionResponse>(response.NextPage);
                }
            } while (list.Count != response.Count);
            

            return list;
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
