using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenManager.common
{
    class CustomerInfo
    {
        public string TokenSerialNumber { get; set; }
        public string TokenModelName { get; set; }
        public List<string> CertSerialList { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public string OtpType { get; set; }

        public string OtpValue { get; set; }


        const string SEPARATOR_SYMBOL = ",";
        public string GetCertListString()
        {
            if(CertSerialList == null || CertSerialList.Count == 0)
            {
                return "";
            }

            StringBuilder builder = new StringBuilder();
            foreach(string serialNum in CertSerialList)
            {
                if(serialNum == null || "".Equals(serialNum))
                {
                    continue;
                }
                builder.Append(serialNum.ToLower());
                builder.Append(SEPARATOR_SYMBOL);
            }
            string result = builder.ToString();
            return result.Substring(0, result.LastIndexOf(SEPARATOR_SYMBOL));
        }
    }
}
