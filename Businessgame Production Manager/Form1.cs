using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        #region Fields

        public string BStatus = "";

        public List<TProduct> Product = new List<TProduct>();
        public List<TSector> Sector = new List<TSector>();
        public List<TSector> SectorsToBuy = new List<TSector>();
        public List<Label> label = new List<Label>();
        public List<Color> labelColors = new List<Color>();
        public List<TrackBar> trackBar = new List<TrackBar>();
        public List<string> netProduct = new List<string>();
        public List<Label> netProduction = new List<Label>();
        public List<Label> netIncome = new List<Label>();
        public List<Label> netName = new List<Label>();
        public int iLeft = 120;
        public int iTop = 5;
        public int iTop2 = 5;
        public int iInterval = 16;
        public double totalProduction = 0;
        public double totalIncome = 0;
        public String selectedSector = "";
        public Label netTotalProduction = new Label();
        public Label netTotalIncome = new Label();
        public Label netTotalName = new Label();
        public Label netLabel = new Label();
        public Label sectorLabel = new Label();
        public TrackBar ScaleBar;
        public Boolean bSectorsDrawn = false;
        public int Multiplier = 1;

        public bool BuyMode = false;
        public List<Label> buyModeLabels1 = new List<Label>();
        public List<Label> buyModeLabels2 = new List<Label>();

        public TMarket Market;

        public Thread AutoLoadThread;
        public Label LoadingLabel1 = new Label();
        public Label LoadingLabel2 = new Label();
        public bool isLoading = false;
        public int dotCount = 0;

        public bool isTransitionizing = false;

        #endregion Fields

        #region SectorManagement

        public static void DrawSectorNames(int left, int top, int intervalSpace, List<Object> sector, ref List<Label> LABEL)
        {
            //Ugly, yes i know. For some reason i could not ref in a list of TSector (insifciant access)
            //What i have done is take in an object and cast it into a TSector variable
            TSector SECTOR = (TSector)sector[1];
            for (int k = 0; k <= sector.Count - 1; k++)
            {
                SECTOR = (TSector)sector[k];
                LABEL.Add(new Label());
                LABEL[k].Parent = Form1.ActiveForm;
                LABEL[k].Left = left;
                LABEL[k].Top = (k + 1) * intervalSpace;
                LABEL[k].Text = SECTOR.name;
            }
        }

        public List<int> getSortedSectorsArray()
        {
            List<int> arr = new List<int>();
            List<int> arrInv = new List<int>();
            for (int i = 0; i < Sector.Count; i++)
                arr.Add(i);

            for (int i = 0; i < Sector.Count - 1; i++)
                for (int j = i + 1; j < Sector.Count; j++)
                    if (Sector[arr[i]].name.CompareTo(Sector[arr[j]].name) > 0)
                    {
                        int t = arr[i];
                        arr[i] = arr[j];
                        arr[j] = t;
                    }

            for (int i = 0; i < Sector.Count; i++)
                arrInv.Add(0);
            for (int i = 0; i < Sector.Count; i++)
                arrInv[arr[i]] = i;
            return arrInv;
        }

        public void drawSectors()
        {
            List<int> sortedSectorID = getSortedSectorsArray();

            if (!bSectorsDrawn)
            {
                bSectorsDrawn = true;
                for (int k = 0; k <= Sector.Count - 1; k++)
                {
                    iLeft = 20;//temporary, cuz we removed the trackbars

                    label.Add(new Label());
                    label[k].Text = "[" + Sector[k].units.ToString() + "]" + Sector[k].name;
                    label[k].Name = k.ToString();
                    label[k].AutoSize = true;
                    label[k].Location = new System.Drawing.Point(iLeft, (sortedSectorID[k] + 1) * iInterval + iTop);
                    label[k].Font = templateLBL.Font;
                    label[k].ForeColor = labelColors[k];

                    label[k].MouseDoubleClick += onSectorDoubleClick;
                    label[k].MouseWheel += onWheelMove;
                    label[k].MouseEnter += onMouseEnter;
                    label[k].MouseLeave += onMouseLeave;
                    label[k].MouseDown += onSectorMiddleMouseClick;

                    //label[k].Tag = new Stopwatch();
                    //label[k].MouseDown += new MouseEventHandler(onSectorDownClick);
                    //label[k].MouseUp += new MouseEventHandler(onSectorUpClick);

                    if (BuyMode && Sector[k].units > Sector[k].unitsBeforeBuyMode)
                        label[k].Text = "[" + Sector[k].unitsBeforeBuyMode.ToString() + " + " + (Sector[k].units - Sector[k].unitsBeforeBuyMode).ToString() + "]" + Sector[k].name;

                    Controls.Add(label[k]);
                }
                sectorLabel.Dispose();
                sectorLabel = new Label();
                sectorLabel.Text = "[Units] Sector Name";
                sectorLabel.Font = new System.Drawing.Font(templateLBL.Font, FontStyle.Underline | FontStyle.Bold);
                sectorLabel.AutoSize = true;
                sectorLabel.Location = new System.Drawing.Point(iLeft, (iTop - 4));
                Controls.Add(sectorLabel);
                netLabel.Dispose();
                netLabel = new Label();
                netLabel.Text = "Net Productions";
                netLabel.Font = new System.Drawing.Font(templateLBL.Font, FontStyle.Underline | FontStyle.Bold);
                netLabel.AutoSize = true;
                netLabel.Location = new System.Drawing.Point(iLeft + 360, (iTop - 4));
                if (selectedSector == "all")
                    netLabel.ForeColor = Color.Yellow;
                else
                    netLabel.ForeColor = Color.Black;
                netLabel.MouseClick += (object sender, MouseEventArgs e) =>
                {
                    if (netLabel.ForeColor == Color.Yellow)
                    {
                        netLabel.ForeColor = Color.Black;
                        selectedSector = "";
                        for (int z = 0; z < Sector.Count; z++)
                            labelColors[z] = Color.Black;
                    }
                    else
                    {
                        netLabel.ForeColor = Color.Yellow;
                        selectedSector = "all";

                        for (int z = 0; z < Sector.Count; z++)
                        {
                            //  bool posBalance

                            if (Sector[z].units != 0)
                                labelColors[z] = Color.Orange;
                            else
                                labelColors[z] = Color.Black;
                            //if (Product[z].amount < 0)
                            //    labelColors[z] = Color.Red;
                            //else if (Product[z].amount > 0)
                            //    labelColors[z] = Color.GreenYellow;
                            //else
                            //    labelColors[z] = Color.Black;
                        }
                    }
                    DisposeRecalculateAndDrawNets();
                };
                Controls.Add(netLabel);
            }
            else
            {
                foreach (Label lbl in label)
                {
                    Controls.Remove(lbl);
                    lbl.Dispose();
                }
                label.Clear();
                bSectorsDrawn = false;
                drawSectors();
            }
        }

        public void onMouseLeave(object sender, EventArgs e)
        {
            if ((sender as Label).Focused)
                (sender as Label).Parent.Focus();
        }

        public void onMouseEnter(object sender, EventArgs e)
        {
            if (!(sender as Label).Focused)
                (sender as Label).Focus();
        }

        public void onWheelMove(object sender, MouseEventArgs e)
        {
            Label lbl = (Label)sender;
            int k = int.Parse(lbl.Name); //we use the label's names to store the link(ID) to their respective data.
            int changes = e.Delta / 120;
            Sector[k].units += changes;

            lbl.Text = "[" + Sector[k].units.ToString() + "]" + Sector[k].name;
            if (BuyMode && Sector[k].units > Sector[k].unitsBeforeBuyMode)
                lbl.Text = "[" + Sector[k].unitsBeforeBuyMode.ToString() + " + " + (Sector[k].units - Sector[k].unitsBeforeBuyMode).ToString() + "]" + Sector[k].name;


            //limits: will only work for sectors that have a single outputer/inputer
            if (BuyMode && chkBxChainAutoBuy.Checked)
                for (int f = 0; f < 20; f++)
                {
                    for (int iSec = 0; iSec < Sector.Count; iSec++)
                    {
                        if (!(getPosofProductInList(Sector[iSec].Output, "Energy") != -1 && Sector[iSec].name != cbxPowerSource.Text))
                        if (Sector[iSec].units > 0)
                        {
                            Sector[iSec].units += getNumberOfUnitsToFufullRequirements(iSec);

                            if (Sector[iSec].units > Sector[iSec].unitsBeforeBuyMode)
                                label[iSec].Text = "[" + Sector[iSec].unitsBeforeBuyMode.ToString() + " + " + (Sector[iSec].units - Sector[iSec].unitsBeforeBuyMode).ToString() + "]" + Sector[iSec].name;
                        }
                    }
                }

            disposeNets();
            refreshProductionNets();
            drawNets();
            Save();
        }

        public void onSectorMiddleMouseClick(object sender, MouseEventArgs e)
        {
            Label lbl = (Label)sender;
            int k = int.Parse(lbl.Name); //we use the label's names to store the link(ID) to their respective data.

            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                lbl.Text = "hi";
                Sector[k].units += getNumberOfUnitsToFufullRequirements(k);

                lbl.Text = "[" + Sector[k].units.ToString() + "]" + Sector[k].name;

                if (BuyMode && Sector[k].units > Sector[k].unitsBeforeBuyMode)
                    lbl.Text = "[" + Sector[k].unitsBeforeBuyMode.ToString() + " + " + (Sector[k].units - Sector[k].unitsBeforeBuyMode).ToString() + "]" + Sector[k].name;

                DisposeRecalculateAndDrawNets();
                Save();
            }


        }

        public void onSectorDoubleClick(object sender, EventArgs e)
        {
            Label lbl = (Label)sender;
            int k = int.Parse(lbl.Name); //we use the label's names to store the link(ID) to their respective data.
            try
            {
                Sector[k].units = int.Parse(Microsoft.VisualBasic.Interaction.InputBox("Enter new amount of units in " + Sector[k].name + " sector:", Sector[k].name, Sector[k].units.ToString()));
            }
            catch { }
            lbl.Text = "[" + Sector[k].units.ToString() + "]" + Sector[k].name;

            if (BuyMode && Sector[k].units > Sector[k].unitsBeforeBuyMode)
                lbl.Text = "[" + Sector[k].unitsBeforeBuyMode.ToString() + " + " + (Sector[k].units - Sector[k].unitsBeforeBuyMode).ToString() + "]" + Sector[k].name;

            DisposeRecalculateAndDrawNets();
            Save();
        }

        public void onSectorDownClick(object sender, EventArgs e)
        {
            ((sender as Label).Tag as Stopwatch).Start();
        }

        public void onSectorUpClick(object sender, EventArgs e)
        {
            Stopwatch watch = ((sender as Label).Tag as Stopwatch);
            watch.Stop();
            double ElapsedTime = watch.Elapsed.TotalMilliseconds;
            watch.Reset();

            if (ElapsedTime > 500)
            {
                Label lbl = (Label)sender;
                int k = int.Parse(lbl.Name);
                int mainProductID = 0;
                double maxProduce = 0;
                for (int i = 0; i < Sector[k].Output.Count; i++)
                {
                    if (Math.Abs(Sector[k].Output[i].amount) > maxProduce)
                    {
                        mainProductID = i;
                        maxProduce = Math.Abs(Sector[k].Output[i].amount);
                    }
                }
                int productID = getProductID(Sector[k].Output[mainProductID].name);
                int iUnitsToBuy = (int)Math.Ceiling((Product[productID].amount * -1) / maxProduce);
                Sector[k].units += iUnitsToBuy;

                if (MousePosition.X >= (lbl.Width / 2 + Form1.ActiveForm.Left + iLeft + 18))//if user clicks on right half of label >> Increment units
                {
                    Sector[k].units--;
                }

                DisposeRecalculateAndDrawNets();
            }
        }

        #endregion SectorManagement

        #region NetManagement

        public void DisposeRecalculateAndDrawNets()
        {
            disposeNets();
            refreshProductionNets();
            drawSectors();
            drawNets();
        }

        public void refreshProductionNets()
        {
            bool WasteManager = cbxWasteManager.Checked;
            bool ProductionManager = cbxProductionManager.Checked;

            int i;
            int id;
            totalIncome = 0;
            totalProduction = 0;
            for (i = 0; i <= Product.Count - 1; i++) Product[i].amount = 0;//clears array
            for (int s = 0; s <= Sector.Count - 1; s++)
            {
                for (i = 0; i <= Sector[s].Input.Count - 1; i++)
                {
                    id = getProductID(Sector[s].Input[i].name);
                    Product[id].amount -= (Sector[s].Input[i].amount * Sector[s].units * getMultiplierValue(Sector[s].Input[i].name, ProductionManager, WasteManager, false));
                }
                for (i = 0; i <= Sector[s].Output.Count - 1; i++)
                {
                    id = getProductID(Sector[s].Output[i].name);
                    Product[id].amount += (Sector[s].Output[i].amount * Sector[s].units * getMultiplierValue(Sector[s].Output[i].name, ProductionManager, WasteManager, true));
                }
            }
            for (i = 0; i <= Product.Count - 1; i++)
            {
                totalProduction += Product[i].amount;
                totalIncome += Product[i].amount * Product[i].price;
            }
        }

        public int getMainOutputProductID(int sectorID)
        {
            int s = sectorID;
            int maxID = -1;
            for (int i = 0; i < Sector[s].Output.Count; i++)
            {
                int id = getProductID(Sector[s].Output[i].name);
                if (maxID == -1)
                    maxID = getPosofProductInList(Sector[s].Output, Product[id].name);
                else
                    if (Sector[s].Output[i].amount * Product[id].price > Sector[s].Output[maxID].amount * Product[getProductID(Sector[s].Output[maxID].name)].price)
                        maxID = getPosofProductInList(Sector[s].Output, Product[id].name);
            }
            if (maxID == -1)
                return -1;
            return getPosofProductInList(Product, Sector[s].Output[maxID].name);
        }

        public int getMainInputProductID(int sectorID)
        {
            int s = sectorID;
            int maxID = -1;
            for (int i = 0; i < Sector[s].Input.Count; i++)
            {
                int id = getProductID(Sector[s].Input[i].name);
                if (maxID == -1)
                    maxID = getPosofProductInList(Sector[s].Input, Product[id].name);
                else
                    if (Sector[s].Input[i].amount * Product[id].price < Sector[s].Input[maxID].amount * Product[getProductID(Sector[s].Input[maxID].name)].price)
                        maxID = getPosofProductInList(Sector[s].Input, Product[id].name);
            }
            if (maxID == -1)
                return -1;
            return getPosofProductInList(Product, Sector[s].Input[maxID].name);
        }

        public int getPosofProductInList(List<TProduct> list, string prodName)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i].name.Equals(prodName))
                    return i;
            return -1;
        }

        public int getNumberOfUnitsToFufullRequirements(int sectorID)
        {
            refreshProductionNets();

            int mainOutputProductID = getMainOutputProductID(sectorID);
            int mainInputProductID = getMainInputProductID(sectorID);

            double requiredOutPerHour = 0;
            double requiredInPerHour = 0;
            double requiredOutCost = 0;
            double requiredInCost = 0;
            double valueOut = 0;
            double valueIn = 0;

            if (mainOutputProductID != -1)
            {
                requiredOutPerHour = -1 * Product[mainOutputProductID].amount;
                requiredOutCost = requiredOutPerHour * Product[mainOutputProductID].price;
                valueOut = Sector[sectorID].Output[getPosofProductInList(Sector[sectorID].Output, Product[mainOutputProductID].name)].amount * Product[mainOutputProductID].price;
            }
            if (mainInputProductID != -1)
            {
                requiredInPerHour = -1 * Product[mainInputProductID].amount;
                requiredInCost = requiredInPerHour * Product[mainInputProductID].price;
                valueIn = Sector[sectorID].Input[getPosofProductInList(Sector[sectorID].Input, Product[mainInputProductID].name)].amount * Product[mainInputProductID].price;
            }

            int unitsRequired;

            if (mainOutputProductID == -1)
                unitsRequired = (int)Math.Floor((requiredInPerHour * -1) / Sector[sectorID].Input[getPosofProductInList(Sector[sectorID].Input, Product[mainInputProductID].name)].amount);
            else if (mainInputProductID == -1)
                unitsRequired = (int)Math.Ceiling(requiredOutPerHour / Sector[sectorID].Output[getPosofProductInList(Sector[sectorID].Output, Product[mainOutputProductID].name)].amount);
            else
                if (valueIn >= 0)
                    unitsRequired = (int)Math.Ceiling(requiredOutPerHour / Sector[sectorID].Output[getPosofProductInList(Sector[sectorID].Output, Product[mainOutputProductID].name)].amount);
                else
                    unitsRequired = (int)Math.Floor((requiredInPerHour * -1 ) / Sector[sectorID].Input[getPosofProductInList(Sector[sectorID].Input, Product[mainInputProductID].name)].amount);
                if (requiredOutCost < 0 && requiredInCost < 0)
                    unitsRequired = 0;
                return unitsRequired;
        }

        public double getMultiplierValue(string productName, bool productionManager, bool wasteManager, bool output)
        {
            double multiplier = 1;
            if (productionManager) multiplier = 1.1;
            if (output)
            {
                if (wasteManager && (productName == "Wastewater"))
                    multiplier *= 0.7;
                else if (wasteManager && (productName == "Trash"))
                    multiplier *= 0.5;
            }
            return multiplier;
        }

        public void disposeNets()
        {
            for (int k = 0; k <= netProduct.Count - 1; k++)
            {
                netIncome[k].Dispose();
                netProduction[k].Dispose();
                netName[k].Dispose();
            }
            netIncome.Clear();
            netProduction.Clear();
            netName.Clear();
            netProduct.Clear();
            netTotalIncome.Dispose();
            netTotalProduction.Dispose();
            netTotalName.Dispose();
        }

        public void drawNets()
        {
            int k = 0;
            netProduct.Clear();
            for (int i = 0; i <= Product.Count - 1; i++)
            {
                //user must refreshProductionNets() before calling this method
                //find which products should be displayed
                if (Product[i].amount != 0)
                {
                    netProduct.Add(Product[i].name);
                    k = netProduct.Count - 1;
                    netName.Add(new Label());
                    netName[k].Name = k.ToString();
                    netName[k].AutoSize = true;
                    netName[k].Text = netProduct[k];
                    netName[k].Font = templateLBL.Font;
                    netName[k].Location = new System.Drawing.Point(iLeft + 360, iTop2 + iInterval * (k + 1));
                    if (netName[k].Text == selectedSector)
                        netName[k].ForeColor = Color.Yellow;
                    netProduction.Add(new Label());
                    netProduction[k].Name = k.ToString();
                    netProduction[k].AutoSize = true;
                    netProduction[k].Text = Product[i].amount.ToString() + "CBM";
                    netProduction[k].Font = templateLBL.Font;
                    netProduction[k].Location = new System.Drawing.Point(netName[k].Left - netProduction[k].Width, iTop2 + iInterval * (k + 1)); //alligns to the right and on the left of netName[k]
                    netIncome.Add(new Label());
                    netIncome[k].Name = k.ToString();
                    netIncome[k].AutoSize = true;
                    netIncome[k].Text = (Product[i].amount * Product[i].price).ToString();
                    netIncome[k].Font = templateLBL.Font;
                    if (Product[i].amount * Product[i].price > 0)
                    {
                        netIncome[k].Text = netIncome[k].Text.Insert(0, "€");
                        netIncome[k].Location = new System.Drawing.Point(netName[k].Left + 120, iTop2 + iInterval * (k + 1)); //alligns to the left and on the right of netName[k]
                    }
                    else
                    {
                        netIncome[k].Text = netIncome[k].Text.Insert(1, "€");
                        netIncome[k].Location = new System.Drawing.Point(netName[k].Left + 120 - 5, iTop2 + iInterval * (k + 1));
                    }

                    //color highlighting
                    netName[k].MouseClick += (object sender, MouseEventArgs e) =>
                        {
                            int pos = int.Parse(((Label)sender).Name);
                            selectedSector = ((Label)sender).Text;
                            Color newColor = Color.Yellow;
                            if (netName[pos].ForeColor == Color.Yellow)
                            {
                                newColor = Color.Black;
                                selectedSector = "";
                                for (int z = 0; z < Sector.Count; z++)
                                    labelColors[z] = Color.Black;
                            }
                            else
                            {
                                for (int z = 0; z < Sector.Count; z++)
                                {
                                    bool isIn = false;
                                    bool isOut = false;

                                    foreach (TProduct p in Sector[z].Input)
                                        if (p.name == ((Label)sender).Text)
                                            isIn = true;
                                    foreach (TProduct p in Sector[z].Output)
                                        if (p.name == ((Label)sender).Text)
                                            isOut = true;

                                    if (isIn)
                                        labelColors[z] = Color.DeepSkyBlue;
                                    else if (isOut)
                                        labelColors[z] = Color.GreenYellow;
                                    else
                                        labelColors[z] = Color.Black;
                                }
                            }
                            netName[pos].ForeColor = newColor;
                            DisposeRecalculateAndDrawNets();
                        };

                    Controls.Add(netName[k]);
                    Controls.Add(netProduction[k]);
                    Controls.Add(netIncome[k]);
                    netProduction[k].Location = new System.Drawing.Point(netName[k].Left - netProduction[k].Width, iTop2 + iInterval * (k + 1)); //alligns to the right and on the left of netName[k]
                }
            }
            netTotalName = new Label();
            netTotalName.AutoSize = true;
            netTotalName.Text = "Total";
            netTotalName.Font = templateLBL.Font;
            netTotalName.Location = new System.Drawing.Point(iLeft + 360, iTop2 + iInterval * (k + 2));
            netTotalProduction = new Label();
            netTotalProduction.AutoSize = true;
            netTotalProduction.Font = templateLBL.Font;
            netTotalProduction.Text = totalProduction.ToString() + "CBM";
            netTotalProduction.Location = new System.Drawing.Point(netTotalName.Left - netTotalProduction.Width, iTop2 + iInterval * (k + 2));
            netTotalIncome = new Label();
            netTotalIncome.AutoSize = true;
            netTotalIncome.Font = templateLBL.Font;

            if (totalIncome >= 0)
            {
                netTotalIncome.Text = "€" + totalIncome.ToString();
                netTotalIncome.Location = new System.Drawing.Point(netTotalName.Left + 120, iTop2 + iInterval * (k + 2));
            }
            else
            {
                netTotalIncome.Text = totalIncome.ToString();
                netTotalIncome.Text = netTotalIncome.Text.Insert(1, "€");
                netTotalIncome.Location = new System.Drawing.Point(netTotalName.Left + 120 - 5, iTop2 + iInterval * (k + 2));
            }

            Controls.Add(netTotalIncome);
            Controls.Add(netTotalName);
            Controls.Add(netTotalProduction);

            netTotalProduction.Location = new System.Drawing.Point(netTotalName.Left - netTotalProduction.Width, iTop2 + iInterval * (k + 2));
            DrawBuyModeStuff();

            vScrollBar1.Maximum = iInterval * Product.Count - (int)Math.Round(Height * 0.8);
        }

        #endregion NetManagement

        #region Tools

        public void LoadDataFiles()
        {
            isLoading = true;

            #region Products

            //LOAD PRODUCTS XML
            // string xml = Program.GetXMLFileText("E:\\C#\\DataBases\\products.xml");
            string xml = Program.GetXMLFileText("http://businessgame.be/xml/products.xml");
            // string xml = Program.GetXMLFileText("products.xml");
            string subxml = "";
            string subsubxml = "";
            int iProduct = 0;
            xml = Program.Extract("products", xml);//removes 1st line and all other text that does not fall within the main body of xml
            while (xml.IndexOf("<product>") != (-1))
            {
                subxml = Program.Extract("product", xml);//extracts the first product into subxml
                Product.Add(new TProduct());
                iProduct = Product.Count - 1;
                Product[iProduct].name = Program.Extract("name", subxml);
                Product[iProduct].price = double.Parse(Program.Extract("price", subxml));
                Product[iProduct].maximum = double.Parse(Program.Extract("maximum", subxml));
                Product[iProduct].minimum = double.Parse(Program.Extract("minimum", subxml));
                Product[iProduct].amount = 0;
                Program.Remove("product", ref xml);//removes the product we just extracted from the xml
            }

            #endregion Products

            #region Sectors

            //LOAD SECTORS XML
            xml = Program.GetXMLFileText("http://businessgame.be/xml/sectors.xml");
            subxml = "";
            int iSector = 0;
            int iMachine = 0;
            int iInput = 0;
            int iOutput = 0;
            xml = Program.Extract("sectors", xml);//removes 1st line and all other text that does not fall within the main body of xml
            while (xml.IndexOf("<sector>") != (-1))
            {
                subxml = Program.Extract("sector", xml);//extracts the first sector into subxml
                Sector.Add(new TSector());
                iSector = Sector.Count - 1;
                Sector[iSector].name = Program.Extract("name", subxml);
                Sector[iSector].price = double.Parse(Program.Extract("price", subxml));
                Sector[iSector].employees = double.Parse(Program.Extract("employees", subxml));
                Sector[iSector].fixedprice = double.Parse(Program.Extract("fixed", subxml));

                subsubxml = Program.Extract("machinery", subxml);
                while (subsubxml.IndexOf("<product>") != (-1))
                {
                    Sector[iSector].Machinery.Add(new TProduct());
                    iMachine = Sector[iSector].Machinery.Count - 1;
                    Sector[iSector].Machinery[iMachine].name = Program.Extract("name", subsubxml);
                    Sector[iSector].Machinery[iMachine].amount = double.Parse(Program.Extract("amount", subsubxml));
                    Program.Remove("product", ref subsubxml);
                }
                subsubxml = Program.Extract("input", subxml);
                while (subsubxml.IndexOf("<product>") != (-1))
                {
                    Sector[iSector].Input.Add(new TProduct());
                    iInput = Sector[iSector].Input.Count - 1;
                    Sector[iSector].Input[iInput].name = Program.Extract("name", subsubxml);
                    Sector[iSector].Input[iInput].amount = double.Parse(Program.Extract("amount", subsubxml));
                    Program.Remove("product", ref subsubxml);
                }
                subsubxml = Program.Extract("output", subxml);
                while (subsubxml.IndexOf("<product>") != (-1))
                {
                    Sector[iSector].Output.Add(new TProduct());
                    iOutput = Sector[iSector].Output.Count - 1;
                    Sector[iSector].Output[iOutput].name = Program.Extract("name", subsubxml);
                    Sector[iSector].Output[iOutput].amount = double.Parse(Program.Extract("amount", subsubxml));
                    Program.Remove("product", ref subsubxml);
                }
                Program.Remove("sector", ref xml);//removes the product we just extracted from the xml
            }
            TSector[] tempArray = new TSector[500];//random number
            Sector.CopyTo(tempArray);
            for (int k = 0; k < Sector.Count; k++)
                SectorsToBuy.Add(tempArray[k]);

            #endregion Sectors

            isLoading = false;
            Market = new TMarket(this.Controls, this, label1, progressBar1, Sector, Product, 0, wBrowser, DisposeRecalculateAndDrawNets);
        }

        public void StartLoadingDataFiles()
        {
            LoadingLabel1 = new Label();
            LoadingLabel2 = new Label();
            LoadingLabel1.Text = "Loading    ";
            LoadingLabel2.Text = "Please be patient while we load the required datafiles and autosaves!\n                     This should take no more than 10 seconds";
            LoadingLabel1.AutoSize = true;
            LoadingLabel2.AutoSize = true;
            Font newfont1 = new Font("Microsoft Sans Serif", (float)20.0, FontStyle.Bold);
            Font newfont2 = new Font("Microsoft Sans Serif", (float)10.0, FontStyle.Regular);
            LoadingLabel1.Font = newfont1;
            LoadingLabel2.Font = newfont2;
            LoadingLabel1.Left = (Width / 2) - 70;
            LoadingLabel2.Left = (Width / 2) - 230;
            LoadingLabel1.Top = (Height / 2) - 70;
            LoadingLabel2.Top = (Height / 2) - 70 + (LoadingLabel1.Height) + 10;
            Controls.Add(LoadingLabel1);
            Controls.Add(LoadingLabel2);
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            AutoLoadThread = new Thread(() => LoadDataFiles());
            AutoLoadThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            AutoLoadThread.Start();
        }

        public string RemoveSpaces(string Data)
        {
            while (Data.IndexOf(" ") != -1)
            {
                Data = Data.Remove(Data.IndexOf(" "), 1);
            }
            while (Data.IndexOf("\n") != -1)
            {
                Data = Data.Remove(Data.IndexOf("\n"), 1);
            }
            while (Data.IndexOf(",") != -1)
            {
                Data = Data.Remove(Data.IndexOf(","), 1);
            }
            return Data;
        }

        public void SmartLoad(string OriginalData)
        {
            int errorCount = 0;
            string Data = OriginalData;

            int position;
            foreach (TSector sector in Sector)
            {
                try
                {
                    Data = OriginalData;
                    position = Data.IndexOf(sector.name);
                    if (position != -1)
                    {
                        Data = Data.Remove(0, position + sector.name.Length);//remove everything before the number
                        position = Data.IndexOf("units installed");
                        Data = Data.Remove(position);//remove everything after the number
                        Data = RemoveSpaces(Data);
                        if (Data.Length > 0)
                            sector.units = int.Parse(Data);
                    }
                }
                catch { errorCount++; }
            }
            //richTextBox1.Visible = false;
            if (errorCount == 0)
                MessageBox.Show("Smart Load Complete!");
            else
                MessageBox.Show("Smart Load Complete!\nErrors: " + errorCount.ToString());

            drawSectors();
            disposeNets();
            refreshProductionNets();
            drawNets();
            Save();
        }

        public double getProductPrice(string productName)
        {
            foreach (TProduct product in Product)
            {
                if (product.name == productName)
                {
                    return product.price;
                }
            }
            //else
            return -1;
        }

        public void DrawBuyModeStuff()//displays how many machines, pipes etc the user has to buy.
        {
            foreach (Label lbl in buyModeLabels1)
            {
                lbl.Dispose();
            }
            foreach (Label lbl in buyModeLabels2)
            {
                lbl.Dispose();
            }
            buyModeLabels1.Clear();
            buyModeLabels2.Clear();

            if (BuyMode)
            {
                List<string> requirements = new List<string>();
                List<int> requirementQuantity = new List<int>();
                double TotalMachinesCost = 0;
                double TotalUnitsCost = 0;
                foreach (TSector sector in Sector)
                {
                    int iPurchaseCount = sector.units - sector.unitsBeforeBuyMode;
                    if (iPurchaseCount > 0)
                    {
                        foreach (TProduct requirement in sector.Machinery)
                        {
                            if (requirements.Contains(requirement.name))
                            {
                                requirementQuantity[requirements.IndexOf(requirement.name)] += (int)(iPurchaseCount * requirement.amount);//we know that all machine requirements are integers
                            }
                            else
                            {
                                requirements.Add(requirement.name);
                                requirementQuantity.Add((int)(iPurchaseCount * requirement.amount));
                            }
                            TotalMachinesCost += getProductPrice(requirement.name) * iPurchaseCount * requirement.amount;
                        }
                        TotalUnitsCost += sector.price * iPurchaseCount;
                    }
                }
                for (int k = 0; k < requirements.Count; k++)
                {
                    buyModeLabels1.Add(new Label());
                    buyModeLabels1[k].Text = requirements[k];
                    buyModeLabels1[k].AutoSize = true;
                    buyModeLabels1[k].Location = new System.Drawing.Point(this.Width - 180, (k + 1) * (iInterval - 3) * 3 + iTop);//(iInterval - 3) just to group them nicely
                    buyModeLabels1[k].Font = templateLBL.Font;

                    buyModeLabels2.Add(new Label());
                    buyModeLabels2[k].Text = requirementQuantity[k].ToString();
                    buyModeLabels2[k].AutoSize = true;
                    buyModeLabels2[k].Location = new System.Drawing.Point(this.Width - 180, (k + 1) * (iInterval - 3) * 3 + 17 + iTop);
                    buyModeLabels2[k].Font = templateLBL.Font;

                    Controls.Add(buyModeLabels1[k]);
                    Controls.Add(buyModeLabels2[k]);
                }

                int m = buyModeLabels1.Count;
                buyModeLabels1.Add(new Label());
                buyModeLabels1[m].Text = "Total Machines Cost: ";
                buyModeLabels1[m].AutoSize = true;
                buyModeLabels1[m].Location = new System.Drawing.Point(this.Width - 180, (m + 1) * (iInterval - 3) * 3 + iTop);
                buyModeLabels1[m].Font = templateLBL.Font;

                buyModeLabels2.Add(new Label());
                buyModeLabels2[m].Text = "€" + TotalMachinesCost.ToString();
                buyModeLabels2[m].AutoSize = true;
                buyModeLabels2[m].Location = new System.Drawing.Point(this.Width - 180, (m + 1) * (iInterval - 3) * 3 + 17 + iTop);
                buyModeLabels2[m].Font = templateLBL.Font;

                Controls.Add(buyModeLabels1[m]);
                Controls.Add(buyModeLabels2[m]);

                m++;
                buyModeLabels1.Add(new Label());
                buyModeLabels1[m].Text = "Total Units Cost: ";
                buyModeLabels1[m].AutoSize = true;
                buyModeLabels1[m].Location = new System.Drawing.Point(this.Width - 180, (m + 1) * (iInterval - 3) * 3 + iTop);
                buyModeLabels1[m].Font = templateLBL.Font;

                buyModeLabels2.Add(new Label());
                buyModeLabels2[m].Text = "€" + TotalUnitsCost.ToString();
                buyModeLabels2[m].AutoSize = true;
                buyModeLabels2[m].Location = new System.Drawing.Point(this.Width - 180, (m + 1) * (iInterval - 3) * 3 + 17 + iTop);
                buyModeLabels2[m].Font = templateLBL.Font;

                Controls.Add(buyModeLabels1[m]);
                Controls.Add(buyModeLabels2[m]);

                m++;
                buyModeLabels1.Add(new Label());
                buyModeLabels1[m].Text = "Grand Total: ";
                buyModeLabels1[m].AutoSize = true;
                buyModeLabels1[m].Location = new System.Drawing.Point(this.Width - 180, (m + 1) * (iInterval - 3) * 3 + iTop);
                buyModeLabels1[m].Font = templateLBL.Font;

                buyModeLabels2.Add(new Label());
                buyModeLabels2[m].Text = "€" + (TotalUnitsCost + TotalMachinesCost).ToString();
                buyModeLabels2[m].AutoSize = true;
                buyModeLabels2[m].Location = new System.Drawing.Point(this.Width - 180, (m + 1) * (iInterval - 3) * 3 + 17 + iTop);
                buyModeLabels2[m].Font = templateLBL.Font;

                Controls.Add(buyModeLabels1[m]);
                Controls.Add(buyModeLabels2[m]);
            }
        }

        public int getProductID(string ProductName)
        {
            int k = 0;
            while (Product[k].name != ProductName)
            {
                k++;
                if (k > Product.Count - 1)
                {
                    MessageBox.Show("Error: Cannot find product '{0}'", ProductName);
                    k = 100;
                    break;
                }
            }
            return (k);
        }

        public int GetSectorID(string name)
        {
            for (int k = 0; k < Sector.Count; k++)
            {
                if (Sector[k].name == name)
                    return k;
            }
            return -1;
        }

        public void Save()
        {
            StreamWriter SW = new StreamWriter("BPM AutoSave");
            try
            {
                SW.WriteLine(cbxProductionManager.Checked.ToString());
                SW.WriteLine(cbxWasteManager.Checked.ToString());

                for (int k = 0; k < Sector.Count; k++)
                {
                    SW.WriteLine(Sector[k].name);
                    SW.WriteLine(Sector[k].units);
                }
            }
            catch { }
            SW.Close();
        }

        public void LoadAutoSave()
        {
            string name;
            if (File.Exists("BPM AutoSave"))
            {
                StreamReader SR = new StreamReader("BPM AutoSave");

                if (!SR.EndOfStream)
                {
                    cbxProductionManager.Checked = (SR.ReadLine() == "True");
                    cbxWasteManager.Checked = (SR.ReadLine() == "True");
                }

                while (!SR.EndOfStream)
                {
                    try
                    {
                        name = SR.ReadLine();
                        Sector[GetSectorID(name)].units = int.Parse(SR.ReadLine());
                    }
                    catch { }
                }
                SR.Close();
            }
        }

        #endregion Tools

        #region Other

        public void button7_Click(object sender, EventArgs e)
        {
            foreach (TSector sector in Sector)
            {
                sector.units = 0;
            }
            drawSectors();
            disposeNets();
            refreshProductionNets();
            drawNets();
            Save();
        }

        public void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            //if (richTextBox1.Text.Length > 20)
            //{
            //    SmartLoad(richTextBox1.Text);
            //}
            //richTextBox1.Clear();
        }

        public void button6_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("How to use Smart Load:\n1)In your browser, go to the units page.\n2)Press 'Ctrl + A' to select everything on the page.\n3)Press 'Ctrl + C' to copy everything to the clipboard.\n-In the white text box that will apear when you close this message:\n4)Paste the text into the box by pressing 'Ctrl + V'.\n5)Wait a second or two!");
            //string OriginalData = Microsoft.VisualBasic.Interaction.InputBox("Contents of Units Page:","Smart Load (Beta)","");
            //string OriginalData = richTextBox1.Text;
            //richTextBox1.Visible = true;
            //richTextBox1.Focus();
            Market.ForeignSectors = Sector;
            Market.LoadUsersUnitsData();
        }

        public void button2_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("How to use BPM:\n-Tick 'display web-browser' and log in. Untick when you are done.\n-To change the number of units you have in a sector:\n  -Move mouse over sector, use scroll wheel to change value\n  -Double click to enter amount manually \n - Click one of the products in the middle pannel to highlight related sectors");
        }

        public void label1_Click(object sender, EventArgs e)
        {
        }

        public void onScaleChange(object sender, EventArgs e)
        {
            TrackBar trackbar = (TrackBar)sender;
        }

        public void button3_Click(object sender, EventArgs e)
        {
            if (!BuyMode)
            {
                Market.ForeignSectors = Sector;
                foreach (TSector sector in Sector)
                {
                    sector.unitsBeforeBuyMode = sector.units;
                }
                button3.Text = "Purchase / Exit";
                button4.Visible = true;
                BuyMode = true;
            }
            else
            {
                MessageBox.Show("NOTE: This feature has been temporarily disabled due to complications arising from the introduction of 'shipments'");
                int unitsToBuy = 0;
                int iActions = 1;
                List<string> productnames = new List<string>();
                List<double> productamounts = new List<double>();
                List<string> sectornames = new List<string>();
                List<int> sectoramounts = new List<int>();

                foreach (TSector sector in Sector)
                {
                    unitsToBuy = sector.units - sector.unitsBeforeBuyMode;
                    if (unitsToBuy > 0)
                    {
                        iActions++;
                        sectornames.Add(sector.name);
                        sectoramounts.Add(unitsToBuy);
                        foreach (TProduct product in sector.Machinery)
                        {
                            bool productfound = false;
                            foreach (string name in productnames)
                            {
                                if (name.Equals(product.name))
                                {
                                    productfound = true;
                                    productamounts[productnames.IndexOf(name)] += product.amount * unitsToBuy;
                                }
                            }
                            if (!productfound)
                            {
                                productnames.Add(product.name);
                                productamounts.Add(product.amount * unitsToBuy);
                            }
                        }
                    }
                }
                if (productnames.Count != 0)
                    Market.PerformActionBuyUnits(productnames, productamounts, sectornames, sectoramounts, iActions);

                if (Market.ErrorFound)
                    foreach (TSector sector in Sector) //ROLLBACK
                    {
                        sector.unitsBeforeBuyMode = sector.units;
                    }

                button3.Text = "EnterBuy Mode";
                button4.Visible = false;
                BuyMode = false;

                drawSectors();
                disposeNets();
                refreshProductionNets();
                drawNets();
            }
            Save();
        }

        public void button4_Click(object sender, EventArgs e)
        {
            foreach (TSector sector in Sector)
            {
                sector.units = sector.unitsBeforeBuyMode;
            }
            drawSectors();

            disposeNets();
            refreshProductionNets();
            drawNets();

            Save();
        }

        public void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Businessgame Production Manager\nDevelped by Michael McQuirk!\n\nContact: michaelcmcquirk@gmail.com\nVersion 0.8 (Cornflower Blue 1) - 26 November 2014");
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            Application.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            labelColors = new List<Color>();
            for (int i = 0; i < 100; i++)
                labelColors.Add(Color.Black);
            StartLoadingDataFiles();
            loadingTimer.Enabled = true;
        }

        public void loadingTimer_Tick(object sender, EventArgs e)
        {
            if (isLoading)
            {
                label2.Visible = true;
                label3.Visible = true;
                //think about pausing the loading thread...
                dotCount++;
                if (dotCount > 5)
                    dotCount = 0;
                LoadingLabel1.Text = "Loading";
                for (int k = 1; k <= dotCount; k++)
                {
                    LoadingLabel1.Text += " .";
                }
            }
            else if (!isTransitionizing) //has finnished loading
            {
                isTransitionizing = true;
                SystemSounds.Beep.Play();
            }
            else //do the transitions
            {
                loadingTimer.Interval = 1;
                LoadingLabel1.Left -= 8;
                LoadingLabel2.Left += 10;

                if ((LoadingLabel1.Left + LoadingLabel1.Width < 0) && (LoadingLabel2.Left > Width))
                {
                    LoadingLabel1.Dispose();
                    LoadingLabel2.Dispose();

                    LoadAutoSave();
                    drawSectors();
                    disposeNets();
                    refreshProductionNets();
                    drawNets();

                    button2.Visible = true;
                    button3.Visible = true;
                    button5.Visible = true;
                    button6.Visible = true;
                    button7.Visible = true;

                    cbxProductionManager.Visible = true;
                    cbxWasteManager.Visible = true;

                    loadingTimer.Enabled = false;

                    label2.Visible = false;
                    label3.Visible = false;

                    cbxPowerSource.Items.Clear();
                    foreach (TSector s in Sector)
                    {
                        if (getPosofProductInList(s.Output, "Energy") != -1)
                        {
                            cbxPowerSource.Items.Add(s.name);
                            if (s.units > 0)
                                cbxPowerSource.Text = s.name;
                        }
                    }
                }
            }
        }

        public void wBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (Market != null)
                Market.BrowserStatus = "Navigating";
            BStatus = "Navigating";
        }

        public void wBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (Market != null)
                Market.BrowserStatus = "Complete";
            BStatus = "Complete";
        }

        public void cbxWasteManager_CheckedChanged(object sender, EventArgs e)
        {
            disposeNets();
            refreshProductionNets();
            drawNets();
        }

        public void cbxProductionManager_CheckedChanged(object sender, EventArgs e)
        {
            disposeNets();
            refreshProductionNets();
            drawNets();
        }

        public void cbxWasteManager_MouseClick(object sender, MouseEventArgs e)
        {
            Save();
        }

        public void cbxProductionManager_MouseClick(object sender, MouseEventArgs e)
        {
            Save();
        }

        public void templateLBL_MouseHover(object sender, EventArgs e)
        {
        }

        public void timer1_Tick(object sender, EventArgs e)
        {
            if (Market != null && BStatus == "Complete")
            {
                bool found = false;
                //var elements = Market.WBrowser.Document.GetElementsByTagName("login");
                var elements = Market.WBrowser.Document.All;
                foreach (HtmlElement element in elements)
                {
                    if (element.GetAttribute("type") == "submit" && element.GetAttribute("value") == "Log in")
                    {
                        //element.InvokeMember("click");
                        //Log("Units bought");
                        lblPassword.Visible = true;
                        lblUsername.Visible = true;
                        edtPassword.Visible = true;
                        edtUsername.Visible = true;
                        btnLogIn.Visible = true;
                        found = true;
                    }
                }
                if (!found)
                {
                    lblPassword.Visible = false;
                    lblUsername.Visible = false;
                    edtPassword.Visible = false;
                    edtUsername.Visible = false;
                    btnLogIn.Visible = false;
                }
            }
        }

        public void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                wBrowser.Visible = true;
            else
                wBrowser.Visible = false;
        }

        public void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Market.ActionThread.Abort();
            AutoLoadThread.Abort();
        }

        public void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            iTop = 5 - vScrollBar1.Value;
            drawSectors();
        }

        public void button1_Click(object sender, EventArgs e)
        {
        }

        public void button8_Click(object sender, EventArgs e)
        {
            
        }

        public Form1()
        {
            InitializeComponent();
        }

        #endregion Other

        private void button1_Click_1(object sender, EventArgs e)
        {
            Form2 f2 = new Form2(this);
            f2.Show();
        }
    }
}