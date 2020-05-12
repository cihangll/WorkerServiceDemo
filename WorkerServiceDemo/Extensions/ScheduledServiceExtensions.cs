using Microsoft.Extensions.DependencyInjection;
using System;
using WorkerServiceDemo.Abstract;
using WorkerServiceDemo.Concrete;

namespace WorkerServiceDemo.Extensions
{
	public static class ScheduledServiceExtensions
	{
		public static IServiceCollection AddCronJob<T>(this IServiceCollection services, Action<IScheduleConfig<T>> options) where T : CronJobService
		{
			if (options is null)
			{
				throw new ArgumentNullException(nameof(options), @"Please provide Schedule Configurations.");
			}

			var config = new ScheduleConfig<T>();
			options.Invoke(config);
			if (string.IsNullOrWhiteSpace(config.CronExpression))
			{
				throw new ArgumentNullException(nameof(ScheduleConfig<T>.CronExpression), @"Empty Cron Expression is not allowed.");
			}

			services.AddSingleton<IScheduleConfig<T>>(config);
			services.AddHostedService<T>();
			return services;
		}
	}
}
