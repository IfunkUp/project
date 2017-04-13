using SyncWPF.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZendeskApi_v2.HelpCenter;

namespace SyncWPF.workers
{
    class KnowledgeBaseWorker
    {

        public static async void GetAll()
        {
            GetAllKBCategories();
            GetAllKBSections();
        }



        public static async void GetAllKBCategories()
        {
            var list = await KnowledgeBaseHelper.GetAllKBCategories();
            foreach (var item in list)
            {
                Firebirdhelper.SaveCategories(item);
            }
        }
        public static async void GetAllKBSections()
        {
            var list = await KnowledgeBaseHelper.GetAllKBSections();
            foreach (var item in list)
            {
                Firebirdhelper.SaveSections(item);
            }
        }
        public static async void GetAllKBArticles()
        {
            var collection = await KnowledgeBaseHelper.GetAllKBArticles();
            foreach (var item in collection)
            {
                Firebirdhelper.SaveArticles(item);
            }
        }
    }
}
