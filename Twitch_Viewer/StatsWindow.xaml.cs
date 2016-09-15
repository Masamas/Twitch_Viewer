using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Twitch_Viewer.Types;

namespace Twitch_Viewer
{
    /// <summary>
    /// Interaction logic for StatsTest.xaml
    /// </summary>
    public partial class StatsWindow : Window
    {
        public StatsWindow(StatsItem stats)
        {
            //Random rnd = new Random();

            //for (int i = 0; i < 50; i++)
            //    stats.ViewTimeData.Add(new ViewTimeData(new DateTime(2016, 8, rnd.Next(1, 32), rnd.Next(24), rnd.Next(60), rnd.Next(60)), new TimeSpan(rnd.Next(1), rnd.Next(1, 60), rnd.Next(60))));

            stats.ViewTimeData.Sort(item => item.Start);

            InitializeComponent();

            this.DataContext = stats;

            DoWeekdayAnimations(stats);
        }

        private void DoWeekdayAnimations(StatsItem stats)
        {
            ExponentialEase ease = new ExponentialEase();
            ease.Exponent = 10;
            ease.EasingMode = EasingMode.EaseOut;

            DoubleAnimation da = new DoubleAnimation();
            da.From = 0;
            da.Duration = new Duration(TimeSpan.FromSeconds(3));
            da.EasingFunction = ease;
            da.FillBehavior = FillBehavior.Stop;

            var days = Enum.GetValues(typeof(DayOfWeek));
            foreach (DayOfWeek day in days)
            {
                da.To = stats.calcDayStats(day) * 200;
                DoWeekdayAnimation(da, day);
            }
        }

        private void DoWeekdayAnimation(DoubleAnimation da, DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday:
                    MondayBar.BeginAnimation(Rectangle.HeightProperty, da);
                    break;
                case DayOfWeek.Tuesday:
                    TuesdayBar.BeginAnimation(Rectangle.HeightProperty, da);
                    break;
                case DayOfWeek.Wednesday:
                    WednesdayBar.BeginAnimation(Rectangle.HeightProperty, da);
                    break;
                case DayOfWeek.Thursday:
                    ThursdayBar.BeginAnimation(Rectangle.HeightProperty, da);
                    break;
                case DayOfWeek.Friday:
                    FridayBar.BeginAnimation(Rectangle.HeightProperty, da);
                    break;
                case DayOfWeek.Saturday:
                    SaturdayBar.BeginAnimation(Rectangle.HeightProperty, da);
                    break;
                case DayOfWeek.Sunday:
                    SundayBar.BeginAnimation(Rectangle.HeightProperty, da);
                    break;
            }
        }
    }
}
