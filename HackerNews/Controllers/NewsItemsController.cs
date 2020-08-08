using HackerNews.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace HackerNews.Controllers
{
    /*
     * TO DO:
     * 1. Caching -- use IHostedService?
     * 2. Unit Tests
     * 3. Dependency Injection (any time you "new" up, you can DI
     * 4. Make it faster
     * 
     * 
     * 
     */


    [EnableCors("http://localhost:4200", "*", "*")]
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

        private static async Task<List<int>> GetMostRecentNewsStoryIdsListAsync(HttpClient httpClient)
        {
                HttpResponseMessage response = httpClient.GetAsync("newstories.json").Result;
                
                var mostRecentNewsStoryIdsList = await response.Content.ReadAsAsync<List<int>>();

                return mostRecentNewsStoryIdsList;           
        }
        /*
        private async Task<int> GetMostRecentNewsItemId(List<int> mostRecentNewsItemsList)
        {
            int mostRecentNewsItemId = mostRecentNewsItemsList[0];
            return mostRecentNewsItemId;
        }
        */

        private static async Task<NewsItem> GetSingleNewsItemAsync(int newsItemId, HttpClient httpClient)
        {
            HttpResponseMessage response = httpClient.GetAsync("item/" + newsItemId + ".json").Result;
            NewsItem newsItem = await response.Content.ReadAsAsync<NewsItem>();
            return newsItem;
        }

        private static async Task<List<NewsItem>> GetMostRecentNewsItemsAsync()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                List<int> mostRecentNewsStoryIdsList = await GetMostRecentNewsStoryIdsListAsync(httpClient);
                // Option 1
                //List<Task<NewsItem>> newsItemTasksList = new List<Task<NewsItem>>();

                // Option 2
                List<NewsItem> newsItemsList = new List<NewsItem>();
                List<Task> tasks = new List<Task>();

                Parallel.ForEach<int>(mostRecentNewsStoryIdsList, (newsItemId) =>
                   {
                       // Option 1 -- Uses Tasks
                       //newsItemTasksList.Add(GetSingleNewsItemAsync(newsItemId, httpClient));
                       tasks.Add(Task.Run(async () =>
                       {
                           var result = await GetSingleNewsItemAsync(newsItemId, httpClient);
                           if (NewsItemIsNull(result)) return;
                           if (NewsItemUrlIsNull(result)) return;

                           newsItemsList.Add(result);
                       }));
                       
                       // Option 2
                       
                   }
                );
                Task t = Task.WhenAll(tasks);
                try
                {
                    t.Wait();
                }
                catch (Exception)
                {

                    throw;
                }
                // Option 1
                //var taskResults = await Task.WhenAll(newsItemTasksList);

                // Option 1
                // return new List<NewsItem>(taskResults);

                // Option 2
                return newsItemsList;
            }
        } 

        private static bool NewsItemIsNull(NewsItem newsItem)
        {
            if (newsItem is null) return true;
            return false;
        }

        // Sometimes HackerNews "new stories" are new discussions and don't have an outside link.
        private static bool NewsItemUrlIsNull(NewsItem newsItem)
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
 *  
 */
