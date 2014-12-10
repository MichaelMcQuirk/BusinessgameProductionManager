using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WindowsFormsApplication1
{
    public class TPerson
    {
        public List<TProduct> Product = new List<TProduct>();
        public List<TSector> Sector = new List<TSector>();

        public TSector MySector;

        public string MyID = "";
        public double Cash = 850000;

        public double SpareMoneyToKeep = 0;
        public int InXMinutes = 1;
        public int LogInInterval = 10;

        TMarket Market;

        public int NextLogin = 0;

        public int Warehouses = 0;
        public int Processors = 0;

        public double NetWorth = 0;


        public double CashRestrictionInXMinutes(int X)
        {
            if (X >= InXMinutes) X = InXMinutes - 1;//to prevent 0 / 60.
            return SpareMoneyToKeep / Math.Ceiling((InXMinutes - X) / 60.0);
        }

        public int Hours(int i)
        {
            return 60 * i;
        }

        public int RemainingProductionTime(int predictedExtraUnits = 0)
        {
            double requiredGoods = -1;//per hour
            int id = -1;
            if (MySector.Input.Count != 0)
            {
                id = Market.getProductID(MySector.Input[0].name);
                requiredGoods = MySector.Input[0].amount * (MySector.units + predictedExtraUnits);
            }
            if (id != -1)
            {
                double avalableGoods = Product[id].amount;
                return (int)Math.Floor((avalableGoods / requiredGoods) * 60);
            }
            else
            {
                return 10000;
            }
        }

        public int AvalableStorageSpace()
        {
            int totalStorage = 0;
            foreach (TProduct product in Product)
            {
                totalStorage += (int)Math.Floor(product.amount);
            }
            return Warehouses * Market.WarehouseCapacity - totalStorage;
        }

        public int TotalStorageSpace()
        {
            return Warehouses * Market.WarehouseCapacity;
        }

        public int StorageSpaceRemainingPerc()
        {
            return (int)Math.Floor(100 * (AvalableStorageSpace() / (TotalStorageSpace() * 1.0)));
        }

        public double getNewUnitPrice(TSector sector)
        {
            double newUnitCost = 0;
            foreach (TProduct Machine in sector.Machinery)
            {
                newUnitCost += Market.Product[Market.getProductID(Machine.name)].price * Machine.amount;
            }
            newUnitCost += sector.price;
            return newUnitCost * Market.TransportCost;
        }


        public TPerson(int ID, List<TProduct> _product, List<TSector> _sector, TMarket _market, double StartingCash)
        {
            Market = _market;
            Cash = StartingCash;
            MyID = ID.ToString();
            foreach (TProduct product in _product)
            {
                Product.Add(new TProduct(product.name, product.price, product.minimum, product.maximum, 0));
            }

            foreach (TSector sector in _sector)
            {
                List<TProduct> Machinery = new List<TProduct>();
                List<TProduct> Input = new List<TProduct>();
                List<TProduct> Output = new List<TProduct>();
                foreach (TProduct product in sector.Input)
                {
                    Input.Add(new TProduct(product.name, product.price, product.minimum, product.maximum, product.amount));
                }
                foreach (TProduct product in sector.Machinery)
                {
                    Machinery.Add(new TProduct(product.name, product.price, product.minimum, product.maximum, product.amount));
                }
                foreach (TProduct product in sector.Output)
                {
                    Output.Add(new TProduct(product.name, product.price, product.minimum, product.maximum, product.amount));
                }
                Sector.Add(new TSector(sector.name, sector.price, sector.employees, sector.fixedprice, sector.units, Machinery, Input, Output));
            }

            int iRandomSector = ID;
            MySector = Sector[Market.getSectorID("Wind turbines assembly")];//assigns AI to a specific sector
            Market.Log("New Player: " + ID.ToString() + " - " + MySector.name);
        }
    }
}