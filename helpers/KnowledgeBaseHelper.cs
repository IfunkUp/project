using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZendeskApi_v2.Models;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.Articles;
using ZendeskApi_v2.Models.HelpCenter.Categories;
using ZendeskApi_v2.Models.Sections;
using ZendeskApi_v2.HelpCenter;

namespace SyncWPF.helpers
{
    class KnowledgeBaseHelper
    {
        private static string s_UserName = "info@sms-timing.com";
        private static string s_PassWord = "smssms123";
        private static string s_Url = "http://smstiming.zendesk.com";


        private static ZendeskApi s_Client = new ZendeskApi(s_Url, s_UserName, s_PassWord);



        #region GetAllAndEverything


        public static async Task<List<Article>> GetAllKBArticles()
        {
            var list = new List<Article>();
            var response = await s_Client.HelpCenter.Articles.GetArticlesAsync(ZendeskApi_v2.Requests.HelpCenter.ArticleSideLoadOptionsEnum.None,null,100);

            do
            {
                list.AddRange(response.Articles);
                if (!string.IsNullOrWhiteSpace(response.NextPage))
                {
                    response = await s_Client.HelpCenter.Articles.GetByPageUrlAsync<GroupArticleResponse>(response.NextPage);
                }




            } while (list.Count != response.Count);

            return list;
        }

        public static async Task<List<Category>> GetAllKBCategories()
        {

            var list = new List<Category>();
            var response = await s_Client.HelpCenter.Categories.GetCategoriesAsync();

            do
            {
                list.AddRange(response.Categories);

               


            } while (list.Count != response.Count);
            return list;



            
        }

        public static async Task<List<Section>> GetAllKBSections()
        {
            var list = new List<Section>();
            var response = await s_Client.HelpCenter.Sections.GetSectionsAsync();

            do
            {
                list.AddRange(response.Sections);
                if (!string.IsNullOrWhiteSpace(response.NextPage))
                {
                    response = await s_Client.HelpCenter.Sections.GetByPageUrlAsync<GroupSectionResponse>(response.NextPage);
                }



            } while (list.Count != response.Count);
            return list;
        }


        #endregion





    }
}
