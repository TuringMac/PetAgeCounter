using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeCounter
{
    public class DateDifference
    {
        public int Years { get; private set; }
        public int Months { get; private set; }
        public int Days { get; private set; }
        public TimeSpan diff { get; private set; }

        public DateDifference(DateTime startDate, DateTime endDate)
        {
            // Ensure startDate is earlier than endDate
            if (startDate > endDate)
            {
                DateTime temp = startDate;
                startDate = endDate;
                endDate = temp;
            }

            diff = endDate - startDate;

            Years = endDate.Year - startDate.Year;
            Months = endDate.Month - startDate.Month;
            Days = endDate.Day - startDate.Day;

            // Adjust for negative days or months
            if (Days < 0)
            {
                Months--;
                Days += DateTime.DaysInMonth(startDate.Year, startDate.Month); // Add days of the start month
            }

            if (Months < 0)
            {
                Years--;
                Months += 12;
            }
        }
    }
}
