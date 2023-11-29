using System.IO.Compression;
using BenchmarkDotNet.Attributes;
using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace WebApplication1.Controllers;
public interface Routes
{
    //}
    //public static class Routes
    //{
    static async Task Generation(HttpContext context)
    {
        try
        {
            await using var db = new MySqlConnection("Server=localhost;Database=test;Uid=rawello;Pwd=rawello;");
            //await using var db = new MySqlConnection("Server=localhost;Database=test;Uid=root;Pwd=password;");
            await db.OpenAsync();

            context.Response.ContentType = "application/json";
            var response = new Dictionary<string, string[]>();

            // var destinationTo = context.Request.Form["destTo"].ToString();
            // var destinationFrom = context.Request.Form["destFrom"].ToString();
            // var build = context.Request.Form["build"].ToString();

            var destinationTo = "1104";
            var destinationFrom = "qr";
            var build = "college256";

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

            var temp = new List<string>() { };
            if (Directory.Exists(folderPath))
            {
                var files = Directory.GetFiles(folderPath);
                foreach (var f in files)
                {
                    Console.WriteLine(f);
                    if (Path.GetFileName(f).Contains("routed.png"))
                    {
                        //var temp = Images.ConvertPng2Svg(f, folderPathTemp);
                        //response[Path.GetFileName(f)] = temp;                        
                        temp.Add(Images.ConvertPng2Svg(f, folderPathTemp));
                    }
                }
            }

            response["floors"] = temp.ToArray();
            var jsonResponse = JsonConvert.SerializeObject(response);

            await context.Response.WriteAsync(jsonResponse);
            Console.WriteLine(folderPath);
            Directory.Delete(folderPath, true);
            File.Delete($"{folderPath}.zip");
        }
        catch (Exception ex)
        {
            await context.Response.WriteAsync(ex.ToString());
        }
    }

    //     public static async Task Getting(HttpContext context)
    //     {
    //
    //     }
    //     
    //     public static async Task Editing(HttpContext context)
    // {
    //
    // }
    //
    // public static async Task Deleting(HttpContext context)
    // {
    //
    // }
    //
    // public static async Task QrGenerating(HttpContext context)
    // {
    //
    // }
    //
    // public static async Task Loging(HttpContext context)
    // {
    //
    // }
    //
    // public static async Task Registering(HttpContext context)
    // {
    //
    // }

    public static async Task Adding(HttpContext context)
    {
        try
        {
            await using var db = new MySqlConnection("Server=localhost;Database=test;Uid=rawello;Pwd=rawello;");
            //await using var db = new MySqlConnection("Server=localhost;Database=test;Uid=root;Pwd=password;");
            await db.OpenAsync();

            context.Response.ContentType = "application/json";
            var response = new Dictionary<string, string>();

            var rooms = context.Request.Form["rooms"];
            Console.WriteLine(rooms);
            var login = context.Request.Form["login"].ToString();
            var build = context.Request.Form["build"].ToString();

            var zipFile = context.Request.Form.Files["zipFile"];

            byte[] zipData = new byte[zipFile.Length];
            using (var stream = new MemoryStream())
            {
                await zipFile.CopyToAsync(stream);
                zipData = stream.ToArray();
            }

            await db.QueryFirstOrDefaultAsync<string>("INSERT INTO maps(mapsZip, rooms, login, build) VALUES(@zipData, @rooms, @login, @build)", new { zipData, rooms, login, build });

            await context.Response.WriteAsync("200");
        }
        catch (Exception ex)
        {
            await context.Response.WriteAsync(ex.ToString());
        }
    }

    public static async Task Connecting(HttpContext context)
    {
        try
        {
            await using var db = new MySqlConnection("Server=localhost;Database=test;Uid=rawello;Pwd=rawello;");
            //await using var db = new MySqlConnection("Server=localhost;Database=test;Uid=root;Pwd=password;");
            await db.OpenAsync();

            context.Response.ContentType = "application/json";
            var response = new Dictionary<string, string[]>();

            var build = context.Request.Form["build"].ToString();

            var test = await db.QueryFirstOrDefaultAsync<string>("SELECT rooms FROM maps WHERE build = @build", new { build });
            var rooms = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(test ?? throw new InvalidOperationException());

            response["rooms"] = rooms.Keys.ToArray();


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
            var temp = new List<string>() { };

            if (Directory.Exists(folderPath))
            {
                var files = Directory.GetFiles(folderPath);
                foreach (var f in files)
                {
                    Console.WriteLine(f);
                    if (Path.GetFileName(f).Contains(".png"))
                    {
                        temp.Add(Images.ConvertPng2Svg(f, folderPathTemp));
                        //response[Path.GetFileName(f)] = temp;
                    }
                }
            }

            response["floors"] = temp.ToArray();

            var jsonResponse = JsonConvert.SerializeObject(response);

            await context.Response.WriteAsync(jsonResponse);
            Console.WriteLine(folderPath);
            Directory.Delete(folderPath, true);
            File.Delete($"{folderPath}.zip");
        }
        catch (Exception ex)
        {
            await context.Response.WriteAsync(ex.ToString());
        }
    }
}