using System.Text;
using System.Text.Json;
using VirtualTable;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000").WithMethods("PUT", "DELETE", "GET").AllowAnyHeader();
        });
});



builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
//app.UseStaticFiles();
app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);



app.UseAuthorization();


//app.MapDefaultControllerRoute();

app.UseEndpoints(endpoints =>
{
    Shape kwadrat = new Shape
    {
        Date = DateTime.Now,
        sDescription = "kwadrat",
        iColor = 0xFF0000,  //domyślnie niebieski kolor linii
    };

    //prostokąt
    kwadrat.addPoint(new Point(0, 0));
    kwadrat.addPoint(new Point(100, 0));
    kwadrat.addPoint(new Point(100, 100));
    kwadrat.addPoint(new Point(0, 100));

 
    endpoints.MapGet("/update_shapes", async context =>
    {
        var response = context.Response;
        response.Headers.Add("connection", "keep-alive");
        response.Headers.Add("cach-control", "no-cache");
        response.Headers.Add("content-type", "text/event-stream");

        while (true)
        {
            await response.Body
                    .WriteAsync(Encoding.UTF8.GetBytes($"data: {JsonSerializer.Serialize(kwadrat)}\n\n"));
            kwadrat.Date = DateTime.Now;
            await response.Body.FlushAsync();
            await Task.Delay(2 * 1000);
        }

    });
});

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapGet("/echo",
//        context => context.Response.WriteAsync("echo"))
//        .RequireCors(MyAllowSpecificOrigins);

//    endpoints.MapControllers()
//             .RequireCors(MyAllowSpecificOrigins);


//});

app.MapControllers();





app.Run();
