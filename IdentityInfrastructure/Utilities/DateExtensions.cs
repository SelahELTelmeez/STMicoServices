namespace IdentityInfrastructure.Utilities
{
    public static class DateExtensions
    {
        public static DateTime EndOfDay(this DateTime theDate)
             => theDate.Date.AddDays(1).AddTicks(-1);

        public static DateTime StartOfDay(this DateTime theDate)
            => theDate.Date.Date;
    }
}
