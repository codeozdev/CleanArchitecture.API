namespace App.Application.Contracts.Caching;

public static class CacheDurations
{
    public static TimeSpan Short => TimeSpan.FromMinutes(5);
    public static TimeSpan Medium => TimeSpan.FromMinutes(10);
    public static TimeSpan Long => TimeSpan.FromHours(1);
}