using MediaSyncControl.EF;
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

namespace MediaSyncControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Context.connectionstring = "Data Source=.\\SQLEXPRESS;Initial Catalog=MediaSyncControl;Integrated Security=true";
            
            InitializeComponent();

        }

        private void MediaSyncControl_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            inHouseMediaPlayer.Closing();
        }
    }
}
