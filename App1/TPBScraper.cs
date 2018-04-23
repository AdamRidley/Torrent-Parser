using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace Torrent_Parser
{
    public class TPBScraper
    {

        public TPBScraper Scraper
        {
            get { return Scraper; }
            set { Scraper = value; }
        }
        string proxyBayUrl = @"https://proxybay.bz/list.txt";
        string googleCache = @"http://webcache.googleusercontent.com/search?q=cache%3Awww.proxybay.bz%2Flist.txt";
        string debugLog;

        public bool TestTPBURL(string TPBUrl)
        {
            string html = GetHTML(TPBUrl+"/search/test/0/7/0");
            if (html == null) return false;
            MatchCollection mc = Regex.Matches(html, @"<title>.*The Pirate Bay.*<\/title>");
            return mc.Count>0;
        }

        public string GetHTML(string url)
        {
            //string html = string.Empty;

            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //request.AutomaticDecompression = DecompressionMethods.GZip;
            //try
            //{
            //    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            //    using (Stream stream = response.GetResponseStream())
            //    using (StreamReader reader = new StreamReader(stream))
            //    {
            //        html = reader.ReadToEnd();
            //    }
            //}
            //catch (WebException)
            //{
            //    return null;

            //}

            ////Console.WriteLine(html);
            //return html;

            //Trying new http request call!!!
            string html = string.Empty;
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("Accept-Language", " en-US");
                    client.Headers.Add("Accept", " text/html, application/xhtml+xml, */*");
                    client.Headers.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)");
                    html = client.DownloadString(url);
                }
            }
            catch (WebException e)
            {
                return null;
            }
            return html;
        }

        public string GetNewTPBUrl()
        {
            Log("get new tpb url - 77");

            string html = GetHTML(proxyBayUrl);
            string[] urlList = new string[] { };
            if (html != null)
            {
                Log("got proxybay - 83");
                try
                {
                    urlList = html.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).Skip(3).ToArray();
                    urlList = urlList.Take(urlList.Length - 1).ToArray();
                }
                catch
                {
                   // MessageBox.Show("Failed to parse proxybay list");
                }
            }
            else {
                html = GetHTML(googleCache);
                if (html != null)
                {
                    Log("got cache - 98");
                    try
                    {
                        urlList = html.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).Skip(5).ToArray();
                        urlList = urlList.Take(urlList.Length - 1).ToArray();
                    }
                    catch
                    {
                       // MessageBox.Show("Failed to parse google cache of proxybay list");
                    }
                }
            }
            string newURL = "";
            foreach (string urlItem in urlList)
            {
                Log("testing "+urlItem+" - 113");
                if (TestTPBURL(urlItem))
                {
                    Log("success - 116");
                    newURL = urlItem;
                    break;
                }
            }
            return newURL;
        }
        //public MatchCollection Scrapeold (string url)
        //{
        //    string html = GetHTML(url);
        //    if (html == null)
        //    {
        //        return null;
        //    }
        //    return Regex.Matches(html, @"(<div class=""detName"">.*?<a href=""(?<url>/torrent.*?)"".*?title="".*?>(?<title>.*?)<.*?(?<magnet>magnet.*?)"".*?Uploaded\s(?<date>.*?),.*?<td align=""right"">(?<seeders>.*?)</td>.*?<td align=""right"">(?<leechers>.*?)</td>)", RegexOptions.Singleline);
        //}
        public HtmlAgilityPack.HtmlNodeCollection Scrape(string url)
        {
            Log("get html - 134");
            string html = GetHTML(url);
            Log("finished - 136");
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            Log("load html - 138");
            doc.LoadHtml(html);
            Log("finished - 140");
            HtmlAgilityPack.HtmlNode n = doc.DocumentNode.SelectSingleNode("//table[@id='searchResult']");
            if (n != null)
            {
                HtmlAgilityPack.HtmlNodeCollection nc = n.SelectNodes("//tr[not(@class='header')]");
                return nc;
            }
            else
            {
                return null;
            }
        }
        public HtmlAgilityPack.HtmlNodeCollection Scrape(string url, out string logText)
        {
            debugLog = "";
            HtmlAgilityPack.HtmlNodeCollection ret = Scrape(url);
            logText = debugLog;
            return ret;
        }
        public string GetNewTPBUrl(out string logText)
        {
            debugLog = "";
            string ret = GetNewTPBUrl();
            logText = debugLog;
            return ret;
        }
        void Log(string txt)
        {
            TimeSpan elapsedTime = DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime;
            debugLog += elapsedTime.TotalSeconds.ToString() + "." + elapsedTime.Milliseconds.ToString();
            debugLog += txt + "\r\n";
        }
    }
}
