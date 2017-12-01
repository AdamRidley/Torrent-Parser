using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Android.Views.InputMethods;

namespace AR.TorrentFinder
{
    [Activity(Label = "Torrent Finder", MainLauncher = true, Icon = "@mipmap/ic_launcher",RoundIcon ="@mipmap/ic_round_launcher")]
    public class MainActivity : Activity
    {
        string lastTPBUrl;
        List<SearchResult> searchResults;
        DataTable dataTable;
        Torrent_Parser.CConfigMng oConfigMng = new Torrent_Parser.CConfigMng();
        Torrent_Parser.TPBScraper scraper = new Torrent_Parser.TPBScraper();

        protected override void OnCreate(Bundle bundle)
        {
            Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);
            
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //Get the textbox and attach edit event to it
            EditText searchText = FindViewById<EditText>(Resource.Id.searchText);
            searchText.EditorAction += SearchText_EditorAction;

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.searchButton);
            button.Click += delegate { Search(searchText.Text); };

            //Get scrollview and add dataTable to it
            ScrollView scrollView = FindViewById<ScrollView>(Resource.Id.scrollView1);
            dataTable = new DataTable(scrollView.Context,this);
            TableLayout.LayoutParams layoutParameters = new TableLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            dataTable.LayoutParameters = layoutParameters;
            dataTable.SetColumnStretchable(0, true);
            dataTable.SetColumnShrinkable(0, true);
            //dataTable.ShrinkAllColumns = true;
            dataTable.SetColumnCollapsed(4, true);
            dataTable.SetColumnCollapsed(5, true);
            scrollView.AddView(dataTable);
            oConfigMng.LoadConfig();

            if (bundle is null || bundle.GetString("tpbUrl") is null)
            {
                //If opening for first time
                //Config Loading Code & Find working TPB Proxy
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
                        Message("Could not find a proxy site sorry!\nThis app will now close");
                        //MessageBox.Show("Could not find a proxy site sorry!\nThis app will now close");
                        (Xamarin.Forms.Forms.Context as Activity).Finish();
                    }
                }
            }
            else
            {
                //If resuming from rotation
                lastTPBUrl = bundle.GetString("tpbUrl");
            }
            if (bundle == null || bundle.GetString("searchResults") == null)
            {
                //If first time running
                searchText.RequestFocus();
            }
            else
            {
                //If resuming from rotation
                searchResults = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SearchResult>>(bundle.GetString("searchResults"));
            }
            UpdateTable();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString("tpbUrl", lastTPBUrl);
            outState.PutString("searchResults", Newtonsoft.Json.JsonConvert.SerializeObject(searchResults));
        }

        void SearchText_EditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == Android.Views.InputMethods.ImeAction.Search)
            {
                Search(((EditText)sender).Text);
                e.Handled = true;
            }
        }
        private void DismissKeyboard()
        {
            var view = CurrentFocus;
            if (view != null)
            {
                var imm = (InputMethodManager)GetSystemService(InputMethodService);
                imm.HideSoftInputFromWindow(view.WindowToken, 0);
            }
        }

        void Search(string searchString)
        {
            DismissKeyboard();
            FindViewById<LinearLayout>(Resource.Id.linearLayout1).RequestFocus();
            //Get Search result and Parse Data
            string searchAppendage = @"/search/$s/0/7/0";
            searchString = searchString.Trim().Replace(" ", "+");
            //Message($"Search Clicked Text:{searchString}");

            if (searchString != "")
            {
                string searchUrl = searchAppendage.Replace("$s", Uri.EscapeUriString(searchString));
                MatchCollection mc = scraper.Scrape(lastTPBUrl + searchUrl);
                if (mc != null)
                {
                    searchResults = new List<SearchResult>();
                    foreach (Match match in mc)
                    {
                        searchResults.Add(new SearchResult(match.Groups["title"].Value, Convert.ToInt32(match.Groups["seeders"].Value), Convert.ToInt32(match.Groups["leechers"].Value), GetDate(match.Groups["date"].Value), match.Groups["magnet"].Value, lastTPBUrl + match.Groups["url"].Value));
                    }
                }
                UpdateTable();
            }
        }
        void UpdateTable()
        {
            dataTable.ResetTable();
            if (searchResults is null) return;
            foreach (SearchResult sr in searchResults)
            {
                dataTable.AddRow(new String[] { sr.Title, sr.Seeders.ToString(), sr.Leechers.ToString(), sr.Uploaded.Date.ToString("dd/MM/yy"), sr.Magnet, sr.Url });
            }

        }
        public void Message(string msg)
        {
            (Toast.MakeText(Application.Context, msg, ToastLength.Short)).Show();
        }
        private DateTime GetDate(string tmp)
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
                Message("Date error occured, try to find how to reproduce it and report it!");
                tmpDate = DateTime.MinValue;
            }

            return tmpDate;
        }
    }
    public class DataTable:TableLayout
    {
        String[] headers = { "Title", "S", "L", "Uploaded", "Magnet","URL"};
        Activity activity;
        public DataTable(Context context, Activity _activity) : base(context)
        {
            activity = _activity;
            ResetTable();
        }
        public void ResetTable()
        {
            this.RemoveAllViews();
            AddRow(headers);
        }
        public void AddRow(string[] arr)
        {
            TableRow row = new TableRow(this.Context);
            if (arr.Length != headers.Length) return;
            foreach(string a in arr)
            {
                TextView textView = new TextView(this.Context);
                textView.SetTextAppearance(Resource.Style.TextAppearance_AppCompat_Medium);
                textView.SetPadding(5, 0, 5, 0);
                if (row.ChildCount == 0)
                {
                    textView.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent,1);
                }
            else
                {
                    //textView.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.MatchParent, 0);
                }
                textView.Text = a;
                row.AddView(textView);
            }
            if (this.ChildCount > 0)
            {
                row.Clickable = true;
                //row click control
                row.Touch += Row_Touch;
            }
            this.AddView(row);
        }

        private void Row_Touch(object sender, View.TouchEventArgs args)
        {
            TableRow tr = (TableRow)sender;
            switch (args.Event.Action)
            {
                case MotionEventActions.Cancel:
                    tr.SetBackgroundColor(Android.Graphics.Color.Transparent);
                    break;
                case MotionEventActions.Down:
                    Android.Graphics.Color color = Android.Graphics.Color.White;
                    color.A = 50;
                    tr.SetBackgroundColor(color);
                    break;
                case MotionEventActions.Up:
                    tr.SetBackgroundColor(Android.Graphics.Color.Transparent);
                    string magnet = ((TextView)(tr.GetChildAt(4))).Text;
                    try
                    {
                        Context.StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse(magnet)));
                    }
                    catch (Android.Content.ActivityNotFoundException)
                    {
                        (Toast.MakeText(Application.Context, "Could not find app to open magnet link", ToastLength.Short)).Show();
                    }
                    catch (Exception)
                    {
                        (Toast.MakeText(Application.Context, "Unkown error when opening magnet link", ToastLength.Short)).Show();

                    }
                    break;
            }
        }
    }
    public class SearchResult
    {
        public SearchResult(string title, int seeders, int leechers, DateTime uploaded, string magnet, string url)
        {
            this.Title = title;
            this.Seeders = seeders;
            this.Leechers = leechers;
            this.Uploaded = uploaded;
            this.Magnet = magnet;
            this.Url = url;
        }

        public string Title { set; get; }
        public int Seeders { set; get; }
        public int Leechers { set; get; }
        public DateTime Uploaded { set; get; }
        public string Magnet { set; get; }
        public string Url{ set; get; }
    }
}

