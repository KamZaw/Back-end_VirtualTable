namespace VirtualTable
{
    public enum FontType
    {
        NORMAL,
        BOLD,
        ITALIC
    }

    public class Text : Shape
    {
        public String sText { get; set; }
        public int fontSize {get; set;}
        public string fontName { get; set; }
        public int fontWeight { get; set; }

        public FontType FontType { get; set; }

        public Text() : base()
        {
            
            this.sText = "";
            setDefaultFont();
        }
        public Text(string sText) : base()
        {
            this.sText = sText;
            setDefaultFont();
        }

        private void setDefaultFont()
        {
            fontSize = 14;
            fontName = "Arial";
            fontWeight = 500;
            FontType = FontType.NORMAL;
        }
    }
}
