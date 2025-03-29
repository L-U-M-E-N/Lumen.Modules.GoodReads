namespace Lumen.Modules.GoodReads.Common.Models {
    public class GoodReadsItem {
        public string BookName { get; set; } = null!;
        public DateTime Date { get; set; }
        public int? Percentage { get; set; }
        public int? PagesRead { get; set; }
        public string ProgressText { get; set; } = null!;
        public int? BookSize { get; set; }
    }
}
