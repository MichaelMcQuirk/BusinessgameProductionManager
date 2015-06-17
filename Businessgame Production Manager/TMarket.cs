using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.IO;

namespace WindowsFormsApplication1
{
    public class TMarket
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

        public double PlayerCash = -1;
        public double RemainingWarehouseSpace = -1;

        public WebBrowser logBrowser = null;
        public enum LogCodes {ProgramStartup, LoadPrivateData, AutoBuy };

        public BuyModeInfo buyModeInfoForm = null;



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

            control.Invoke(new Method(() =>
            {
               // buyModeInfoForm = new BuyModeInfo();
               // buyModeInfoForm.Show();
                ////temperorily removed until completed
            }));

            Log_Low("market object created");
        }

        public delegate void Method();
        public delegate void Method2(List<String> productNames, List<double> amounts, List<string> sectorNames, List<int> sectorAmounts);

        public void LoadUserData()
        {
            Log_Low("starting to load user data");
            (new Thread(() =>
            {
                iActions = 3;
                iActioncount = 0;
                Control.Invoke((MethodInvoker)delegate
                {
                    PGBar.Visible = true;
                    PGBar.Maximum = iActions;
                    PGBar.Value = 0;
                });

                Thread.Sleep(500);

                Control.Invoke((MethodInvoker)delegate
                {
                    PGBar.Increment(1);
                    PGBar.Update();
                    PGBar.Refresh();
                });

                //unit counts
                Control.Invoke((MethodInvoker)delegate { WBrowser.Navigate("businessgame.be/sectors"); });
                WaitForBrowserToLoad();
                string originalData = "";
                Control.Invoke((MethodInvoker)delegate { originalData = WBrowser.Document.Body.InnerText; });
                SmartLoad(originalData);

                List<String> amounts = new List<string>();
                foreach (TSector s in ForeignSectors)
                    if (s.units != 0) amounts.Add(s.name + " : " + s.units);
                Log_Low("(1/4) units loaded", amounts.ToArray());

                Control.Invoke((MethodInvoker)delegate
                {
                    PGBar.Increment(1);
                    PGBar.Update();
                    PGBar.Refresh();
                });

                //cash
                Control.Invoke((MethodInvoker)delegate { WBrowser.Navigate("businessgame.be/storage"); });
                WaitForBrowserToLoad();

                string Data = "";
                Control.Invoke((MethodInvoker)delegate { Data = WBrowser.Document.Body.InnerText; });

                int position = Data.IndexOf("€");
                if (position != -1)
                {
                    Data = Data.Remove(0, position + 1);//remove everything before the euro sign (inc the euro sign)
                    position = Data.IndexOf(".");
                    Data = Data.Substring(0, position);//remove everything after the decimal point (a few extra cents makes little difference)
                    Data = RemoveSpaces(Data);//remove the commas
                    if (Data.Length > 0)
                        PlayerCash = double.Parse(Data);
                }

                Log_Low("(2/4) cash loaded", PlayerCash.ToString());

                try
                {

                    //warehouse remaining space
                    WaitForBrowserToLoad();
                    Control.Invoke((MethodInvoker)delegate { Data = WBrowser.Document.Body.InnerText; });

                    position = Data.IndexOf("Total capacity");
                    Data = Data.Remove(0, position + "Total capacity is ".Length);                   //"Total capacity is 24,000,000.00 CBM 7,217,549.04 CBM in use (30.07%) ..."
                    double totalCapacity = double.Parse(RemoveSpaces(Data.Substring(0, Data.IndexOf("."))));    //"24,000,000"
                    Data = Data.Remove(0, Data.IndexOf(" CBM") + " CBM\r\n".Length);                                 //"7,217,549.04 CBM in use (30.07%) ..."
                    double usedSpace = double.Parse(RemoveSpaces(Data.Substring(0, Data.IndexOf("."))));        //"7,217,549"
                    RemainingWarehouseSpace = totalCapacity - usedSpace;

                    Log_Low("(3/4) remaining warehouse space loaded", "" + RemainingWarehouseSpace);

                }
                catch (Exception e)
                {
                    MessageBox.Show("ERROR: " + e.Message);
                }

                //warehouse contents
                originalData = "";
                Control.Invoke((MethodInvoker)delegate { originalData = WBrowser.Document.Body.InnerText; });

                foreach (TProduct product in Product)
                {
                    if (product.name.ToLower() != "shipment")
                    try
                    {
                        Data = originalData;
                        position = Data.IndexOf(product.name + "\r");
                        if (position != -1)
                        {
                            Data = Data.Remove(0, position + product.name.Length + 1);//remove everything before the number

                            position = Data.IndexOf(".");
                            Data = Data.Substring(0, position);
                            Data = RemoveSpaces(Data);
                            if (Data.Length > 0)
                            {
                                product.amountOwnedByPlayer = double.Parse(Data);
                            }
                        }
                    }
                    catch { MessageBox.Show("An error has occured whilst trying to load warehouse contents. Rest of program that does not use warehouse contents should still work fine."); }
                }

                List<string> prdkts = new List<string>();
                foreach(TProduct p in Product)
                    if (p.amountOwnedByPlayer != 0) prdkts.Add(p.name + " " + p.amountOwnedByPlayer);
                Log_Low("(4/4) warehouse contents loaded", prdkts.ToArray()); 

                Control.Invoke((MethodInvoker)delegate
                {
                    PGBar.Increment(1);
                    PGBar.Update();
                    PGBar.Refresh();
                });

                Thread.Sleep(1000);

                Control.Invoke((MethodInvoker)delegate
                {
                    PGBar.Visible = false;
                });

                Log_Low("load private data complete");

            }
            )).Start();
        }


        public void PerformAction(Method method)
        {
            ActionThread = new Thread(() => method());
            ActionThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            ActionThread.Start();
        }

        public void PerformActionBuyUnits(List<String> productNames, List<double> amounts, List<string> sectorNames, List<int> sectorAmounts, int iactions, bool useMyMachines)
        {
            List<String> s1 = new List<string>();
            for (int i = 0; i < productNames.Count; i++)
                s1.Add(productNames[i] + " : " + amounts[i]);
            for (int i = 0; i < sectorNames.Count; i++)
                s1.Add(sectorNames[i] + " : " + sectorAmounts[i]);
            Log_Low("PerformActionBuyUnits()", s1.ToArray());

            iActions = iactions;
            iActioncount = 0;
            Control.Invoke((MethodInvoker)delegate
            {
                PGBar.Maximum = iActions;
                PGBar.Increment(iActioncount - PGBar.Value);
                PGBar.Update();
                PGBar.Refresh();

                if (useMyMachines)
                {
                    for (int i = 0; i < productNames.Count; i++)
                    {
                        foreach (TProduct p in Product)
                        {
                            if (p.name == productNames[i])
                            {
                                double amountBeingUsed = amounts[i];
                                amounts[i] = Math.Max(0, amountBeingUsed - p.amountOwnedByPlayer);
                                p.amountOwnedByPlayer -= amountBeingUsed;
                            }
                        }
                    }
                }
                
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

            Control.Invoke((MethodInvoker)delegate
            {
                //text = WBrowser.Document.Body.InnerText;
                if (WBrowser.Document.All["warning success"] != null)
                    text = "You have bought";
            });
            
            if (text.Contains("You have bought")) return true;

            return false;
        }

        public void WaitForBrowserToLoad()
        {
            Thread.Sleep(2000);
            string status = "";
            int timeSpentWaiting = 2000;
            loop:
            Thread.Sleep(250);
            Control.Invoke((MethodInvoker)delegate
            {
                status = BrowserStatus;
            });
            timeSpentWaiting += 250;
            if (timeSpentWaiting > 5000) ///IMPORTANT!!!!!!!! : This is a quick fix for an issue users are facing (browser not completely loading). Find a better solution!
            {
                Control.Invoke((MethodInvoker)delegate
                { WBrowser.Stop(); });
                return;
            }
            if (status != "Complete") goto loop;
        }

        public void BuyUnitsAndProducts(List<String> productNames, List<double> amounts, List<string> sectorNames, List<int> sectorAmounts)
        {
            Log_Low("BuyUnitsAndProducts()");
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
                StatusMessageLabel.ForeColor = Color.White;
            });

            Thread.Sleep(1000);
            Control.Invoke((MethodInvoker)delegate { StatusMessageLabel.Text = "Operation Commencing.."; });
            Thread.Sleep(1000);
            Control.Invoke((MethodInvoker)delegate { StatusMessageLabel.Text = "Operation Commencing..."; });
            Thread.Sleep(1000);
            Control.Invoke((MethodInvoker)delegate { StatusMessageLabel.Text = "Operation Commencing...."; });

            bool thereAreMachinesToBuy = false;
            foreach (double d in amounts)
                if (d > 0)
                    thereAreMachinesToBuy = true;

            Log_Low("BuyProduct() - starting");
            if (thereAreMachinesToBuy)
                BuyProduct(productNames, amounts);
            Log_Low("BuyProduct() - complete. Beginning success check.");

            Thread.Sleep(1000);
            WaitForBrowserToLoad();
            iActioncount++;
            Control.Invoke((MethodInvoker)delegate
            {
                PGBar.Increment(iActioncount - PGBar.Value);
                PGBar.Update();
                PGBar.Refresh();
            });
            if (thereAreMachinesToBuy && !CheckForSuccess())
            {
                Log_High("ERROR! NOT ENOUGH CASH.");
                Control.Invoke((MethodInvoker)delegate
                {
                    PGBar.ForeColor = System.Drawing.Color.Red;
                    PGBar.Update();
                    PGBar.Refresh();
                });
                ErrorFound = true;
            }
            else
            //***********************************************************************************************************
            {
                Log_Low("BuyProduct() - successful");

                for (int i = 0; i < sectorNames.Count; i++)
                {
                    Log_Low("BuyUnits()", sectorNames[i] + " : " + sectorAmounts[i]);
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
                        Log_High("ERROR! NOT ENOUGH CASH.");
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
                    Log_Low("BuyUnits() - success");
                }
            }
            Thread.Sleep(300);
            Control.Invoke((MethodInvoker)delegate { StatusMessageLabel.Text = "Operation Complete."; });
            Thread.Sleep(2500);
            Thread.Sleep(1000);
            Control.Invoke((MethodInvoker)delegate { StatusMessageLabel.Visible = false; PGBar.Visible = false; });
            Log_Low("BuyUnitsAndProducts() - complete");
        }

        public void BuyProduct(string productName, int amount)
        {
            Log_High("Buying " + amount + "CBMs of " + productName);
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
            Log_High(compondedlog);
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
                    Log_High("ERROR: Something went wrong when buying " + productName + ". Retrying...");
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
            Log_High("Selling " + amount + "CBMs of " + productName);
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
                Log_High("ERROR: Something went wrong when selling " + productName + ". Retrying...");
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
                Log_High("Selling " + amount + "CBMs of " + productName);

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
                    Log_High("ERROR: Something went wrong when selling " + productName + ". Retrying...");
                    goto SellProduct;
                }
            }
            Control.Invoke((MethodInvoker)delegate
            {
                WBrowser.Document.All["sell"].InvokeMember("Click");
            });
            WaitForBrowserToLoad();
        }


        public int getUnitsCount(string sectorName)
        {
            string adaptedSectorName = sectorName; //note: the name in the url has spaces replaced with %20 's
            while (adaptedSectorName.IndexOf(' ') != -1)
            {
                int pos = adaptedSectorName.IndexOf(' ');
                adaptedSectorName = adaptedSectorName.Remove(pos, 1);
                adaptedSectorName = adaptedSectorName.Insert(pos, "%20");
            }

            WaitForBrowserToLoad();
            Control.Invoke((MethodInvoker)delegate
            {
                WBrowser.Navigate("businessgame.be/sector/" + adaptedSectorName);
            });
            WaitForBrowserToLoad();

            string BrowserInnerText = "";

            Control.Invoke((MethodInvoker)delegate
            {
                BrowserInnerText = WBrowser.Document.Body.InnerText;
            });

            if (!BrowserInnerText.Contains("units installed"))
                return 0;

            try
            {
                int position = BrowserInnerText.IndexOf(sectorName);
                while (position != -1 && BrowserInnerText.IndexOf("units installed") > position)
                {
                    BrowserInnerText = BrowserInnerText.Remove(0, position + sectorName.Length);//remove everything before the number
                    position = BrowserInnerText.IndexOf(sectorName);
                }

                BrowserInnerText = BrowserInnerText.Remove(BrowserInnerText.IndexOf("units installed"));//remove everything after the number
                BrowserInnerText = RemoveSpaces(BrowserInnerText);

                char[] listOfNumbers = new char[]{'0','1','2','3','4','5','6','7','8','9'};
                for (int i = BrowserInnerText.Length -1; i >= 0; i--) //remove everything thats not a number
                    if (!listOfNumbers.Contains(BrowserInnerText[i]))
                        BrowserInnerText.Remove(i, 1);

                if (BrowserInnerText.Length > 0)
                {
                    return int.Parse(BrowserInnerText);
                }
                    
            }
            catch { return -1; }

            return -1;

        }

        public void BuyUnits(string sectorName, int amount)
        {
            Log_High("Buying " + amount + "units for the " + sectorName + " sector");

            string adaptedSectorName = sectorName; //note: the name in the url has spaces replaced with %20 's
            while (adaptedSectorName.IndexOf(' ') != -1)
            {
                int pos = adaptedSectorName.IndexOf(' ');
                adaptedSectorName = adaptedSectorName.Remove(pos, 1);
                adaptedSectorName = adaptedSectorName.Insert(pos, "%20");
            }

            
            //////////////////////////////////
            int unitsBefore = getUnitsCount(sectorName);
            /////////////////////////////////

            Log_Low("navigating to : " + "businessgame.be/sector/" + adaptedSectorName);
            WaitForBrowserToLoad();
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
                        Log_High("Unlocking sector");
                        element.InvokeMember("click");
                        Log_High("Sector unlocked");
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
                        Log_High("Activating sector");
                        element.InvokeMember("click");
                        Log_High("Sector activated");

                        unitsBefore = getUnitsCount(sectorName);
                    }
                }
            });
            WaitForBrowserToLoad();

            int trys = 0;
        reTryBuyUnits:
            trys++;

            if (trys > 1)
            {
                if (getUnitsCount(sectorName) == unitsBefore + amount)//if we now have the number of units we originally wanted, do not buy again...
                    return;
            }

            WaitForBrowserToLoad();
            Control.Invoke((MethodInvoker)delegate
            {
                textbox = WBrowser.Document.GetElementById("buyUnits");
            });

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
                            Log_High("Units bought");
                        }
                    }
                });
                Thread.Sleep(500);
                WaitForBrowserToLoad();

                if (trys < 60 && CheckForSuccess() == false)
                {
                    for (int i = 60; i > 0; i += -5)
                    {
                        Log_High("Waiting " + i + " seconds!");
                        Thread.Sleep(5000);
                    }

                    Control.Invoke((MethodInvoker)delegate
                    {
                        WBrowser.Navigate("businessgame.be/sector/" + adaptedSectorName);
                    });
                    WaitForBrowserToLoad();

                    if (trys > 0)
                        Log_High("(Attempt " + trys + ") Waiting for required goods to arrive. Retrying every 60s for up to 1 hour.");
                    
                    goto reTryBuyUnits;
                }

                iActioncount++;
            }
            else
                throw new Exception("Buy Units Error!");
        }

        public void UpdateBotData() /////////////////////ALSO UPDATE NO OF UNITS, so we don't have to do saves 'n all!
        {
            Log_High("Updating bot data...");
            UpdateUnitAmounts();
            UpdateCashAmount();
            UpdateProductAmounts();
            Log_High("Bot update complete");

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

            Log_High("Cash balance successfully loaded: €" + money.ToString());
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
                                Log_High("Product amount retrieved: " + product.name + " = " + product.amount.ToString());
                            }
                            catch
                            {
                                Log_High("ERROR: Product amount not retrieved: " + product.name + " = " + product.amount.ToString());
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
                WBrowser.Navigate("businessgame.be/sectors");
            });
            WaitForBrowserToLoad();

            Control.Invoke((MethodInvoker)delegate
            {
                BrowserContents = WBrowser.Document.Body.InnerText;
            });

            SmartLoad(BrowserContents);

            Log_High("Units successfully loaded, sector: " + Player.MySector.name + ", Units: " + Player.MySector.units.ToString());
        }

        public string CleanseOfSymbol(char symbol, string text)
        {
            while (text.IndexOf(symbol) != -1)
            {
                text = text.Remove(text.IndexOf(symbol), 1);
            }
            return text;
        }

        public void Log_High(string text)
        {
            Control.Invoke((MethodInvoker)delegate
            {
                if (StatusMessageLabel != null)
                {
                    StatusMessageLabel.Text = text;
                    StatusMessageLabel.Refresh();
                }

                if (buyModeInfoForm != null)
                {
                    buyModeInfoForm.lblStatus.Text = text;
                    buyModeInfoForm.lblStatus.Refresh();
                }
            });

            Log_Low(text);
        }

        public void Log_Low(string text, params string[] args)
        {
            Control.Invoke((MethodInvoker)delegate
            {
                StreamWriter sw = new StreamWriter("BPM_LogFile.txt", true);
                sw.WriteLine("Y" + DateTime.Now.Year + "M" + DateTime.Now.Month + "D" + DateTime.Now.Day + "S" + DateTime.Now.Second + " : " + text);
                foreach (String s in args)
                    sw.WriteLine("  - " + s);
                sw.Close();
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
                        //Data = Data.Remove(0, position + sector.name.Length);//remove everything before the number
                        //position = Data.IndexOf(sector.name);

                        while (position != -1) //repeatedly remove any further sector names (for the special case of petrochemistry)
                        {
                            Data = Data.Remove(0, position + sector.name.Length);
                            position = Data.IndexOf(sector.name);
                        }

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

        

        public void UploadLog(String errorText)
        {
            Thread T = new Thread(() =>
            {
                logBrowser = new WebBrowser();
                logBrowser.Navigate(@"http://businessgame.be/ajax/bpm/log.php?error=" + System.Web.HttpUtility.UrlEncode(errorText));
            });
            T.SetApartmentState(ApartmentState.STA);
            T.Start();
            
        }

        public void UploadLog(LogCodes code)
        {
            int iCode = -1;
            if (code == LogCodes.ProgramStartup) iCode = 1;
            if (code == LogCodes.LoadPrivateData) iCode = 2;
            if (code == LogCodes.AutoBuy) iCode = 3;
            if (iCode == -1) MessageBox.Show("ERROR: UploadLog() Unknown log code!");
            
            Thread T = new Thread(() =>
            {
                logBrowser = new WebBrowser();
                logBrowser.Navigate(@"http://businessgame.be/ajax/bpm/log.php?log=" + iCode);
            });
            T.SetApartmentState(ApartmentState.STA);
            T.Start();
        }


    }
}
