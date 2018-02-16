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


        static string GenerateFile()
        {
            CustomerService dl = new CustomerService();
            string fnm = Path.Combine(Path.GetTempPath(), DateTime.Now.ToString("ddMMyyyy") + ".txt");
            foreach (var s in dl.GetSalesForPost())
                File.AppendAllText(fnm, Barcode.BarcodeWithCheckSum(s.SaleId) + ";"
                    + s.CustomerName + ";"
                    + s.CustomerPhone + ";"
                    + s.DeliveryAddress + ";"
                    + s.DeliveryCity + ";"
                    + s.DeliveryZip + ";"
                    + s.DeliveryCountry+ "\r\n");

            return fnm;

        }
        static void Main(string[] args)
        {
            using (WebClient client = new WebClient())
            {

                string fnm = GenerateFile();
                client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["ftpUser"], ConfigurationManager.AppSettings["ftpPassword"]);
                client.UploadFile(ConfigurationManager.AppSettings["ftpAddress"] + "/" + Path.GetFileName(fnm), fnm);
                File.Delete(fnm);
            }
        }
    }
}
