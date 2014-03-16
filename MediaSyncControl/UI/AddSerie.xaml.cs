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
using System.IO;
using MediaSyncControl.EF;

namespace MediaSyncControl.UI
{
    /// <summary>
    /// Interaction logic for AddSerie.xaml
    /// </summary>
    public partial class AddSerie : UserControl
    {
        string path;
        string seriespath;
        string episodePath;
        List<string> seasonslist;
        List<string> episodelist;

        List<string> addlist;

        public AddSerie()
        {
            InitializeComponent();
            
        }

        private void btnGetInformation_Click(object sender, RoutedEventArgs e)
        {
            path = @tbPath.Text;
            combSerieName.Items.Clear();
            lbSeasons.Items.Clear();
            lbEpisodes.Items.Clear();
            string[] series = Directory.GetDirectories(path);
            List<string> serieslist = new List<string>();
            foreach (string serie in series)
            {
                serieslist.Add(System.IO.Path.GetFileName(serie));
            }
            serieslist.Sort();
            foreach (string serie in serieslist)
            {
                combSerieName.Items.Add(System.IO.Path.GetFileName(serie));
            }
            combSerieName.SelectedIndex = 1;
            combSerieName.SelectedIndex = 0;

        }

        private void combSerieName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lbSeasons.Items.Clear();
            seriespath = path + "\\" + combSerieName.SelectedItem;
                        
            string[] seasons = Directory.GetDirectories(seriespath);
            foreach (string season in seasons)
            {
                for (int l = 1; l < 50; l++)
                {
                    if (season.Contains("Season " + l) || season.Contains("Season." + l) || season.Contains("Season 0" + l) || season.Contains("Season.0" + l) || season.Contains("season " + l) || season.Contains("season." + l) || season.Contains("season 0" + l) || season.Contains("season.0" + l) || season.Contains("S0" + l) || season.Contains("S" + l) || season.Contains("s0" + l) || season.Contains("s" + l))
                    {
                        string Fromfol = seriespath + "\\" + System.IO.Path.GetFileName(season) + "\\";
                        string ToFol = seriespath + "\\Season " + l + "\\";
                        if (Fromfol != ToFol)
                        {
                            Directory.Move(Fromfol, ToFol);
                        }
                        
                    }
                }
            }
            seasons = Directory.GetDirectories(seriespath);
            seasonslist = new List<string>();
            foreach(string season in seasons)
            {
                seasonslist.Add(System.IO.Path.GetFileName(season));
            }
            seasonslist.Sort();
            foreach (string season in seasonslist)
            {
                lbSeasons.Items.Add(season);
            }
            lbSeasons.SelectedIndex = 1;
            lbSeasons.SelectedIndex = 0;
            

        }


        private void lbSeasons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lbEpisodes.Items.Clear();
            episodelist = new List<string>();
            if (lbSeasons.SelectedItem != null)
            {
                string selectedSeason = lbSeasons.SelectedItem.ToString();
                episodelist = GetEpisodeList(seriespath, selectedSeason);
            }
            
            foreach (string file in episodelist)
            {
                lbEpisodes.Items.Add(file);
            }
            lbEpisodes.SelectedIndex = 0;

        }

        private List<string> GetEpisodeList(string seriespath, string selectedSeason)
        {
            episodelist.Clear();
            episodePath = seriespath + "\\" + selectedSeason ;
            
            string[] episodeFolders = Directory.GetDirectories(episodePath);
            if (episodeFolders.Length != 0)
            {
                for (int j = 0; j < episodeFolders.Length; j++)
                {
                    string[] episodesinFolders = Directory.GetFiles(episodeFolders[j].ToString());
                    for (int k = 0; k < episodesinFolders.Length; k++)
                    {
                        string Fromfol = episodesinFolders[k];
                        string ToFol = episodePath + "\\" + System.IO.Path.GetFileName(episodesinFolders[k]);
                        if (Fromfol != ToFol)
                        {
                            try
                            {
                                Directory.Move(Fromfol, ToFol);
                                
                            }
                            catch
                            {

                            }
                        }

                    }
                    Directory.Delete(episodeFolders[j].ToString(), true);
                }
            }
            string[] episodes = Directory.GetFiles(episodePath);
            for (int i = 0; i < episodes.Length; i++)
            {
                checkExtension(System.IO.Path.GetFullPath(episodes[i]));
            }
            episodelist.Sort();
            return episodelist;
        }

       

        private void checkExtension(string episodePath)
        {
            string extension = System.IO.Path.GetExtension(episodePath);
            switch (extension)
            {
                case "": break;
                case ".txt": break;
                case ".nfo": break;
                case ".sfv": break;
                case ".srr": break;
                case ".nzb": break;
                case ".nfo-orig": break;
                case ".mp3": break;
                case ".srt": break;
                case ".dat": break;
                case ".jpg": 
                    break;
                case ".mp4":
                    episodelist.Add(System.IO.Path.GetFileName(episodePath));
                    break;
                case ".mkv":
                    episodelist.Add(System.IO.Path.GetFileName(episodePath));
                    break;
                case ".avi":
                    episodelist.Add(System.IO.Path.GetFileName(episodePath));
                    break;
                case ".mpg":
                    episodelist.Add(System.IO.Path.GetFileName(episodePath));
                    break;
                default:
                    MessageBox.Show(extension + " not configured!");
                    break;
            }
        }

        private void btnAddSerie_Click(object sender, RoutedEventArgs e)
        {
            if (combSerieName.SelectedItem != null)
            {
                string serie = combSerieName.SelectedItem.ToString();
                AddSeries(serie, seriespath);
            }
            else
            {
                MessageBox.Show("No serie selected!");
            }
        }

        private void btnAddSeason_Click(object sender, RoutedEventArgs e)
        {
            if (combSerieName.SelectedItem != null || lbSeasons.SelectedItem != null)
            {
                string serie = combSerieName.SelectedItem.ToString();
                string season = lbSeasons.SelectedItem.ToString();
                AddSeason(serie, season, seriespath);
            }
            else
            {
                MessageBox.Show("No season selected!");
            }
        }

        private void btnAddEpisode_Click(object sender, RoutedEventArgs e)
        {
            if (combSerieName.SelectedItem != null || lbSeasons.SelectedItem != null || lbEpisodes.SelectedItem != null)
            {
                string serie = combSerieName.SelectedItem.ToString();
                string season = lbSeasons.SelectedItem.ToString();
                string episode = lbEpisodes.SelectedItem.ToString();
                AddEpisode(serie, season, episode, seriespath);
            }
            else
            {
                MessageBox.Show("No episode selected!");
            }                                 
        }

        private void AddSeries(string serie, string seriespath)
        {
            foreach (string season in seasonslist)
            {
                AddSeason(serie, season, seriespath);
            }
        }

        private void AddSeason(string serie, string season, string seriespath)
        {
            episodelist = GetEpisodeList(seriespath, season);
            foreach (string episode in episodelist)
            {
                AddEpisode(serie, season, episode, seriespath);
            }
        }

        private void AddEpisode(string serie, string season, string episode, string seriespath)
        {
            int seasonid = 0;
            for (int i = 1; i < 50; i++)
            {
                if (i < 10)
                {
                    if (season.Contains(" " + i)||season.Contains("0" + i))
                    {
                        seasonid = i;
                    }
                }
                else
                {
                    if (season.Contains(i.ToString()))
                    {
                        seasonid = i;
                    }
                }
            }
            DatabaseAdapter.SaveNewEpisode(serie, seasonid, episode, seriespath + "\\" + season + "\\" + episode); 
        }
        
    }
}
