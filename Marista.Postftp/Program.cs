using Marista.Common.Tools;
using Marista.DL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Marista.Postftp
{
    class Program
    {

        static CustomerService dl = new CustomerService();

        static string GenerateFile()
        {

            string fnm = Path.Combine(Path.GetTempPath(), DateTime.Now.ToString("ddMMyyyy") + ".txt");
            File.AppendAllText(fnm, "");
            foreach (var s in dl.GetSalesForPost())
                File.AppendAllText(fnm, Barcode.BarcodeWithCheckSum(s.SaleId) + ";"
                    + s.CustomerName + ";"
                    + s.CustomerPhone + ";"
                    + s.DeliveryAddress + ";"
                    + s.DeliveryCity + ";"
                    + s.DeliveryZip + ";"
                    + s.DeliveryCountry + "\r\n");

            return fnm;

        }
        static void Main(string[] args)
        {
            switch (args[0])
            {
                case "1":
                    using (WebClient client = new WebClient())
                    {

                        string fnm = GenerateFile();
                        client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["ftpUser"], ConfigurationManager.AppSettings["ftpPassword"]);
                        client.UploadFile(ConfigurationManager.AppSettings["ftpAddress"] + "/" + Path.GetFileName(fnm), fnm);
                        File.Delete(fnm);
                    };
                    break;
                case "2":
                    dl.CalcAllProfits();
                    break;
                case "3":
                    dl.EndMonth();
                    break;
            }

            Console.WriteLine(DateTime.Now.ToString("yyyyMMdd hh:mm") + " " + args[0] + " OK");
        }
    }
}
