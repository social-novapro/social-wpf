using social_wpf.Models;
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

namespace social_wpf.Controls
{
    /// <summary>
    /// Interaction logic for ThreadStatusCardControl.xaml
    /// </summary>
    public partial class ThreadStatusCardControl : UserControl
    {
        public ThreadStatusCardControl(ThreadStatus status)
        {
            InitializeComponent();

            NameTextBlock.Text = status.Name;
            StateTextBlock.Text = $"State: {status.State}";
            MessageTextBlock.Text = status.Message;

            string updatedText = $"Updated: {status.LastUpdated:T}";

            if (status.NextUpdateAt != null)
            {
                double secondsLeft = (status.NextUpdateAt.Value - DateTime.Now).TotalSeconds;

                if (secondsLeft > 0)
                {
                    updatedText += $" • Next update in {Math.Ceiling(secondsLeft)}s";
                }
                else
                {
                    updatedText += " • Next update due";
                }
            }

            UpdatedTextBlock.Text = updatedText;
        }
    }
}
