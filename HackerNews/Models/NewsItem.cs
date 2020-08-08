namespace HackerNews.Models
{
    public class NewsItem : INewsItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}