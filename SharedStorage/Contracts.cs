using System;

namespace SharedStorage
{
    public  class Contracts
    {
        public const string AUTHORITY = "com.contentproviders.SharedContentProvider";

        public static readonly Android.Net.Uri BASE_CONTENT_URI = Android.Net.Uri.Parse($"content://{AUTHORITY}");


        public const string PATH_DATA = "package_names";

        // Content URI for accessing data
        public static readonly Android.Net.Uri CONTENT_URI_DATA = BASE_CONTENT_URI.BuildUpon().AppendPath(PATH_DATA).Build();

        // Column names for the data table
        public const string COLUMN_ID = "id";
        public const string COLUMN_NAME = "name";
    }
}
