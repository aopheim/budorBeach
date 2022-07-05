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
                    Date = new DateTime(2021, 06, 01, 13, 00, 00),
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
                        { "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-4/10-30-6.jpg" }
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
                        "I dag morges klekket eggene! Se i bildeserien hvordan alle seks eggene ble klekket i løpet av dagen. Dessverre er ikke kameraet i fuglekassa " +
                        "av den beste kvaliteten, så det er vanskelig å se baby-fluesnapperne så tydelig. " +
                        "Men vi håper det står bra til med alle sammen. Fru Fluesnapper ser veldig stolt ut. Herr Fluesnapper, derimot, er det fortsatt ingen tegn til.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-23/4-7-32.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-23/4-32-32.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-23/6-27-32.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-23/7-7-32.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-23/12-12-32.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-23/13-57-32.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-23/14-12-32.jpg"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "j",
                    Date = new DateTime(2021, 06, 24),
                    Header = "Første mating",
                    Body =
                        "Nå ser det ut som alle eggene er klekket, og Fru Fluesnapper er i gang med å snappe fluer til barna sine. Fuglekassa tok et fint bilde av matende " +
                        "fluesnapper i dag morges. Etter god respons på fuglekasse-prosjektet i går, finnes nå dette nettstedet på budorbeach.no i stedet for den forrige " +
                        "adressen det var vanskelig å huske.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-24/5-32-32.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-24/8-20-18.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-24/8-30-18.jpg"
                    }
                },
                new NewsFeedEntry
                {
                    Date = new DateTime(2021, 6, 26),
                    Id = "k",
                    Header = "Herr Fluesnapper vender tilbake!",
                    Body =
                        "I dag fikk vi et gjensyn med Herr Fluesnapper for første gang på over en uke. Han kom raskt innom for å hilse på baby-fluesnapperne sine, " +
                        "men forsvant raskt etter det. Baby-fluesnapperne har vokst mye på bare noen dager, og man kan nå blant annet se at de har fått tydelige nebb",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-26/12-28-55.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-26/15-28-55.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-26/13-38-55.jpg"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "l",
                    Header = "Kjærligheten blomstrer!",
                    Body =
                        "I dag fikk vi det første parbildet av Herr og Fru Fluesnapper. Herr Fluesnapper er nå oftere innom og hilser på, og det ser ut som kjærligheten blomstrer " +
                        "i fuglekassa.",
                    Date = new DateTime(2021, 06, 27),
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-27/15-53-55.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-27/8-28-55.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-27/13-3-55.jpg"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "m",
                    Date = new DateTime(2021, 06, 29),
                    Header = "Baby-fluesnapperne vokser fort",
                    Body =
                        "På bare seks dager har baby-fluesnapperne vokst fort, og har nå tydelige nebb og det som ser ut som begynnelsen på en fjærdrakt. Fru Fluesnapper " +
                        "ligger nå mye mindre og ruger, og er mye frem og tilbake for å mate sine håpefulle",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-29/2-23-55.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-29/4-58-55.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-29/6-13-55.jpg"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "n",
                    Date = new DateTime(2021, 07, 05),
                    Header = "Snart flyedyktige?",
                    Body =
                        "Baby-fluesnapperne har nå fått en tydelig fjærdrakt, og det er nesten ikke plass til hele familien Fluesnapper i fuglekassa lenger. Kommer de snart til å forlate redet? ",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-5/5-42-44.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-5/9-32-44.jpg"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "o",
                    Date = new DateTime(2021, 07, 07),
                    Header = "Utålmodige tenårings-fluesnappere",
                    Body =
                        "Det er nå mye aktivitet blant baby-fluesnapperne, som ikke lengre er babyer, men ordentlige tenåringer. De ligger sjeldnere og sover, og begynner " +
                        "heller å prøve ut vingene sine. Kanskje første flytur skjer i løpet av uka?",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-7/9-12-44.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-7/10-47-44.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-7/7-57-44.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-7/8-22-44.jpg"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "p",
                    Date = new DateTime(2021, 07, 09),
                    Header = "Fluesnapperne har forlatt redet",
                    Body =
                        "I går og i dag tidlig forlot fluesnapperne redet én etter én. Hvordan den første flyveturen gikk vet vi ikke, men vi håper det gikk bra. Nå håper vi de finner veien tilbake neste år også.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-9/4-52-23.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-9/5-17-23.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-9/5-27-23.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-9/6-17-23.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-9/6-27-23.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-9/6-32-23.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-9/6-37-23.jpg"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "q",
                    Date = new DateTime(2022, 04, 13),
                    Header = "Klar for ny fluesnapper-sesong!",
                    Body =
                        "Påsken er kommet, og fuglekassa har våknet til liv igjen etter å ha vært skrudd ned i noen måneder. Fuglekassa er nå tømt og innflyttingsklar. Vi håper å få et gjensyn med Herr og Fru Fluesnapper etter hvert som det blir varmere i været.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-4-13/11-25-56.jpg"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "r",
                    Date = new DateTime(2022, 04, 26),
                    Header = "Fluesnapperen er tilbake på visning!",
                    Body =
                        "I dag har fluesnapperne kommet tilbake! Dette er mye tidligere enn i fjor, da de kom først i starten av juni. Men tydeligvis har de startet å gå på fuglekasse-visning tidligere i år. Håper de finner seg til rette og har finansieringsbeviset i orden",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-4-26/11-14-11.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-4-26/9-29-11.jpg"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "s",
                    Date = new DateTime(2022, 04,
                        28),
                    Header = "Innflyttingen er i gang!",
                    Body =
                        "Det ble en rask budrunde, og Herr Fluesnapper er nå stolt eier av fuglekassa i Størigardsvegen. Nå flyttes det inn for harde livet, og på bare noen timer har han laget til en myk og fin madrass til Fru Fluesnapper",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-4-27/6-29-11.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-4-27/6-44-11.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-4-27/8-59-11.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-4-27/9-29-11.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-4-27/15-14-11.jpg"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "t",
                    Date = new DateTime(2022, 05, 02),
                    Header = "Bygningsarbeid pågår",
                    Body =
                        "De siste dagene har Herr Fluesnapper virkelig stått på. Se hvordan han har bygget et lunt og godt rede for sin fremtidige Fluesnapperkone: Først et lag med det som ser ut som lyng, deretter ser det ut som han har nappet av fjær for ekstra isolering",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-4-29/9-44-11.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-4-30/9-44-11.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-4-30/13-44-11.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-3/9-14-11.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-3/3-44-11.jpg"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "u",
                    Date = new DateTime(2022, 05, 10),
                    Header = "Nå mangler bare Fru Fluesnapper",
                    Body =
                        "Nå ser det ut som Herr Fluesnapper er fornøyd med redet sitt. Det ser veldig mykt og fint ut. Så nå mangler bare en Fru Fluesnapper. Herr Fluesnapper er nå mye sjeldnere innom fuglekassa, så alt tyder på at han er på sjekkern langs Størigardsvegen.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-8/11-59-34.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-10/6-59-34.jpg"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "v",
                    Date = new DateTime(2022, 05, 12),
                    Header = "Jomfrufødsel fra Herr Fluesnapper?",
                    Body =
                        "I dag kom det egg i fuglekassa! Men hvem som har lagt egget er et mysterium. Vi har ikke sett noe til besøk fra Fru Fluesnapper, og selve fødselen må ha skjedd veldig fort i morgentimene. Kameraet har bare greid å plukke opp det svart-hvite hodet til Herr Fluesnapper, så sannsynligvis må dette være verdens første dokumenterte jomfrufødsel fra enn mannlig fluesnapper. Sensasjon!",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-12/3-29-34.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-12/2-59-34.jpg",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-12/3-14-34.jpg"
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