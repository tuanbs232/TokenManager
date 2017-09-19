using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenManager.tmsclient
{
    class UpdateProfileTemplate
    {
        public string tokenSerialNumber { get; set; }
        public string tokenModelName { get; set; }
        public string emailOwner { get; set; }
        public string phoneNumberOwner { get; set; }
    }
}
