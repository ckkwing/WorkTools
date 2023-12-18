using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Common
{
    public class Utility
    {
        /// <summary>
        /// Disable FIPS policy
        /// </summary>
        public static void DisableFIPS()
        {
            try
            {
                RegistryKey fipsAlgorithmPolicy = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Lsa\FipsAlgorithmPolicy");
                fipsAlgorithmPolicy.SetValue("Enabled", 0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
