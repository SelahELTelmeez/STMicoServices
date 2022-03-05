namespace CurriculumEntites
{
    public class Clip : BaseEntity
    {
        public int Sort { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Status { get; set; } // ENUM
        public string? MyCode { get; set; }
        public string? KNLDBank { get; set; }
        public string? keyWords { get; set; }
        public bool IsMEDU { get; set; }
        public int Usability { get; set; }
        public int LOPoints { get; set; }
        public int Orientation { get; set; } // ENUM
        public bool IsPremium { get; set; }
    }
}
