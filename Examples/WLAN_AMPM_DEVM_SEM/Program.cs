using System;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Example Application WLAN_DPD_CFR_AMPM_EVM_SEM\n");
            WLAN_AMPM_DEVM_SEM wlanDpdCfrAmPmDevmSem = new WLAN_AMPM_DEVM_SEM();
            try
            {
                wlanDpdCfrAmPmDevmSem.Run();

            }
            catch (Exception e)
            {

                DisplayError(e);
            }
            Console.WriteLine("Press any key to finish.");
            Console.ReadKey();
        }
        static void DisplayError(Exception e)
        {
            Console.WriteLine("ERROR:\n" + e.GetType() + ": " + e.Message);
        }
    }
}
