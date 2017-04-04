using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoETradeScrapeTester {
    public class GraphNode {
        public List<Edge> nodeEdges;

        private GraphNode parent;

        private float f;
        private float g;
        private float h;
        private string dataid;
        private string currencyItem;

        public GraphNode Parent { get { return parent; } set { parent = value; } }

        public float fScore { get { return f; } set { f = value; } }
        public float gScore { get { return g; } set { g = value; } }
        public float heuristic { get { return h; } set { h = value; } }
        public string Dataid { get { return dataid; } set { dataid = value; } }
        public string CurrencyItem { get { return currencyItem; } set { currencyItem = value; } }

        public struct Edge {
            private float rate;
            private string currencyItem;

            public float Rate { get { return rate; } set { rate = value; } }
            public string CurrencyItem { get { return currencyItem; } set { currencyItem = value; } }
        }

        public GraphNode() {
            nodeEdges = new List<Edge>();
            fScore = 0;
            gScore = 0;
            heuristic = 0;
            Parent = null;
        }

        public GraphNode(string dataid, string currencyItem) {
            Dataid = dataid;
            CurrencyItem = currencyItem;
            nodeEdges = new List<Edge>();
            fScore = 0;
            gScore = 0;
            heuristic = 0;
            Parent = null;
        }

        public void addEdge(float rate, string currencyItem) {
            Edge newEdge = new Edge();
            newEdge.Rate = rate;
            newEdge.CurrencyItem = currencyItem;
            nodeEdges.Add(newEdge);
        }
    }
}
