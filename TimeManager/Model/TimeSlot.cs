using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TimeManager.Model
{
    public class TimeSlot
    {
        public required string ProcessName { get; set; } 
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string ColorHex{ get; set; } = "#FFFFFF";
        public TimeSpan Duration
        {
            get
            {
                if(EndTime >= StartTime)
                    return EndTime - StartTime;

                return (TimeSpan.FromHours(24) - StartTime.ToTimeSpan()) + EndTime.ToTimeSpan();
            }
        }
    }
}
