using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace WindowsFormsApplication1
{
    class TMarket
    {
        public List<TProduct> Product = new List<TProduct>();
        public List<TSector> Sector = new List<TSector>();
        public TPerson Player;

        public Thread ActionThread;
        public WebBrowser WBrowser;

        public int RoundNumber = 0;
        public int CurrentTime = 0;//in minutes

        public static Random r = new Random();

        public double TransportCost = 1.05;

        public double WarehouseCost = 50000;
        public double ProcessorCost = 50000;
        public int WarehouseCapacity = 250000;
        public int ProcessorCapacity = 60000;

        public ProgressBar PGBar;
        public Label StatusMessageLabel;

        public int DisplayInterval = 30;//updates display every 30 minutes
        private int timeSinceLastDisplay = 0;

        public int FixCount = 0;
        public double TotalFixed = 0;

        public int PriceCalculationsInterval = 8 * 60;//change prices every 8 hours
        private int timeSinceLastPriceCalculation = 0;

        Control.ControlCollection Controls;
        Control Control;

        public string BrowserStatus = "";
        public bool BuyingUnitsAllowed = true;
        public int BuyingUnitsAllowanceRemainingTime = 0;

        public int RunCount = 0;
        public int MaxRuns = 40;

        public int iActions = 0;
        public int iActioncount = 0;
        public bool ErrorFound = false;
        public List<TSector> ForeignSectors = new List<TSector>();

        public Action DisposeRecalculateAndRedrawNets;


        public TMarket(Control.ControlCollection controls, Control control, Label statusMessage, ProgressBar pgBar, List<TSector> _sector, List<TProduct> _product, int VirtualPlayers, WebBrowser webBrowser, Action refreshLabels)
        {
            DisposeRecalculateAndRedrawNets = refreshLabels;
            PGBar = pgBar;
            StatusMessageLabel = statusMessage;

            foreach (TProduct product in _product)
            {
                Product.Add(new TProduct(product.name, product.price, product.minimum, product.maximum, product.amount));
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
            WBrowser = webBrowser;
            Control = control;

            Player = new TPerson(0, Product, Sector, this, 800000);
            Controls = controls;

            

        }

        public delegate void Method();
        public delegate void Method2(List<String> productNames, List<double> amounts, List<string> sectorNames, List<int> sectorAmounts);

        public void LoadUsersUnitsData()
        {
            (new Thread(() => {
                Control.Invoke((MethodInvoker)delegate
            { WBrowser.Navigate("businessgame.be/units"); });
                WaitForBrowserToLoad();
                string originalData = "";
                Control.Invoke((MethodInvoker)delegate { originalData = WBrowser.Document.Body.InnerText; });
                SmartLoad(originalData);
            })).Start();
        }

        public void PerformAction(Method method)
        {
            ActionThread = new Thread(() => method());
            ActionThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            ActionThread.Start();
        }

        public void PerformActionBuyUnits(List<String> productNames, List<double> amounts, List<string> sectorNames, List<int> sectorAmounts, int iactions)
        {
            iActions = iactions;
            iActioncount = 0;
            Control.Invoke((MethodInvoker)delegate
            {
                PGBar.Maximum = iActions;
                PGBar.Increment(iActioncount - PGBar.Value);
                PGBar.Update();
                PGBar.Refresh();
                
            });
            Method2 method = (List<String> productNames2, List<double> amounts2, List<string> sectorNames2, List<int> sectorAmounts2) => BuyUnitsAndProducts(productNames2, amounts2, sectorNames2,sectorAmounts2);
            ActionThread = new Thread(() => method(productNames,amounts,sectorNames,sectorAmounts));
            ActionThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            ActionThread.Start();
            
        }

        //warning success
        //warning1
        //You have bought
        //You do not have enough

        public bool CheckForSuccess()
        {
            string text = "";
            Control.Invoke((MethodInvoker)delegate
            {
                if (WBrowser.Document.All["warning1"] != null)
                text = WBrowser.Document.All["warning1"].InnerText;
            });
            if (text.Contains("You have bought")) return true;
            text = WBrowser.Document.Body.InnerText;
            if (text.Contains("You have bought")) return true;
            return false;
        }

        public void WaitForBrowserToLoad()
        {
            string status = "";
            loop:
            Thread.Sleep(250);
            Control.Invoke((MethodInvoker)delegate
            {
                status = BrowserStatus;
            });
            if (status != "Complete") goto loop;
        }

        public void BuyUnitsAndProducts(List<String> productNames, List<double> amounts, List<string> sectorNames, List<int> sectorAmounts)
        {
            ErrorFound = false;
            Control.Invoke((MethodInvoker)delegate
            {
                PGBar.ForeColor = System.Drawing.Color.Green;
                PGBar.Maximum = iActions;
                PGBar.Value = iActioncount;
                PGBar.Update();
                PGBar.Visible = true;
                PGBar.Refresh();
                StatusMessageLabel.Text = "Operation Commencing.";
                StatusMessageLabel.Visible = true;
            });
            Thread.Sleep(1000);
            Control.Invoke((MethodInvoker)delegate { StatusMessageLabel.Text = "Operation Commencing.."; });
            Thread.Sleep(1000);
            Control.Invoke((MethodInvoker)delegate { StatusMessageLabel.Text = "Operation Commencing..."; });
            Thread.Sleep(1000);
            Control.Invoke((MethodInvoker)delegate { StatusMessageLabel.Text = "Operation Commencing...."; });
            BuyProduct(productNames, amounts);
            iActioncount++;
            Control.Invoke((MethodInvoker)delegate
            {
                PGBar.Increment(iActioncount - PGBar.Value);
                PGBar.Update();
                PGBar.Refresh();
            });
            if (!CheckForSuccess())
            {
                Log("ERROR! NOT ENOUGH CASH.");
                Control.Invoke((MethodInvoker)delegate
                {
                    PGBar.ForeColor = System.Drawing.Color.Red;
                    PGBar.Update();
                    PGBar.Refresh();
                });
                ErrorFound = true;
            }
            else
            for (int i = 0; i < sectorNames.Count; i++)
            {
                BuyUnits(sectorNames[i], sectorAmounts[i]);
                Control.Invoke((MethodInvoker)delegate
                {
                    PGBar.Maximum = iActions;
                    PGBar.Increment(iActioncount - PGBar.Value);
                    PGBar.Update();
                    PGBar.Refresh();
                    Control.Refresh();
                });
                if (!CheckForSuccess())
                {
                    Log("ERROR! NOT ENOUGH CASH.");
                    Control.Invoke((MethodInvoker)delegate
                    {
                        PGBar.ForeColor = System.Drawing.Color.Red;
                    });
                    ErrorFound = true;
                    for (int k = 0; k < i - 1; k++)
                    {
                        ForeignSectors[getSectorID(sectorNames[k])].units += sectorAmounts[k];
                    }
                    break;
                }
            }
            Thread.Sleep(300);
            Control.Invoke((MethodInvoker)delegate { StatusMessageLabel.Text = "Operation Complete."; });
            Thread.Sleep(2500);
            Thread.Sleep(1000);
            Control.Invoke((MethodInvoker)delegate { StatusMessageLabel.Visible = false; PGBar.Visible = false; });
        }

        public void BuyProduct(string productName, int amount)
        {
            Log("Buying " + amount + "CBMs of " + productName);
            Control.Invoke((MethodInvoker)delegate
            {
                WBrowser.Navigate("businessgame.be/market");
            });
            WaitForBrowserToLoad();

            int ID = getProductID(productName);
            HtmlElement textbox = null;
            Control.Invoke((MethodInvoker)delegate
            {
                textbox = WBrowser.Document.GetElementById(productName);
            });
            WaitForBrowserToLoad();
            if (textbox != null)
            {
                Control.Invoke((MethodInvoker)delegate
                {
                    textbox.SetAttribute("value", amount.ToString());
                });
                WaitForBrowserToLoad();
                Control.Invoke((MethodInvoker)delegate
                {
                    WBrowser.Document.All["buy"].InvokeMember("Click");
                });
                WaitForBrowserToLoad();
            }
            else
                throw new Exception("Buy Product Error!");
            Product[ID].amount -= amount;
            Product[ID].BuysThisRound += amount;
        }

        public void BuyProduct(List<String> productNames, List<double> amounts)
        {
            int ID = 0;
            Control.Invoke((MethodInvoker)delegate
            {
                WBrowser.Navigate("businessgame.be/market");
            });
            WaitForBrowserToLoad();

            string compondedlog = "Buying ";
            for (int k = 0; k < productNames.Count; k++)
            {
                compondedlog += amounts[k] + "CBMs of " + productNames[k] + ", ";
            }
            compondedlog = compondedlog.Remove(compondedlog.Length - 3, 2);
            Log(compondedlog);
            for (int i = 0; i < productNames.Count; i++)
            {
            BuyProduct:
                string productName = productNames[i];
                double amount = amounts[i];
                

                    ID = getProductID(productName);
                HtmlElement textbox = null;
                Control.Invoke((MethodInvoker)delegate
                {
                    textbox = WBrowser.Document.GetElementById(productName);
                });
                if (textbox != null)
                {
                    Control.Invoke((MethodInvoker)delegate
                    {
                        textbox.SetAttribute("value", amount.ToString());
                    });
                    Thread.Sleep(200);
                    WaitForBrowserToLoad();
                    Product[ID].amount -= amount;
                    Product[ID].BuysThisRound += amount;
                }
                else
                {
                    // throw new Exception("Sell Product Error!");
                    Log("ERROR: Something went wrong when buying " + productName + ". Retrying...");
                    goto BuyProduct;
                }
            }
            Control.Invoke((MethodInvoker)delegate
            {
                WBrowser.Document.All["buy"].InvokeMember("Click");
            });
            WaitForBrowserToLoad();
        }

        public void SellProduct(string productName, double amount)
        {
            SellProduct:
            Log("Selling " + amount + "CBMs of " + productName);
            Control.Invoke((MethodInvoker)delegate
            {
                WBrowser.Navigate("businessgame.be/market");
            });
            WaitForBrowserToLoad();

            int ID = getProductID(productName);
            HtmlElement textbox = null;
            Control.Invoke((MethodInvoker)delegate
            {
                textbox = WBrowser.Document.GetElementById(productName);
            });
            if (textbox != null)
            {
                Control.Invoke((MethodInvoker)delegate
                {
                    textbox.SetAttribute("value", amount.ToString());
                });
                WaitForBrowserToLoad();
                Control.Invoke((MethodInvoker)delegate
                {
                    WBrowser.Document.All["sell"].InvokeMember("Click");
                });
                WaitForBrowserToLoad();
            }
            else
            {
               // throw new Exception("Sell Product Error!");
                Log("ERROR: Something went wrong when selling " + productName + ". Retrying...");
                goto SellProduct;
            }
            Product[ID].amount += amount;
            Product[ID].SalesThisRound += amount;
        }

        public void SellProduct(List<String> productNames, List<double> amounts)
        {
            int ID = 0;
            Control.Invoke((MethodInvoker)delegate
            {
                WBrowser.Navigate("businessgame.be/market");
            });
            WaitForBrowserToLoad();

            for (int i = 0; i < productNames.Count; i++)
            {
            SellProduct:
                string productName = productNames[i];
                double amount = amounts[i];
                Log("Selling " + amount + "CBMs of " + productName);

                ID = getProductID(productName);
                HtmlElement textbox = null;
                Control.Invoke((MethodInvoker)delegate
                {
                    textbox = WBrowser.Document.GetElementById(productName);
                });
                if (textbox != null)
                {
                    Control.Invoke((MethodInvoker)delegate
                    {
                        textbox.SetAttribute("value", amount.ToString());
                    });
                    Thread.Sleep(200);
                    WaitForBrowserToLoad();
                    Product[ID].amount += amount;
                    Product[ID].SalesThisRound += amount;
                }
                else
                {
                    // throw new Exception("Sell Product Error!");
                    Log("ERROR: Something went wrong when selling " + productName + ". Retrying...");
                    goto SellProduct;
                }
            }
            Control.Invoke((MethodInvoker)delegate
            {
                WBrowser.Document.All["sell"].InvokeMember("Click");
            });
            WaitForBrowserToLoad();
        }

        public void BuyUnits(string sectorName, int amount)
        {
            Log("Buying " + amount + "units for the " + sectorName + " sector");

            string adaptedSectorName = sectorName; //note: the name in the url has spaces replaced with %20 's
            while (adaptedSectorName.IndexOf(' ') != -1)
            {
                int pos = adaptedSectorName.IndexOf(' ');
                adaptedSectorName = adaptedSectorName.Remove(pos, 1);
                adaptedSectorName = adaptedSectorName.Insert(pos, "%20");
            }
            

            Control.Invoke((MethodInvoker)delegate
            {
                WBrowser.Navigate("businessgame.be/sector/" + adaptedSectorName);
            });
            WaitForBrowserToLoad();

            HtmlElement textbox = null;
            Control.Invoke((MethodInvoker)delegate
            {
                var elements = WBrowser.Document.GetElementsByTagName("input");
                foreach (HtmlElement element in elements)
                {
                    if (element.GetAttribute("type") == "submit" && element.GetAttribute("value") == "Unlock " + sectorName + " sector")
                    {
                        Log("Unlocking sector");
                        element.InvokeMember("click");
                        Log("Sector unlocked");
                        Thread.Sleep(500);
                        WBrowser.Navigate("businessgame.be/sector/" + adaptedSectorName);
                        break;
                    }
                }
            });
            WaitForBrowserToLoad();
            Control.Invoke((MethodInvoker)delegate
            {
                var elements = WBrowser.Document.GetElementsByTagName("input");
                foreach (HtmlElement element in elements)
                {
                    if (element.GetAttribute("type") == "submit" && element.GetAttribute("value") == "Activate " + sectorName)
                    {
                        Log("Activating sector");
                        element.InvokeMember("click");
                        Log("Sector activated");
                    }
                }
            });
            WaitForBrowserToLoad();
            Control.Invoke((MethodInvoker)delegate
            {
                textbox = WBrowser.Document.GetElementById("buyUnits");
            });


            int retries = 0;
        reTryBuyUnits:
            retries++;
            if (textbox != null)
            {
                Control.Invoke((MethodInvoker)delegate
                {
                    textbox = WBrowser.Document.GetElementById("buyUnits");
                    textbox.SetAttribute("value", amount.ToString());
                });
                WaitForBrowserToLoad();
                Control.Invoke((MethodInvoker)delegate
                {
                    var elements = WBrowser.Document.GetElementsByTagName("input");
                    foreach (HtmlElement element in elements)
                    {
                        if (element.GetAttribute("type") == "submit" && element.GetAttribute("value") == "Buy")
                        {
                            element.InvokeMember("click");
                            Log("Units bought");
                        }
                    }
                });
                WaitForBrowserToLoad();
                if (retries < 60 && CheckForSuccess() == false)
                {
                    Thread.Sleep(60000);
                    Control.Invoke((MethodInvoker)delegate
                    {
                        WBrowser.Navigate("businessgame.be/sector/" + adaptedSectorName);
                    });

                    if (retries > 0)
                        Log("(Attempt " + retries + ") Waiting for required goods to arrive. Retrying every 60s for up to 1 hour.");
                    WaitForBrowserToLoad();
                    goto reTryBuyUnits;
                }

                iActioncount++;
            }
            else
                throw new Exception("Buy Units Error!");
        }

        public void UpdateBotData() /////////////////////ALSO UPDATE NO OF UNITS, so we don't have to do saves 'n all!
        {
            Log("Updating bot data...");
            UpdateUnitAmounts();
            UpdateCashAmount();
            UpdateProductAmounts();
            Log("Bot update complete");

        }

        public void UpdateCashAmount(bool moneylogit = false)
        {
            Control.Invoke((MethodInvoker)delegate
            {
                WBrowser.Navigate("businessgame.be/market");
            });
            WaitForBrowserToLoad();
            string money = "";
            Control.Invoke((MethodInvoker)delegate
            {
                foreach (HtmlElement ele in WBrowser.Document.All)
                {
                    if (ele.GetAttribute("className") == "statistic")
                    {
                        money = ele.InnerText;
                        if (money.IndexOf('€') != -1) goto outofloop;
                    }
                }
            outofloop:
                money = CleanseOfSymbol('€', money);
                money = CleanseOfSymbol(',', money);
                money = CleanseOfSymbol(' ', money);
            });
            Player.Cash = double.Parse(money);

            Log("Cash balance successfully loaded: €" + money.ToString());
        }

        public void UpdateProductAmounts()
        {
            foreach (TProduct product in Player.Product)
            {
                product.amount = 0;
            }

            Control.Invoke((MethodInvoker)delegate
                {
                    WBrowser.Navigate("businessgame.be/market");
                });
            WaitForBrowserToLoad();

            string value = null;
            Control.Invoke((MethodInvoker)delegate
            {
                foreach (HtmlElement element in WBrowser.Document.All)
                {
                    string style = element.GetAttribute("style");
                    if (element.GetAttribute("style") == "width: 20px; height: 20px;" && element.OuterHtml.Contains("/images/products/"))
                    foreach (TProduct product in Player.Product)
                    {
                        if (element.GetAttribute("title") == product.name)
                        {
                            value = element.Parent.Parent.InnerText;
                            if (value.Contains('€')) break;
                            value = CleanseOfSymbol('\n', value);
                            value = CleanseOfSymbol('\t', value);
                            value = CleanseOfSymbol(' ', value);
                            value = CleanseOfSymbol(',', value);
                            value = CleanseOfSymbol('C', value);
                            value = CleanseOfSymbol('B', value);
                            value = CleanseOfSymbol('M', value);
                            for (char c = 'a'; c <= 'z'; c++)
                            {
                                value = CleanseOfSymbol(c, value);
                            }
                            for (char c = 'A'; c <= 'Z'; c++)
                            {
                                value = CleanseOfSymbol(c, value);
                            }
                            try
                            {
                                product.amount = Double.Parse(value);
                                Log("Product amount retrieved: " + product.name + " = " + product.amount.ToString());
                            }
                            catch
                            {
                                Log("ERROR: Product amount not retrieved: " + product.name + " = " + product.amount.ToString());
                            }
                        }
                    }

                }
            });
        }

        public void UpdateUnitAmounts()
        {
            string BrowserContents = "";
            Control.Invoke((MethodInvoker)delegate
            {
                WBrowser.Navigate("businessgame.be/units");
            });
            WaitForBrowserToLoad();

            Control.Invoke((MethodInvoker)delegate
            {
                BrowserContents = WBrowser.Document.Body.InnerText;
            });

            SmartLoad(BrowserContents);

            Log("Units successfully loaded, sector: " + Player.MySector.name + ", Units: " + Player.MySector.units.ToString());
        }

        public string CleanseOfSymbol(char symbol, string text)
        {
            while (text.IndexOf(symbol) != -1)
            {
                text = text.Remove(text.IndexOf(symbol), 1);
            }
            return text;
        }

        public void Log(string text)
        {
            Control.Invoke((MethodInvoker)delegate
            {
                if (StatusMessageLabel != null)
                {
                    StatusMessageLabel.Text = text;
                    StatusMessageLabel.Refresh();
                }
            }); 
        }

        public int getProductID(string ProductName)
        {
            int k = 0;
            while (Product[k].name.ToLower() != ProductName.ToLower())
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

        public int getSectorID(string SectorName)
        {
            int k = 0;
            while (Sector[k].name.ToLower() != SectorName.ToLower())
            {
                k++;
                if (k > Sector.Count - 1)
                {
                    MessageBox.Show("Error: Cannot find Sector '{0}'", SectorName);
                    k = 100;
                    break;
                }
            }
            return (k);
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
                        {
                            int iunits = int.Parse(Data);
                            ForeignSectors[getSectorID(sector.name)].units = iunits;
                        }
                    }
                }
                catch { errorCount++; }
            }
            Control.Invoke((MethodInvoker)delegate
            {
                DisposeRecalculateAndRedrawNets();
            });
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


    }
}
