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
                new()
                {
                    Id = "a",
                    Date = new DateTime(2021, 04, 01),
                    Header = "Fuglekassa er satt opp!",
                    Body =
                        "Fuglekassen er nå satt opp. I overetasjen er det satt opp et lite kamera som tar bilder med faste mellomrom. Nå er det bare å vente på besøk!",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images/2021-4-1/kassa.jpg",
                        "https://budorbeach.blob.core.windows.net/images/2021-4-1/8-37-40.webp"
                    }
                },
                new()
                {
                    Id = "b",
                    Date = new DateTime(2021, 05, 1),
                    Header = "Fortsatt ingen besøk",
                    Body = "Etter en måned er det dessverre fortsatt ingen besøkende i fuglekassa. ",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images/2021-5-1/9-39-33.webp"
                    }
                },
                new()
                {
                    Id = "c",
                    Date = new DateTime(2021, 05, 30),
                    Header = "Første fugl har kommet!",
                    Body =
                        "Rett før kl 14 fikk fuglekassa sitt første besøk! Det ser ut som det var en vellykket visning - etter bare noen timer har det allerede kommet mye inventar på plass.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-5-30/11-51-48.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-5-31/5-52-4.webp"
                    }
                },
                new()
                {
                    Id = "d",
                    Date = new DateTime(2021, 06, 01),
                    Header = "Første bilde av innflytterne",
                    Body =
                        "Nå flyttes det inn for harde livet! Det ser ut som det er to forskjellige fugler som flytter inn i samme fuglekasse! Én grå og en mindre, svart.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-1/4-45-5.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-1/7-50-5.webp"
                    }
                },
                new()
                {
                    Id = "e",
                    Date = new DateTime(2021, 06, 01, 13, 00, 00),
                    Header = "Svarthvit fluesnapper",
                    Body =
                        "Etter å ha fått eksperthjelp av søstrene Ulvensøen, som har studiekompetanse i fugletitting, har vi fått avklart fugleartene. Det er svarthvit fluesnapper som har flyttet inn - hannen og hunnen ser veldig ulike ut. "
                },
                new()
                {
                    Id = "f",
                    Date = new DateTime(2021, 06, 03),
                    Header = "Strømbrudd!",
                    Body =
                        "Akkurat i den mest intense innflyttingsperioden får vi plutselig ikke lenger kontakt med fuglekassa. Sannsynligvis har strømmen gått i løpet av natten, og datamaskinen i fuglekassa har ikke greid å få kontakt med internettet igjen. Krise! " +
                        "Fuglekasseteknikeren gjør det han kan for å få reddet situasjonen. Herr og fru fluesnapper har nå bygget seg et fint rede og er nok straks klare for å legge egg",
                    ImageUrls = new List<string>
                        { "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-4/10-30-6.webp" }
                },
                new()
                {
                    Id = "g",
                    Date = new DateTime(2021, 06, 14),
                    Header = "Fuglekassa er oppe og går igjen!",
                    Body =
                        "Plutselig fikk vi kontakt med fuglekassa igjen! Og nå har det skjedd mye - Fru Fluesnapper har lagt seks fine egg! Man ser dem litt uklart i bildet her - det var en våt og grå dag på Budor Beach. Fru Fluesnapper " +
                        "ligger nå for det meste og ruger på eggene. Herr Fluesnapper har vi ikke sett på lenge. Kameraet er nå skrudd til å ta bilder hvert 5.minutt for å få med seg dramatikken.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-14/12-53-28.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-14/18-43-28.webp"
                    }
                },
                new()
                {
                    Id = "h",
                    Date = new DateTime(2021, 06, 21),
                    Header = "Nærmer vi oss klekking?",
                    Body =
                        "Det kan se ut som det nærmer seg klekking! Se på bildene hvordan det har dannet seg en liten sprekk i de øverste eggene på bare en drøy time. Kan Fru Fluesnapper være i gang med klekking?",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-21/8-32-32.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-21/9-7-32.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-21/9-2-32.webp"
                    }
                },
                new()
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
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-23/4-7-32.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-23/4-32-32.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-23/6-27-32.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-23/7-7-32.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-23/12-12-32.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-23/13-57-32.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-23/14-12-32.webp"
                    }
                },
                new()
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
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-24/5-32-32.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-24/8-20-18.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-24/8-30-18.webp"
                    }
                },
                new()
                {
                    Date = new DateTime(2021, 6, 26),
                    Id = "k",
                    Header = "Herr Fluesnapper vender tilbake!",
                    Body =
                        "I dag fikk vi et gjensyn med Herr Fluesnapper for første gang på over en uke. Han kom raskt innom for å hilse på baby-fluesnapperne sine, " +
                        "men forsvant raskt etter det. Baby-fluesnapperne har vokst mye på bare noen dager, og man kan nå blant annet se at de har fått tydelige nebb",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-26/12-28-55.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-26/15-28-55.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-26/13-38-55.webp"
                    }
                },
                new()
                {
                    Id = "l",
                    Header = "Kjærligheten blomstrer!",
                    Body =
                        "I dag fikk vi det første parbildet av Herr og Fru Fluesnapper. Herr Fluesnapper er nå oftere innom og hilser på, og det ser ut som kjærligheten blomstrer " +
                        "i fuglekassa.",
                    Date = new DateTime(2021, 06, 27),
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-27/15-53-55.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-27/8-28-55.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-27/13-3-55.webp"
                    }
                },
                new()
                {
                    Id = "m",
                    Date = new DateTime(2021, 06, 29),
                    Header = "Baby-fluesnapperne vokser fort",
                    Body =
                        "På bare seks dager har baby-fluesnapperne vokst fort, og har nå tydelige nebb og det som ser ut som begynnelsen på en fjærdrakt. Fru Fluesnapper " +
                        "ligger nå mye mindre og ruger, og er mye frem og tilbake for å mate sine håpefulle",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-29/2-23-55.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-29/4-58-55.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-6-29/6-13-55.webp"
                    }
                },
                new()
                {
                    Id = "n",
                    Date = new DateTime(2021, 07, 05),
                    Header = "Snart flyedyktige?",
                    Body =
                        "Baby-fluesnapperne har nå fått en tydelig fjærdrakt, og det er nesten ikke plass til hele familien Fluesnapper i fuglekassa lenger. Kommer de snart til å forlate redet? ",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-5/5-42-44.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-5/9-32-44.webp"
                    }
                },
                new()
                {
                    Id = "o",
                    Date = new DateTime(2021, 07, 07),
                    Header = "Utålmodige tenårings-fluesnappere",
                    Body =
                        "Det er nå mye aktivitet blant baby-fluesnapperne, som ikke lengre er babyer, men ordentlige tenåringer. De ligger sjeldnere og sover, og begynner " +
                        "heller å prøve ut vingene sine. Kanskje første flytur skjer i løpet av uka?",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-7/9-12-44.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-7/10-47-44.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-7/7-57-44.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-7/8-22-44.webp"
                    }
                },
                new()
                {
                    Id = "p",
                    Date = new DateTime(2021, 07, 09),
                    Header = "Fluesnapperne har forlatt redet",
                    Body =
                        "I går og i dag tidlig forlot fluesnapperne redet én etter én. Hvordan den første flyveturen gikk vet vi ikke, men vi håper det gikk bra. Nå håper vi de finner veien tilbake neste år også.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-9/4-52-23.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-9/5-17-23.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-9/5-27-23.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-9/6-17-23.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-9/6-27-23.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-9/6-32-23.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2021-7-9/6-37-23.webp"
                    }
                },
                new()
                {
                    Id = "q",
                    Date = new DateTime(2022, 04, 13),
                    Header = "Klar for ny fluesnapper-sesong!",
                    Body =
                        "Påsken er kommet, og fuglekassa har våknet til liv igjen etter å ha vært skrudd ned i noen måneder. Fuglekassa er nå tømt og innflyttingsklar. Vi håper å få et gjensyn med Herr og Fru Fluesnapper etter hvert som det blir varmere i været.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-4-13/11-25-56.webp"
                    }
                },
                new()
                {
                    Id = "r",
                    Date = new DateTime(2022, 04, 26),
                    Header = "Fluesnapperen er tilbake på visning!",
                    Body =
                        "I dag har fluesnapperne kommet tilbake! Dette er mye tidligere enn i fjor, da de kom først i starten av juni. Men tydeligvis har de startet å gå på fuglekasse-visning tidligere i år. Håper de finner seg til rette og har finansieringsbeviset i orden",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-4-26/11-14-11.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-4-26/9-29-11.webp"
                    }
                },
                new()
                {
                    Id = "s",
                    Date = new DateTime(2022, 04,
                        28),
                    Header = "Innflyttingen er i gang!",
                    Body =
                        "Det ble en rask budrunde, og Herr Fluesnapper er nå stolt eier av fuglekassa i Størigardsvegen. Nå flyttes det inn for harde livet, og på bare noen timer har han laget til en myk og fin madrass til Fru Fluesnapper",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-4-27/6-29-11.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-4-27/6-44-11.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-4-27/8-59-11.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-4-27/9-29-11.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-4-27/15-14-11.webp"
                    }
                },
                new()
                {
                    Id = "t",
                    Date = new DateTime(2022, 05, 02),
                    Header = "Bygningsarbeid pågår",
                    Body =
                        "De siste dagene har Herr Fluesnapper virkelig stått på. Se hvordan han har bygget et lunt og godt rede for sin fremtidige Fluesnapperkone: Først et lag med det som ser ut som lyng, deretter ser det ut som han har nappet av fjær for ekstra isolering",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-4-29/9-44-11.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-4-30/9-44-11.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-4-30/13-44-11.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-3/9-14-11.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-3/3-44-11.webp"
                    }
                },
                new()
                {
                    Id = "u",
                    Date = new DateTime(2022, 05, 10),
                    Header = "Nå mangler bare Fru Fluesnapper",
                    Body =
                        "Nå ser det ut som Herr Fluesnapper er fornøyd med redet sitt. Det ser veldig mykt og fint ut. Så nå mangler bare en Fru Fluesnapper. Herr Fluesnapper er nå mye sjeldnere innom fuglekassa, så alt tyder på at han er på sjekkern langs Størigardsvegen.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-8/11-59-34.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-10/6-59-34.webp"
                    }
                },
                new()
                {
                    Id = "v",
                    Date = new DateTime(2022, 05, 12),
                    Header = "Jomfrufødsel fra Herr Fluesnapper?",
                    Body =
                        "I dag kom det egg i fuglekassa! Men hvem som har lagt egget er et mysterium. Vi har ikke sett noe til besøk fra Fru Fluesnapper, og selve fødselen må ha skjedd veldig fort i morgentimene. Kameraet har bare greid å plukke opp det svart-hvite hodet til Herr Fluesnapper, så sannsynligvis må dette være verdens første dokumenterte jomfrufødsel fra enn mannlig fluesnapper. Sensasjon!",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-12/3-29-34.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-12/2-59-34.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-12/3-14-34.webp"
                    }
                },
                new()
                {
                    Id = "w",
                    Date = new DateTime(2022, 05, 17),
                    Header = "Herr Fluesnapper har fått seg kjæreste!",
                    Body =
                        "På selveste nasjonaldagen  dukket det opp en Fru Fluesnapper i fuglekassa! Det ser ut som hun ispiserer redebyggingen til Herr Fluesnapper, men har så langt kommet tilbake flere ganger, så det ser ut som hun lar seg sjarmere. Det ene egget som på uforklarig vis hadde havnet i fuglekassa har ligget uten tilsyn i mange dager. Vi begynner å tro at det er en gjøk som har vært innom. Det ser ut som det nå ligger nedgravd under litt ekstra kvist som har kommet til.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-17/8-18-39.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-17/7-23-39.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-18/3-53-39.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-17/7-43-39.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-17/14-53-39.webp"
                    }
                },
                new()
                {
                    Id = "x", Date = new DateTime(2022, 05, 26),
                    Header = "Nytt egg!",
                    Body =
                        "I dag tidlig kom det et nytt egg i fuglekassa! Og denne gangen ser det ut som det er Fru Fluesnapper som har lagt det. Hun har ikke vært så mye innom de siste dagene, men vannet gikk tydeligvis i dag morges. Vi gratulerer så mye!",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-26/4-43-39.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-26/5-23-39.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-26/4-48-39.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-5-26/4-58-39.webp"
                    }
                },
                new()
                {
                    Id = "y",
                    Date = new DateTime(2022, 06, 03),
                    Header = "Hektiske dager på fødestua",
                    Body =
                        "De siste dagene har vært hektiske for Fru Fluesnapper. Hver morgen har hun måttet haste seg inn i fuglekassa for å legge et nytt egg. Seks egg har hun lagt nå. Det er like mange som hun la i fjor.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-6-3/4-21-1.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-6-3/6-41-1.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-6-3/5-31-1.webp"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "z",
                    Date = new DateTime(2022, 06, 13),
                    Header = "Ruging pågår",
                    Body =
                        "Fru Fluesnapper har nå lagt et egg nesten hver eneste morgen, og har fått syv fine egg. Hun ligger nå betydelig mer og ruger enn tidligere, da hun ofte lot eggene ligge for seg selv. Forhåpentligvis betyr det at det nærmer seg klekking! Vi har dessverre hatt ustabil forbindelse til fuglekassa i det siste. Dels har dette vært på grunn av at internett-tilgangen har falt ut, og dels er det fordi en litt for ivrig fuglekasse-tekniker har prøvd å få til direktesendt video fra fuglekassa, og derfor har måttet deaktivere kamera-funkjsonen for å teste det ut. Pr nå har vi ikke lenger kontakt med fuglekassa, men vi krysser fingrene for at vi får den tilbake. ",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-6-13/10-26-23.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-6-13/9-19-46.webp"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "za",
                    Date = new DateTime(2022, 06, 21),
                    Header = "Småsnapper-livet har startet",
                    Body =
                        "Etter en uke uten kontakt med fuglekassa har den våknet til liv igjen, og fluesnapper-familien har blitt en ordentlig storfamilie i mellomtiden. Alle eggene er nå klekket, og Herr og Fru Fluesnapper er travelt opptatt med å ruge og fine mat til sine små håpefulle.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-6-21/5-22-33.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-6-21/4-47-33.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-6-21/4-57-33.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-6-21/4-37-33.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-6-21/4-22-33.webp"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "zb",
                    Date = new DateTime(2022, 08, 05),
                    Header = "Rapport fra fuglekasse-teknikeren",
                    Body =
                        "På grunn av diverse tekniske problemer var dessverre forbindelsen til fuglekassa nede da årets mini-fluesnappere vokste opp og forlot redet. Nå har vi endelig fått en fuglekasse-tekniker på plass for å få fuglekassa på beina, og den er nå oppe og går igjen. Igjen lå bare et ensomt egg som aldri ble klekt. Kanskje det var egget fra Herr Fluesnappers jomfrufødsel? Egget er i hvert fall mye mindre i virkeligheten enn det ser ut som på bildet. Nå er også fuglekassa oppdatert med nytt og bedre kamera. Se så fine bilder det blir nå! Fluesnapper-entusiast-miljøet gleder seg allerede til neste sesong!",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2022-8-5/egg.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-4-20/12-31-29.webp"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "zc",
                    Date = new DateTime(2023, 04, 21),
                    Header = "The return of the fluesnapper",
                    Body =
                        "I dag returnerte plutselig fluesnapperne til fuglekassa fra sydenferie. Både herr og fru dukket opp på første visning, og kjenner vi dem rett har de finansieringsbeviset i orden og er klare for å flytte inn på kort varsel",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-4-21/8-16-18.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-4-21/8-17-51.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-4-21/8-24-54.webp"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "zd",
                    Date = new DateTime(2023, 05, 11),
                    Header = "På tide å starte byggearbeidet?",
                    Body =
                        "Det har vært helt stille i fuglekassa nå i nesten tre uker, men i dag kom Herr Fluesnapper innom for en rask befaring. Vi håper det betyr at redebyggingen kan begynne",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-5-11/5-54-34.webp"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "ze",
                    Date = new DateTime(2023, 05, 15),
                    Header = "Et rede blir til",
                    Body =
                        "I løpet av de siste to dagene har det vært hektisk byggeaktivitet i fuglekassa. Det ser ut som det er Fru Fluesnapper som har stått for mesteparten av redebyggingen. Se hvordan utviklingen i byggearbeidet har vært",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-5-13/8-54-34.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-5-13/5-54-34.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-5-13/9-54-34.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-5-13/11-54-34.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-5-14/6-54-34.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-5-14/7-54-34.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-5-16/11-54-34.webp"
                    },
                },
                new NewsFeedEntry
                {
                    Id = "zf",
                    Date = new DateTime(2023, 05, 25),
                    Header = "Lite aktivitet i det nye redet",
                    Body =
                        "Etter byggearbeidet stilnet har det vært lite aktivitet i fuglekassa. Fru Fluesnapper er en sjelden gang innom, men mesteparten av tiden står redet tomt. Vi håper hun ikke velger seg et annet rede",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-5-25/4-54-34.webp"
                    },
                },
                new NewsFeedEntry
                {
                    Id = "zg",
                    Date = new DateTime(2023, 06, 01),
                    Header = "Fødelsboom i fuglekassa",
                    Body =
                        "De siste dagene har Fru Fluesnapper levert ett nytt egg som en klokke. Hver morgen i 6-tiden har hun lagt et nytt egg. Nå har hun lagt seks fine egg, og ligger nå og ruger. I fjor la hun sitt første egg 26.mai, i år kom det 27.mai. Snakk om pålitelig Fluesnapper!",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-5-27/5-54-34.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-5-28/5-54-34.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-5-29/6-54-34.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-5-30/7-54-34.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-5-31/7-54-34.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-6-1/8-34-54.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-6-1/9-34-53.webp"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "zh",
                    Date = new DateTime(2023, 06, 09),
                    Header = "Siste ruge-innspurt?",
                    Body =
                        "Nå har Fru Fluesnapper ligget og ruget i åtte dager på eggene sine. Det betyr at det er 14 dager siden hun la det første egget. Det må da bety at det snart er tid for klekking? Vi følger spent med",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-6-9/5-14-9.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-6-8/12-14-4.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-6-8/8-14-4.webp"
                    },
                },
                new NewsFeedEntry
                {
                    Id = "zi",
                    Date = new DateTime(2023, 06, 15),
                    Header = "Klekking!",
                    Body =
                        "I morgentimene i dag klekket eggene. Det ser ut som Fru Fluesnapper har hakket hull i egget for å hjelpe baby-snapperne, som ser veldig små og hjelpesløse ut. I dag er også første gang Herr Fluesnapper er sett på lang tid. Han har tydeligvis fått meldingen om at hans baby-snappere har kommet til verden",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-6-15/5-21-44.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-6-15/4-51-44.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-6-15/12-36-55.webp"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "zj",
                    Date = new DateTime(2023, 06, 22),
                    Header = "Hurtigvoksende baby-snappere",
                    Body =
                        "Baby-snapperne har vokst fort de siste dagene. De har nå fått seg tydelige nebb, og bittesmå vinger. Det har også vist seg at det er ett egg som ikke er blitt klekket. Fru Fluesnapper snapper nå så mange fluer hun kan for å mette barna sine. ",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-6-23/6-7-23.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-6-23/5-52-23.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-6-23/8-7-35.webp"
                    },
                },
                new NewsFeedEntry
                {
                    Id = "zk",
                    Date = new DateTime(2023, 06, 26),
                    Header = "Tenåringer i hus",
                    Body =
                        "Baby-snapperne har kommet i tenårene, og har nå tydelige vinger og fjærdrakt. Det er også mye mer aktivitet i fuglekassa - det er tydelig at det nok nærmer seg utflytting",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-6-26/5-37-35.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-6-26/8-52-35.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-6-26/9-7-35.webp"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "zl",
                    Date = new DateTime(2023, 6, 30),
                    Header = "Klare for å forlate redet",
                    Body =
                        "Nå er det like før fluesnapperne forlater redet! De har nå en tydelig fjærdrakt, og ser klare ut for voksenlivet.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-6-30/9-49-42.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-6-30/10-49-42.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-6-30/12-19-42.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-7-1/7-49-42.webp"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "zm",
                    Date = new DateTime(2023, 7, 1),
                    Header = "Sees neste år!",
                    Body =
                        "I dag forsvant fluesnapperne én etter én. Håper den første flyveturen gikk bra, og at dere kommer tilbake neste år!",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-7-1/3-19-42.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-7-1/3-34-42.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-7-1/5-19-42.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-7-1/6-4-42.webp",
                        "https://budorbeach.blob.core.windows.net/images-thumbnails/2023-7-1/8-19-42.webp"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "zn",
                    Date = new DateTime(2024, 05, 21),
                    Header = "Familiebyggingen er i gang",
                    Body =
                        "Etter diverse oppstartsproblemer har vi akkurat i tide fått kontakt med kameraet i fuglekassa igjen. Og der har det skjedd mye! Et nytt rede er bygd, og det er kommet to fine egg. Det ser igjen ut som det er svarthvit fluesnapper som har slått seg til ro i fuglekassa på Budor.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images/2024-05-21/16-21-20.jpg"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "zo",
                    Date = new DateTime(2024, 05, 24),
                    Header = "Nye egg",
                    Body =
                        "Fru Fluesnapper leverer nye egg som bestilt. Hver morgen har det kommet et nytt egg i redet. Fem egg ser det ut som det skal bli i år. Det siste egget ble lagt i dag. Etter å ha vært mye frem og tilbake, er hun nå mye mer inne og ruger på eggene.",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images/2024-05-22/08-50-03.jpg",
                        "https://budorbeach.blob.core.windows.net/images/2024-05-23/11-32-13.jpg",
                        "https://budorbeach.blob.core.windows.net/images/2024-05-24/05-00-01.jpg",
                        "https://budorbeach.blob.core.windows.net/images/2024-05-23/16-00-01.jpg",
                        "https://budorbeach.blob.core.windows.net/images/2024-05-24/06-00-01.jpg"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "zp",
                    Date = new DateTime(2024, 06, 03),
                    Header = "Tålmodig fluesnapper-ruging",
                    Body =
                        "Fru Fluesnapper ligger nå hele dagen og ruger på sine fem egg. Herr Fluesnapper har vi enda ikke sett",
                    ImageUrls = new List<string>
                    {
                        "https://budorbeach.blob.core.windows.net/images/2024-05-28/08-00-01.jpg",
                        "https://budorbeach.blob.core.windows.net/images/2024-05-28/16-00-01.jpg"
                    }
                },
                new NewsFeedEntry
                {
                    Id = "zq",
                    Date = new DateTime(2024, 06, 07, 05, 00, 00),
                    Header = "Klekking!",
                    Body =
                        "I morgentimene i dag begynte eggene å klekke. Og hvem andre enn Herr Fluesnapper kom flyvende til fødestua for å hjelpe til. Foreløpig ser det ut som det bare er to egg som er klekket, men flere vil nok klekke i løpet av dagen. Fru Fluesnapper har nå ruget i 14 dager siden hun la det siste egget. Nå er det full aktivitet med mating av baby-fluesnappere.",
                    ImageUrls = new List<string>
                        { "https://budorbeach.blob.core.windows.net/images/2024-06-07/03-00-01.jpg" },
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