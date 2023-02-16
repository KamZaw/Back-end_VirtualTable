namespace VirtualTable
{
    public interface IShape
    {
        //data utworzenia obiektu
        public DateTime Date { get; set; }

        public LinkedList<Point> points { get; }

        public string? sDescription { get; set; }

        public int iColor { get; set; }
        public string ticks { get; set; }


    }
}
