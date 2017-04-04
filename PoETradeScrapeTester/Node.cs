using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoETradeScrapeTester
{
    class Node
    {
        public struct Edge
        {
            private float rate;
            private string currencyItem;
            private Node vertex;

            public float Rate { get { return rate; } set { rate = value; } }
            public string CurrencyItem { get { return currencyItem; } set { currencyItem = value; } }
            public Node Vertex { get { return vertex; } set { vertex = value; } }
        }
        //*
    }
}
