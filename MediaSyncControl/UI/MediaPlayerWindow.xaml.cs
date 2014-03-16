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
using System.Windows.Shapes;

namespace MediaSyncControl.UI
{
    /// <summary>
    /// Interaction logic for MediaPlayerWindow.xaml
    /// </summary>
    public partial class MediaPlayerWindow : Window
    {
        int episodeID;
        string episodepath;

        public MediaPlayerWindow()
        {
            InitializeComponent();            
        }

        internal void GiveEpisodeID(int episodeid)
        {
            this.episodeID = episodeid;
            this.Title = DatabaseAdapter.GetEpisodeName(episodeID);          
        }

        internal void Start()
        {
            mediaPlayerControl.setSource(episodeID);
            mediaPlayerControl.Play();
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mediaPlayerControl.Closing();
        }
    }
}
