namespace DotNetUnknown.Scheduling;

public static class JobServiceExtension
{
    public static void AddJobServices(this IServiceCollection services)
    {
        services.AddSingleton<Job1>();
    }
}