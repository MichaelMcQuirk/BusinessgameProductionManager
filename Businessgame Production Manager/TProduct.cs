using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication1
{
    public class TProduct
    {
        public string name = "";
        public double price = 0;
        public double minimum = 0;
        public double maximum = 0;
        public double amount = 0;
        //history here
        public double OriginalPrice = 0;
        public double PreviousePrice = 0;
        public List<double> PriceHistory = new List<double>();
        public List<double> BuyHistory = new List<double>();
        public List<double> SellHistory = new List<double>();
        public double BuysThisRound = 0;
        public double SalesThisRound = 0;

        public void NewRound()
        {
            PriceHistory.Insert(0,price);
            if (PriceHistory.Count > 8) PriceHistory.RemoveAt(8);
            BuyHistory.Insert(0, BuysThisRound);
            if (BuyHistory.Count > 8) BuyHistory.RemoveAt(8);
            SellHistory.Insert(0, SalesThisRound);
            if (SellHistory.Count > 8) SellHistory.RemoveAt(8);
            BuysThisRound = 0;
            SalesThisRound = 0;
            PreviousePrice = price;
        }

        public TProduct(string _name = "", double _price = 0, double _minimum = 0, double _maximum = 0, double _amount = 0)
        {
            name = _name;
            price = _price;
            minimum = _minimum;
            maximum = _maximum;
            amount = _amount;
        }

        
    }
}
