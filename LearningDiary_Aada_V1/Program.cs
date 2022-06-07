using System;
using System.Collections.Generic;

namespace LearningDiary_Aada_V1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Topic> AuthorList = new List<Topic>();
            bool newTopic = true;
            while (newTopic)
            {

            }


        }

        public class Topic
        {
            // initiate all the variables
            public int id;
            public string title;
            public string description;
            public double estimatedTimeToMaster;
            public double timeSpent;
            public string source;
            public DateTime startLearningDate;
            public bool inProgress;
            public DateTime completionDate;

            //constructor to set the values
            /*public Topic(string title, string description, double estimatedTimeToMaster,
                double timeSpent, string source, DateTime startLearningDate, bool inProgress, DateTime completionDate)
            {
                this.title = title;
                this.description = description;
                this.estimatedTimeToMaster = estimatedTimeToMaster;
                this.timeSpent = timeSpent;
                this.source = source;
                this.startLearningDate = startLearningDate;
                this.inProgress = inProgress;
                this.completionDate = completionDate;
            }
            */

            //properties
            public string GetTitle()
            {
                return title;
            }
            public string GetDescription()
            {
                return description;
            }
            public double GetEstimatedTime()
            {
                return estimatedTimeToMaster;
            }
            public double GetTimeSpent()
            {
                return timeSpent;
            }
            public string GetSource()
            {
                return source;
            }
            public DateTime GetStartingDate()
            {
                return startLearningDate;
            }
            public bool GetInProgress()
            {
                return inProgress;
            }
            public DateTime GetCompletionDate()
            {
                return completionDate;
            }

        }
    }
}
