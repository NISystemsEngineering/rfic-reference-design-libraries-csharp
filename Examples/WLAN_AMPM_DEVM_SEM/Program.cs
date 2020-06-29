using System;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class Program
    {
        /// <summary>
        /// This example illustrates how to use the RFmx and RFSG drivers and the WLAN toolkit to perform SEM and dynamic EVM measurements.
        /// Before executing this application, please check and ensure you have the right configurations in the InitializeParameters() function.
        /// </summary>
        static void Main(string[] args)
        {
            Console.WriteLine("Example Application WLAN_AMPM_DEVM_SEM\n");
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
