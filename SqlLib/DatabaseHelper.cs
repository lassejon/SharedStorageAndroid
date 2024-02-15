using System;
using Android.Content;
using Android.Database.Sqlite;

public class DatabaseHelper : SQLiteOpenHelper
{
    const string create_table_sql =
    "CREATE TABLE [" + package_names + "] ([id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE, [name] TEXT NOT NULL UNIQUE)";

    const string DATABASE_NAME = "shared_storage.db";
    const int DATABASE_VERSION = 1;

    public DatabaseHelper(Context context) : base(context, DATABASE_NAME, null, DATABASE_VERSION) { }

    public override void OnCreate(SQLiteDatabase db)
    {
        db.ExecSQL(create_table_sql);
        // seed with data
        db.ExecSQL("INSERT INTO vegetables (name) VALUES ('initializer')");
    }

    public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
    {
        throw new NotImplementedException();
    }
}
