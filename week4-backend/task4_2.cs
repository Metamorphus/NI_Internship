using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;

namespace week4_2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string mainURL = "http://nz.ukma.edu.ua/index.php?option=com_content&task=category&sectionid=10&id=60&Itemid=47";

            Crawler crawler = new Crawler();
            crawler.CrawlWebsite(mainURL);
        }
    }

    class Crawler
    {
        HttpClient client;

        public Crawler()
        {
            client = new HttpClient();
        }

        public string GetPage(HttpClient client, string url)
        {
            var bytes = client.GetByteArrayAsync(url).Result;
            return Encoding.ASCII.GetString(bytes);
        }

        public void CrawlWebsite(string mainURL)
        {
            string mainPage = GetPage(client, mainURL);

            string outerPattern = @"a href=""(http://nz\.ukma\.edu\.ua/index.php\?option=com_content&amp;task=view&amp;id=\d+&amp;Itemid=47)""";

            var outerMatches = Regex.Matches(mainPage, outerPattern);
            List<string> outerLinks = new List<string>();

            for (int i = 0; i < outerMatches.Count; i++)
            {
                outerLinks.Add(outerMatches[i].Groups[1].Value);
            }

            string dir = "Papers"; 
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string innerPattern = @"""(http://www\.ekmair\.ukma\.edu\.ua/bitstream/handle/\d+/\d+/.*?\.pdf)""";
            int totalFiles = 0;

            foreach (string outerLink in outerLinks)
            {
                string innerPage = GetPage(client, outerLink.Replace("amp;", ""));
                var innerMatches = Regex.Matches(innerPage, innerPattern);

                for (int i = 0; i < innerMatches.Count; i++)
                {
                    string fileName = innerMatches[i].Groups[1].Value;
                    string path = dir + "\\" + totalFiles.ToString() + ".pdf";

                    File.WriteAllBytes(path, client.GetByteArrayAsync(fileName).Result);
                    ++totalFiles;

                    Thread.Sleep(5000);
                }
            }

        }

    }
}
