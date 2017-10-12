using System;

namespace TaskTracker.Models
{
    public class ItemViewModel
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool Completed { get; set; }

        public DateTime CompletedDate { get; set; }

        public string User { get; set; }
    }
}