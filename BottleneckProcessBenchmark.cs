using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using WebApplication1.Controllers;

[MemoryDiagnoser]
[RankColumn]
public class MyBenchmark
{
    private HttpContext? _context;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _context = new DefaultHttpContext();
    }

    [Benchmark]
    public async Task Generation()
    {
        await Routes.Generation(_context);
    }
}