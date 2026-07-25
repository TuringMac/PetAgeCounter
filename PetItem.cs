using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace PetAgeCounter
{
    public class PetItem : ObservableObject
    {
        bool isDeceased;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime BirthDate { get; set; } = DateTime.MinValue;
        public DateTime DeathDate { get; set; } = DateTime.MaxValue;

        public bool IsDeceased
        {
            get => isDeceased;
            set => SetProperty(ref isDeceased, value);
        }

        [Ignore]
        public bool IsAlive => !IsDeceased;

        [Ignore]
        public DateDifference Offset => IsDeceased
            ? new DateDifference(BirthDate, DeathDate)
            : new DateDifference(BirthDate, DateTime.Today);

        [Ignore]
        public string FormattedOffset => $"{Offset.Years}г {Offset.Months}м {Offset.Days}д";
    }
}
