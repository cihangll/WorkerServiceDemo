using System;
using WorkerServiceDemo.Abstract;

namespace WorkerServiceDemo.Concrete
{
	public class ScheduleConfig<T> : IScheduleConfig<T>
	{
		public string CronExpression { get; set; }
		public TimeZoneInfo TimeZoneInfo { get; set; }
	}
}
