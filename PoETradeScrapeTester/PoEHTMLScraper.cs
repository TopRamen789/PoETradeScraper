using System;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using HtmlAgilityPack;
using Newtonsoft.Json;
using PoETradeScrapeTester;
using System.Globalization;

namespace PoETradeScrape.Scraper
{
    public struct Currency {
        public string dataid;
        public string currencyitem;
    }

    public class PoEHTMLScraper {
        List<Currency> currencyList;
        List<GraphNode> graph = new List<GraphNode>();

        public PoEHTMLScraper() {
            LoadJSON();
        }

        public void Scrape() {
            GraphNode goal = new GraphNode();
            foreach(Currency c in currencyList.Reverse<Currency>()) {
                GraphNode currentNode = new GraphNode(c.dataid, c.currencyitem);
                if(c.currencyitem == "Chaos_Orb") {
                    goal.Dataid = c.dataid;
                    goal.CurrencyItem = c.currencyitem;
                }
                foreach(Currency x in currencyList) {
                    if(c.dataid != x.dataid) {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(getPoETradeBody(x.dataid, c.dataid));
                        doc.Save("poetradehtml/Want " + x.currencyitem + " Have " + c.currencyitem + ".html");
                        //parseDocumentForRate(doc);
                        currentNode.addEdge((-1*returnRateFromDocument(doc)), x.currencyitem);
                    }
                }
                graph.Add(currentNode);
                //currencyList.Remove(c);
            }
            //List<GraphNode> bestResult = AStar(graph[0], goal);
            Console.WriteLine("");
        }

        /*
        public List<GraphNode> AStar(GraphNode start, GraphNode goal) {
            List<GraphNode> openList = new List<GraphNode>();
            List<GraphNode> closedList = new List<GraphNode>();
            List<GraphNode> result = new List<GraphNode>();

            openList.Add(graph.First());

            while(openList.Count > 0) {
                int lowestIndex = 0;
                for(int i = 0; i < openList.Count; i++) {
                    if(openList[i].fScore < openList[lowestIndex].fScore) { lowestIndex = i; }
                }

                GraphNode currentNode = openList[lowestIndex];

                if(currentNode == goal) {
                    while(currentNode.Parent != null) {
                        result.Add(currentNode);
                        currentNode = currentNode.Parent;
                    }
                    return result;
                }

                GraphNode valueToRemove = currentNode;
                closedList.Add(currentNode);
                openList.Remove(currentNode);

                //grab neighbors from current node
                List<GraphNode> neighbors = new List<GraphNode>();
                for(int i = 0; i < neighbors.Count; i++) {
                    GraphNode neighbor = neighbors[i];
                    if(closedList.IndexOf(neighbor) > 0) {
                        continue;
                    }

                    float gScore = currentNode.gScore + 1;
                    bool gScoreIsBest = false;

                    if(openList.IndexOf(neighbor) > 0) {
                        gScoreIsBest = true;
                        neighbor.heuristic = heuristic(neighbor, goal);
                        openList.Add(neighbor);
                    } else if(gScore < neighbor.gScore) {
                        gScoreIsBest = true;
                    }

                    if(gScoreIsBest) {
                        neighbor.Parent = currentNode;
                        neighbor.gScore = gScore;
                        neighbor.fScore = neighbor.gScore + neighbor.heuristic;
                    }
                }
            }
            return result;
        }

        public float heuristic(GraphNode a, GraphNode b) {
            //return a.nodeEdges.IndexOf(b).Rate;
            //a.nodeEdges.IndexOf(b.currencyItem);
            return 0.0f;
        }
        */

        public float returnRateFromDocument(HtmlDocument doc)
        {
            string rate = "";
            float rateF = 0.0f;
            try {
                rate = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'large-3')]")
                            .SelectSingleNode("//div[contains(@class, 'columns')]")
                            .SelectSingleNode("//div[contains(@class, 'displayoffer-centered')]")
                            .InnerText;

                rate = WebUtility.HtmlDecode(rate);
                rate = Regex.Replace(rate, @"[^0-9a-zA-Z.]+", " ");
                rate = rate.Substring(3);

                rateF = float.Parse(rate);

            } catch (Exception e) {
                //
            }

            return rateF;
        }

        public void parseDocumentForRate(HtmlDocument doc) {
            string rate = "No deals found!";
            string stock = "No stock specified!";

            try {
                rate = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'large-3')]")
                            .SelectSingleNode("//div[contains(@class, 'columns')]")
                            .SelectSingleNode("//div[contains(@class, 'displayoffer-centered')]")
                            .InnerText;
            } catch(Exception e) {
                //
            }

            try {
                //stock = doc.DocumentNode.SelectSingleNode("//div[text()='Stock:']").InnerText;
            } catch(Exception e) {
                //
            }

            rate = WebUtility.HtmlDecode(rate);
            rate = Regex.Replace(rate, @"[^0-9a-zA-Z.]+", " ");
            rate = rate.Substring(3);

            Console.WriteLine("Rate: " + rate + " Stock: " + stock);
        }

        public string getPoETradeBody(string want, string have) {
            //the url we want to hit and manipulate looks like this:
            //  currency.poe.trade/search?league=Standard&online=x&want= ( WANT )&have= ( HAVE )
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://currency.poe.trade/search?league=Legacy&online=&want=" + want + "&have=" + have);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if(response.StatusCode == HttpStatusCode.OK) {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null) {
                    readStream = new StreamReader(receiveStream);
                } else {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                string data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();
                return data;
            }

            return "Error";
        }

        public void LoadJSON() {
            using(StreamReader r = new StreamReader("currency.json")) {
                string json = r.ReadToEnd();
                currencyList = JsonConvert.DeserializeObject<List<Currency>>(json);
            }
        }

        public void OutputJSON() {
            foreach (Currency c in currencyList) {
                string value = c.dataid + ": " + c.currencyitem;
                Console.WriteLine(value);
            }
        }
    }
}