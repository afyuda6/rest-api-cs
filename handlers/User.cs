using System.Data.SQLite;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Http;

public static class User
{ 
    private static async Task HandleReadUsers(HttpResponse response)
    {
        var users = new List<object>();
        await using (var connection = Sqlite.Connect())
        {
            var command = new SQLiteCommand("SELECT id, name FROM users;", connection);
            await using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    users.Add(new
                    {
                        id = reader["id"],
                        name = reader["name"]
                    });
                }
            }
        }
        response.StatusCode = (int)HttpStatusCode.OK;
        response.ContentType = "application/json";
        var responseBody = new
        {
            status = "OK",
            code = 200,
            data = users
        };
        var jsonResponse = JsonSerializer.Serialize(responseBody);
        var buffer = Encoding.UTF8.GetBytes(jsonResponse);
        response.ContentLength = buffer.Length;
        await response.Body.WriteAsync(buffer, 0, buffer.Length);
    }

    private static async Task HandleCreateUser(HttpRequest request, HttpResponse response)
    {
        var requestBody = await new StreamReader(request.Body, Encoding.UTF8).ReadToEndAsync();
        var userData = HttpUtility.ParseQueryString(requestBody);
        if (string.IsNullOrWhiteSpace(userData["name"]))
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            response.ContentType = "application/json";
            var errorResponse = new
            {
                status = "Bad Request",
                code = 400,
                errors = "Missing 'name' parameter"
            };
            var jsonResponse = JsonSerializer.Serialize(errorResponse);
            var buffer = Encoding.UTF8.GetBytes(jsonResponse);
            response.ContentLength = buffer.Length;
            await response.Body.WriteAsync(buffer, 0, buffer.Length);
        }
        else
        {
            await using (var connection = Sqlite.Connect())
            {
                var command = new SQLiteCommand("INSERT INTO users (name) VALUES (@Name);", connection);
                command.Parameters.AddWithValue("@Name", userData["name"]);
                command.ExecuteNonQuery();
            }
            response.StatusCode = (int)HttpStatusCode.Created;
            response.ContentType = "application/json";
            var responseBody = new
            {
                status = "Created",
                code = 201
            };
            var jsonResponse = JsonSerializer.Serialize(responseBody);
            var buffer = Encoding.UTF8.GetBytes(jsonResponse);
            response.ContentLength = buffer.Length;
            await response.Body.WriteAsync(buffer, 0, buffer.Length);
        }
    }

    private static async Task HandleUpdateUser(HttpRequest request, HttpResponse response)
    {
        var requestBody = await new StreamReader(request.Body, Encoding.UTF8).ReadToEndAsync();
        var userData = HttpUtility.ParseQueryString(requestBody);
        if (string.IsNullOrWhiteSpace(userData["name"]) || string.IsNullOrWhiteSpace(userData["id"]))
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            response.ContentType = "application/json";
            var errorResponse = new
            {
                status = "Bad Request",
                code = 400,
                errors = "Missing 'id' or 'name' parameter"
            };
            var jsonResponse = JsonSerializer.Serialize(errorResponse);
            var buffer = Encoding.UTF8.GetBytes(jsonResponse);
            response.ContentLength = buffer.Length;
            await response.Body.WriteAsync(buffer, 0, buffer.Length);
        }
        else
        {
            await using (var connection = Sqlite.Connect())
            {
                var command = new SQLiteCommand("UPDATE users SET name = @Name WHERE id = @Id;", connection);
                command.Parameters.AddWithValue("@Name", userData["name"]);
                command.Parameters.AddWithValue("@Id", userData["id"]);
                command.ExecuteNonQuery();
            }
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = "application/json";
            var responseBody = new
            {
                status = "OK",
                code = 200
            };
            var jsonResponse = JsonSerializer.Serialize(responseBody);
            var buffer = Encoding.UTF8.GetBytes(jsonResponse);
            response.ContentLength = buffer.Length;
            await response.Body.WriteAsync(buffer, 0, buffer.Length);
        }
    }

    private static async Task HandleDeleteUser(HttpRequest request, HttpResponse response)
    {
        var requestBody = await new StreamReader(request.Body, Encoding.UTF8).ReadToEndAsync();
        var userData = HttpUtility.ParseQueryString(requestBody);
        if (string.IsNullOrWhiteSpace(userData["id"]))
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            response.ContentType = "application/json";
            var errorResponse = new
            {
                status = "Bad Request",
                code = 400,
                errors = "Missing 'id' parameter"
            };
            var jsonResponse = JsonSerializer.Serialize(errorResponse);
            var buffer = Encoding.UTF8.GetBytes(jsonResponse);
            response.ContentLength = buffer.Length;
            await response.Body.WriteAsync(buffer, 0, buffer.Length);
        }
        else
        {
            await using (var connection = Sqlite.Connect())
            {
                var command = new SQLiteCommand("DELETE FROM users WHERE id = @Id;", connection);
                command.Parameters.AddWithValue("@Id", userData["id"]);
                command.ExecuteNonQuery();
            }
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = "application/json";
            var responseBody = new
            {
                status = "OK",
                code = 200
            };
            var jsonResponse = JsonSerializer.Serialize(responseBody);
            var buffer = Encoding.UTF8.GetBytes(jsonResponse);
            response.ContentLength = buffer.Length;
            await response.Body.WriteAsync(buffer, 0, buffer.Length);
        }
    }
    
    public static async Task UserHandler(HttpContext context)
    {
        var request = context.Request;
        var response = context.Response;
        if (request.Path.Equals("/users") || request.Path.Equals("/users/"))
        {
            switch (request.Method.ToUpper())
            {
                case "GET":
                    await HandleReadUsers(response);
                    break;
                case "POST":
                    await HandleCreateUser(request, response);
                    break;
                case "PUT":
                    await HandleUpdateUser(request, response);
                    break;
                case "DELETE":
                    await HandleDeleteUser(request, response);
                    break;
                case "OPTIONS":
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.ContentType = "application/json";
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                    response.ContentType = "application/json";
                    var errorResponse = new
                    {
                        status = "Method Not Allowed",
                        code = 405
                    };
                    var jsonResponse = JsonSerializer.Serialize(errorResponse);
                    var buffer = Encoding.UTF8.GetBytes(jsonResponse);
                    response.ContentLength = buffer.Length;
                    await response.Body.WriteAsync(buffer, 0, buffer.Length);
                    break;
            }
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
            var jsonResponse = JsonSerializer.Serialize(errorResponse);
            var buffer = Encoding.UTF8.GetBytes(jsonResponse);
            response.ContentLength = buffer.Length;
            await response.Body.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}