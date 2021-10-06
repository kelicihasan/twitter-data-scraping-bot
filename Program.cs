using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace twitterHkBot
{
    public class Program
    {
        static ChromeDriver driver;
        public static int Pageheight { get; set; }
        public static int Index { get; set; }
        public static int twIndex { get; set; }
        public static int actionIndex { get; set; }
        //
        static List<Tw> twList = new List<Tw>();
        static List<TwActions> actionList = new List<TwActions>();
        static void Main(string[] args)
        {
            Console.WriteLine("###############################################################################");
            Console.WriteLine("###############################################################################");
            Console.WriteLine("####################### Twitter Web Scraping Bot  #############################");
            Console.WriteLine("###############################################################################");
            Console.WriteLine("###############################################################################");
            Connect();
        }
        public static void Connect()
        {
            System.Media.SystemSounds.Beep.Play();
            Console.Write("Enter Scroll Value :");
            string scValue = Console.ReadLine();
            Console.WriteLine("Connecting.....\n\n");
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://twitter.com/billgates");
            //driver.Manage().Window.Maximize();
            Pageheight = driver.Manage().Window.Size.Height;
            OpenQA.Selenium.IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            string scrl = string.Format("window.scrollBy(0,{0})",
               Convert.ToInt32(scValue) * Pageheight);
            for (int i = 0; i < Convert.ToInt32(scValue); i++)
            {
                Thread.Sleep(10000);
                MainProc();
                js.ExecuteScript(scrl);
            }
            TwitListWrite();
        }
        public static void MainProc()
        {
            IEnumerable<IWebElement> elements1 = null;
            IEnumerable<IWebElement> elements2 = null;
            IWebElement elements3 = null;
            IEnumerable<IWebElement> element4 = null;
            IEnumerable<IWebElement> elements = driver.FindElements(By.XPath("//div[@class='css-1dbjc4n r-1iusvr4 r-16y2uox r-1777fci r-kzbkwu']"));
            //
            //tweet content
            foreach (IWebElement item in elements)
            {
                elements1 = item.FindElements(By.XPath("//div[@class='css-1dbjc4n']"));
            }
            foreach (IWebElement item in elements1)
            {
                elements2 = item.FindElements(By.XPath("//div[@class='css-901oao r-18jsvk2 r-37j5jr r-a023e6 r-16dba41 r-rjixqe r-bcqeeo r-bnwqim r-qvutc0']"));
                element4 = item.FindElements(By.XPath("//div[@class='css-1dbjc4n r-1ta3fxp r-18u37iz r-1wtj0ep r-1s2bzr4 r-1mdbhws']"));
            }
            //
            //tweet id
            foreach (IWebElement item in elements2)
            {
                elements3 = item.FindElement(By.XPath("//span[@class='css-901oao css-16my406 r-poiln3 r-bcqeeo r-qvutc0']"));
                if (elements3.Text != "" && !elements3.Text.StartsWith("@"))
                {
                    twList.Add(
                        new Tw
                        {
                            id = twIndex,
                            tweet = item.Text,
                            twid = item.GetAttribute("id").ToString()
                        });
                    twIndex++;
                }
            }
            //
            //tweet like , comment and retweet list
            foreach (IWebElement item in element4)
            {
                string action = item.GetAttribute("aria-label").ToString() == "" 
                    ? " 0" 
                    : item.GetAttribute("aria-label").ToString();
                actionList.Add(
                    new TwActions
                    {
                        id = actionIndex,
                        actions = action
                    });
                actionIndex++;
            }
            //TwitListWrite();
        }
        //print the list
        public static void TwitListWrite()
        {
            Console.WriteLine("-------------------------------------------------------------------");
            var join = (from d in actionList
                        join t in twList
                        on d.id equals t.id
                        select new { t.id, t.tweet, d.actions }).Distinct().ToList();
            //0 is id
            //1 is tweet
            //2 is action
            foreach (var item in join)
            {
                Console.WriteLine("{0}-{1}-{2}",item.id,item.tweet,item.actions);
            }
            Console.WriteLine("\n\n");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
