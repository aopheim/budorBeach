// using System;
// using System.IO;
// using System.Threading.Tasks;
// using Quartz;
// using rpiDaemon.DateTimeHelpers;
// using Shared;
//
// namespace rpiDaemon.Jobs
// {
//     public class DeleteLocalImagesJob : BaseJob
//     {
//         private const int NumberOfDaysCutOff = 30;
//
//         public override string JobName => nameof(DeleteLocalImagesJob);
//         public override string GroupName => GlobalConstants.DailyJobs;
//         public override string TriggerName => "DeleteLocalImagesTrigger";
//         public override TimeSpan TriggerInterval => TimeSpan.FromDays(1);
//
//         public override bool IsActive => false;
//
//         public override Task Execute(IJobExecutionContext context)
//         {
//             var cutOffDate = DateTime.UtcNow.AddDays(-NumberOfDaysCutOff);
//
//             var folderNames = Directory.GetDirectories("/home/pi/images/");
//
//             foreach (var fullPath in folderNames)
//             {
//                 var folderName = Path.GetDirectoryName(fullPath);
//                 var folderDate = DateTimeParser.GetDateTimeDateFromFolderName(folderName);
//
//                 if (folderDate > cutOffDate)
//                     continue;
//
//                 var dir = new DirectoryInfo(fullPath);
//                 dir.Delete();
//             }
//
//             return Task.CompletedTask;
//         }
//     }
// }