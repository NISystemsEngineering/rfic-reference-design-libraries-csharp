using System;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class Program
    {
        /// <summary>
        /// This example illustrates how to use the RFmx WLAN and RFSG drivers to perform SEM and EVM measurements with or without digital predistortion (DPD) of the waveform input to the DUT.
        /// Before executing this application, please check and ensure you have the right configurations in the InitializeParameters() function.
        /// </summary>
        static void Main(string[] args)
        {
            Console.WriteLine("Example Application WLAN_DPD_CFR_AMPM_EVM_SEM\n");
            WLAN_DPD_AMPM_EVM_SEM wlanDpdCfrAmPmEvmSem = new WLAN_DPD_AMPM_EVM_SEM();
            try
            {
                wlanDpdCfrAmPmEvmSem.Run();
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
