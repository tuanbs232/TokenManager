using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenManager.common
{
    interface ObserverAble
    {
        void attach(Observer obj);
        void detach(Observer obj);
        void notify(String message, int messageType, Object[] param);
    }
}
