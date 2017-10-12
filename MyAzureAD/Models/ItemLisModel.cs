using System.Collections.Generic;

namespace TaskTracker.Models
{
    public class ItemLisModel
    {       
        public string EnvironmentName { get; set; }
        public string CountInfo { get; set; }
        public IEnumerable<Item> List { get; set; }
    }
}