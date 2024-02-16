using Android.Content;
using Android.Database;
using Android.Database.Sqlite;
using Android.Util;
using System;
using Exception = System.Exception;

namespace SharedStorage
{

    [ContentProvider(new string[] { Contracts.AUTHORITY }, Exported = true, Name = Contracts.AUTHORITY , Enabled = false)]
    public class SharedContentProvider : ContentProvider
    {
        public const int GET_ALL = 0; 
        public const int GET_ONE = 1; 
        public static UriMatcher uriMatcher = BuildUriMatcher();

        public const string PACKAGE_NAMES_MIME_TYPE = ContentResolver.CursorDirBaseType + "/vnd.com.contentproviders.Shared";
        public const string PACKAGE_NAME_MIME_TYPE = ContentResolver.CursorItemBaseType + "/vnd.com.contentproviders.Shared";

        static UriMatcher BuildUriMatcher()
        {
            var matcher = new UriMatcher(UriMatcher.NoMatch);

            // Uris to match, and the code to return when matched 
            matcher.AddURI(Contracts.AUTHORITY, Contracts.PATH_DATA, GET_ALL);
            matcher.AddURI(Contracts.AUTHORITY, Contracts.PATH_DATA + "/#", GET_ONE);

            return matcher;
        }

        public DatabaseHelper dbHelper { get; private set; }

        public override int Delete(Android.Net.Uri uri, string selection, string[] selectionArgs)
        {
            throw new NotImplementedException();
        }

        public override string GetType(Android.Net.Uri uri)
        {
            switch (uriMatcher.Match(uri))
            {
                case GET_ALL:
                    return PACKAGE_NAMES_MIME_TYPE;
                case GET_ONE:
                    return PACKAGE_NAME_MIME_TYPE;
                default:
                    throw new Java.Lang.IllegalArgumentException("Unknown Uri: " + uri);
            }
        }

        public override Android.Net.Uri Insert(Android.Net.Uri uri, ContentValues values)
        {
            SQLiteDatabase db = dbHelper.WritableDatabase;
            long id;

            id = db.Insert(Contracts.PATH_DATA, null, values);

            Context.ContentResolver.NotifyChange(uri, null);
            return Android.Net.Uri.WithAppendedPath(uri, id.ToString());
        }

        public override bool OnCreate()
        {
            dbHelper = new DatabaseHelper(Context);

            Log.Debug("SharedContentProvider", "DatabaseHelper initialized");

            return true;
        }

        public override ICursor Query(Android.Net.Uri uri, string[] projection, string selection, string[] selectionArgs, string sortOrder)
        {
            switch (uriMatcher.Match(uri))
            {
                case GET_ALL:
                    return GetFromDatabase();
                case GET_ONE:
                    var id = uri.LastPathSegment;
                    return GetFromDatabase(id); // the ID is the last part of the Uri
                default:
                    throw new Java.Lang.IllegalArgumentException("Unknown Uri: " + uri);
            }
        }

        public override int Update(Android.Net.Uri uri, ContentValues values, string selection, string[] selectionArgs)
        {
            throw new NotImplementedException();
        }

        ICursor GetFromDatabase()
        {
            return dbHelper.ReadableDatabase.RawQuery("SELECT id, name FROM " + Contracts.PATH_DATA, null);
        }
        ICursor GetFromDatabase(string id)
        {
            return dbHelper.ReadableDatabase.RawQuery("SELECT id, name FROM " + Contracts.PATH_DATA + " WHERE id = " + id, null);
        }

        private bool IsProviderInstalled(Context context)
        {
            // Check if the flag indicating provider installation is present in shared preferences
            ISharedPreferences prefs = context.GetSharedPreferences("ProviderPrefs", FileCreationMode.Private);
            return prefs.GetBoolean("IsProviderInstalled", false);
        }

        private void SetProviderInstalledFlag(Context context)
        {
            // Set the flag indicating provider installation in shared preferences
            ISharedPreferences prefs = context.GetSharedPreferences("ProviderPrefs", FileCreationMode.Private);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("IsProviderInstalled", true);
            editor.Apply();
        }

        private bool IsContentProviderInstalled()
        {
            try
            {
                // Try to query the content provider
                var cursor = Context.ContentResolver.Query(Contracts.BASE_CONTENT_URI, null, null, null, null);
                return cursor != null;
            }
            catch (Exception)
            {
                // An exception indicates that the content provider is not installed
                return false;
            }
        }
    }
}