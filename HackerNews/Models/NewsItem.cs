using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackerNews.Models
{
    public class NewsItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}