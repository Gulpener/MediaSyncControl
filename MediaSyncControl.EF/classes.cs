/*
This General Public License does not permit incorporating your program into
proprietary programs.  If your program is a subroutine library, you may
consider it more useful to permit linking proprietary applications with the
library.  If this is what you want to do, use the GNU Lesser General
Public License instead of this License.
*/
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaSyncControl.EF
{
    public class Episode
    {
        public Episode()
        {
        }
        public int EpisodeId { get; set; }
        public int SerieID { get; set; }
        public string SerieName { get; set; }
        public int SeasonID { get; set; }
        public string EpisodeName { get; set; }
        public string FilePath { get; set; }
        public double TotalTime { get; set; }
        public double WatchedTime { get; set; }
        public bool HaveSeen { get; set; }

    }

    public class Context : DbContext
    {
        public static string connectionstring;
        public Context()
            : base(connectionstring)
        {
            Database.SetInitializer<Context>(new CreateDatabaseIfNotExists<Context>());
        }
        public DbSet<Episode> Episodes { get; set; }
    }
}
