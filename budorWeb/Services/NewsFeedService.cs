using System;
using System.Collections.Generic;
using System.Linq;

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
                    Id = "a",
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
                    Id = "b",
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
                    Id = "c",
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
                    Id = "d",
                    Date = new DateTime(2021, 06, 01),
                    Header = "Første bilde av innflytterne",
                    Body =
                        "Nå flyttes det inn for harde livet! Det ser ut som det er to forskjellige fugler som flytter inn i samme fuglekasse! Én grå og en mindre, svart.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-1/4-45-5.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-1/7-50-5.jpg"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "e",
                    Date = new DateTime(2021, 06, 01),
                    Header = "Svarthvit fluesnapper",
                    Body =
                        "Etter å ha fått eksperthjelp av søstrene Ulvensøen, som har studiekompetanse i fugletitting, har vi fått avklart fugleartene. Det er svarthvit fluesnapper som har flyttet inn - hannen og hunnen ser veldig ulike ut. ",
                },
                new NewsFeedEntry
                {
                    Id = "f",
                    Date = new DateTime(2021, 06, 03),
                    Header = "Strømbrudd!",
                    Body =
                        "Akkurat i den mest intense innflyttingsperioden får vi plutselig ikke lenger kontakt med fuglekassa. Sannsynligvis har strømmen gått i løpet av natten, og datamaskinen i fuglekassa har ikke greid å få kontakt med internettet igjen. Krise! " +
                        "Fuglekasseteknikeren gjør det han kan for å få reddet situasjonen. Herr og fru fluesnapper har nå bygget seg et fint rede og er nok straks klare for å legge egg",
                    ImageUrls = new List<string>
                        {"https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-4/10-30-6.jpg"}
                },
                new NewsFeedEntry
                {
                    Id = "g",
                    Date = new DateTime(2021, 06, 14),
                    Header = "Fuglekassa er oppe og går igjen!",
                    Body =
                        "Plutselig fikk vi kontakt med fuglekassa igjen! Og nå har det skjedd mye - Fru Fluesnapper har lagt seks fine egg! Man ser dem litt uklart i bildet her - det var en våt og grå dag på Budor Beach. Fru Fluesnapper " +
                        "ligger nå for det meste og ruger på eggene. Herr Fluesnapper har vi ikke sett på lenge. Kameraet er nå skrudd til å ta bilder hvert 5.minutt for å få med seg dramatikken.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-14/12-53-28.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-14/18-43-28.jpg"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "h",
                    Date = new DateTime(2021, 06, 21),
                    Header = "Nærmer vi oss klekking?",
                    Body =
                        "Det kan se ut som det nærmer seg klekking! Se på bildene hvordan det har dannet seg en liten sprekk i de øverste eggene på bare en drøy time. Kan Fru Fluesnapper være i gang med klekking?",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-21/8-32-32.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-21/9-7-32.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-21/9-2-32.jpg"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "i",
                    Date = new DateTime(2021, 06, 23),
                    Header = "Klekking!",
                    Body =
                        "I dag morges klekket eggene! Se i bildeserien hvordan alle seks eggene ble klekket i løpet av dagen. Dessverre er det ganske dårlig " +
                        "oppløsning på bildene fra fuglekassa, så det er vanskelig å se baby-fluesnapperne så tydelig. " +
                        "Men vi håper det står bra til med alle sammen. Fru Fluesnapper ser veldig stolt ut. Herr Fluesnapper, derimot, er det fortsatt ingen tegn til.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images/2021-6-23/4-7-32.jpg",
                        "https://budorbeach.blob.core.windows.net/images/2021-6-23/4-32-32.jpg",
                        "https://budorbeach.blob.core.windows.net/images/2021-6-23/6-27-32.jpg",
                        "https://budorbeach.blob.core.windows.net/images/2021-6-23/7-7-32.jpg",
                        "https://budorbeach.blob.core.windows.net/images/2021-6-23/12-12-32.jpg",
                        "https://budorbeach.blob.core.windows.net/images/2021-6-23/13-57-32.jpg",
                        "https://budorbeach.blob.core.windows.net/images/2021-6-23/14-12-32.jpg"
                    }
                }
            }.OrderByDescending(e => e.Date).ToList();
        }
    }
}

public class NewsFeedEntry
{
    public string Id { get; set; }
    public DateTime Date { get; set; }
    public string Header { get; set; }
    public string Body { get; set; }
    public List<string> ImageUrls { get; set; }
}