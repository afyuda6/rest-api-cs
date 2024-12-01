using System;
using System.Net;
using System.Threading.Tasks;

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
            var request = context.Request;
            var response = context.Response;

            try
            {
                if (request.Url.AbsolutePath.Equals("/users") || request.Url.AbsolutePath.Equals("/users/"))
                {
                    await User.UserHandle(request, response);
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.ContentType = "application/json";
                    var errorResponse = new
                    {
                        status = "Not Found",
                        code = 404
                    };
                    var jsonResponse = System.Text.Json.JsonSerializer.Serialize(errorResponse);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(jsonResponse);
                    response.ContentLength64 = buffer.Length;
                    await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                    response.OutputStream.Close();
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ContentType = "application/json";
                var errorResponse = new
                {
                    status = "Internal Server Error",
                    code = 500
                };
                var jsonResponse = System.Text.Json.JsonSerializer.Serialize(errorResponse);
                var buffer = System.Text.Encoding.UTF8.GetBytes(jsonResponse);
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
        }
    }
}