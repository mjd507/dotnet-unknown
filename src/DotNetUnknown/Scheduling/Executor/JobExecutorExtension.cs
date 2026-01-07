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
        var jobDescriptors = services.Where(sd => typeof(IJob).IsAssignableFrom(sd.ServiceType)).ToList();
        foreach (var descriptor in jobDescriptors)
        {
            var jobImplementationType = GetImplementationType(descriptor);

            var jobExecutorType = typeof(JobExecutor<>).MakeGenericType(jobImplementationType);

            services.TryAddSingleton(jobExecutorType);

            services.TryAddKeyedSingleton<JobExecutor>(jobImplementationType.Name,
                (provider, _) => (JobExecutor)provider.GetRequiredService(jobExecutorType));

            services.AddSingleton<IHostedService>(provider =>
                (JobExecutor)provider.GetRequiredService(jobExecutorType));
        }
    }

    private static Type GetImplementationType(ServiceDescriptor descriptor)
    {
        return (descriptor.ImplementationType ??
                descriptor.ImplementationInstance?.GetType() ??
                descriptor.ImplementationFactory?.GetType()
                    .GetMethod("Invoke")?
                    .ReturnType) ?? throw new InvalidOperationException();
    }
}