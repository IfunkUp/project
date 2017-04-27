using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncWPF.helpers
{
    class DataHelper
    {

        public static long ConvertToPrimitive(long? given)
        {
            return given ?? default(long);
        }

        public static int Score(string score)
        {
            switch (score.ToLower())
            {
                case "unofferd":
                    return 0;
                case "offered":
                    return 1;
                case "good":
                    return 2;
                case "bad":
                    return 3;
                default:
                    return 4;


            }
        }
        public static int BoolToNum(string given)
        {
            if (given == "true")
            {
                return 1;
            }
            return 0;

        }

        public static string ArrToString(String[] given)
        {
            var res = "";
            if (given != null && given.Length != 0)
            {
                foreach (var item in given)
                {
                    res += item + " ";
                }
            }
           
            return res;
        }
        public static string ListToString(IList<string> given)
        {
            var res = "";
            if (given != null && given.Count != 0)
            {
                foreach (var item in given)
                {
                    res += item + " ";
                }
            }
            return res;
        }
        public static int Status(string given)
        {
            switch (given.ToLower())
            {
                case "new":
                    return 0;
                case "open":
                    return 1;
                case "on hold":
                    return 3;
                case "solved":
                    return 4;
                default:
                    return 5;
            }

        }
        public static string NulltoString(string given)
        {
            if (string.IsNullOrEmpty(given))
            {
                return "empty";
            }
            else
            {
                return given;
            }
        }


        public static DateTimeOffset GetDateTimeOffset(DateTime? aDateTime)
        {
            if (aDateTime == null)
            {
                return (DateTimeOffset)DateTime.Now;
            }

            return (DateTimeOffset)aDateTime;
        }







    }
}
