using System.Net;

class MainClass
{
    private static HttpListener listener;
    private static string baseUrl = "http://localhost:6005/";

    private static void Main(string[] args)
    {
        Sqlite.InitializeDatabase();
        listener = new HttpListener();
        listener.Prefixes.Add(baseUrl);
        listener.Start();
        Task.Run(() => ListenForRequests());
        Console.ReadLine();
    }

    private static async Task ListenForRequests()
    {
        while (true)
        {
            await User.UserHandler(listener);
        }
    }
}