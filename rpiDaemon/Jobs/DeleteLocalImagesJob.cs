using System;
using System.IO;
using System.Threading.Tasks;
using Quartz;
using rpiDaemon.DateTimeHelpers;

namespace rpiDaemon.Jobs
{
    public class DeleteLocalImagesJob : IJob
    {
        private const int NumberOfDaysCutOff = 30;

        public Task Execute(IJobExecutionContext context)
        {
            var cutOffDate = DateTime.UtcNow.AddDays(-NumberOfDaysCutOff);

            var folderNames = Directory.GetDirectories("/home/pi/images/");

            foreach (var fullPath in folderNames)
            {
                var folderName = Path.GetDirectoryName(fullPath);
                var folderDate = DateTimeParser.GetDateTimeDateFromFolderName(folderName);

                if (folderDate > cutOffDate)
                    continue;

                var dir = new DirectoryInfo(fullPath);
                dir.Delete();
            }

            return Task.CompletedTask;
        }
    }
}