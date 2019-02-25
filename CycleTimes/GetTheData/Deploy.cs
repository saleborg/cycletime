using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace ThroughputCalculation.GetTheData
{
    class Deploy
    {

        public string releaseEnvironment { get; set; }
 
        public string id { get; set; }
        public string name { get; set; }
        public DateTime createdOn { get; set; }
        public string operationStatus { get; set; }
        public string releaseId { get; internal set; }
        public string releaseName { get; internal set; }
        public string buildId { get; internal set; }
        public string deploymentStatus { get; internal set; }
        public DateTime completedOn { get; internal set; }

        public IList<Item> items = new List<Item>();

      
    }
}