using System;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class Program
    {
        /// <summary>
        /// This example illustrates how to use the RFmx SpecAn, RFSG, and Focus Tuner drivers to perform TXP measurements with load pull. 
        /// Before executing this application, please check and ensure you have the right configurations in the InitializeParameters() function.
        /// </summary>
        static void Main(string[] args)
        {
            Console.WriteLine("Example Application SPECAN_TXP_LOADPULL\n");
            SPECAN_TXP_LOADPULL specan_TXP_LOADPULL = new SPECAN_TXP_LOADPULL();
            try
            {
                specan_TXP_LOADPULL.Run();
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