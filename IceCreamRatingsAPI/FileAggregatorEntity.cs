using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ice_Cream_Ratings_API
{
    internal class FileAggregatorEntity
    {
        private readonly ILogger logger;

        public FileAggregatorEntity(ILogger logger)
        {
            this.logger = logger;
            FileUrls = new Dictionary<FileType, string>();
            Status = EntityStatus.Open;
        }

        public Dictionary<FileType, string> FileUrls { get; set; }
        public EntityStatus Status { get; set; }

        public void FileReceived(string fileUrl)
        {
            if (Status == EntityStatus.Close)
            {
                return;
            }

            var fileType = GetFileTypeFromUrl(fileUrl);
            this.FileUrls[fileType] = fileUrl;

            if (AllFileReceived())
            {
                // here put the external service request
                this.Status = EntityStatus.Close;
            }
        }

        private bool AllFileReceived()
        {
            bool result = true;

            result &= this.FileUrls.ContainsKey(FileType.OrderHeaderDetails) && !string.IsNullOrWhiteSpace(this.FileUrls[FileType.OrderHeaderDetails]);
            result &= this.FileUrls.ContainsKey(FileType.OrderLineItems) && !string.IsNullOrWhiteSpace(this.FileUrls[FileType.OrderLineItems]);
            result &= this.FileUrls.ContainsKey(FileType.ProductInformation) && !string.IsNullOrWhiteSpace(this.FileUrls[FileType.ProductInformation]);

            return result;
        }

        private FileType GetFileTypeFromUrl(string fileUrl)
        {
            var fileName = fileUrl.Split(@"/").Last();
            if (fileName.ToLower().Contains(nameof(FileType.OrderHeaderDetails).ToLower()))
            {
                return FileType.OrderHeaderDetails;
            }
            else if (fileName.ToLower().Contains(nameof(FileType.OrderLineItems).ToLower()))
            {
                return FileType.OrderLineItems;
            }
            else if (fileName.ToLower().Contains(nameof(FileType.ProductInformation).ToLower()))
            {
                return FileType.ProductInformation;
            }
            return FileType.Unknown;
        }

        [FunctionName(nameof(FileAggregatorEntity))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx, ILogger logger)
           => ctx.DispatchAsync<FileAggregatorEntity>(logger);

        public enum FileType
        {
            Unknown,
            ProductInformation,
            OrderHeaderDetails,
            OrderLineItems
        }

        public enum EntityStatus
        {
            Open,
            Close
        }
    }
}
