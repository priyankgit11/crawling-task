using System;
using System.Globalization;
using System.Text.RegularExpressions;
using CrawlingTask.Models;
using HtmlAgilityPack;

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
                    HttpResponseMessage response = await client.GetAsync(webUrl);
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
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }
            // Auction Cards Collection
            var auctionCards = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class, 'auction-item') and not(ancestor::div[contains(@class, 'auction-item')])]");
            List<Auction> auctionList = new List<Auction>();
            foreach (var Card in auctionCards)
            {
                Auction auctionObj = new Auction();
                // Extract Title
                var title = Card.SelectSingleNode($"div[contains(@class, 'auction-item__title')]");
                if (title != null)
                {
                    await Console.Out.WriteLineAsync(title.InnerText.Trim());
                    auctionObj.Title = title.InnerText.Trim();
                }
                else
                    await Console.Out.WriteLineAsync("Title not found");

                // Extract Description
                var dateAndLocation = Card.SelectSingleNode($"div[contains(@class, 'auction-date-location')]");
                var dateAndLocationItems = dateAndLocation.SelectNodes($"div[contains(@class, 'auction-date-location__item')]");
                string desc = "";
                foreach (var item in dateAndLocationItems)
                {
                    // Replace newLine in datetimelocation items with space
                    string result = Regex.Replace(item.InnerText.Trim(), @"\r\n?|\n", " ");
                    desc += result + "\n";
                }
                Console.WriteLine("DESC" + desc);
                var dateItem = dateAndLocation.SelectSingleNode($"div[contains(@class, 'auction-date-location__item')]");
                string startDateString = "", endDateString = "";
                SeparateDates(dateItem.InnerText.Trim(),out startDateString,out endDateString);
                GetDates(startDateString.Trim());
            }
        }
        public static void SeparateDates(string inputDates, out string startDateString, out string endDateString)
        {
            string[] split = Regex.Split(inputDates, @"(\(CET\))|(CET)|-");
            startDateString = split[0];
            if (split.Length > 1)
            {
                endDateString = split[1];
            }
            endDateString = "";
            return;
        }
        public static DateClass? GetDates(string dateInnerText)
        {
            Regex regexDateExists = new Regex(@"^\d{1,2}");
            Regex regexStartMonth = new Regex(@"\b[A-Za-z]+\b");
            Regex regexStartYear = new Regex(@"([0-4]{4})");
            Regex regexStartTime = new Regex(@"(([0-9]{2}):([0-9]{2}))");
            if (regexDateExists.IsMatch(dateInnerText))
            {
                var match = regexDateExists.Match(dateInnerText);
                Console.WriteLine("START DATE " + match.Groups[0].Value);
                if (regexStartMonth.IsMatch(dateInnerText))
                {
                    var startMonthMatch = regexStartMonth.Match(dateInnerText);
                    Console.WriteLine("START MONTH " + startMonthMatch.Groups[0].Value);
                }
                if (regexStartYear.IsMatch(dateInnerText))
                {
                    var startYearMatch = regexStartYear.Match(dateInnerText);
                    Console.WriteLine("START YEAR " + startYearMatch.Groups[0].Value);
                }
                if (regexStartTime.IsMatch(dateInnerText))
                {
                    var startTime = regexStartTime.Match(dateInnerText);
                    Console.WriteLine("START TIME " + startTime.Groups[0].Value);
                }
            }
            else
            {
                Console.WriteLine("NO DATES");
                return null;
            }
            return new DateClass();
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
    }
    public class DateClass
    {
        public int startDate { get; set; }
        public int startMonth { get; set; }
        public int startYear { get; set; }
        public TimeOnly startTime { get; set; }
        public int endDate { get; set; }
        public int endMonth { get; set; }
        public int endYear { get; set; }
        public TimeOnly endTime { get; set; }

    }
}
