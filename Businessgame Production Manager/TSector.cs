using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication1
{
    class TSector
    {
        public string name = "";
        public double price = 0.00;
        public double employees = 0.00;
        public double fixedprice = 0.00;
        public int units = 0;
        public List<TProduct> Machinery = new List<TProduct>();
        public List<TProduct> Input = new List<TProduct>();
        public List<TProduct> Output = new List<TProduct>();

        //For Extra Features
        public int unitsBeforeBuyMode = 0;

        public TSector(string _name = "", double _price = 0, double _employees = 0, double _fixedprice = 0, int _units = 0, List<TProduct> _machinery = null, List<TProduct> _input = null, List<TProduct> _output = null)
        {
            name = _name;
            price = _price;
            employees = _employees;
            fixedprice = _fixedprice;
            units = _units;
            if (_machinery != null) Machinery = _machinery;
            if (_input != null) Input = _input;
            if (_output != null) Output = _output;
        }
    }
}
