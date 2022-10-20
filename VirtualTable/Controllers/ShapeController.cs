using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace VirtualTable.Controllers
{

    public static class SessionShapes
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }

    [ApiController]
    [Route("[controller]")]
    public class ShapeController : ControllerBase
    {

        private readonly ILogger<ShapeController> _logger;
        private Shape[] tab = null;

        //status wybranej figury
        public int? selectedFigure
        {
            get
            {
                return HttpContext.Session.GetInt32("A");
            }
            set
            {
                if(value is not null)
                    HttpContext.Session.SetInt32("A", (int)value);
            }
        }


        public ShapeController(ILogger<ShapeController> logger)
        {
            _logger = logger;

            tab = Enumerable.Range(1, 2).Select(index => new Shape
            {
                Date = DateTime.Now.AddDays(index),
                sDescription = index + "",
                iColor = index == 1 ? 0x0000FF : 0xFF0000,  //domyślnie niebieski kolor linii
            }).ToArray();

            //prostokąt
            tab[0].addPoint(new Point(0, 0));
            tab[0].addPoint(new Point(100, 0));
            tab[0].addPoint(new Point(100, 100));
            tab[0].addPoint(new Point(0, 100));

            //trójkąt
            tab[1].addPoint(new Point(0, 0));
            tab[1].addPoint(new Point(300, 0));
            tab[1].addPoint(new Point(0, 300));        
        }

        [HttpDelete("id")]
        public void rmShape(int id)
        {
  //          selectedFigure = id;
        }

        [HttpGet("GetShapes")]
        public IEnumerable<Shape> Get()
        {


            //if(selectedFigure > 0)
            //    tab[1] = tab[0];
            return tab;
        }
    }
}