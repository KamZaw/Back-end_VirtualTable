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
//app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);



app.UseAuthorization();


//app.MapDefaultControllerRoute();



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
