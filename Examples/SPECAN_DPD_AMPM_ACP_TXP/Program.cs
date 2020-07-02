using System;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class Program
    {
        /// <summary>
        /// This example illustrates how to use the RFmx SpecAn together with the RFSG drivers to perform 
        /// AMPM, ACP and TXP measurements with or without digital predistortion (DPD) of the waveform input to the DUT. 
        /// Before executing this application, please check and ensure you have the right configurations in the InitializeParameters() function.
        /// </summary>
        static void Main(string[] args)
        {
            Console.WriteLine("Example Application SPECAN_DPD_AMPM_ACP_TXP\n");
            SPECAN_DPD_AMPM_ACP_TXP specan_DPD_AMPM_ACP_TXP = new SPECAN_DPD_AMPM_ACP_TXP();
            try
            {
                specan_DPD_AMPM_ACP_TXP.Run();              
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