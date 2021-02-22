using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace WebScrapingConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            GetHtmlAsync();
            Console.ReadLine();
        }

        private static async void GetHtmlAsync()
        {
            var url = "https://www.ebay.com/sch/i.html?_from=R40&_nkw=xbox+series+x&_in_kw=1&_ex_kw=&_sacat=0&LH_Complete=1&_udlo=&_udhi=&_ftrt=901&_ftrv=1&_sabdlo=&_sabdhi=&_samilow=&_samihi=&_sadis=15&_stpos=&_sargn=-1%26saslc%3D1&_salic=1&_sop=13&_dmd=1&_ipg=50&_fosrp=1";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var productsHtml = htmlDocument.DocumentNode.Descendants("ul")
                .Where(n => n.GetAttributeValue("id", "")
                    .Equals("ListViewInner")).ToList();

            var productListItems = productsHtml[0].Descendants("li")
                .Where(n => n.GetAttributeValue("id", "")
                    .Contains("item")).ToList();

            foreach (var productListItem in productListItems)
            {
                // Id
                Console.WriteLine(productListItem.GetAttributeValue("listingid", ""));
                
                // Product name
                Console.WriteLine(productListItem.Descendants("h3")
                    .Where(n => n.GetAttributeValue("class", "")
                        .Equals("lvtitle")).FirstOrDefault().InnerText.Trim('\r', '\n', '\t'));
                
                // Price
                Console.WriteLine(Regex.Match(productListItem.Descendants("li")
                    .Where(n => n.GetAttributeValue("class", "")
                        .Equals("lvprice prc")).FirstOrDefault().InnerText.Trim('\r', '\n', '\t')
                ,@"\d+,\d+.\d+"));
                
                // Url
                Console.WriteLine(productListItem.Descendants("a")
                    .FirstOrDefault().GetAttributeValue("href", "").Trim('\r', '\n', '\t'));
                
                Console.WriteLine();
            }
        }
    }
}