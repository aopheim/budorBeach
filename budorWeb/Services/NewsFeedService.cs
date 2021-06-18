using System;
using System.Collections.Generic;

namespace budorWeb.Services
{
    public static class NewsFeedService
    {
        public static List<NewsFeedEntry> GetNewsFeedEntries()
        {
            return new List<NewsFeedEntry>
            {
                new NewsFeedEntry
                {
                    Id = 1,
                    Date = new DateTime(2021, 04, 01),
                    Header = "Fuglekassa er satt opp!",
                    Body =
                        "Fuglekassen er nå satt opp. I overetasjen er det satt opp et lite kamera som tar bilder med faste mellomrom. Nå er det bare å vente på besøk!",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images/2021-4-1/8-37-40.jpg",
                        "https://budorbeach.blob.core.windows.net/images/2021-4-1/kassa.JPG"
                    }
                },
                new NewsFeedEntry
                {
                    Id = 2,
                    Date = new DateTime(2021, 05, 1),
                    Header = "Fortsatt ingen besøk",
                    Body = "Etter en måned er det dessverre fortsatt ingen besøkende i fuglekassa.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images/2021-5-1/9-39-33.jpg"
                    }
                }
            };
        }
    }
}

public class NewsFeedEntry
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Header { get; set; }
    public string Body { get; set; }
    public List<string> ImageUrls { get; set; }
}