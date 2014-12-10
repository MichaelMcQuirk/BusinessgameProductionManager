using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication1
{
    public class TSchedualedSale : IComparable<TSchedualedSale>
    {
        public int RemainingTime;
        public String ProductName;
        public int Amount;

        public int CompareTo(TSchedualedSale other)
        {
            return this.RemainingTime.CompareTo(other.RemainingTime);
        }

        public TSchedualedSale(int remTime, String prodName, int amt)
        {
            RemainingTime = remTime;
            ProductName = prodName;
            Amount = amt;
        }
    }
}
