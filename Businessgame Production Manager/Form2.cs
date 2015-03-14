using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        public List<TSchedualedSale> SchedualedSales = new List<TSchedualedSale>();
        public Form1 MainForm;

        public Form2(Form1 mainForm)
        {
            InitializeComponent();
            MainForm = mainForm;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int amt = int.Parse(tbxAmount.Text);
                int time = int.Parse(tbxTime.Text);
                String prodName = cbxProductName.Text;
                String timeType = cbxTimeType.Text;
                if (timeType == "Hours")
                    time *= 60;
                if (MainForm.getProductID(prodName) == -1)
                    throw new Exception();
                SchedualedSales.Add(new TSchedualedSale(time, prodName, amt));
                SchedualedSales.Sort();
                RefreshSchedualsList();

            }
            catch (Exception a)
            {
                MessageBox.Show("An error has occured, please ensure that both the time and amount are integers.");
            }
        }

        public void RefreshSchedualsList()
        {
            lbxSchedualedSales.Items.Clear();
            for (int i = 0; i < SchedualedSales.Count; i++)
            {
                TSchedualedSale cur = SchedualedSales[i];
                lbxSchedualedSales.Items.Add(cur.RemainingTime + "m -> " + cur.ProductName + " : " + cur.Amount + " CBM");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bool salePerformed = false;
            List<TSchedualedSale> schedualsToRemove = new List<TSchedualedSale>();
            try
            {
                for (int i = 0; i < SchedualedSales.Count; i++)
                {
                    if (!salePerformed)
                        SchedualedSales[i].RemainingTime--;
                    if (SchedualedSales[i].RemainingTime == 0)
                    {
                        string name = SchedualedSales[i].ProductName;
                        int amt = SchedualedSales[i].Amount;
                        MainForm.Market.PerformAction(() => { MainForm.Market.SellProduct(name, amt); });
                        schedualsToRemove.Add(SchedualedSales[i]);
                        salePerformed = true;
                    }
                }
            }
            catch (Exception a)
            {
                MessageBox.Show("ERROR: " + a.Message);
            }

            foreach (TSchedualedSale sale in schedualsToRemove)
                SchedualedSales.Remove(sale);

            RefreshSchedualsList();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            cbxProductName.Items.Clear();
            foreach (TProduct product in MainForm.Product)
                cbxProductName.Items.Add(product.name);
            cbxProductName.Text = "Energy";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(
            String s = "";
            foreach (TSector t in MainForm.Sector)
            {
                double price = 0;
                foreach (TProduct p in t.Machinery)
                    price += p.amount * MainForm.getProductPrice(p.name);
                price += t.price;

                double profit = 0;
                foreach (TProduct p in t.Output)
                    profit += p.amount * MainForm.getProductPrice(p.name);
                foreach (TProduct p in t.Input)
                    profit -= p.amount * MainForm.getProductPrice(p.name);

                //s = s + "\n" + t.name + ": $/h " + profit + "\t Payback Hours = " + price / profit;
                s = s + "\n" + t.name;
            }
            tbxAmount.Text = s;
            MessageBox.Show(s);
        }

        private void lbxSchedualedSales_SelectedIndexChanged(object sender, EventArgs e)
        {
            

        }

        private void lbxSchedualedSales_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lbxSchedualedSales.SelectedIndex != -1)
            {
                SchedualedSales.RemoveAt(lbxSchedualedSales.SelectedIndex);
                lbxSchedualedSales.Items.RemoveAt(lbxSchedualedSales.SelectedIndex);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
