using System;
using System.Collections.Generic;

#nullable disable

namespace LearningDiary_Aada_V1.Models
{
    public partial class Topic
    {
        public Topic()
        {
            Notes = new HashSet<Note>();
            TaskInTopics = new HashSet<TaskInTopic>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? EstimatedTimeToMaster { get; set; }
        public int? TimeSpent { get; set; }
        public string Source { get; set; }
        public DateTime? StartLearningDate { get; set; }
        public bool? InProgress { get; set; }
        public DateTime? CompletionDate { get; set; }

        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<TaskInTopic> TaskInTopics { get; set; }
    }
}
