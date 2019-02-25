using System;
using System.Collections.Generic;
using System.Text;

namespace ThroughputCalculation.GetTheData
{
    class Item
    {
        

        public string SystemState { get; internal set; }
        public string WorkItemType { get; internal set; }
        public DateTime CreatedDate { get; internal set; }
       
        public string Id { get; internal set; }
        public DateTime commitedDate { get; internal set; }
        public string severity { get; internal set; }
    }
}
