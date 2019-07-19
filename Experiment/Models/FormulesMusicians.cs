using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Experiment.Models
{
    public class FormulesMusicians
    {
        [ForeignKey(typeof(Formule))]
        public int FormuleID { get; set; }

        [ForeignKey(typeof(Musician))]
        public int MusicianID { get; set; }
    }
}
