using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;
using VirtualTable;

var builder = WebApplication.CreateBuilder(args);

var AllowOrigins = "_AllowOrigins";

//$env:GOOGLE_APPLICATION_CREDENTIALS="C:\Users\mirek\Downloads\wirtualnatablica.json"

builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
            .WithMethods("PUT", "DELETE", "GET").AllowAnyHeader();
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

app.UseCors(AllowOrigins);



app.UseAuthorization();


//app.MapDefaultControllerRoute();
bool refresh = true;
IShape tmp = null;
    Shape kwadrat = new Shape
    {
        sDescription = "kwadrat",
        iColor = 0xFF0000,  //domyślnie niebieski kolor linii
    };

    //prostokąt
    kwadrat.addPoint(new Point(0, 0));
    kwadrat.addPoint(new Point(100, 0));
    kwadrat.addPoint(new Point(100, 100));
    kwadrat.addPoint(new Point(0, 100));
    
    Text kwadrat1 = new Text
    {
        sText = "Jakiś tekst",
        sDescription = "tekst",
        iColor = 0x000000,  //domyślnie czarny kolor
    };

    //pozycja startowa
    kwadrat1.addPoint(new Point(220, 220));

    var secret = "RW0RFB4CYNlpIU7NrNUqeVYmQs2t8YCzmARAIgLX";

    //var authProvider = new FirebaseAuthProvider(new FirebaseConfig(secret));

    //var auth = await authProvider.SignInWithEmailAndPasswordAsync("email", "pass");

    FirebaseClient firebase = new FirebaseClient(
      "https://wirtualnatablica-4faf3-default-rtdb.firebaseio.com/",
      new FirebaseOptions
      {
          AuthTokenAsyncFactory = () => Task.FromResult(secret)
      });

Console.WriteLine($">>{kwadrat.ticks}");
Console.WriteLine($">>{kwadrat1.ticks}");
//await firebase
//    .Child("Sessions")
//    .Child("Sesja_1")
//    .Child(kwadrat.ticks + "")
//    .Child(kwadrat.GetType().Name)
//    .PutAsync(kwadrat);
//    await firebase
//    .Child("Sessions")
//    .Child("Sesja_1")
//    .Child(kwadrat1.ticks + "")
//    .Child(kwadrat1.GetType().Name)
//    .PutAsync(kwadrat1);
var sesje = await firebase
   .Child("Sessions")
   .Child("Sesja_1")
    //.Child("638025818114077190")
    //.OrderByKey()
    .OnceAsync<Object>();

//ustaw listenera na wszystkie obiekty W AKTYWNEJ sesji
//TODO: ustawiaj tylko dla nowej sesji i pomiń sesje historyczne
    if(sesje != null )
        foreach (var s in sesje)
        {
            JObject jo = JObject.Parse(s.Object.ToString());
        
            Console.WriteLine($"{s.Key}: {jo.Properties().First().Name}");
            object ob = JsonSerializer.Deserialize<object>(s.Object.ToString());
    

            //jeśli obiekt typu Text to rejestruj nasłuch dla tego obiektu 
            if (jo.Properties().First().Name.CompareTo("Text")== 0)
            {
                firebase
                .Child("Sessions")
                .Child("Sesja_1")
                .Child(s.Key)
                .AsObservable<Text>()
                .Subscribe(d =>
                {
                    if (d.Object != null)
                    lock (app)
                    {
                        //TODO: odkomentować jak dodamy obsługę tekstu
                        if (d.Object != null) Console.WriteLine(d.Key + "_" + ((Shape)d.Object).iColor + ": " + s.Key);
                        //tmp = ((Text)d.Object); //odblokuj przekazywanie obiektu przez stream
                        //refresh = true;         //odblokuj przekazywanie obiektu przez stream
                    }
                    //Console.WriteLine(d.Key + "_" + (d.Object));
                });
            }
            //jeśli obiekt typu Shape to rejestruj nasłuch dla tego obiektu 
            if (jo.Properties().First().Name.CompareTo("Shape")==0)
            {
                firebase
                .Child("Sessions")
                .Child("Sesja_1")
                .Child(s.Key)
                .AsObservable<Shape>()
                .Subscribe(d =>
                {
                    Console.WriteLine(d.Key + "_?");
                    if (d.Object != null)
                     lock (app)
                    {
                            Console.WriteLine(d.Key + "_" + ((Shape)d.Object).iColor + ": " + s.Key);

                            //jeśli obiekt usunięty to tworzymy pusty obiekt z komunikatem DELETED aby można było usunąć go po ticks z ekanów
                            var shape = new Shape();
                        shape.sDescription = "DELETED";
                        shape.ticks = s.Key;
                        tmp = d.Object != null?((Shape)d.Object): shape;    //odblokuj przekazywanie obiektu przez stream
                        refresh = true;             //odblokuj przekazywanie obiektu przez stream
                    }
                        //Console.WriteLine(d.Key + "_" + (d.Object));
                });
            }
        }

app.UseEndpoints(endpoints =>
{
    //listener dla zmian do przesyłania modyfikowanych/nowych/usuniętych obiektów do frontendu
    endpoints.MapGet("/update_shapes", async context =>
    {
        var response = context.Response;
        response.Headers.Add("connection", "keep-alive");
        response.Headers.Add("cach-control", "no-cache");
        response.Headers.Add("content-type", "text/event-stream");

        while (true)
        {
            
            if (refresh && tmp != null)
            {
                String serializedShape = "";
                lock (app)
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = false
                    };
                    serializedShape = JsonSerializer.Serialize<Shape>((Shape)tmp, options);
                    //serializedShape = JsonSerializer.Serialize(tmp);
                    refresh = false;
                    tmp = null;
                }
                if (serializedShape != null)
                {
                    string sshape = serializedShape;
                    Console.WriteLine($"%%%{sshape}");
                    //wysyłaj zmodyfikowany obiekt do frontendu
                    await response.Body
                        .WriteAsync(Encoding.UTF8.GetBytes($"data: {sshape}\n\n"));
                    await response.Body.FlushAsync();
                }
            }
            await Task.Delay(2 * 1000);
        }

    });
    endpoints.MapControllers().RequireCors(AllowOrigins);
});

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapGet("/echo",
//        context => context.Response.WriteAsync("echo"))
//        .RequireCors(AllowOrigins);

//    endpoints.MapControllers()
//             .RequireCors(AllowOrigins);
//});

//app.MapControllers();


app.Run();

