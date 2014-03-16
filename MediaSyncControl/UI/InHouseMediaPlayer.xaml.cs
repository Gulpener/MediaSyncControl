using MediaSyncControl.EF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace MediaSyncControl.UI
{
    /// <summary>
    /// Interaction logic for InHouseMediaPlayer.xaml
    /// </summary>
    public partial class InHouseMediaPlayer : UserControl
    {
        bool expanded = false;

        DataTable table = new DataTable();
        CollectionView view;

        bool firstview = false;

        public InHouseMediaPlayer()
        {
            InitializeComponent();
            GridLength gridlength = new GridLength(0, GridUnitType.Pixel);
            clmEpisodeSelector.Width = gridlength;
            clmEpisodeSelectorContentBorder.Width = gridlength;
            gridlength = new GridLength(4, GridUnitType.Pixel);

            table.Columns.Add("EpisodeID");
            table.Columns.Add("EpisodeName");
            table.Columns.Add("Season");
            table.Columns.Add("seen");
            table.Columns.Add("Gezien");
            
            InitializeComponent();
            fillComboBox();
            combSerieList.SelectedIndex = 0;
            
        }

        private void expandEpisodeContent(bool toggle)
        {
            int expander = 1;
            int borderlength = 0;
            bool happened = false;
            if (expanded)
            {
                expander = -1;
                expanded = false;
                borderlength = 0;
                happened = true;
            }
            else if (toggle == true)
            {
                expander = 1;
                borderlength = 1;
                expanded = true;
                happened = true;
            }
            else
            {
                
            }
            if (happened == true)
            {
                double contentlength = clmEpisodeSelectorContent.Width.Value;
                for (int i = 0; i < 400; i++)
                {
                    GridLength gridlength = new GridLength(contentlength + i * expander, GridUnitType.Pixel);

                    clmEpisodeSelectorContent.Width = gridlength;
                    clmEpisodeSelectorContentBorder.Width = new GridLength(borderlength, GridUnitType.Pixel);
                    gridlength = new GridLength(clmEpisodeSelectorContent.Width.Value + 32, GridUnitType.Pixel);
                }
            }
        }

        private void Grid_MouseLeave_1(object sender, MouseEventArgs e)
        {
            expandEpisodeContent(false);
            GridLength gridlength = new GridLength(0, GridUnitType.Pixel);
            clmEpisodeSelector.Width = gridlength;
            gridlength = new GridLength(4, GridUnitType.Pixel);     
        }

        private void Grid_MouseEnter_1(object sender, MouseEventArgs e)
        {
            GridLength gridlength = new GridLength(30, GridUnitType.Pixel);
            clmEpisodeSelector.Width = gridlength;
            gridlength = new GridLength(32 + clmEpisodeSelectorContent.Width.Value, GridUnitType.Pixel);
        }

        private void StackPanel_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            expandEpisodeContent(true);
        }

        private void resultDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null && e.ChangedButton == MouseButton.Left)
            {
                DataGridRow dgr = sender as DataGridRow;
                DataRow drow = table.Rows[dgr.GetIndex()];
                mediaPlayerControl.setSource(Int32.Parse(drow[0].ToString()));
                mediaPlayerControl.Play();
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

                        table.Rows.Add(epp.EpisodeId, epp.EpisodeName, epp.SeasonID, seen + "%", epp.HaveSeen/*, epp.FilePath*/);
                    }
                    
                    view = (CollectionView)CollectionViewSource.GetDefaultView(table);
                    if (firstview == false)
                    {
                        view.GroupDescriptions.Add(new PropertyGroupDescription("Season"));
                        firstview = true;
                    }
                                                             
                    gvEpisodeList.DataContext = view;
                    view = null;
                    gvEpisodeList.CanUserAddRows = false;
                }
            }

        }


        internal void Closing()
        {
            mediaPlayerControl.Closing();
        }
    }
}
