namespace VirtualTable
{
    public class Shape
    {
         //data utworzenia obiektu
        public DateTime Date { get; set; }

        public LinkedList<Point> points { get; }

        public string? sDescription{ get; set; }     

        public int iColor { get; set; }
        public Shape(LinkedList<Point> points)
        {
            Date = DateTime.Now;
            this.points = points;
            this.sDescription = "brak";
            iColor = 0x0000FF;  //domyślnie niebieski kolor linii
        }
        public Shape()
        {
            Date = DateTime.Now;
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