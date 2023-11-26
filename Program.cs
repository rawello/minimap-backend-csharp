using WebApplication1;
using WebApplication1.Controllers;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Map("/generateImages", async context =>
{
    await Routes.Generation(context);
});

app.Map("/connectWithMobile", async context =>
{
    await Routes.Connecting(context);
});

app.Map("/addRouteToDbFromFront", async context =>
{
    await Routes.Adding(context);
});
//
// app.Map("/allRoutes", async context =>
// {
//     await Routes.Getting(context);
// });
//
// app.Map("/editRoute", async context =>
// {
//     await Routes.Editing(context);
// });
//
// app.Map("/deleteRoute", async context =>
// {
//     await Routes.Deleting(context);
// });
//
// app.Map("/generateQR", async context =>
// {
//     await Routes.QrGenerating(context);
// });
//
// app.Map("/register", async context =>
// {
//     await Routes.Registering(context);
// });
//
// app.Map("/login", async context =>
// {
//     await Routes.Loging(context);
// });

app.Run();