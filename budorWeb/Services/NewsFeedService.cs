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
                        "https://budorbeach.blob.core.windows.net/images/2021-4-1/kassa.JPG",
                        "https://budorbeach.blob.core.windows.net/images/2021-4-1/8-37-40.jpg"
                    }
                },
                new NewsFeedEntry
                {
                    Id = 2,
                    Date = new DateTime(2021, 05, 1),
                    Header = "Fortsatt ingen besøk",
                    Body = "Etter en måned er det dessverre fortsatt ingen besøkende i fuglekassa. ",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images/2021-5-1/9-39-33.jpg"
                    }
                },
                new NewsFeedEntry
                {
                    Id = 3,
                    Date = new DateTime(2021, 05, 30),
                    Header = "Første fugl har kommet!",
                    Body =
                        "Rett før kl 14 fikk fuglekassa sitt første besøk! Det ser ut som det var en vellykket visning - etter bare noen timer har det allerede kommet mye inventar på plass.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-5-30/11-51-48.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-5-31/5-52-4.jpg"
                    },
                },
                new NewsFeedEntry
                {
                    Id = 4,
                    Date = new DateTime(2021, 06, 01),
                    Header = "Første bilde av innflytterne",
                    Body =
                        "Nå flyttes det inn for harde livet! Det ser ut som det er to forskjellige fugler som flytter inn i samme fuglekasse! Én grå og en mindre, svart.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-1/4-45-5.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-1/7-50-5.jpg"
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