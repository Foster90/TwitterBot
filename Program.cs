using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TweetSharp;

namespace TwitterBot
{

    class Program
    {
        private static string apikey = ConfigurationManager.AppSettings.Get("apikey");
        private static string apisercetkey = ConfigurationManager.AppSettings.Get("apisercetkey");
        private static string accesstoken = ConfigurationManager.AppSettings.Get("accesstoken");
        private static string accesstokensecert = ConfigurationManager.AppSettings.Get("accesstokensecert");


        private static TwitterService service = new TwitterService(apikey, apisercetkey, accesstoken, accesstokensecert);

        static void Main(string[] args)


        {

            Console.WriteLine($"<{DateTime.Now}> - Bot Started");

            SendTweet("Oh right. I see. I get it. You were lampooning me. It was a simple lampoon.");
            Console.ReadLine();

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
    }
}
