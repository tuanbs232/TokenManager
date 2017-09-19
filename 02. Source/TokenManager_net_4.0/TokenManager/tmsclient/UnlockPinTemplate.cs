using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenManager.tmsclient
{
    class UnlockPinTemplate
    {
        public string tokenSerialNumber { get; set; }
        public string tokenModelName { get; set; }
        public string certificateSerialNumberList { get; set; }
        public string codeUnlockUserPin { get; set; }
    }
}
