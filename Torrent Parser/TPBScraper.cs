using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Windows.Forms;

namespace Torrent_Parser
{
    public class TPBScraper
    {

        public TPBScraper Scraper
        {
            get { return Scraper; }
            set { Scraper = value; }
        }
        string proxyBayUrl = @"https://proxybay.one/list.txt";
        string googleCache = @"http://webcache.googleusercontent.com/search?q=cache%3Awww.proxybay.one%2Flist.txt";
        public bool TestTPBURL(string TPBUrl)
        {
            string html = GetHTML(TPBUrl + "/search/test/0/7/0");
            if (html == null) return false;
            MatchCollection mc = Regex.Matches(html, @"<title>.*The Pirate Bay.*<\/title>");
            return mc.Count > 0;
        }

        public string GetHTML(string url)
        {
            string html = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }
            }
            catch (WebException)
            {
                return null;
                
            }

            //Console.WriteLine(html);
            return html;
        }

        public string GetNewTPBUrl()
        {
            string html = GetHTML(proxyBayUrl);
            string[] urlList = new string[] { };
            if (html != null)
            {
                try
                {
                    urlList = html.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).Skip(3).ToArray();
                    urlList = urlList.Take(urlList.Length - 1).ToArray();
                }
                catch
                {
                    MessageBox.Show("Failed to parse proxybay list");
                }
            }
            else {
                html = GetHTML(googleCache);
                if (html != null)
                {
                    try
                    {
                        urlList = html.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).Skip(5).ToArray();
                        urlList = urlList.Take(urlList.Length - 1).ToArray();
                    }
                    catch
                    {
                        MessageBox.Show("Failed to parse google cache of proxybay list");
                    }
                }
            }
            string newURL = "";
            foreach (string urlItem in urlList)
            {
                if (TestTPBURL(urlItem))
                {
                    newURL = urlItem;
                    break;
                }
            }
            return newURL;
        }
        public MatchCollection Scrape (string url)
        {
            string html = GetHTML(url);
            return Regex.Matches(html, @"(<div class=""detName"">.*?<a href=""(?<url>/torrent.*?)"".*?title="".*?>(?<title>.*?)<.*?(?<magnet>magnet.*?)"".*?Uploaded\s(?<date>.*?),.*?<td align=""right"">(?<seeders>.*?)</td>.*?<td align=""right"">(?<leechers>.*?)</td>)", RegexOptions.Singleline);
        }
    }
}
