using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace VirtualTable.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public class ShapeController : ControllerBase
    {

        private readonly ILogger<ShapeController> _logger;


        public ShapeController(ILogger<ShapeController> logger)
        {
            _logger = logger;
        }
        [HttpPut("AudioChunk")]
        public async void AudioChunk(AudioChunks audio)
        {
            Console.WriteLine("Doszło " + audio.id);
        }
        [HttpPut("AddShape")]
        public async void AddShape(Shape shape)
        {
            if(shape == null)
            {
                Console.WriteLine("Pusty obiekt");
                return;
            }
            try
            {
                var dt = DateTime.Now;
                shape.Date = dt;
                shape.ticks = $"{dt.Ticks}";
                var secret = "RW0RFB4CYNlpIU7NrNUqeVYmQs2t8YCzmARAIgLX";
                FirebaseClient firebase = new FirebaseClient(
                  "https://wirtualnatablica-4faf3-default-rtdb.firebaseio.com/",
                  new FirebaseOptions
                  {
                      AuthTokenAsyncFactory = () => Task.FromResult(secret)
                  });
                await firebase
                .Child("Sessions")
                .Child("Sesja_1")
                .Child(shape.ticks)
                .Child("Shape")
                .PutAsync(shape);
            }
            catch (Exception)
            {
                Console.WriteLine ($"{StatusCodes.Status500InternalServerError}, Error updating data");
            }
        }

        [HttpDelete("delete/{ticks}")]
        public async void rmShape(String ticks)
        {
            if (ticks == null || ticks.Length <= 0 || ticks.Contains("undefined")) return;
            try
            {
                DateTime dt = new DateTime(long.Parse(ticks));

                Console.WriteLine($">>{dt.ToShortTimeString()}");

                var secret = "RW0RFB4CYNlpIU7NrNUqeVYmQs2t8YCzmARAIgLX";
                FirebaseClient firebase = new FirebaseClient(
                  "https://wirtualnatablica-4faf3-default-rtdb.firebaseio.com/",
                  new FirebaseOptions
                  {
                      AuthTokenAsyncFactory = () => Task.FromResult(secret)
                  });
                await firebase
                .Child("Sessions")
                .Child("Sesja_1")
                .Child(ticks)
                .DeleteAsync();
            }
            catch (Exception)
            {
                Console.WriteLine($"{StatusCodes.Status500InternalServerError}, Error updating data");
            }
        }

        [HttpGet("GetShapes")]
        public async Task<IEnumerable<Object>> GetAsync()
        {

            var secret = "RW0RFB4CYNlpIU7NrNUqeVYmQs2t8YCzmARAIgLX";

            FirebaseClient firebase = new FirebaseClient(
              "https://wirtualnatablica-4faf3-default-rtdb.firebaseio.com/",
              new FirebaseOptions
              {
                  AuthTokenAsyncFactory = () => Task.FromResult(secret)
              });


            DateTime dt = DateTime.Now;

            var sesje = await firebase
           .Child("Sessions")
           .Child("Sesja_1")
            //.Child("638025818114077190")
            //.OrderByKey()
            .OnceAsync<object>();

            LinkedList<Object> ll = new LinkedList<Object>();
            //wyszukaj daty (podkatalogi) dla obiektw IShape
            foreach (var s in sesje)
            {
                JObject jo = JObject.Parse(s.Object.ToString());
                //Console.WriteLine(s.Object);
                //jeśli obiekt typu Text to rejestruj nasłuch dla tego obiektu 
                //if (jo.Properties().First().Name.CompareTo("Text") == 0) 
                //{
                //    var obj = await firebase
                //    .Child("Sessions")
                //    .Child("Sesja_1")
                //    .Child(s.Key)
                //    .OnceAsync<Text>();

                //    foreach (var shape in obj)
                //    {
                //        ll.AddLast(shape.Object);
                //    }
                //    //Console.WriteLine(d.Key + "_" + (d.Object));
                //};
                //jeśli obiekt typu Shape to rejestruj nasłuch dla tego obiektu 
                if (jo.Properties().First().Name.CompareTo("Shape") == 0)
                {
                    var obj = await firebase
                    .Child("Sessions")
                    .Child("Sesja_1")
                    .Child(s.Key)
                    .OnceAsync<Shape>();
                    foreach (var shape in obj)
                    {
                        ll.AddLast(shape.Object);
                    }
                }
            }
            //tab = ll.ToArray();
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            //String ret = JsonSerializer.Serialize<object>(ll.ToArray(), options);
            return ll.ToArray();
        }
    }
}