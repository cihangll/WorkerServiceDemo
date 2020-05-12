using System;

namespace WorkerServiceDemo.Abstract
{
	public interface IScheduleConfig<T>
	{
		string CronExpression { get; set; }
		TimeZoneInfo TimeZoneInfo { get; set; }
	}
}
