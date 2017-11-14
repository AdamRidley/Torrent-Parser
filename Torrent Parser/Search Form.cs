﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Torrent_Parser
{
    public partial class SearchForm : Form
    {
        public CConfigMng oConfigMng = new CConfigMng();
        string proxyBayUrl = @"https://proxybay.one/list.txt";
        string googleCache = @"http://webcache.googleusercontent.com/search?q=cache%3Awww.proxybay.one%2Flist.txt";
        string searchAppendage = @"/search/$s/0/7/0";
        string lastTPBUrl;

        public SearchForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView.Columns["UploadedColumn"].DefaultCellStyle.Format = "dd/MM/yy";
            //MessageBox.Show(GetNewTPBUrl());
            oConfigMng.LoadConfig();
            if (oConfigMng.Config.LastTPBUrl != "")
            {
                lastTPBUrl = oConfigMng.Config.LastTPBUrl;
                if (lastTPBUrl == null || !TestTPBURL(lastTPBUrl))
                {
                    lastTPBUrl = GetNewTPBUrl();
                }
            }
            else
            {
                lastTPBUrl = GetNewTPBUrl();
                if (lastTPBUrl == null)
                {
                    MessageBox.Show("Could not find a proxy site sorry!\nThis app will now close");
                    Application.Exit();
                }
            }
        }

        private bool TestTPBURL(string TPBUrl)
        {
            string html = GetHTML(TPBUrl);
            if (html == null) return false;
            MatchCollection mc = Regex.Matches(html, @"<title>.*The Pirate Bay.*<\/title>");
            bool hasMatch = false;
            foreach (Match m in mc)
            {
                hasMatch = true;
                //Console.WriteLine(m);
            }
            return hasMatch;
        }

        private string GetHTML(string url)
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
            catch (WebException e)
            {
                return null;
                
            }

            //Console.WriteLine(html);
            return html;
        }

        private string GetNewTPBUrl()
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
            if (newURL != "")
            {
                oConfigMng.Config.LastTPBUrl = newURL;
                oConfigMng.SaveConfig();
            }
            else if (oConfigMng.Config.LastTPBUrl != "")
            {
                newURL = oConfigMng.Config.LastTPBUrl;
            }
            else return null;
            return newURL;
        }

        private void searchBut_Click(object sender, EventArgs e)
        {
            dataGridView.Rows.Clear();
            dataGridView.Refresh();
            string searchText = searchTextBox.Text.Trim().Replace(" ", "+");
            Console.WriteLine(searchText);
            if (searchText != "")
            {
                string searchUrl = searchAppendage.Replace("$s", Uri.EscapeUriString(searchText));
                Console.WriteLine(searchUrl);
                string html = GetHTML(lastTPBUrl + searchUrl);
                MatchCollection mc = Regex.Matches(html, @"(<div class=""detName"">.*?<a href=""(?<url>/torrent.*?)"".*?title="".*?>(?<title>.*?)<.*?(?<magnet>magnet.*?)"".*?Uploaded\s(?<date>.*?),.*?<td align=""right"">(?<seeders>.*?)</td>.*?<td align=""right"">(?<leechers>.*?)</td>)", RegexOptions.Singleline);
                foreach (Match match in mc)
                {
                    int rowId = dataGridView.Rows.Add();
                    DataGridViewRow row = dataGridView.Rows[rowId];
                    row.Cells["TitleColumn"].Value = match.Groups["title"].Value;
                    row.Cells["SeedersColumn"].Value = Convert.ToInt32(match.Groups["seeders"].Value);
                    row.Cells["LeechersColumn"].Value = Convert.ToInt32(match.Groups["leechers"].Value);
                    DateTime tmpDate = GetDate(match.Groups["date"].Value);
                    row.Cells["UploadedColumn"].Value = tmpDate.Date;
                    row.Cells["MagnetColumn"].Value = match.Groups["magnet"].Value;
                    row.Cells["URLColumn"].Value = lastTPBUrl + match.Groups["url"].Value;
                }
                Console.WriteLine("");
            }

        }

        private static DateTime GetDate(string tmp)
        {
            tmp = Regex.Replace(tmp, @"&nbsp;|\xA0", " ");
            DateTime tmpDate;
            try
            {
                if (Regex.IsMatch(tmp, "[0-6]?[0-9] mins ago", RegexOptions.IgnoreCase))
                {
                    tmpDate = DateTime.Now;
                    Match m = Regex.Match(tmp, "(?<mins>[0-6]?[0-9]) mins ago", RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        tmpDate = tmpDate.AddMinutes(-Convert.ToInt32(m.Groups["mins"]));
                    }
                }
                else if (tmp.Contains("Today"))
                {
                    tmpDate = DateTime.Today;
                }
                else if (tmp.Contains("Y-day"))
                {
                    tmp = tmp.Replace("Y-day ", "");
                    tmpDate = DateTime.ParseExact(DateTime.Today.AddDays(-1).ToString("MM-dd") + " " + tmp, "MM-dd HH:mm", null);
                }
                else if (tmp.Contains(":"))
                {
                    tmpDate = DateTime.ParseExact(tmp, "MM-dd HH:mm", null);
                }
                else
                {
                    tmp = tmp.Replace(" ", "-");
                    tmpDate = DateTime.ParseExact(tmp, "MM-dd-yyyy", null);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Date error occured, try to find how to reproduce it and report it!");
                tmpDate = DateTime.MinValue;
            }

            return tmpDate;
        }

        private void dataGridView_MouseDown(object sender, MouseEventArgs e)
        {
            {
                if (e.Button == MouseButtons.Right)
                {
                    var hti = dataGridView.HitTest(e.X, e.Y);
                    dataGridView.ClearSelection();
                    if (hti.RowIndex != -1)
                    {
                        dataGridView.Rows[hti.RowIndex].Selected = true;
                    }
                }
            }
        }

        private void openTPBBut_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedCells.Count > 0)
            {
                System.Diagnostics.Process.Start(dataGridView.SelectedCells[0].OwningRow.Cells["URLColumn"].Value.ToString());
            }
        }

        private void openMagnetBut_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedCells.Count > 0)
            {
                if (!oConfigMng.Config.RemoteEnabled)
                {
                    System.Diagnostics.Process.Start(dataGridView.SelectedCells[0].OwningRow.Cells["MagnetColumn"].Value.ToString());
                }
                else
                {

                    //This is for qBittorrent, for transmission see https://github.com/bogenpirat/remote-torrent-adder/blob/master/webuiapis/TransmissionWebUI.js
                    string magnet = dataGridView.SelectedCells[0].OwningRow.Cells["MagnetColumn"].Value.ToString();
                    string rootURL = "http://" + oConfigMng.Config.RemoteServer + ":" + oConfigMng.Config.RemotePort.ToString();
                    string username = oConfigMng.Config.RemoteUsername;
                    string password = oConfigMng.Config.RemotePassword;
                    string formParams = "username=" + Uri.EscapeDataString(username) + "&password=" + Uri.EscapeDataString(password);
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(rootURL + "/login");
                    httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
                    httpWebRequest.Method = "POST";
                    byte[] bytes = Encoding.ASCII.GetBytes(formParams);
                    httpWebRequest.ContentLength = bytes.Length;
                    httpWebRequest.Timeout = 2000;
                    HttpWebResponse resp=null;
                    try
                    {
                        using (Stream os = httpWebRequest.GetRequestStream())
                        {
                            os.Write(bytes, 0, bytes.Length);
                        }
                    }
                    catch (System.Net.WebException ex)
                    {
                        if (ex.Status == System.Net.WebExceptionStatus.Timeout)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        else{
                            throw ;
                        }
                    }

                    using (resp = (HttpWebResponse)httpWebRequest.GetResponse()) { 
                        if (resp == null || resp.StatusCode == HttpStatusCode.OK)
                        {

                            string cookieHeader = "";

                            if (resp != null)
                            {
                                cookieHeader = resp.Headers["Set-cookie"];
                            }


                            httpWebRequest = (HttpWebRequest)WebRequest.Create(rootURL + "/command/download");
                            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                            httpWebRequest.Method = "POST";
                            httpWebRequest.Headers.Add("Cookie", cookieHeader);
                            string message = "urls=" + Uri.EscapeUriString(magnet);
                            byte[] bytes2 = Encoding.ASCII.GetBytes(message);
                            httpWebRequest.ContentLength = bytes2.Length;
                            using (Stream os = httpWebRequest.GetRequestStream())
                            {
                                os.Write(bytes2, 0, bytes2.Length);
                                os.Flush();
                                os.Close();
                            }
                            using (HttpWebResponse resp2 = (HttpWebResponse)httpWebRequest.GetResponse())
                            {
                                if (resp2.StatusCode == HttpStatusCode.OK)
                                {
                                    MessageBox.Show("Torrent Sent Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Send Failed:\nRecieved Code: " + resp.StatusCode + " " + resp.StatusDescription, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Login Failed:\nRecieved Code: " + resp.StatusCode + " " + resp.StatusDescription, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void openOnTPBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openTPBBut_Click(sender, e);
        }

        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            openMagnetBut_Click(sender, e);
        }

        private void settingsBut_Click(object sender, EventArgs e)
        {
            SettingsForm newForm = new SettingsForm();
            newForm.Owner = this;
            DialogResult res = newForm.ShowDialog();
        }

        private void searchTextBox_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                searchBut.PerformClick();
            }

        }

        private void dataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                openMagnetBut.PerformClick();
            }
        }
    }
}
