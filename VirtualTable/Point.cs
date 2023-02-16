namespace VirtualTable
{
    public class Point
    {
 

        public Point()
        {
            x = 0;
            y = 0;
        }
        public Point(Point pt)
        {
            x = pt.x;
            y = pt.y;
        }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int x { get; set; }
        public int y { get; set; }

    }
}
