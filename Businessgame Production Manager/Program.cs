using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Net;

namespace WindowsFormsApplication1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static string GetXMLFileText(string FileLocation)
        {
            string XML = "";
            try
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(FileLocation);
                StreamReader reader = new StreamReader(stream);
                XML = reader.ReadToEnd();
                //using (StreamReader sr = new StreamReader(FileLocation))
                //{
                //    string line;
                //    while ((line = sr.ReadLine()) != null)
                //    {
                //        XML += line;
                //    }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return XML;
        }

        public static string Extract(string Field, string XML)
        {
            int BeginPos = XML.IndexOf('<' + Field + '>');
            int EndPos = XML.IndexOf("</" + Field + '>');
            if (BeginPos < 0)
                throw new Exception("Oh shiet, we can't find " + '<' + Field + '>' + " in the downloaded xml file...");
            return (XML.Substring(BeginPos + Field.Length + 2, EndPos - (BeginPos + Field.Length + 2)));
        }
        public static void Remove(string Field, ref string XML)
        {
            int BeginPos = XML.IndexOf('<' + Field + '>');
            int EndPos = XML.IndexOf("</" + Field + '>');
            XML=XML.Remove(BeginPos, EndPos + Field.Length + 3 - BeginPos);
        }
    }
}
