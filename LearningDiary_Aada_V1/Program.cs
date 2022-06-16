using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace LearningDiary_Aada_V1
{
    class Program
    {
        static void Main(string[] args)
        {
            // a csv file to store all the title of topics
            string path = @"C:\Users\Aada\source\repos\AWACADEMY\LearningDiaryAadaV1\topiclist.csv";

            // a bool value to display the MainMenu 
            bool displayMenu = true;

            
            // main loop
            while (displayMenu)
            {
                displayMenu = MainMenu(path);
            }
        }

        private static bool MainMenu(string path) 
        {
            // content of the MainMenu
            Console.Clear();
            Console.WriteLine("Welcome to the learning diary application");
            Console.WriteLine("Please choose an option: ");
            Console.WriteLine("1) Add a new topic");
            Console.WriteLine("2) Look up a topic");
            Console.WriteLine("3) Edit a topic");
            Console.WriteLine("4) Delete one topic");
            Console.WriteLine("5) Delete all topics");
            Console.WriteLine("6) Show the list of topics");
            Console.WriteLine("0) Exit");

            // change input string to int
            int userChoice = int.Parse(Console.ReadLine());
            //dict of csv data

            if (!File.Exists(path))
            {
                File.WriteAllText(path, "");
            }
            
            string[] readText = File.ReadAllLines(path);
            Dictionary<int, Topic> dict = ReadAndConvert(path);
            

            // different code blocks for different choices
            switch (userChoice)
            {
                case 1:
                    // ask for title of the topic
                    // call the method AddNewTopic to write it into txt file
                    AddNewTopic(path, dict, ref readText); 
                    return true;

                case 2:
                    {
                        // look up a topic based on id
                        Console.WriteLine("Please enter the id of the topic");
                        int id = int.Parse(Console.ReadLine());

                        if (dict.ContainsKey(id))
                        {
                            Topic topic = dict[id];
                            Topic.TopicToString(topic);
                        }
                        else
                        {
                            Console.WriteLine("Not found!");
                        }
                        Console.WriteLine("Press any key to return to the main menu");
                        Console.ReadKey();
                        return true;
                    }

                case 3:
                    {
                        // to edit a topic
                        Console.WriteLine("Please enter the id of the topic you want to edit");
                        int id = int.Parse(Console.ReadLine());
                        Topic topicToEdit = dict[id];
                        bool displayEdit = true;
                        while (displayEdit)
                        {
                            displayEdit = TopicMenu(ref topicToEdit, ref readText, ref dict);
                        }
                        string taskInfo = string.Join(",", topicToEdit.TaskList);

                        //original solution
                        string info = $"{topicToEdit.Id},{topicToEdit.Title},{topicToEdit.Description},{topicToEdit.EstimatedTimeToMaster},{Math.Round(topicToEdit.TimeSpent, 2).ToString(CultureInfo.InvariantCulture.NumberFormat)},{topicToEdit.Source},{topicToEdit.StartLearningDate.ToShortDateString()},{topicToEdit.InProgress},{topicToEdit.CompletionDate.ToShortDateString()},{taskInfo}";

                        readText[id - 1] = info;
                        File.WriteAllText(path, "");
                        File.AppendAllLines(path, readText);
                        Console.WriteLine("Press any key to return to the main menu");
                        Console.ReadKey();
                        return true;
                    }
                case 4:
                    {
                        //delete one topic
                        Console.WriteLine("Please enter the id of the topic: ");
                        int id = int.Parse(Console.ReadLine());

                        DeleteAndUpdateId(readText, id, path);

                        Console.WriteLine("Deletion completed!");
                        Console.WriteLine("Press any key to return to the main menu");
                        Console.ReadKey();
                        return true;
                    }

                case 5:
                    {
                        //delete all topics by writing over it with a empty string
                        File.WriteAllText(path, "");
                        Console.WriteLine("Deletion completed");
                        Console.WriteLine("Press any key to return to the main menu");
                        Console.ReadKey();
                        return true;
                       
                    }

                case 6:
                    {
                        // print all topics in topic format
                        foreach (KeyValuePair<int,Topic> pair in dict)
                        {
                            Topic tempTopic = pair.Value;
                            Topic.TopicToString(tempTopic);
                        }
                        Console.WriteLine("Press any key to return to the main menu");
                        Console.ReadKey();
                        return true;
                    }

                case 0:
                    // user stops the while loop
                    return false;

                default:
                    return true;
            }
        }

        // method to write new topic into csv file
        private static void AddNewTopic(string path, Dictionary<int,Topic> dict, ref string[] readText) 
        {
            Topic newTopic = new Topic();
            // read the id from the file
            if (File.Exists(path) && new FileInfo(path).Length != 0)
            {
                // get the last key and increment by 1 => new id
                newTopic.Id = dict.Keys.Last() + 1;
            }
            else
            {
                newTopic.Id = 1;
            }

            Console.WriteLine($"The topic is created!");
            
            // set starting date immediately when the topic is created
            newTopic.StartLearningDate = DateTime.Now;

            //a topic menu loop
            bool displayTopicMenu = true;
            while (displayTopicMenu)
            {
                displayTopicMenu = TopicMenu(ref newTopic,ref readText, ref dict);
            }
            
            string taskInfo = string.Join(",", newTopic.TaskList);
            
            //original solution
            string info = $"{newTopic.Id},{newTopic.Title},{newTopic.Description},{newTopic.EstimatedTimeToMaster},{Math.Round(newTopic.TimeSpent, 2).ToString(CultureInfo.InvariantCulture.NumberFormat)},{newTopic.Source},{newTopic.StartLearningDate.ToShortDateString()},{newTopic.InProgress},{newTopic.CompletionDate.ToShortDateString()},{taskInfo}";
            
            // call the method to save into file
            AppendIntoFile(path, info);
        }

        private static bool TopicMenu(ref Topic newTopic, ref string[] readText, ref Dictionary<int,Topic> dict)
        {
            Console.WriteLine("");
            Console.WriteLine("Please choose an option: ");
            Console.WriteLine("1) Title");
            Console.WriteLine("2) Description");
            Console.WriteLine("3) Estimated time to master in minute unit");
            Console.WriteLine("4) Source");
            Console.WriteLine("5) Completion date (dd-mm-yyyy)");
            Console.WriteLine("6) Manage tasks");
            Console.WriteLine("7) Show information of this topic");
            Console.WriteLine("0) Return to the main menu and save the topic");

            int topicChoice = int.Parse(Console.ReadLine());
            
            switch (topicChoice)
            {
                case 1:
                    {
                        // set title
                        Console.WriteLine("Please enter a title");
                        string title = Console.ReadLine();
                        newTopic.Title = title;
                        Console.WriteLine($"Title saved. {newTopic.Title}");
                        return true;
                    }
                case 2:
                    {
                        // set description
                        Console.WriteLine("Please enter a short description");
                        string userDescription = Console.ReadLine();
                        newTopic.Description = userDescription;
                        Console.WriteLine($"Description saved. {newTopic.Description}");
                        return true;
                    }

                case 3:
                    {
                        // set estimated time 
                        Console.WriteLine("Please enter the estimated time to master in minute unit");
                        double userEstimated = Convert.ToDouble(Console.ReadLine());
                        newTopic.EstimatedTimeToMaster = userEstimated;
                        Console.WriteLine($"Estimated time saved. {newTopic.EstimatedTimeToMaster}");
                        return true;
                    }
                    
                case 4:
                    {
                        // set source
                        Console.WriteLine("Please enter a source web url or a book");
                        string userSource = Console.ReadLine();
                        newTopic.Source = userSource;
                        Console.WriteLine($"Source saved. {newTopic.Source}");
                        return true;
                    }
                    
                case 5:
                    {
                        // set completion date
                        Console.WriteLine("Please enter the completion date (dd-mm-yyyy)");
                        DateTime userCompletionDate = DateTime.Parse(Console.ReadLine());
                        newTopic.CompletionDate = userCompletionDate;
                        
                        // set in progress automatically to false
                        // and calculate time spent on the topic
                        newTopic.InProgress = false;
                        newTopic.TimeSpent = (newTopic.CompletionDate - newTopic.StartLearningDate).TotalMinutes;

                        // congrats the user if timeSpent less than estimated master time
                        if (newTopic.EstimatedTimeToMaster >= newTopic.TimeSpent)
                        {
                            Console.WriteLine("Congratulations! You are faster than you thought!");
                        }
                        Console.WriteLine($"Completion date saved. {newTopic.CompletionDate.ToString("dd-MM-yyyy")}");
                        return true;
                    }
                    
                case 6:
                    {
                        // trigger a task menu
                        bool displayManageTasks = true;
                        while (displayManageTasks)
                        {
                            displayManageTasks = ManageTasks(newTopic, ref readText, ref dict);
                        }
                        
                        return true;
                    }
                    
                case 7:
                    {
                        // show information of the topic
                        Topic.TopicToString(newTopic);
                        return true;
                    }
                    
                    
                case 0:
                    {
                        return false;
                    }

                default:
                    {
                        return true;
                    }
                    
            }
        }

        private static bool ManageTasks(Topic newTopic, ref string[] readText, ref Dictionary<int, Topic> dict)
        {
            Console.WriteLine("");
            Console.WriteLine("Please choose an option: ");
            Console.WriteLine("1) Add a new task");
            Console.WriteLine("2) Look up a task");
            Console.WriteLine("3) Edit a task");
            Console.WriteLine("4) Delete a task");
            Console.WriteLine("5) Delete all tasks");
            Console.WriteLine("6) Show information of this topic");
            Console.WriteLine("0) Return to the topic menu");

            int taskChoice = int.Parse(Console.ReadLine());

            switch (taskChoice)
            {
                case 1:
                    {
                        // add new task and trigger a task info menu
                        AddNewTask(newTopic);
                        return true;
                    }
                case 2:
                    {
                        // look up a task
                        Console.WriteLine("Please enter the id of the task");
                        int id = int.Parse(Console.ReadLine());

                        if (id <= newTopic.TaskList.Count)
                        {
                            Console.WriteLine("Found!");
                            for(int i=0; i<newTopic.TaskList.Count; i++)
                            {
                                if (i+1 == id)
                                {
                                    Task.TaskToString(newTopic.TaskList[i]);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Not found!");
                        }
                        Console.WriteLine("Press any key to return");
                        Console.ReadKey();
                        return true;
                    }


                case 3:
                    {
                        //Edit a task
                        Console.WriteLine("Enter the id of the task you want to modify");
                        int id = int.Parse(Console.ReadLine());
                        Task newTask = newTopic.TaskList[id - 1];
                        bool displayTaskMenu = true;
                        while (displayTaskMenu)
                        {
                            displayTaskMenu = TaskMenu(newTask);
                        }
                        newTopic.TaskList[id - 1] = newTask;
                        return true;
                    }

                case 4:
                    {
                        //Delete one task
                        Console.WriteLine("Please enter the id of the task to be deleted");
                        int id = int.Parse(Console.ReadLine());
                        newTopic.TaskList.RemoveAt(id - 1);
                        //!!!!!!!!!!!!!!!    have to change the id after deletion  !!!!!!!!!!!!!!!!!!!!!
                        return true;
                    }

                case 5:
                    {
                        // Delete all tasks
                        newTopic.TaskList.Clear();
                        return true;
                    }

                case 6:
                    {
                        // Show info of this topic
                        Topic.TopicToString(newTopic);
                        return true;
                    }

                case 0:
                    {
                        //return to the topic menu
                        Console.WriteLine("Returning...");
                        return false;
                    }

                default:
                    return true;
            }
        }
        private static void AddNewTask(Topic topic)
        {
            Task newTask = new Task();

            // automatically calculate the id
            newTask.Id = topic.TaskList.Count + 1;
            Console.WriteLine($"The new task is created!");
           
            //bool value to loop the task menu
            bool displayTaskMenu = true;
            while (displayTaskMenu)
            {
                displayTaskMenu = TaskMenu(newTask);
            }  
            topic.TaskList.Add(newTask);
        }

        private static bool TaskMenu(Task newTask)
        {
            Console.WriteLine("");
            Console.WriteLine("Please choose an option: ");
            Console.WriteLine("1) Title");
            Console.WriteLine("2) Description");
            Console.WriteLine("3) Notes");
            Console.WriteLine("4) Deadline");
            Console.WriteLine("5) Mark Priority");
            Console.WriteLine("6) Mark Done");
            Console.WriteLine("7) Show all information of this task");
            Console.WriteLine("0) Return to Topic Menu");
            
            int userTaskChoice = int.Parse(Console.ReadLine());

            switch (userTaskChoice)
            {
                
                case 1:
                    {
                        // set description
                        Console.WriteLine("Title for the task");
                        string title = Console.ReadLine();
                        newTask.Title = title;
                        Console.WriteLine($"Title saved! {newTask.Title}");
                        return true;
                    }
                case 2:
                    {
                        // set description
                        Console.WriteLine("Add Description");
                        string description = Console.ReadLine();
                        newTask.Description = description;
                        Console.WriteLine($"Description saved! {newTask.Description}");
                        return true;
                    }
                case 3:
                    {
                        // add notes into list
                        Console.WriteLine("Add Notes");
                        string note = Console.ReadLine();
                        newTask.Notes.Add(note);
                        Console.WriteLine($"Note saved! {string.Join("|",newTask.Notes)}");
                        return true;
                    }
                case 4:
                    {
                        // add deadline date 
                        Console.WriteLine("Add Deadline dd-MM-yyyy");
                        DateTime deadline = DateTime.Parse(Console.ReadLine()); ;
                        newTask.Deadline = deadline;
                        Console.WriteLine($"Deadline saved! {newTask.Deadline}");
                        return true;
                    }
                case 5:
                    {
                        // mark priority with a specific value
                        Console.WriteLine("Mark priority, choose one from the following: Urgent, Important, Unurgent, Unimportant");
                        string priorityInput = Console.ReadLine();
                        newTask.SetPriority(priorityInput);
                        Console.WriteLine($"Priority set! {newTask.GetPriority()}");
                        return true;
                    }
                case 6:
                    {
                        // mark task done
                        newTask.Done = true;
                        Console.WriteLine("Marked done");
                        return true;
                    }
                case 7:
                    {
                        // show information of the task
                        Task.TaskToString(newTask);
                        return true;
                    }
                case 0:
                    {
                        // return to topic menu
                        Console.WriteLine("Returning to Topic Menu...");
                        return false;
                    }
                default:
                    {
                        return true;
                    }
            }
        }

        
        private static Dictionary<int, Topic> ReadAndConvert(string path)
        {
            string[] readText = File.ReadAllLines(path);
            Dictionary<int, Topic> dicTopic = new Dictionary<int, Topic>();
            for (int i = 0; i < readText.Length; i++)
            {
                string[] splitted = readText[i].Split(",");

                int key = int.Parse(splitted[0]);
                Topic tempTopic = new Topic(splitted[1], splitted[2], double.Parse(splitted[3]), splitted[5], DateTime.Parse(splitted[8]));
                tempTopic.Id = int.Parse(splitted[0]);
                tempTopic.TimeSpent = double.Parse(splitted[4], System.Globalization.CultureInfo.InvariantCulture);
                tempTopic.StartLearningDate = DateTime.ParseExact(splitted[6], "dd/MM/yyyy", null);
                tempTopic.InProgress = bool.Parse(splitted[7]);
                tempTopic.TaskList = new List<Task>();

                int numOfCol = 9;
                while(numOfCol < splitted.Length-1)
                {
                    Task task = new Task();
                    task.Id = int.Parse(splitted[numOfCol].Substring(4, splitted[0].Length));
                    task.Title = splitted[numOfCol + 1];
                    task.Description = splitted[numOfCol + 2];
                    task.Notes = splitted[numOfCol + 3].Split("|").ToList();
                    task.Deadline = DateTime.Parse(splitted[numOfCol + 4]);
                    task.SetPriority(splitted[numOfCol + 5]);
                    task.Done = bool.Parse(splitted[numOfCol + 6]);
                    tempTopic.TaskList.Add(task);
                    numOfCol += 7;
                }
                dicTopic.Add(key, tempTopic);
            }
            return dicTopic;
        }
        
        private static void DeleteAndUpdateId(string[] readText, int idToDelete, string path)
        {
            //convert to list so more convenient to delete topics
            List<string> linesToWrite = readText.ToList();
            //bool value to update id after deleted item
            bool idUpdate = false;
            for (int i = 0; i < readText.Length; i++)
            {
                // access each different point
                string[] splitted = readText[i].Split(',');
                if (int.Parse(splitted[0]) == idToDelete)
                {
                    idUpdate = true;
                    linesToWrite.RemoveAt(i);
                    continue;
                }
                //update id for every item after the deleted one
                if (idUpdate)
                {
                    int idToUp = int.Parse(splitted[0]);
                    idToUp -= 1;
                    splitted[0] = Convert.ToString(idToUp);
                    linesToWrite[i - 1] = String.Join(",", splitted);
                }
            }

            File.WriteAllLines(path, linesToWrite);
        }

        private static void AppendIntoFile(string path, string info)
        {
            File.AppendAllText(path, info + Environment.NewLine);
        }


    }

    public class Topic
    {
        // intiate fields
        public int Id;
        public string Title;
        public string Description;
        public double EstimatedTimeToMaster;
        public double TimeSpent;
        public string Source;
        public DateTime StartLearningDate;
        public bool InProgress = true;
        public DateTime CompletionDate;
        public List<Task> TaskList = new List<Task>();

        public Topic(string title="", string description="", double estimatedTimeToMaster=0, string source="", DateTime completionDate= default(DateTime))
        {
            this.Title = title;
            this.Description = description;
            this.EstimatedTimeToMaster = estimatedTimeToMaster;
            this.Source = source;
            this.CompletionDate = completionDate;
        }

        public static void TopicToString(Topic topic)

        {
            Console.WriteLine($"Topic id: {topic.Id}");
            Console.WriteLine($"Topic title: {topic.Title}");
            Console.WriteLine($"Topic description: {topic.Description}");
            Console.WriteLine($"Topic estimated time to master: {topic.EstimatedTimeToMaster}");
            Console.WriteLine("Topic time spent: {0:0.00} minutes", Math.Round(topic.TimeSpent, 2).ToString(CultureInfo.InvariantCulture.NumberFormat));
            Console.WriteLine($"Topic source: {topic.Source}");
            Console.WriteLine($"Topic starting date: {topic.StartLearningDate.ToShortDateString()}");
            Console.WriteLine($"Topic in process: {topic.InProgress}");
            Console.WriteLine($"Topic completion date: {topic.CompletionDate.ToShortDateString()}");
            Console.WriteLine("\n");

            // also show the task info included in the topic
            if (topic.TaskList.Count != 0)
            {
                foreach (Task taskInTopic in topic.TaskList)
                {
                    Task.TaskToString(taskInTopic);
                }
            }
        }
    }

    public class Task
    {
        // initial fields
        public int Id;
        public string Title;
        public string Description;
        public List<string> Notes = new List<string>();
        public DateTime Deadline;
        public bool Done = false;
        
        //set enum priority
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

        
        // used to show information of the task
        public static void TaskToString(Task newTask)
        {
            string notesFromList = string.Join("\n", newTask.Notes);
            Console.WriteLine($"Task id: {newTask.Id}\nTask title: {newTask.Title}\nTask description: {newTask.Description}\nTask notes: {notesFromList}\nTask Deadline: {newTask.Deadline}\nTask priority: {newTask.GetPriority()}\nTask done: {newTask.Done}\n");
        }

        // used to write into csv file
        public override string ToString()
        {
            string notesFromList = string.Join("|", Notes);
            return $"Task{Id},{Title},{Description},{notesFromList},{Deadline.ToShortDateString()},{GetPriority()},{Done}";
        }

    }

    
    
}
