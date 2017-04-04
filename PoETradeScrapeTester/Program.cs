using PoETradeScrape.Scraper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoETradeScrapeTester
{
    class Program {
        static void Main(string[] args) {
            PoEHTMLScraper scraper = new PoEHTMLScraper();
            scraper.Scrape();

            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
        }
    }
}
