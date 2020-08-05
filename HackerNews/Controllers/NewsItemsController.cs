using HackerNews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace HackerNews.Controllers
{
    public class NewsItemsController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetNewsItems()
        {
            try
            {
                List<int> newestNewsStoryIdsList = GetMostRecentNewsStoryIdsList().Result;

                List<NewsItem> mostRecentNewsItems = GetMostRecentNewsItems(newestNewsStoryIdsList).Result;
                return Ok(mostRecentNewsItems);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<List<int>> GetMostRecentNewsStoryIdsList()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("newstories.json").Result;
                
                var newestNewsStoryIdsList = await response.Content.ReadAsAsync<List<int>>();

                return newestNewsStoryIdsList;                               
            }
        }

        private async Task<int> GetMostRecentNewsItemId(List<int> mostRecentNewsItemsList)
        {
            int mostRecentNewsItemId = mostRecentNewsItemsList[0];
            return mostRecentNewsItemId;
        }

        private async Task<NewsItem> GetSingleNewsItem(int newsItemId)
        {
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = client.GetAsync("item/" + newsItemId + ".json").Result;
                    NewsItem newsItem = await response.Content.ReadAsAsync<NewsItem>();
                    return newsItem;
                }
            }
        }

        private async Task<List<NewsItem>> GetMostRecentNewsItems(List<int> mostRecentNewsStoryIdsList)
        {
            List<NewsItem> newsItemsList = new List<NewsItem>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                foreach (int newsItemId in mostRecentNewsStoryIdsList)
                {
                    HttpResponseMessage response = client.GetAsync("item/" + newsItemId + ".json").Result;
                    NewsItem newsItem = await response.Content.ReadAsAsync<NewsItem>();
                    if (newsItem is null) continue;
                    if (String.IsNullOrEmpty(newsItem.Url)) continue;
                    newsItemsList.Add(newsItem);
                }
            }
            return newsItemsList;
        } 
    }
}
