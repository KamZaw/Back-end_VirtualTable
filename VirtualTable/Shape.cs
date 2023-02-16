using Firebase.Database;

namespace VirtualTable
{
    public class Shape : IShape 
    {
        public DateTime Date { get; set; }

        public LinkedList<Point> points { get; set; }

        public string? sDescription { get; set; }
        public int iColor { get; set; }

        LinkedList<Point> IShape.points => new LinkedList<Point>();

        public string ticks { get; set; }

        public Shape(LinkedList<Point> points)
        {
            Date = DateTime.Now;
            this.ticks = $"{Date.Ticks}";
            this.points = points;
            this.sDescription = "brak";
            iColor = 0x0000FF;  //domyślnie niebieski kolor linii
        }
        public Shape()
        {
            Date = DateTime.Now;
            this.ticks = $"{Date.Ticks}";
            points = new LinkedList<Point>();
            this.sDescription = "brak";
            iColor = 0x0000FF;  //domyślnie niebieski kolor linii
        }

        public void addPoint(Point pt)
        {
            points.AddLast(pt);
        }

    }
}