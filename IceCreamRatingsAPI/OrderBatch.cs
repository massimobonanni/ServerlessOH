using System;
using System.Collections.Generic;
using System.Text;

namespace BFYOC.Functions
{
    public class OrderBatch
    {
        public string Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string OrderHeaderDetailsCSVUrl { get; set; }
        public string OrderLineItemsCSVUrl { get; set; }
        public string ProductInformationCSVUrl { get; set; }
    }
}
