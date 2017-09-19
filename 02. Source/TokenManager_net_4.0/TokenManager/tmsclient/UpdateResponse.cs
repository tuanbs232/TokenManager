using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TokenManager.tmsclient
{
    class UpdateResponse
    {
        public string code { get; set; }
        public string description { get; set; }
        public UpdateMeta metaData { get; set; }
    }
}
