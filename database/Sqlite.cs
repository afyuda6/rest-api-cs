﻿using System.Data.SQLite;

public static class Sqlite
{
    private static readonly string _connectionString = "Data Source=rest_api_cs.db;Version=3;";
    
    public static SQLiteConnection Connect()
    {
        var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    public static void InitializeDatabase()
    {
        using (var connection = Connect())
        {
            var dropUsersTableSql = new SQLiteCommand("DROP TABLE IF EXISTS users;", connection);
            dropUsersTableSql.ExecuteNonQuery();
            var createUsersTableSql = new SQLiteCommand("CREATE TABLE IF NOT EXISTS users (id INTEGER PRIMARY KEY, name TEXT NOT NULL);", connection);
            createUsersTableSql.ExecuteNonQuery();
        }
    }
}