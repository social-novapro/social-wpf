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
            UpdatedTextBlock.Text = $"Updated: {status.LastUpdated:T}";
        }
    }
}
