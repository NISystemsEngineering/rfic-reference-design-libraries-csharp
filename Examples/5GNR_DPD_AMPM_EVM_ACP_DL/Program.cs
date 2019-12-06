using System;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Example Application NR5G_DPD_AMPM_EVM_ACP_DL\n");
            NR5G_DPD_AMPM_EVM_ACP_DL nr_DPD_AMPM_EVM_ACP_DL = new NR5G_DPD_AMPM_EVM_ACP_DL();
            try
            {
                nr_DPD_AMPM_EVM_ACP_DL.Run();
              
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