using System;
using System.Collections.Generic;

#nullable disable

namespace LearningDiary_Aada_V1.Models
{
    public partial class TaskInTopic
    {
        public TaskInTopic()
        {
            Notes = new HashSet<Note>();
        }

        public int Id { get; set; }
        public int TopicId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }
        public string Priority { get; set; }
        public bool? Done { get; set; }

        public virtual Topic Topic { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
    }
}
