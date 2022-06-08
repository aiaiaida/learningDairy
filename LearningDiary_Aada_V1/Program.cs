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
            string path = @"C:\Users\Aada\source\repos\AWACADEMY\KoulutusWeek1\LearningDiaryAadaV1\topiclist.txt";
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
            Console.WriteLine("2) Delete a topic");
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
                    return true;

                case 3:
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
            // !!!!!!!! this id doesn't workkkk !!!!!!
            string[] readText = File.ReadAllLines(path);
            newTopic.id = readText.Length + 1;

            newTopic.title = title;
            Console.WriteLine($"The {title} topic is created!");

            bool displayTopicMenu = true;
            while (displayTopicMenu)
            {
                displayTopicMenu = TopicMenu(ref newTopic);
            }
            
            topicList.Add(newTopic);
            string info = $"{newTopic.id},{newTopic.title},{newTopic.description},{newTopic.estimatedTimeToMaster},{newTopic.timeSpent},{newTopic.source},{newTopic.startLearningDate.ToShortDateString()},{newTopic.inProgress},{newTopic.completionDate.ToShortDateString()}";
            if (File.Exists(path))
            {
                File.AppendAllText(path, info + Environment.NewLine);
            }
            else
            {
                File.WriteAllText(path, info + Environment.NewLine);
            }
        }

        private static bool TopicMenu(ref Topic newTopic)
        {
            Console.WriteLine("");
            Console.WriteLine("Please choose an option to Add to your topic: ");
            Console.WriteLine("1) Description");
            Console.WriteLine("2) Estimated time to master in hour unit");
            Console.WriteLine("3) Time spent on it in hour unit");
            Console.WriteLine("4) Source");
            Console.WriteLine("5) Date when you started learning (dd/mm/yyyy)");
            Console.WriteLine("6) Completion date (dd/mm/yyyy)");
            Console.WriteLine("7) Show information of this topic");
            Console.WriteLine("8) Return to the main menu");

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
                    Console.WriteLine("Please enter the time already spent on this topic in hour unit");
                    double userTimeSpent = Convert.ToDouble(Console.ReadLine());
                    newTopic.timeSpent = userTimeSpent;
                    Console.WriteLine($"Time spent saved. {newTopic.timeSpent}");
                    return true;
                case 4:
                    Console.WriteLine("Please enter a source web url or a book");
                    string userSource = Console.ReadLine();
                    newTopic.source = userSource;
                    Console.WriteLine($"Source saved. {newTopic.source}");
                    return true;
                case 5:
                    Console.WriteLine("Please enter the date when you started the topic (dd/mm/yyyy)");
                    DateTime userStartDate = DateTime.ParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    newTopic.startLearningDate = userStartDate;
                    Console.WriteLine($"Start learning date saved. {newTopic.startLearningDate.ToShortDateString()}");
                    return true;
                case 6:
                    Console.WriteLine("Please enter the completion date (dd/mm/yyyy)");
                    DateTime userCompletionDate = DateTime.ParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    newTopic.completionDate = userCompletionDate;
                    newTopic.inProgress = false;
                    Console.WriteLine($"Completion date saved. {newTopic.completionDate.ToShortDateString()}");
                    return true;
                case 7:
                    Topic.TopicToString(newTopic);
                    return true;
                case 8:
                    Console.WriteLine("Returning to the main menu ...");
                    return false;
                default:
                    return true;
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

            //???not sure if a constructor is needed

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

            public static void TopicToString(Topic topic)
            {
            Console.WriteLine($"Topic id: {topic.id}");
            Console.WriteLine($"Topic title: {topic.title}");
            Console.WriteLine($"Topic description: {topic.description}");
            Console.WriteLine($"Topic estimated time to master: {topic.estimatedTimeToMaster}");
            Console.WriteLine($"Topic time spent: {topic.timeSpent}");
            Console.WriteLine($"Topic source: {topic.source}");
            Console.WriteLine($"Topic starting date: {topic.startLearningDate.ToShortDateString()}");
            Console.WriteLine($"Topic in process: {topic.inProgress}");
            Console.WriteLine($"Topic completion date: {topic.completionDate.ToShortDateString()}");
        }

        }
    
}
