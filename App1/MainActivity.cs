using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Android.Widget;
using SharedStorage;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;
using Android.Content;
using Android.Database;

namespace App1
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            Button btnPushData = FindViewById<Button>(Resource.Id.btnPushData);
            btnPushData.Click += (sender, e) => OnPushDataClicked(sender, e);

            // Get the button reference and set its click event
            Button btnDisplayData = FindViewById<Button>(Resource.Id.btnDisplayData);
            btnDisplayData.Click += OnDisplaySharedStorageDataClicked;
        }

        public void OnPushDataClicked(object sender, EventArgs e)
        {
            // Get the current package name
            string packageName = Application.Context.PackageName;

            // Assuming you have a ContentValues object with the data to insert
            ContentValues values = new ContentValues();
            values.Put(Contracts.COLUMN_NAME, packageName);

            // Use ContentResolver to insert data using the content provider
            Android.Net.Uri insertedUri = ContentResolver.Insert(Contracts.CONTENT_URI_DATA, values);

            // Check if the insertion was successful
            if (insertedUri != null)
            {
                long insertedId = long.Parse(insertedUri.LastPathSegment);
                Toast.MakeText(this, "Data inserted with ID: " + insertedId, ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, "Failed to insert data", ToastLength.Short).Show();
            }
        }

        public void OnDisplaySharedStorageDataClicked(object sender, EventArgs e)
        {
            // Get all data from the "data" table using the content provider
            ICursor cursor = ContentResolver.Query(Contracts.CONTENT_URI_DATA, null, null, null, null);

            if (cursor != null)
            {
                try
                {
                    // Move the cursor to the first row
                    if (cursor.MoveToFirst())
                    {
                        do
                        {
                            // Access the values in the cursor
                            int id = cursor.GetInt(cursor.GetColumnIndexOrThrow(Contracts.COLUMN_ID));
                            string name = cursor.GetString(cursor.GetColumnIndexOrThrow(Contracts.COLUMN_NAME));

                            // Print to console log
                            Console.WriteLine($"ID: {id}, Name: {name}");
                        } while (cursor.MoveToNext());
                    }
                }
                finally
                {
                    // Always close the cursor to avoid memory leaks
                    cursor.Close();
                }
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}
}
