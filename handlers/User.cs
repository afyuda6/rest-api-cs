using System.Data.SQLite;
using System.Net;
using System.Text;
using System.Web;

public static class User
{
    public static async Task UserHandle(HttpListenerRequest request, HttpListenerResponse response)
    {
        switch (request.HttpMethod)
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
            default:
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                response.ContentType = "application/json";
                var errorResponse = new
                {
                    status = "Method Not Allowed",
                    code = 405
                };
                var jsonResponse = System.Text.Json.JsonSerializer.Serialize(errorResponse);
                var buffer = Encoding.UTF8.GetBytes(jsonResponse);
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                response.OutputStream.Close();
                break;
        }
    }

    private static async Task HandleReadUsers(HttpListenerResponse response)
    {
        var users = new List<object>();
        using (var connection = Sqlite.Connect())
        {
            var command = new SQLiteCommand("SELECT id, name FROM users;", connection);
            using (var reader = command.ExecuteReader())
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
        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(responseBody);
        var buffer = Encoding.UTF8.GetBytes(jsonResponse);
        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }

    private static async Task HandleCreateUser(HttpListenerRequest request, HttpListenerResponse response)
    {
        string requestBody = await new StreamReader(request.InputStream).ReadToEndAsync();
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
            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(errorResponse);
            var buffer = Encoding.UTF8.GetBytes(jsonResponse);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
        else
        {
            using (var connection = Sqlite.Connect())
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
            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(responseBody);
            var buffer = Encoding.UTF8.GetBytes(jsonResponse);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
    }

    private static async Task HandleUpdateUser(HttpListenerRequest request, HttpListenerResponse response)
    {
        string requestBody = await new StreamReader(request.InputStream).ReadToEndAsync();
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
            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(errorResponse);
            var buffer = Encoding.UTF8.GetBytes(jsonResponse);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
        else
        {
            using (var connection = Sqlite.Connect())
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
            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(responseBody);
            var buffer = Encoding.UTF8.GetBytes(jsonResponse);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
    }

    private static async Task HandleDeleteUser(HttpListenerRequest request, HttpListenerResponse response)
    {
        string requestBody = await new StreamReader(request.InputStream).ReadToEndAsync();
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
            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(errorResponse);
            var buffer = Encoding.UTF8.GetBytes(jsonResponse);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
        else
        {
            using (var connection = Sqlite.Connect())
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
            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(responseBody);
            var buffer = Encoding.UTF8.GetBytes(jsonResponse);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
    }
}