namespace MauiApp1.Model
{
    public class CourseGroup
    {
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public int Credits { get; set; }
        public List<Term> TermsOffered { get; set; } = new List<Term>();
    }
}