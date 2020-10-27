using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using TweetSharp;

namespace TwitterBot
{

    class Program
    {
        private static string apikey = ConfigurationManager.AppSettings.Get("apikey");
        private static string apisercetkey = ConfigurationManager.AppSettings.Get("apisercetkey");
        private static string accesstoken = ConfigurationManager.AppSettings.Get("accesstoken");
        private static string accesstokensecert = ConfigurationManager.AppSettings.Get("accesstokensecert");
        static readonly Random rnd = new Random();
        private static System.Timers.Timer aTimer;
        private static ManualResetEvent mre = new ManualResetEvent(false);
        public static List<Quote> qlist = new List<Quote>();
        public static int quotecount;
        public static bool listempty = false;
        public static bool stillinmenu = true;


        private static TwitterService service = new TwitterService(apikey, apisercetkey, accesstoken, accesstokensecert); 

        static void Main(string[] args)

        {
            while (stillinmenu == true)
            {

                Console.WriteLine();
                string title = "~~~~TwitterBot~~~~";
                Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, Console.CursorTop);
                Console.WriteLine("~~~~~~~~~~~~~~~~~~");
                Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, Console.CursorTop);
                Console.WriteLine(title);
                Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, Console.CursorTop);
                Console.WriteLine("~~~~~~~~~~~~~~~~~~");

                Console.WriteLine("Please select:");
                Console.WriteLine();
                Console.WriteLine("A: Send standard tweet");
                Console.WriteLine();
                Console.WriteLine("B: Read tweets from default JSON file on time delay");
                Console.WriteLine();
                Console.WriteLine("C: Read tweets from input file path JSON file");
                Console.WriteLine();
                Console.WriteLine("D: Read from Database");
                Console.WriteLine();
                Console.WriteLine("Q: Quit");


                string userResponse = Console.ReadLine();

                switch (userResponse)
                {

                    case "A":
                        Console.Clear();
                        Console.WriteLine("Please write your tweet (No more that 280 characters)");
                        string usertweet = Console.ReadLine();

                        if (usertweet.Length <= 280)
                        {
                            SendTweet(usertweet);
                            Console.WriteLine("Tweet Send!");
                        }
                        else
                        {
                            Console.WriteLine("Tweet is to long");
                        }


                        break;

                    case "B":
                        string json = File.ReadAllText("../../datafile.json");
                        qlist = JsonConvert.DeserializeObject<List<Quote>>(json);
                        string list = qlist[quotecount].qutoe;

                        while (listempty == false)
                        {

                            DateTime timeNow = DateTime.Now;
                            DateTime timeWeek = DateTime.Now.AddMinutes(30);
                            DateTime randomdate = GetRandomDate(timeNow, timeWeek);
                            double inter = (randomdate - timeNow).TotalMilliseconds;

                            Console.WriteLine($"<{DateTime.Now}> - Bot Started");
                            Console.WriteLine(timeNow);
                            Console.WriteLine(timeWeek);
                            Console.WriteLine(randomdate);

                            SetTimer(inter, list);
                            mre.Reset();


                            aTimer.Stop();
                            aTimer.Dispose();
                        }
                        break;


                    case "C":
                        Console.Clear();
                        Console.WriteLine("Please write your file path");
                        string userfilepath = Console.ReadLine();
                        string userfilejson;
                        try
                        {
                            userfilejson = File.ReadAllText(userfilepath);
                        }
                        catch (FileNotFoundException e)
                        {
                            Console.WriteLine("Invaild Path", e);

                            break;
                        }                                                                   


                        qlist = JsonConvert.DeserializeObject<List<Quote>>(userfilejson);
                        string userfilelist = qlist[quotecount].qutoe;

                        while (listempty == false)
                        {

                            DateTime timeNow = DateTime.Now;
                            DateTime timeWeek = DateTime.Now.AddMinutes(30);
                            DateTime randomdate = GetRandomDate(timeNow, timeWeek);
                            double inter = (randomdate - timeNow).TotalMilliseconds;

                            Console.WriteLine($"<{DateTime.Now}> - Bot Started");
                            Console.WriteLine(timeNow);
                            Console.WriteLine(timeWeek);
                            Console.WriteLine(randomdate);

                            SetTimer(inter, userfilelist);
                            mre.Reset();


                            aTimer.Stop();
                            aTimer.Dispose();
                        }
                        break;

                    case "E":
                        {
                            Console.Clear();
                            Console.WriteLine("Please enter user screen name");
                            string twitteruser = Console.ReadLine();


                            ListTweetsOnUserTimelineOptions listTweetsOnListOptions = new ListTweetsOnUserTimelineOptions();
                            listTweetsOnListOptions.ScreenName = twitteruser;

                            var tweets = service.ListTweetsOnUserTimeline(listTweetsOnListOptions);

                            foreach (TwitterStatus t in tweets)
                            {
                                Console.WriteLine(t.Text);

                            }

                            break;
                        }


                    case "Q":
                        stillinmenu = false;
                        Console.WriteLine("Comeback when you fancy tweeting");
                        break;

                    default:
                        break;

                }
            }
           
           

        }

        private static void SendTweet(string _status)
        {
            service.SendTweet(new SendTweetOptions { Status = _status }, (tweet, response) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"<{DateTime.Now}> - Tweet Sent!");
                    Console.ResetColor();

                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Tweet Failed" + response.Error.Message);
                    Console.ResetColor();

                }

            });

        }
        
        public static DateTime GetRandomDate(DateTime from, DateTime to)
        {
            var range = to - from;

            var randTimeSpan = new TimeSpan((long)(rnd.NextDouble() * range.Ticks));

            return from + randTimeSpan;
        }



        private static void SetTimer(double time, string quote)
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(time);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = false;
            aTimer.Enabled = true;
            mre.WaitOne();


        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            string quote = GetQuote(quotecount);

            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",e.SignalTime);
            Console.WriteLine("{0}", quote);                    
            
            SendTweet(quote);
            
            mre.Set();
            quotecount++;

            //aTimer.Elapsed -= OnTimedEvent;

        }

        private static string GetQuote(int qnumber)
        {
            if (qnumber + 1 == qlist.Count())
            {
                listempty = true;
                return qlist[qnumber].qutoe;

            }
            else
            {
                return qlist[qnumber].qutoe;
            }
        }
    }
}
