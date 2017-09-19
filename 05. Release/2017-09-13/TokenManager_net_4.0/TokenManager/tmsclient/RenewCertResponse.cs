using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenManager.tmsclient
{
    class RenewCertResponse
    {
        public string code { get; set; }
        public string descrition { get; set; }
        public RenewCertMeta metaData { get;set;}
    }
}
