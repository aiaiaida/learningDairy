using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace LearningDiary_Aada_V1
{
    class Program
    {
        static void Main(string[] args)
        {
            // a txt file to store all the title of topics
            string path = @"C:\Users\Aada\source\repos\AWACADEMY\KoulutusWeek1\LearningDiaryAadaV1\topiclist.csv";
            // a list to store all topic objects
            List<Topic> topicList = new List<Topic>();
            // a bool value to display the MainMenu until user enters 4
            bool displayMenu = true;
            // main loop
            while (displayMenu)
            {
                displayMenu = MainMenu(path, ref topicList);
            }
        }

        private static bool MainMenu(string path, ref List<Topic> topicList)
        {
            // content of the MainMenu
            Console.Clear();
            Console.WriteLine("Welcome to the learning diary application");
            Console.WriteLine("Please choose an option: ");
            Console.WriteLine("1) Add a new topic");
            Console.WriteLine("2) Delete all topics");
            Console.WriteLine("3) Show the list of topics");
            Console.WriteLine("4) Exit");

            // change input string to int
            int userChoice = int.Parse(Console.ReadLine());

            // different code blocks for different choices
            switch(userChoice)
            {
                case 1:
                    // ask for title of the topic
                    // call the method AddNewTopic to write it into txt file
                    Console.WriteLine("Please set a title for the topic");
                    string title = Console.ReadLine();
                    AddNewTopic(title, path, ref topicList);
                    return true;

                case 2:
                    //delete the file by writing over it with a empty string
                    File.WriteAllText(path,"");
                    Console.WriteLine("Deletion completed");
                    Console.WriteLine("Press any key to return to the main menu");
                    Console.ReadKey();
                    return true;

                case 3:
                    // read and print all lines
                    string[] readText = File.ReadAllLines(path);
                    int i;
                    for (i = 0; i < readText.Length; i++)
                    {
                        Console.WriteLine(readText[i]);
                    }
                    Console.WriteLine("Press any key to return to the main menu");
                    Console.ReadKey();
                    return true;

                case 4:
                    // user stops the while loop
                    return false;

                default:
                    return true;
            }
        }

        // method to write new topic into txt file
        private static void AddNewTopic(string title, string path, ref List<Topic> topicList)
        {
            Topic newTopic = new Topic();
            // read the id from the file
            if (File.Exists(path))
            { 
                string[] readText = File.ReadAllLines(path);
                newTopic.id = readText.Length + 1;
            }
            else
            {
                newTopic.id = 1;
            }

            newTopic.title = title;
            Console.WriteLine($"The {title} topic is created!");
            newTopic.startLearningDate = DateTime.Now;

            bool displayTopicMenu = true;
            while (displayTopicMenu)
            {
                displayTopicMenu = TopicMenu(ref newTopic);
            }
            
            topicList.Add(newTopic);
            string taskInfo = string.Join(",", newTopic.taskList);
            string info = $"{newTopic.id},{newTopic.title},{newTopic.description},{newTopic.estimatedTimeToMaster},{newTopic.timeSpent},{newTopic.source},{newTopic.startLearningDate.ToShortDateString()},{newTopic.inProgress},{newTopic.completionDate.ToShortDateString()},{taskInfo}";
            // add also tasks and their values

            File.AppendAllText(path, info + Environment.NewLine);
        }

        private static bool TopicMenu(ref Topic newTopic)
        {
            Console.WriteLine("");
            Console.WriteLine("Please choose an option: ");
            Console.WriteLine("1) Add Description");
            Console.WriteLine("2) Add Estimated time to master in hour unit");
            Console.WriteLine("3) Add Source");
            Console.WriteLine("4) Add Completion date (dd-mm-yyyy)");
            Console.WriteLine("5) Add tasks to this topic");
            Console.WriteLine("6) Show information of this topic");
            Console.WriteLine("7) Return to the main menu and save the topic");

            int topicChoice = int.Parse(Console.ReadLine());
            
            switch (topicChoice)
            {
                case 1:
                    Console.WriteLine("Please enter a short description");
                    string userDescription = Console.ReadLine();
                    newTopic.description = userDescription;
                    Console.WriteLine($"Description saved. {newTopic.description}");
                    return true;
                case 2:
                    Console.WriteLine("Please enter the estimated time to master in hour unit");
                    double userEstimated = Convert.ToDouble(Console.ReadLine());
                    newTopic.estimatedTimeToMaster = userEstimated;
                    Console.WriteLine($"Estimated time saved. {newTopic.estimatedTimeToMaster}");
                    return true;
                case 3:
                    Console.WriteLine("Please enter a source web url or a book");
                    string userSource = Console.ReadLine();
                    newTopic.source = userSource;
                    Console.WriteLine($"Source saved. {newTopic.source}");
                    return true;
                case 4:
                    Console.WriteLine("Please enter the completion date (dd-mm-yyyy)");
                    DateTime userCompletionDate = DateTime.ParseExact(Console.ReadLine(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    newTopic.completionDate = userCompletionDate;
                    newTopic.inProgress = false;
                    newTopic.timeSpent = (newTopic.completionDate - newTopic.startLearningDate).TotalMinutes;
                    Console.WriteLine($"Completion date saved. {newTopic.completionDate.ToString("dd-MM-yyyy")}");
                    return true;
                case 5:
                    // add a task and trigger a task menu
                    AddNewTask(newTopic);
                    return true;
                case 6:
                    Topic.TopicToString(newTopic);
                    return true;
                    
                case 7:
                    Console.WriteLine("Returning to the main menu ...");
                    return false;
                default:
                    return true;
            }
        }
        private static void AddNewTask(Topic topic)
        {
            Task newTask = new Task();
            Console.WriteLine("Please name the task");
            string taskTitle = Console.ReadLine();
            newTask.title= taskTitle;
            int id = topic.taskList.Count + 1;
            newTask.id = id;
            Console.WriteLine($"The task named {taskTitle} is created!");
            //bool value to loop the task menu
            bool displayTaskMenu = true;
            while (displayTaskMenu)
            {
                displayTaskMenu = TaskMenu(newTask);
            }
            
            
            topic.taskList.Add(newTask);
        }

        private static bool TaskMenu(Task newTask)
        {
            Console.WriteLine("");
            Console.WriteLine("Please choose an option: ");
            Console.WriteLine("1) Add Description");
            Console.WriteLine("2) Add Notes");
            Console.WriteLine("3) Add Deadline");
            Console.WriteLine("4) Mark Priority");
            Console.WriteLine("5) Mark Done");
            Console.WriteLine("6) Show all information of this task");
            Console.WriteLine("7) Return to Topic Menu");
            
            int userTaskChoice = int.Parse(Console.ReadLine());

            switch (userTaskChoice)
            {
                case 1:
                    {
                        Console.WriteLine("Add Description");
                        string description = Console.ReadLine();
                        newTask.description = description;
                        Console.WriteLine($"Description saved! {newTask.description}");
                        return true;
                    }
                case 2:
                    {
                        Console.WriteLine("Add Notes");
                        string note = Console.ReadLine();
                        newTask.notes.Add(note);
                        Console.WriteLine($"Note saved! {string.Join(",",newTask.notes)}");
                        return true;
                    }
                case 3:
                    {
                        Console.WriteLine("Add Deadline dd-MM-yyyy");
                        DateTime deadline = DateTime.ParseExact(Console.ReadLine(), "dd-MM-yyyy", CultureInfo.InvariantCulture); ;
                        newTask.deadline = deadline;
                        Console.WriteLine($"Deadline saved! {newTask.deadline}");
                        return true;
                    }
                case 4:
                    {
                        Console.WriteLine("Mark priority, choose one from the following: Urgent, Important, Unurgent, Unimportant");
                        string priorityInput = Console.ReadLine();
                        newTask.SetPriority(priorityInput);
                        Console.WriteLine($"Priority set! {newTask.GetPriority()}");
                        return true;
                    }
                case 5:
                    {
                        newTask.done = true;
                        Console.WriteLine("Marked done");
                        return true;
                    }
                case 6:
                    {
                        Task.TaskToString(newTask);
                        return true;
                    }
                case 7:
                    {
                        Console.WriteLine("Returning to Topic Menu...");
                        return false;
                    }
                default:
                    {
                        return true;
                    }
            }
        }

    }

    public class Topic
    {
        // intiate fields
        public int id;
        public string title;
        public string description;
        public double estimatedTimeToMaster;
        public double timeSpent;
        public string source;
        public DateTime startLearningDate;
        public bool inProgress = true;
        public DateTime completionDate;
        public List<Task> taskList = new List<Task>();

        public Topic(string title="", string description="", double estimatedTimeToMaster=0, string source="", DateTime completionDate= default(DateTime))
        {
            this.title = title;
            this.description = description;
            this.estimatedTimeToMaster = estimatedTimeToMaster;
            this.source = source;
            this.completionDate = completionDate;
        }

        public static void TopicToString(Topic topic)

        {
            Console.WriteLine($"Topic id: {topic.id}");
            Console.WriteLine($"Topic title: {topic.title}");
            Console.WriteLine($"Topic description: {topic.description}");
            Console.WriteLine($"Topic estimated time to master: {topic.estimatedTimeToMaster}");
            Console.WriteLine("Topic time spent: {0:0.00} minutes", topic.timeSpent);
            Console.WriteLine($"Topic source: {topic.source}");
            Console.WriteLine($"Topic starting date: {topic.startLearningDate.ToShortDateString()}");
            Console.WriteLine($"Topic in process: {topic.inProgress}");
            Console.WriteLine($"Topic completion date: {topic.completionDate.ToShortDateString()}");
        }

    }

    public class Task
    {
        // initial fields
        public int id;
        public string title;
        public string description;
        public List<string> notes = new List<string>();
        public DateTime deadline;
        public enum Priority 
        {Urgent, Important, Unurgent, Unimportant};
        public Priority thePriority;
        public void SetPriority(string p)
        {
           Enum.TryParse(p, out thePriority);
        }

        public  Priority GetPriority()
        {
            return thePriority;
        }

        public bool done = false;

        public static void TaskToString(Task newTask)
        {
            string notesFromList = string.Join("\n", newTask.notes);
            Console.WriteLine($"Task id: {newTask.id}\nTask title: {newTask.title}\nTask description: {newTask.description}\nTask notes: {notesFromList}\nTask Deadline: {newTask.deadline}\nTask priority: {newTask.GetPriority()}\nTask done: {newTask.done}");
        }

        public override string ToString()
        {
            string notesFromList = string.Join("\n", notes);
            return $"Task{id},{title},{description},{notesFromList},{deadline.ToShortDateString()},{GetPriority()},{done}";
        }

    }

    
    
}
