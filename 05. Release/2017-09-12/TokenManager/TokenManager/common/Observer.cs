using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenManager.common
{
    interface Observer
    {
        void Update(String message, int messageType, Object[] param);
    }
}
