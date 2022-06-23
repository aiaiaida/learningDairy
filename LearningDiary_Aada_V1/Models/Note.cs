using System;
using System.Collections.Generic;

#nullable disable

namespace LearningDiary_Aada_V1.Models
{
    public partial class Note
    {
        public int Id { get; set; }
        public int TopicId { get; set; }
        public int TaskId { get; set; }
        public string Note1 { get; set; }

        public virtual Task Task { get; set; }
        public virtual Topic Topic { get; set; }
    }
}
