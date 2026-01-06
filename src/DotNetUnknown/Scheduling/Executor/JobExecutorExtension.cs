using Microsoft.Extensions.DependencyInjection.Extensions;
using CrontabOptionsDict =
    System.Collections.Generic.Dictionary<string, DotNetUnknown.Scheduling.Executor.CrontabOptions>;

namespace DotNetUnknown.Scheduling.Executor;

public static class JobExecutorExtension
{
    private static readonly Action<object?> CompleteAction =
        state => ((TaskCompletionSource<object?>?)state)?.TrySetResult(null);

    public static CancellationTokenRegistration AsCompletedTask(this CancellationToken cancellationToken, out Task task,
        bool useSynchronizationContext = false)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            task = Task.CompletedTask;

            return default;
        }

        var tcs = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);

        task = tcs.Task;

        return cancellationToken.Register(CompleteAction, tcs, useSynchronizationContext);
    }


    public static void AddJobExecutors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CrontabOptionsDict>().Bind(configuration.GetSection("Crontab"));

        // foreach (var type in services
        //              .Where(x => typeof(IJob).IsAssignableFrom(x.ServiceType))
        //              // .Select(x => x.GetImplementationType())
        //              .Select(x => x.GetType())
        //              .ToHashSet())
        var serviceProvider = services.BuildServiceProvider();
        var jobs = serviceProvider.GetServices<IJob>();
        foreach (var job in jobs)
        {
            var type = job.GetType();
            var jobType = typeof(JobExecutor<>).MakeGenericType(type);

            services.TryAddSingleton(jobType);

            services.TryAddKeyedSingleton<JobExecutor>(type.Name,
                (provider, _) => (JobExecutor)provider.GetRequiredService(jobType));

            services.AddSingleton<IHostedService>(provider =>
                (JobExecutor)provider.GetRequiredService(jobType));
        }
    }
}