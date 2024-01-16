using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using CrawlingTask.Models;
using HtmlAgilityPack;
using Microsoft.IdentityModel.Tokens;

namespace CrawlingTask
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            List<Auction> count = new List<Auction>();
            string webUrl = "https://ineichen.com/auctions/past/";
            HtmlDocument htmlDocument = new HtmlDocument();
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    await BuildHtmlDocument(webUrl, client, htmlDocument);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }
            // Auction Cards Collection
            List<Auction> auctionList = new List<Auction>();
            var auctionCards = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class, 'auction-item') and not(ancestor::div[contains(@class, 'auction-item')])]");
            foreach (var Card in auctionCards)
            {
                Auction auctionObj = new Auction();

                // Extract Title
                auctionObj.Title = await GetAuctionTitle(Card);

                // Extract Description
                auctionObj.Description = GetAuctionDescription(Card);

                // Set Start And End Dates
                var dateAndLocation = Card.SelectSingleNode($"div[contains(@class, 'auction-date-location')]");
                var dateItem = dateAndLocation.SelectSingleNode($"div[contains(@class, 'auction-date-location__item')]");
                SetAuctionDates(auctionObj, dateItem.InnerText.Trim());

                // Extract Image-Url
                auctionObj.ImageUrl = GetAuctionImageURL(Card);

                // Extract Link
                auctionObj.Link = GetAuctionLink(Card);

                // Extract LotCount
                auctionObj.LotCount = GetAuctionLotCount(Card);

                //Extract Location
                auctionObj.Location = GetAuctionLocation(Card);

                auctionList.Add(auctionObj);
            }
            using (IneichenContext Context = new IneichenContext())
            {
                foreach (var auction in auctionList)
                {
                    Context.Auctions.Add(auction);
                    Context.SaveChanges();
                }
            }
        }
        public static async Task BuildHtmlDocument(string url, HttpClient client, HtmlDocument htmlDocument)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string htmlContent = await response.Content.ReadAsStringAsync();
                // Parse the HTML content using HtmlAgilityPack
                htmlDocument.LoadHtml(htmlContent);
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }
        public static async Task<string> GetAuctionTitle(HtmlNode? Card)
        {
            string title = "";
            if (Card != null)
            {
                var titleHtml = Card.SelectSingleNode($"div[contains(@class, 'auction-item__title')]");
                if (titleHtml != null)
                {
                    await Console.Out.WriteLineAsync(titleHtml.InnerText.Trim());
                    title = titleHtml.InnerText.Trim();
                }
                else
                    await Console.Out.WriteLineAsync("Title not found");
            }
            return title;
        }
        public static int? GetAuctionLotCount(HtmlNode? Card)
        {
            int? lotCount = null;
            if (Card != null)
            {
                var linkButton = Card.SelectSingleNode($"div[contains(@class, 'auction-item__btns')]/a");
                var btnText = linkButton.InnerText.Trim();
                lotCount = ExtractNumFromStr(btnText);
            }
            return lotCount;
        }
        public static string? GetAuctionLocation(HtmlNode? Card)
        {
            string? location = null;
            if (Card != null)
            {
                var dateAndLocation = Card.SelectSingleNode($"div[contains(@class, 'auction-date-location')]");
                var locationItem = dateAndLocation.SelectSingleNode($"div[contains(@class, 'auction-date-location__item')][2]");
                var locationText = locationItem.InnerText.Trim();
                string[] locationSplits = Regex.Split(locationText, @"((Auction,)|(Auction \|))");
                location = locationSplits[locationSplits.Length - 1];
            }
            return location;
        }
        public static string? GetAuctionLink(HtmlNode? Card)
        {
            string? link = null;
            if (Card != null)
            {
                var linkButton = Card.SelectSingleNode($"div[contains(@class, 'auction-item__btns')]/a");
                link = linkButton.GetAttributeValue("href", null).Trim();
            }
            return link;
        }
        public static string? GetAuctionDescription(HtmlNode? Card)
        {
            string? description = null;
            if (Card != null)
            {
                var dateAndLocation = Card.SelectSingleNode($"div[contains(@class, 'auction-date-location')]");
                var dateAndLocationItems = dateAndLocation.SelectNodes($"div[contains(@class, 'auction-date-location__item')]");
                string dateLocationConcat = "";
                foreach (var item in dateAndLocationItems)
                {
                    // Replace newLine in datetimelocation items with space
                    string result = Regex.Replace(item.InnerText.Trim(), @"\r\n?|\n", " ");
                    dateLocationConcat += result + "\n";
                }
                Console.WriteLine("DESC" + dateLocationConcat);
                description = dateLocationConcat;
            }
            return description;
        }
        public static string? GetAuctionImageURL(HtmlNode? Card)
        {
            string? imageURL = null;
            if (Card != null)
            {
                var imageItem = Card.SelectSingleNode($"a[contains(@class, 'auction-item__image')]/img");
                var urlStr = imageItem.GetAttributeValue("src", null);
                if (urlStr.IsNullOrEmpty()) imageURL = null;
                else imageURL = urlStr.Trim();
            }
            return imageURL;
        }
        public static void SetAuctionDates(Auction auction, string inputDateString)
        {
            DateClass? startDateObj = null, endDateObj = null;

            SetAuctionDatesHelper(inputDateString, out startDateObj, out endDateObj);
            if (startDateObj == null) return;
            auction.StartDate = startDateObj.date;
            auction.StartMonth = startDateObj.month;
            auction.StartYear = startDateObj.year;
            auction.StartTime = startDateObj.time;
            if (endDateObj == null) return;
            auction.EndDate = endDateObj.date;
            auction.EndMonth = endDateObj.month;
            auction.EndYear = endDateObj.year;
            auction.EndTime = endDateObj.time;
            return;
        }
        public static void SetAuctionDatesHelper(string inputDateString, out DateClass? startDateObj, out DateClass? endDateObj)
        {
            string startDateString = "", endDateString = "";
            SeparateDates(inputDateString, out startDateString, out endDateString);
            Console.WriteLine("START DATE STRING " + startDateString);
            startDateObj = GetDateTime(startDateString.Trim());
            if (startDateObj == null)
            {
                endDateObj = null;
                return;
            }
            Console.WriteLine("END DATE STRING " + endDateString);
            endDateObj = GetDateTime(endDateString.Trim());
        }
        public static void SeparateDates(string inputDates, out string startDateString, out string endDateString)
        {
            string[] split = inputDates.Split(Regex.Match(inputDates, @"(\(CET\))|(CET)|-").Groups[0].Value);
            startDateString = split[0];
            if (split.Length > 1)
            {
                endDateString = split[1];
            }
            else
                endDateString = "";
            return;
        }
        public static DateClass? GetDateTime(string dateInnerText)
        {
            DateClass dateObj = new DateClass();
            Regex regexDateExists = new Regex(@"^\d{1,2}");
            Regex regexStartMonth = new Regex(@"\b[A-Za-z]+\b");
            Regex regexStartYear = new Regex(@"([0-9]{4})");
            Regex regexStartTime = new Regex(@"(([0-9]{2}):([0-9]{2}))");
            if (regexDateExists.IsMatch(dateInnerText))
            {
                var match = regexDateExists.Match(dateInnerText);
                dateObj.date = ExtractNumFromStr(match.Groups[0].Value);
                Console.WriteLine("DATE " + match.Groups[0].Value);
                if (regexStartMonth.IsMatch(dateInnerText))
                {
                    var startMonthMatch = regexStartMonth.Match(dateInnerText);
                    int monthNum = GetMonthNumber(startMonthMatch.Groups[0].Value);
                    dateObj.month = monthNum;
                    Console.WriteLine("MONTH " + GetMonthNumber(startMonthMatch.Groups[0].Value));
                }
                if (regexStartYear.IsMatch(dateInnerText))
                {
                    var startYearMatch = regexStartYear.Match(dateInnerText);
                    int yearNum = Int32.Parse(startYearMatch.Groups[0].Value);
                    dateObj.year = yearNum;
                    Console.WriteLine("YEAR " + startYearMatch.Groups[0].Value);
                }
                if (regexStartTime.IsMatch(dateInnerText))
                {
                    var startTime = regexStartTime.Match(dateInnerText);
                    dateObj.time = TimeOnly.Parse(startTime.Groups[0].Value);
                    Console.WriteLine("TIME " + startTime.Groups[0].Value);
                }
            }
            else
            {
                Console.WriteLine("NO DATES");
                return null;
            }
            return dateObj;
        }

        public static int GetMonthNumber(string monthString)
        {
            // Use DateTime.ParseExact to parse the month string and extract the month number
            DateTime parsedDate;
            if (DateTime.TryParseExact(monthString, "MMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate) ||
            DateTime.TryParseExact(monthString, "MMMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                return parsedDate.Month;
            }
            else
            {
                // Handle parsing failure
                Console.WriteLine($"Failed to parse month: {monthString}");
                return -1; // Or any other suitable value or throw an exception
            }
        }
        public static int? ExtractNumFromStr(string str)
        {
            if (Regex.IsMatch(str, @"\d+"))
            {
                int num = Int32.Parse(Regex.Match(str, @"\d+").Value);
                return num;
            }
            return null;
        }
    }
    public class DateClass
    {
        public int? date { get; set; }
        public int? month { get; set; }
        public int? year { get; set; }
        public TimeOnly? time { get; set; }
    }
}
