using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace SyncWPF.workers
{
    class ProgressWorker
    {

        public static ProgressBar initProgress(double max, bool indeterminate, Brush color)
        {
            var pbar = new ProgressBar();
            pbar.Maximum = max;
            pbar.IsIndeterminate = indeterminate;
            pbar.BorderBrush = color;

            return pbar;



        }









    }
}
