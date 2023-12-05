using System.IO.Compression;
using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace WebApplication1.Controllers;

public static class Routes
{
    public static async Task Generation(HttpContext context)
    {
        // await using var db = new MySqlConnection("Server=localhost;Database=test;Uid=rawello;Pwd=rawello;");
        await using var db = new MySqlConnection("Server=localhost;Database=test;Uid=root;Pwd=password;");
        await db.OpenAsync();

        context.Response.ContentType = "application/json";
        var response = new Dictionary<string, string>();

        var destinationTo = context.Request.Form["destTo"].ToString();
        var destinationFrom = context.Request.Form["destFrom"].ToString();
        var build = context.Request.Form["build"].ToString();
        // var destinationTo = "1104";
        // var destinationFrom = "qr";
        // var build = "college256";

        var test = await db.QueryFirstOrDefaultAsync<string>("SELECT rooms FROM maps WHERE build = @build", new { build });
        var rooms = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(test ?? throw new InvalidOperationException());

        var folderPath = new Random().Next(1000000).ToString();
        Directory.CreateDirectory(folderPath);

        var blob = await db.QueryFirstOrDefaultAsync<byte[]>("SELECT mapsZip FROM maps WHERE build = @build", new { build });

        if (blob != null) await File.WriteAllBytesAsync($"{folderPath}.zip", blob);

        using (var archive = ZipFile.OpenRead($"{folderPath}.zip"))
        {
            foreach (var entry in archive.Entries)
            {
                entry.ExtractToFile(Path.Combine(folderPath, entry.FullName), true);
            }
        }


        var folderPathTemp = folderPath;
        foreach (var f in Directory.GetFiles(folderPathTemp, "*", SearchOption.AllDirectories))
        {
            var path = Path.Combine(folderPath, Path.GetFileName(f));
            if (Path.GetDirectoryName(f) != null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(f) ?? throw new InvalidOperationException());
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new InvalidOperationException());
                File.Copy(f, path, false);
            }
        }

        if (string.IsNullOrEmpty(folderPathTemp))
        {
            AStar.GenerateRoute(build, (rooms ?? throw new InvalidOperationException()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray()), destinationFrom, destinationTo, folderPath);
        }
        else
        {
            AStar.GenerateRoute(build, (rooms ?? throw new InvalidOperationException()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray()), destinationFrom, destinationTo, folderPathTemp);
        }

        if (Directory.Exists(folderPath))
        {
            var files = Directory.GetFiles(folderPath);
            foreach (var f in files)
            {
                Console.WriteLine(f);
                if (Path.GetFileName(f).Contains("routed.png"))
                {
                    var temp = Images.ConvertPng2Svg(f, folderPathTemp);
                    response[Path.GetFileName(f)] = temp;
                }
            }
        }

        var jsonResponse = JsonConvert.SerializeObject(response);

        await context.Response.WriteAsync(jsonResponse);
        Console.WriteLine(folderPath);
        Directory.Delete(folderPath, true);
        File.Delete($"{folderPath}.zip");
    }

    // public static async Task Getting(HttpContext context)
    // {

    // }

    public static async Task Adding(HttpContext context)
    {
        await using var db = new MySqlConnection("Server=localhost;Database=test;Uid=root;Pwd=password;");
        await db.OpenAsync();

        context.Response.ContentType = "application/json";
        var response = new Dictionary<string, string>();

        using (var reader = new StreamReader(context.Request.Body))
        {
            try
            {
                var body = await reader.ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);

                var build = data["build"];
                var rooms = data["rooms"];
                var login = data["login"];
                var svg = data["svg"];
                var obj = data["obj"];
                var floors = data["floors"];

                var test = await db.QueryFirstOrDefaultAsync<string>("INSERT INTO maps(svg, rooms, login, build, obj, floors) VALUES(@svg, @rooms, @login, @build, @obj, @floors)", new { svg, rooms, login, build, obj, floors });

                if (test != null)
                {
                    await context.Response.WriteAsync("200");
                }
                else
                {
                    await context.Response.WriteAsync("400");
                }
            }
            catch (Exception ex)
            {
                await context.Response.WriteAsync(ex.Message);
                throw;
            }
        }
    }

    public static async Task Connecting(HttpContext context)
    {
        await using var db = new MySqlConnection("Server=localhost;Database=test;Uid=root;Pwd=password;");
        await db.OpenAsync();

        using (var reader = new StreamReader(context.Request.Body))
        {
            try
            {
                var body = await reader.ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);
                var folderPath = new Random().Next(1000000).ToString();
                Directory.CreateDirectory(folderPath);

                var build = data["build"];
                var test = await db.QueryFirstOrDefaultAsync<string>("SELECT rooms, FROM MAPS WHERE build = @build", new { build });
                for (var i = 0; i < test.Length; i++)
                {
                    using (StreamWriter writer = new StreamWriter($"{folderPath}/{i}-{build}.svg")){
                        
                    }

                }
                context.Response.ContentType = "application/json";
                var response = new Dictionary<string, string>();

                await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                await context.Response.WriteAsync(ex.Message);
                throw;
            }
        }
    }
}

// public static async Task Editing(HttpContext context)
// {

// }

// public static async Task Deleting(HttpContext context)
// {

// }

// public static async Task QrGenerating(HttpContext context)
// {

// }

// public static async Task Loging(HttpContext context)
// {

// }

// public static async Task Registering(HttpContext context)
// {

// }
//}