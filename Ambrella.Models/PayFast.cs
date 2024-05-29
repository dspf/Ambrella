using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Ambrella.Models
{
    public class PayFast
    {
        private string MerchantID => ConfigurationManager.AppSettings["MerchantId"];
        private string MerchantKey => ConfigurationManager.AppSettings["MerchantKey"];
        private string PassPhrase => ConfigurationManager.AppSettings["PassPhrase"];
        //Default constructor

        public string GeneratePayFastPaymentSignature(double amount, string itemName)
        {
            string signatureString = string.Format("{0}|{1}|{2}|{3}|{4}|||||{5}",
                MerchantID, MerchantID, itemName, amount, "", PassPhrase);

            using (var hasher = new HMACSHA256(Encoding.UTF8.GetBytes(MerchantKey)))
            {
                byte[] signatureBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(signatureString));
                return BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();
            }
        }

        public string GeneratePaymentFeeUrl(double amount, string item_name, string return_url, string cancel_url)
        {
            // Generate the payment signature
            string paymentSignature = GeneratePayFastPaymentSignature(amount, item_name);

            string payFastPaymentUrl = string.Format("https://sandbox.payfast.co.za/eng/process?cmd=_paynow&receiver={0}&amount={1}&item_name={2}&signature={3}&return_url={4}&cancel_url={5}",
                MerchantID, amount, HttpUtility.UrlEncode(item_name), paymentSignature, HttpUtility.UrlEncode(return_url), HttpUtility.UrlEncode(cancel_url));

            // Redirect the user to the PayFast payment page
            return payFastPaymentUrl;
        }
    }
}
