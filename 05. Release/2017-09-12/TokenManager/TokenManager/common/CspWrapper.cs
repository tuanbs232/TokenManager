using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TokenManager.common
{
    class CspWrapper
    {
        [DllImport("CspInteractive.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int C_LoadAllCertToStore(StringBuilder name);


        [DllImport("CspInteractive.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int C_UnloadAllCertificate(StringBuilder provider);

        internal static object GetErrorMessage(int result)
        {
            switch (result)
            {
                case 1:
                case 2:
                    return "Provider empty or cannot be parse";
                case 3:
                    return "No registered provider match gived name";
                case 4:
                    return "Cannot open Window-MY store";
                case 5:
                    return "Cannot aquire cryptography context";
                case 6:
                    return "No private key container name found";
            }
            return "UNDEFINED ERROR";
        }
    }
}
