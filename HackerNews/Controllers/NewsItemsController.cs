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
                List<NewsItem> mostRecentNewsItems = GetMostRecentNewsItemsAsync().Result;
                return Ok(mostRecentNewsItems);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private static async Task<List<int>> GetMostRecentNewsStoryIdsListAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("newstories.json").Result;
                
                var mostRecentNewsStoryIdsList = await response.Content.ReadAsAsync<List<int>>();

                return mostRecentNewsStoryIdsList;                               
            }
        }

        private async Task<int> GetMostRecentNewsItemId(List<int> mostRecentNewsItemsList)
        {
            int mostRecentNewsItemId = mostRecentNewsItemsList[0];
            return mostRecentNewsItemId;
        }

        private static async Task<NewsItem> GetSingleNewsItemAsync(int newsItemId)
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

        private static async Task<List<NewsItem>> GetMostRecentNewsItemsAsync()
        {
            List<int> mostRecentNewsStoryIdsList = await GetMostRecentNewsStoryIdsListAsync();
            List<Task<NewsItem>> newsItemTasksList = new List<Task<NewsItem>>();

            Parallel.ForEach<int>(mostRecentNewsStoryIdsList, (newsItemId) =>
               {
                   newsItemTasksList.Add(GetSingleNewsItemAsync(newsItemId));
               }
            );
            var taskResults = await Task.WhenAll(newsItemTasksList);
            return new List<NewsItem>(taskResults);
        } 

        private bool NewsItemIsNull(NewsItem newsItem)
        {
            if (newsItem is null) return true;
            return false;
        }

        private bool NewsItemUrlIsNull(NewsItem newsItem)
        {
            if (String.IsNullOrEmpty(newsItem.Url)) return true;
            return false;
        }
    }
}
/*
 * TODO:
 * -- Cancelation tokens 
 * -- if (newsItem is null) return;  
 * -- if (String.IsNullOrEmpty(newsItem.Url)) return;
 * 
 * 
 *
                
                   // Sometimes new stories are discussions on HackerNews and don't have a link URL
                    );
 */
