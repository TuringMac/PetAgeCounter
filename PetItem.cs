using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeCounter
{
    public class PetItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime BirthDate { get; set; } = DateTime.MinValue;
        public DateTime DeathDate { get; set; } = DateTime.MaxValue;
        [Ignore]
        public DateDifference Offset => new DateDifference(BirthDate, DateTime.Today);
        [Ignore]
        public string FormattedOffset => $"{Offset.Years}y {Offset.Months}m {Offset.Days}d";
    }
}
