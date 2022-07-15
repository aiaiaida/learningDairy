using System;
using System.Linq;
using LearningDiary_Aada_V1.Models;
using ClassLibraryForLearningDiary;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LearningDiary_Aada_V1
{
    class Program
    {
        static void Main(string[] args)
        {
            // a bool value to display the MainMenu 
            bool displayMenu = true;
            
            // main loop
            while (displayMenu)
            {
                displayMenu = MainMenu().Result;
            }
        }

        private static async Task<bool> MainMenu()
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


            try
            {
                int userChoice = int.Parse(Console.ReadLine());
                // different code blocks for different choices
                switch (userChoice)
                {
                    case 1:
                        
                        Task<Topic> newTopic= AddNewTopicAsync();
                        using (LearningDiaryContext db = new LearningDiaryContext())
                        {

                            //Topic menu
                            bool displayTopicMenu = true;
                            while (displayTopicMenu)
                            {
                                //Console.WriteLine("Here i am in the display loop");
                                //Console.WriteLine(DateTime.Now); 
                                displayTopicMenu = TopicMenu(newTopic.Result);
                                newTopic.Wait();
                            }
                            db.Topics.Update(newTopic.Result);
                            await db.SaveChangesAsync();
                            
                            // print the time
                            //Console.WriteLine("changes are once again saved");
                            //Console.WriteLine(DateTime.Now);
                        }
                        Console.ReadKey();
                        return true;
                    case 2:
                        {
                            // look up a topic based on id
                            Console.WriteLine("Please enter the id or title to search a topic\nFormat:id-123 or title-mytopicname, but not at the same time");
                            string userInput = Console.ReadLine();
                            string[] splitted = userInput.Split("-");
                            if (userInput.Contains("id") && userInput.Contains("title"))
                            {
                                Console.WriteLine("Not id and title the same time!");
                            }
                            else
                            {
                                LookUpTopic(splitted);
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
                            EditTopic(id);
                            return true;
                        }
                    case 4:
                        {
                            //delete one topic
                            Console.WriteLine("Please enter the id of the topic: ");
                            int id = int.Parse(Console.ReadLine());
                            DeleteATopic(id);
                            Console.WriteLine("Deletion completed!");
                            Console.WriteLine("Press any key to return to the main menu");
                            Console.ReadKey();
                            return true;
                        }

                    case 5:
                        {
                            //delete all topics by writing over it with a empty string
                            DeleteAllTopics();
                            Console.WriteLine("Deletion completed");
                            Console.WriteLine("Press any key to return to the main menu");
                            Console.ReadKey();
                            return true;

                        }

                    case 6:
                        {
                            // print all topics in topic format
                            using (LearningDiaryContext db = new LearningDiaryContext())
                            {
                                var allTopics = db.Topics.Select(topic => topic).ToList();
                                allTopics.ForEach(topic => PrintTopic(topic));
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
            catch (FormatException)
            {
                Console.WriteLine(
                    $"The number you entered is not valid.");
                Console.WriteLine("Press any key to return to the main menu");
                Console.ReadKey();
                return true;
            }
        }

        // method to write new topic into csv file


        public static async Task<Topic> AddNewTopicAsync()
        {
            Topic newTopic = new Topic();
            Console.WriteLine("Please give me a title of this topic");
            string topicTitle = Console.ReadLine();
            newTopic.Title = topicTitle;
            Console.WriteLine($"The topic {newTopic.Title} is created!");

            using (LearningDiaryContext db = new LearningDiaryContext())
            {
                var allTopics = await (from topic in db.Topics select topic).ToListAsync();
                //Console.WriteLine("database read");
                //Console.WriteLine(DateTime.Now);
                if (!allTopics.Any())
                {
                    newTopic.Id = 1;
                }
                else
                {
                    newTopic.Id = db.Topics.Max(topic => (int)topic.Id) + 1;
                }
                // set starting date immediately when the topic is created
                newTopic.StartLearningDate = DateTime.Now;
                db.Topics.Add(newTopic);

                //var t = Task.Run(() => db.SaveChangesAsync()); 00.23

                await db.SaveChangesAsync();
                //Console.WriteLine("changes saved");
                //Console.WriteLine(DateTime.Now);

                /*Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
                Console.WriteLine("RunTime " + elapsedTime);*/
            }
            return newTopic;
        }
        public static void LookUpTopic(string[] splitted)
        {
            using (LearningDiaryContext db = new LearningDiaryContext())
            {
                if (splitted[0].Contains("id"))
                {
                    int id = int.Parse(splitted[1]);
                    if (db.Topics.Any(topic => topic.Id == id))
                    {
                        var topicRes = db.Topics.Where(topic => topic.Id == id).Single();
                        PrintTopic(topicRes);
                    }
                    else
                    {
                        Console.WriteLine("Not Found!");
                    }
                }
                else if (splitted[0].Contains("title"))
                {
                    string title = splitted[1];
                    if (db.Topics.Any(topic => topic.Title == title))
                    {
                        var topicRes = db.Topics.Where(topic => topic.Title == title).Single();
                        PrintTopic(topicRes);
                    }
                    else
                    {
                        Console.WriteLine("Not Found!");
                    }
                }
                else
                {
                    Console.WriteLine("You entered in wrong format");
                }
            }
        }

        public static async void EditTopic(int id)
        {
            using (LearningDiaryContext db = new LearningDiaryContext())
            {
                var topicToEdit = db.Topics.Where(topic => topic.Id == id).Single();

                bool displayEdit = true;
                while (displayEdit)
                {
                    displayEdit = TopicMenu(topicToEdit);
                }

                db.Update(topicToEdit);
                await db.SaveChangesAsync();
            }
        }
        public static async void DeleteATopic(int id)
        {
            using (LearningDiaryContext db = new LearningDiaryContext())
            {
                var topicToRemove = db.Topics.Where(topic => topic.Id.Equals(id)).Single();
                var tasksToDelete = db.TaskInTopics.Where(task => task.TopicId == id);
                db.TaskInTopics.RemoveRange(tasksToDelete);
                db.Topics.Remove(topicToRemove);
                await db.SaveChangesAsync();
            }
        }
        public static async void DeleteAllTopics()
        {
            using (LearningDiaryContext db = new LearningDiaryContext())
            {
                db.Notes.RemoveRange(db.Notes);
                db.TaskInTopics.RemoveRange(db.TaskInTopics);
                db.Topics.RemoveRange(db.Topics);
                await db.SaveChangesAsync();
            }
        }
        public static void PrintTopic(Topic topic)
        {
            // print topic info
            Console.WriteLine($"Topic id: {topic.Id}");
            Console.WriteLine($"Topic title: {topic.Title}");
            Console.WriteLine($"Topic description: {topic.Description}");
            Console.WriteLine($"Topic estimated time to master: {topic.EstimatedTimeToMaster}");
            Console.WriteLine("Topic time spent: {0:0.00} hours", topic.TimeSpent);
            Console.WriteLine($"Topic source: {topic.Source}");
            Console.WriteLine($"Topic starting date: {topic.StartLearningDate}");
            Console.WriteLine($"Topic in process: {topic.InProgress}");
            Console.WriteLine($"Topic completion date: {topic.CompletionDate}");

            // print tasks
            using (LearningDiaryContext db = new LearningDiaryContext())
            {
                var tasksToPrint = db.TaskInTopics.Where(task=>task.TopicId == topic.Id);
                if (tasksToPrint.Any())
                {
                    tasksToPrint.ToList().ForEach(task => PrintTask(task));
                }
            }
            
            // check if the topic overdue
            // based on the starting date and estimated time
            // if it is, print a message to the user
            Class1 ClassLibrary = new Class1();
            DateTime startDate = topic.StartLearningDate.HasValue ? topic.StartLearningDate.Value : new DateTime(00-00-0000);
            double estimatedTime = topic.EstimatedTimeToMaster.HasValue ? topic.EstimatedTimeToMaster.Value : 0;
            bool inProgress = topic.InProgress.HasValue ? topic.InProgress.Value: true;

            if (ClassLibrary.CheckIfLate(startDate, estimatedTime, inProgress))
            {
                Console.WriteLine("Note: This topic is overdue according to your estimated time to master it.");
            }

            // a line to separate topics
            Console.WriteLine("-------------\n\n");
            
        }

        private static void PrintTask(TaskInTopic task)
        {
            using (LearningDiaryContext db = new LearningDiaryContext())
            {
                // notes to a string separated by comma
                var notes = db.Notes.Where(Note => Note.TaskId == task.Id).ToList();
                string noteStr = String.Join(",", notes.Select(note=>note.Note1));
                
                // print task info
                Console.WriteLine($"\nTask id: {task.Id}\n" +
                    $"Task Topic_id: {task.TopicId}\n" +
                    $"Task title: {task.Title}\n" +
                    $"Task description: {task.Description}\n" +
                    $"Task notes: {noteStr}\n" +
                    $"Task Deadline: {task.Deadline}\n" +
                    $"Task priority: {task.Priority}\n" +
                    $"Task done: {task.Done}");
            } 
        }
        private static bool TopicMenu(Topic newTopic)
        {
            Console.WriteLine("");
            Console.WriteLine("Please choose an option: ");
            Console.WriteLine("1) Title");
            Console.WriteLine("2) Description");
            Console.WriteLine("3) Estimated time to master in hour unit");
            Console.WriteLine("4) Source");
            Console.WriteLine("5) Completion date (dd-mm-yyyy)");
            Console.WriteLine("6) Manage tasks");
            Console.WriteLine("7) Show information of this topic");
            Console.WriteLine("0) Return to the main menu and save the topic");

            try
            {
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
                            Console.WriteLine("Please enter the estimated time to master in hour unit");
                            bool parsedOrNot = int.TryParse(Console.ReadLine(),out int userEstimated);
                            if (parsedOrNot)
                            {
                                newTopic.EstimatedTimeToMaster = userEstimated;
                            }
                            else
                            {
                                Console.WriteLine("You entered wrong format, please try again");
                            }
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
                            try
                            {
                                DateTime userCompletionDate = DateTime.Parse(Console.ReadLine());
                                // if completion date in future, fail
                                Class1 ClassLibrary = new Class1();
                                if (ClassLibrary.CheckDateInFuture(userCompletionDate))
                                {
                                    Console.WriteLine("Failed.\nThe completion date can not be in future, Please try again.");
                                    return true;
                                }
                                else
                                {
                                    newTopic.CompletionDate = userCompletionDate;

                                    // set in progress automatically to false
                                    // and calculate time spent on the topic
                                    newTopic.InProgress = false;
                                    newTopic.TimeSpent = (int?)(newTopic.CompletionDate.Value - newTopic.StartLearningDate.Value).TotalHours;
                                    Console.WriteLine($"Completion date saved. {newTopic.CompletionDate}");
                                }
                                return true;
                            }
                            catch(Exception)
                            {
                                Console.WriteLine("You entered in wrong format, please try again");
                                return true;
                            }
                        }

                    case 6:
                        {
                            // trigger a task menu
                            bool displayManageTasks = true;
                            while (displayManageTasks)
                            {
                                displayManageTasks = ManageTasks(newTopic);
                            }

                            return true;
                        }

                    case 7:
                        {
                            // show information of this topic
                            PrintTopic(newTopic);

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
            catch (FormatException)
            {
                Console.WriteLine(
                    $"The number you entered is not valid.");
                Console.WriteLine("Press any key to return to the menu");
                Console.ReadKey();
                return true;
            }
        }

        private static bool ManageTasks(Topic newTopic)
        {
            Console.WriteLine("");
            Console.WriteLine("Please choose an option: ");
            Console.WriteLine("1) Add a new task");
            Console.WriteLine("2) Look up a task");
            Console.WriteLine("3) Edit a task");
            Console.WriteLine("4) Delete a task");
            Console.WriteLine("5) Delete all tasks");
            Console.WriteLine("6) Show information of all tasks of this topic");
            Console.WriteLine("0) Return to the topic menu");

            try
            {
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
                            Console.WriteLine("Please enter the id or title to search a topic\nFormat:id-123 or title-mytopicname, but not at the same time");
                            string userInput = Console.ReadLine();
                            string[] splitted = userInput.Split("-");
                            using (LearningDiaryContext db = new LearningDiaryContext())
                            {
                                if (userInput.Contains("id") && userInput.Contains("title"))
                                {
                                    Console.WriteLine("Not id and title the same time!");
                                }
                                else if (splitted[0].Contains("id"))
                                {
                                    int id = int.Parse(splitted[1]);
                                    if (db.TaskInTopics.Any(task => task.Id == id))
                                    {
                                        var taskRes = db.TaskInTopics.Where(task => task.Id == id).Single();
                                        PrintTask(taskRes);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Not Found!");
                                    }
                                }
                                else if (splitted[0].Contains("title"))
                                {
                                    string title = splitted[1];
                                    if (db.TaskInTopics.Any(task => task.Title == title))
                                    {
                                        var taskRes = db.TaskInTopics.Where(task => task.Title == title).Single();
                                        PrintTask(taskRes);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Not Found!");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("You entered in wrong format");
                                }
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
                            EditTask(id, newTopic);
                            return true;
                        }

                    case 4:
                        {
                            //Delete one task
                            Console.WriteLine("Please enter the id of the task to be deleted");
                            int id = int.Parse(Console.ReadLine());
                            DeleteATask(id, newTopic);
                            Console.WriteLine("Deletion completed!");
                            Console.WriteLine("Press any key to return to the main menu");
                            Console.ReadKey();
                            return true;
                        }

                    case 5:
                        {
                            // Delete all tasks
                            DeleteAllTasks(newTopic);
                            Console.WriteLine("Deletion completed");
                            Console.WriteLine("Press any key to return to the main menu");
                            Console.ReadKey();
                            return true;
                        }

                    case 6:
                        {
                            // Show info of all tasks of this topic
                            using (LearningDiaryContext db = new LearningDiaryContext())
                            {
                                var allTasks = db.TaskInTopics.Where(task => task.TopicId == newTopic.Id).ToList();
                                allTasks.ForEach(task => PrintTask(task));
                            }
                            Console.WriteLine("Press any key to return to the main menu");
                            Console.ReadKey();
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
            catch (FormatException)
            {
                Console.WriteLine(
                    $"The number you entered is not valid.");
                Console.WriteLine("Press any key to return to the menu");
                Console.ReadKey();
                return true;
            }
        }
        private static void AddNewTask(Topic topic)
        {
            TaskInTopic newTask = new TaskInTopic();
            
            using (LearningDiaryContext db = new LearningDiaryContext())
            {
                Console.WriteLine("Please enter a title for the task");
                newTask.Title = Console.ReadLine();
                
                var allTasks = db.TaskInTopics.Select(task => task);
                if (!allTasks.Any())
                {
                    newTask.Id = 1;
                }
                else
                {
                    newTask.Id = db.TaskInTopics.Max(task => (int)task.Id) + 1;
                }
                newTask.TopicId = topic.Id;
                db.TaskInTopics.Add(newTask);
                db.SaveChangesAsync ();
                Console.WriteLine($"The new task is created!");
                //a task menu loop
                bool displayTaskMenu = true;
                while (displayTaskMenu)
                {
                    displayTaskMenu = TaskMenu(newTask, topic);
                }
                // add the task into topic
                //calculate the id
                db.TaskInTopics.Update(newTask);
                db.SaveChangesAsync ();
            }
        }
        
        private static async void EditTask(int id, Topic newTopic)
        {
            using (LearningDiaryContext db = new LearningDiaryContext())
            {
                var taskToEdit = db.TaskInTopics.Where(task => task.Id.Equals(id)).FirstOrDefault();

                bool displayEdit = true;
                while (displayEdit)
                {
                    displayEdit = TaskMenu(taskToEdit, newTopic);
                }
                db.TaskInTopics.Update(taskToEdit);
                await db.SaveChangesAsync();
            }
        }

        private static async void DeleteATask(int id, Topic newTopic)
        {
            using (LearningDiaryContext db = new LearningDiaryContext())
            {
                var taskToRemove = db.TaskInTopics.Where(task => task.Id.Equals(id)).Single();
                db.TaskInTopics.Remove(taskToRemove);
                await db.SaveChangesAsync ();
            }
        }
        private static async void DeleteAllTasks(Topic newTopic)
        {
            using (LearningDiaryContext db = new LearningDiaryContext())
            {
                var tasksToRemove = db.TaskInTopics.Where(task => task.TopicId.Equals(newTopic.Id)).ToList();
                tasksToRemove.ForEach(task => db.TaskInTopics.Remove(task));
                await db.SaveChangesAsync();
            }
        }
        private static bool TaskMenu(TaskInTopic newTask, Topic topic)
        {
            Console.WriteLine("");
            Console.WriteLine("Please choose an option: ");
            Console.WriteLine("1) Edit Title");
            Console.WriteLine("2) Description");
            Console.WriteLine("3) Notes");
            Console.WriteLine("4) Deadline");
            Console.WriteLine("5) Mark Priority");
            Console.WriteLine("6) Mark Done");
            Console.WriteLine("7) Show all information of this task");
            Console.WriteLine("0) Return to Task Menu");

            try
            {
                int userTaskChoice = int.Parse(Console.ReadLine());

                switch (userTaskChoice)
                {

                    case 1:
                        {
                            // set description
                            Console.WriteLine("Please Enter Title for the Task");
                            string title = Console.ReadLine();
                            newTask.Title = title;
                            Console.WriteLine($"Title saved! {newTask.Title}");
                            return true;
                        }
                    case 2:
                        {
                            // set description
                            Console.WriteLine("Please Enter Description");
                            string description = Console.ReadLine();
                            newTask.Description = description;
                            Console.WriteLine($"Description saved! {newTask.Description}");
                            return true;
                        }
                    case 3:
                        {
                            // add notes into list
                            Console.WriteLine("Please Enter Notes");
                            string note = Console.ReadLine();
                            Note newNote = new Note();
                            newNote.Note1 = note;
                            newNote.TaskId = newTask.Id;
                            newNote.TopicId = topic.Id;
                            AddNote(newNote);
                            return true;
                        }
                    case 4:
                        {
                            // add deadline date 
                            Console.WriteLine("Please Enter Deadline dd-MM-yyyy");
                            try
                            {
                                DateTime deadline = DateTime.Parse(Console.ReadLine()); ;
                                newTask.Deadline = deadline;
                                Console.WriteLine($"Deadline saved! {newTask.Deadline}");
                                return true;
                            }
                            catch(Exception)
                            {
                                Console.WriteLine("You entered in wrong format, please try again");
                                return true;
                            }
                            
                        }
                    case 5:
                        {
                            // mark priority with a specific value
                            string[] prioritySet = new string[] { "Urgent", "Important", "Unurgent", "Unimportant" };
                            Console.WriteLine("Mark priority, choose one from the following: Urgent, Important, Unurgent, Unimportant");
                            string priorityInput = Console.ReadLine();
                            if (prioritySet.Contains(priorityInput))
                            {
                                newTask.Priority = priorityInput;
                                Console.WriteLine($"Priority set! {newTask.Priority}");
                            }
                            else
                            {
                                Console.WriteLine("You entered in wrong format, please try again");
                            }
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
                            PrintTask(newTask);
                            return true;
                        }
                    case 0:
                        {
                            // return to topic menu
                            Console.WriteLine("Returning to Task Menu...");
                            return false;
                        }
                    default:
                        {
                            return true;
                        }
                }
            }
            catch (FormatException)
            {
                Console.WriteLine(
                    $"The number you entered is not valid.");
                Console.WriteLine("Press any key to try return to the menu");
                Console.ReadKey();
                return true;
            }
        }
        private static async void AddNote(Note newNote)
        {
            using (LearningDiaryContext db = new LearningDiaryContext())
            {
                var allNotes = db.Notes.Select(note => note);
                if (!allNotes.Any())
                {
                    newNote.Id = 1;
                }
                else
                {
                    newNote.Id = db.Notes.Max(note => (int)note.Id) + 1;
                }

                db.Notes.Add(newNote);
                await db.SaveChangesAsync();
            }
        }
    }
 }

