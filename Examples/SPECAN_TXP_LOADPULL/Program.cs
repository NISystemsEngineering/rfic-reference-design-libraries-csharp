using System;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class Program
    {
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