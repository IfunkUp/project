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













    }
}
