using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ContestCalendarCS;

class Program
{
    private static string API_KEY;
    // Codeforces(1), LeetCode(102), AtCoder(93), CodeChef(2)
    private static readonly int[] RESOURCE_IDS = { 1, 102, 93, 2 };
    private const string CLIST_URL = "https://clist.by/api/v4/contest/";
    private const string CALENDAR_ID = "primary";
    private static readonly string[] SCOPES = { CalendarService.Scope.Calendar };
    private const string APP_NAME = "Contest Calendar Script";
    static async Task Main(string[] args)
    {
        
        LoadConfiguration();
        
        Console.WriteLine("Trying To Fetching contests from Clist...");
        try
        {
            var contests = await GetContestsAsync();
            Console.WriteLine($"Fetched {contests.Count} contests.");
            
            Console.WriteLine("Authenticating with Google...");
            var service = await GetCalendarServiceAsync();
            
            Console.WriteLine("Adding events to calendar...");
            foreach (var contest in contests)
            {
                await AddEventAsync(service, contest);
            }

            Console.WriteLine("Done.. See You In Next Time :)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Critical Error: {ex.Message}");
        }
     }
    
    private static void LoadConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        IConfiguration config = builder.Build();
        
        API_KEY = config["Clist:ApiKey"];
        
        if (string.IsNullOrEmpty(API_KEY))
        {
            throw new Exception("Please fill in your Clist API Key in appsettings.json");
        }
    }

    
        private static async Task AddEventAsync(CalendarService service, ContestObject contest)
        {
            string uniqueId = $"clist{contest.Id}v2"; 

            var newEvent = new Event()
            {
                Id = uniqueId,
                Summary = contest.EventName,
                Description = $"Contest URL: {contest.Href}",
                Start = new EventDateTime() { DateTimeRaw = contest.Start, TimeZone = "UTC" },
                End = new EventDateTime() { DateTimeRaw = contest.End, TimeZone = "UTC" },
                Reminders = new Event.RemindersData()
                {
                    UseDefault = false,
                    Overrides = new List<EventReminder>()
                    {
                        new EventReminder() { Method = "popup", Minutes = 60 },   // 1 hour
                        new EventReminder() { Method = "popup", Minutes = 300 },  // 5 hours
                        new EventReminder() { Method = "popup", Minutes = 1440 }  // 1 day
                    }
                }
            };

            try
            {
                await service.Events.Insert(newEvent, CALENDAR_ID).ExecuteAsync();
                Console.WriteLine($"Added: {contest.EventName}");
            }
            catch (Google.GoogleApiException e)
            {
                if (e.HttpStatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    Console.WriteLine($"Skipped (Already exists): {contest.EventName}");
                }
                else
                {
                    Console.WriteLine($"Error adding {contest.EventName}: {e.Message}");
                }
            }
        }
        

    private static async Task<CalendarService> GetCalendarServiceAsync()
    {
        UserCredential credential;
        string exePath = AppContext.BaseDirectory;
        string jsonPath = Path.Combine(exePath,"credentials.json");
        
        if (!File.Exists(jsonPath))
        {
            throw new Exception($"Could not find credentials.json at {jsonPath}. Please download it from Google Cloud.");
        }
        
        using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
        {
            string credPath = "token.json";
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                SCOPES,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true));
        }
        return new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = APP_NAME,
        });
    }

    private static async Task<List<ContestObject>> GetContestsAsync()
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"ApiKey {API_KEY}");
            
            var resourceIdsStr = string.Join(",", RESOURCE_IDS);
            
            var nowStr = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
            
            var query = $"?start__gte={nowStr}&resource_id__in={resourceIdsStr}&limit=50&order_by=start&format=json";

            var fullUrl = CLIST_URL + query;
            
            var response = await client.GetAsync(fullUrl);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error ({response.StatusCode}): {errorBody}");
            }
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<ClistResponse>(json);
            return data?.Objects ?? new List<ContestObject>();
        }   
    }
    
    public class ClistResponse
    {
        [JsonProperty("objects")]
        public List<ContestObject> Objects { get; set; }
    }
    public class ContestObject
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("event")]
        public string EventName { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("start")]
        public string Start { get; set; }

        [JsonProperty("end")]
        public string End { get; set; }
    }
    
}