using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProactiveBot.Models
{
    public class NotifyModel
    {
        public long GlobalId { get; set; }
        public string StateName { get; set; }
        public string Name { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Description { get; set; }
        public string Build { get; set; }
        public Guid ProjectId { get; set; }
        public string ProductName { get; set; }
        public string Duration { get; set; }
        public List<JObject> Tags { get; set; }
        public string Link { get; set; }
        public bool IsDeleted { get; set; }
    }
}
