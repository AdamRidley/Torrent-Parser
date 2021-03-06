﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Torrent_Parser
{
    public partial class SearchForm : Form
    {
        TPBScraper scraper = new TPBScraper();
        public CConfigMng oConfigMng = new CConfigMng();
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
                if (lastTPBUrl == null || !scraper.TestTPBURL(lastTPBUrl))
                {
                    string newURL = scraper.GetNewTPBUrl();

                    if (newURL != "")
                    {
                        oConfigMng.Config.LastTPBUrl = newURL;
                        lastTPBUrl = newURL;
                        oConfigMng.SaveConfig();
                    }
                }
            }
            else
            {
                lastTPBUrl = scraper.GetNewTPBUrl();
                if (lastTPBUrl == null)
                {
                    MessageBox.Show("Could not find a proxy site sorry!\nThis app will now close");
                    Application.Exit();
                }
            }
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
                HtmlAgilityPack.HtmlNodeCollection nc = scraper.Scrape(lastTPBUrl + searchUrl);
                if (nc != null)
                {
                    foreach(HtmlAgilityPack.HtmlNode node in nc)
                    {
                        int rowId = dataGridView.Rows.Add();
                        DataGridViewRow row = dataGridView.Rows[rowId];
                        row.Cells["TitleColumn"].Value = node.SelectSingleNode(".//a[@class='detLink']").InnerText;
                        row.Cells["SeedersColumn"].Value = node.SelectSingleNode(".//td[3]").InnerText;
                        row.Cells["LeechersColumn"].Value = node.SelectSingleNode(".//td[4]").InnerText;
                        string dateText = node.SelectSingleNode(".//td[2]").SelectSingleNode(".//font").InnerText;
                        Match match = Regex.Match(dateText, @"\b\w+\s(?<date>.*?),.*?");
                        DateTime tmpDate = GetDate(match.Groups["date"].Value);
                        row.Cells["UploadedColumn"].Value = tmpDate.Date;
                        row.Cells["MagnetColumn"].Value =node.SelectSingleNode(".//a[contains(@href,'magnet:')]").Attributes["href"].Value;
                        row.Cells["URLColumn"].Value = lastTPBUrl + node.SelectSingleNode(".//a[@class='detLink']").Attributes["href"].Value;
                    }
                    
                }
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
                    switch (oConfigMng.Config.RemoteType)
                    {
                        case "qBittorrent":
                            RemoteClient.SendQBittorrent(magnet, rootURL, username, password);
                            break;
                        case "Transmission":
                            RemoteClient.SendTransmission(magnet, rootURL, username, password,"");
                            break;
                        default:
                            break;
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
            else if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            {
                copyMagnet();
                e.Handled = true;
            }
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyMagnet();
        }
        private void copyMagnet()
        {
            Clipboard.SetText(dataGridView.SelectedCells[0].OwningRow.Cells["MagnetColumn"].Value.ToString());
        }
    }
}

