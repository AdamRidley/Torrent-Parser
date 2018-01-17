using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Torrent_Parser
{
    static class RemoteClient
    {
        public static void SendQBittorrent(string magnet, string rootURL, string username, string password)
        {
            string formParams = "username=" + Uri.EscapeDataString(username) + "&password=" + Uri.EscapeDataString(password);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(rootURL + "/login");
            httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            httpWebRequest.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(formParams);
            httpWebRequest.ContentLength = bytes.Length;
            httpWebRequest.Timeout = 2000;
            HttpWebResponse resp = null;
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
                else
                {
                    throw;
                }
            }

            using (resp = (HttpWebResponse)httpWebRequest.GetResponse())
            {
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
                            MessageBox.Show("Send Failed:\nRecieved Code: " + resp2.StatusCode + " " + resp2.StatusDescription, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Login Failed:\nRecieved Code: " + resp.StatusCode + " " + resp.StatusDescription, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static void SendTransmission(string magnet, string rootURL, string username, string password, string idheader)
        {
            HttpWebRequest httpWebRequest;
            httpWebRequest = (HttpWebRequest)WebRequest.Create(rootURL + "/transmission/rpc");
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "POST";
            httpWebRequest.Credentials = new NetworkCredential(username, password);
            httpWebRequest.Headers.Add("X-Transmission-Session-Id", idheader);
            string message = Newtonsoft.Json.JsonConvert.SerializeObject(new { method= "torrent-add", arguments= new { paused= "false", filename= Uri.EscapeUriString(magnet)}});
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            httpWebRequest.ContentLength = bytes.Length;
            using (Stream os = httpWebRequest.GetRequestStream())
            {
                os.Write(bytes, 0, bytes.Length);
                os.Flush();
                os.Close();
            }
            try
            {
                using (HttpWebResponse resp = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    if (resp.StatusCode == HttpStatusCode.OK)
                    {
                        MessageBox.Show("Torrent Sent Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Send Failed:\nRecieved Code: " + resp.StatusCode + " " + resp.StatusDescription, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (WebException e)
            {
                HttpWebResponse resp = (HttpWebResponse)e.Response;
                if (resp.StatusCode == HttpStatusCode.Conflict) {
                    string id= resp.GetResponseHeader("X-Transmission-Session-Id");
                    SendTransmission(magnet, rootURL, username, password, id);
                }
                else
                {
                    throw e;
                }
            }
            
        }
    }
}
