using System.Net;

class MainClass
{
    private static HttpListener _listener;
    private static string _baseUrl = "http://localhost:6005/";

    private static void Main(string[] args)
    {
        Sqlite.InitializeDatabase();
        _listener = new HttpListener();
        _listener.Prefixes.Add(_baseUrl);
        _listener.Start();
        Task.Run(() => ListenForRequests());

        Console.ReadLine();
    }

    private static async Task ListenForRequests()
    {
        while (true)
        {
            var context = await _listener.GetContextAsync();
            await User.UserHandler(context.Request, context.Response);
        }
    }
}