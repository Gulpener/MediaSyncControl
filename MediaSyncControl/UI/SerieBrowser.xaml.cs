using MediaSyncControl.EF;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace MediaSyncControl.UI
{
    /// <summary>
    /// Interaction logic for SerieBrowser.xaml
    /// </summary>
    public partial class SerieBrowser : UserControl
    {
        DataTable table = new DataTable();
        public SerieBrowser()
        {
            table.Columns.Add("EpisodeID");
            table.Columns.Add("EpisodeName");
            table.Columns.Add("Season");
            table.Columns.Add("seen");
            table.Columns.Add("Gezien");
            
            InitializeComponent();
            fillComboBox();
            combSerieList.SelectedIndex = 0;
            

        }

        private void resultDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null && e.ChangedButton == MouseButton.Left)
            {
                DataGridRow dgr = sender as DataGridRow;

                DataRow drow = table.Rows[dgr.GetIndex()];
                //MessageBox.Show(dgr.GetIndex().ToString() + " " + drow[0]);
                /*
                string episodepath = DatabaseAdapter.GetEpisodePath(Int32.Parse(drow[0].ToString()));
                MessageBox.Show(episodepath);
                 * */
                MediaPlayerWindow mpw = new MediaPlayerWindow();
                mpw.GiveEpisodeID(Int32.Parse(drow[0].ToString()));
                mpw.Show();
                mpw.Start();

            }
        }

        private void fillComboBox()
        {
            List<string> seriesList = new List<string>();

            seriesList = DatabaseAdapter.getSeriesList();
            combSerieList.Items.Clear();
            foreach (string serie in seriesList)
            {
                combSerieList.Items.Add(serie);
            }
            
        }

        private void combSerieList_DropDownOpened(object sender, EventArgs e)
        {
            fillComboBox();
        }

        private void combSerieList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<Episode> episodeList = new List<Episode>();
            if (combSerieList.SelectedItem != null)
            {
                string seriename = combSerieList.SelectedItem.ToString();
                episodeList = DatabaseAdapter.getEpisodeList(seriename);
                if (episodeList != null)
                {                                                                               
                    //table.Columns.Add("EpisodePath");
                    table.Clear();
                    foreach (Episode epp in episodeList)
                    {
                        double watched = epp.WatchedTime;
                        double total = epp.TotalTime;
                        long seen = 0;
                        if (watched == 0 || total == 0)
                        {
                            seen = 0;
                        }
                        else
                        {
                            seen = Convert.ToInt64(watched / total * 100);
                        }
                        
                        table.Rows.Add(epp.EpisodeId, epp.EpisodeName, epp.SeasonID,seen + "%", epp.HaveSeen/*, epp.FilePath*/);
                    }
                    gvEpisodeList.DataContext = table;
                    
                    gvEpisodeList.CanUserAddRows = false;                    
                }
            }
            
        }

        
    }
}

/*
string path = "C:\\stagelist.txt";

	string extension = Path.GetExtension(path);
	string filename = Path.GetFileName(path);
	string filenameNoExtension = Path.GetFileNameWithoutExtension(path);
	string root = Path.GetPathRoot(path);
 * 
 * Om geen gebruik te hoeven maken van de // kan er een @ voorgezet worden
 * string value = @"C:\directory\word.txt";
*/