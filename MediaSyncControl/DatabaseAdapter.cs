using MediaSyncControl.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaSyncControl
{
    class DatabaseAdapter
    {
        internal static void SaveNewEpisode(string serienaam, int seasonid, string episode, string episodepath)
        {
            int serieid = getSerieID(serienaam);

            bool existcheck = existCheck(episode);
            if(existcheck == false)
            {
                using (var ctx = new Context())
                {
                    Episode epi = new Episode()
                    {
                        SerieID = serieid,
                        SerieName = serienaam,
                        SeasonID = seasonid,
                        EpisodeName = episode,
                        FilePath = episodepath
                    };
                    ctx.Episodes.Add(epi);
                    ctx.SaveChanges();
                }
            }             
        }

        private static int getSerieID(string serienaam)
        {
            List<int> serieIdList = new List<int>();
            bool containcheck = false;
            int serieid = 0;
            using (var ctx = new Context())
            {
                foreach (Episode episode in ctx.Episodes)
                {
                    if (serienaam == episode.SerieName)
                    {
                        serieid = episode.SerieID;
                        containcheck = true;
                    }
                    if(serieIdList.Contains(episode.SerieID) == false)
                    {
                        serieIdList.Add(episode.SerieID);
                    }
                }
            }

            if (containcheck == false)
            {
                serieIdList = serieIdList.OrderByDescending(c => c).ToList();
                if (serieIdList.Count != 0)
                {
                    serieid = serieIdList.Last() + 1;
                }
                else
                {
                    serieid = 1;
                }
            }

            return serieid;
        }

        private static bool existCheck(string episode)
        {
            bool existcheck = false;
            using (var ctx = new Context())
            {
                foreach (Episode epi in ctx.Episodes)
                {
                    if (epi.EpisodeName == episode)
                    {
                        existcheck = true;
                    }
                }
            }
            return existcheck;
        }

        internal static List<string> getSeriesList()
        {
            List<string> seriesList = new List<string>();
            using (var ctx = new Context())
            {
                List<Episode> serieList = ctx.Episodes.ToList<Episode>();

                foreach (Episode epp in serieList)
                {
                    if (seriesList.Contains(epp.SerieName) == false)
                    {
                        seriesList.Add(epp.SerieName);
                    }
                }
            }
            return seriesList;
        }

        internal static List<Episode> getEpisodeList(string serieName)
        {
            List<Episode> epplist = new List<Episode>(); 

            using (var ctx = new Context())
            {
                foreach (Episode epp in ctx.Episodes)
                {
                    if (epp.SerieName == serieName)
                    {
                        epplist.Add(epp);
                    }
                }
            }
            return epplist;
        }



        internal static string GetEpisodePath(int episodeid)
        {
            using (var ctx = new Context())
            {
                Episode episode = ctx.Episodes.Where(b => b.EpisodeId == episodeid).FirstOrDefault();
                return episode.FilePath;
            }           
        }

        internal static string GetEpisodeName(int episodeID)
        {
            using (var ctx = new Context())
            {
                Episode episode = ctx.Episodes.Where(b => b.EpisodeId == episodeID).FirstOrDefault();
                return episode.EpisodeName;
            } 
        }

        internal static TimeSpan GetLastWatchedTime(int episodeID)
        {
            using (var ctx = new Context())
            {
                Episode episode = ctx.Episodes.Where(b => b.EpisodeId == episodeID).FirstOrDefault();
                TimeSpan timespan = new TimeSpan(0,0,0,0,Convert.ToInt32(episode.WatchedTime));
                return timespan;
            } 
        }

        internal static void setTotalTime(int episodeid, double totaltime)
        {
            using (var ctx = new Context())
            {
                Episode episode = ctx.Episodes.Where(b => b.EpisodeId == episodeid).FirstOrDefault();
                episode.TotalTime = totaltime;
                ctx.SaveChanges();
            }
        }

        internal static void setWatchedTime(int episodeid, TimeSpan timeSpan)
        {
            using (var ctx = new Context())
            {
                Episode episode = ctx.Episodes.Where(b => b.EpisodeId == episodeid).FirstOrDefault();
                episode.WatchedTime = timeSpan.TotalMilliseconds;
                ctx.SaveChanges();
            }
        }
    }
}
