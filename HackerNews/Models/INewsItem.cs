namespace HackerNews.Models
{
    public interface INewsItem
    {
        int Id { get; set; }
        string Title { get; set; }
        string Url { get; set; }
    }
}