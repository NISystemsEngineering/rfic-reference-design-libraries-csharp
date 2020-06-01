using System;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Example Application LTE_NanosemiDPD_EVM_ACP_UL\n");
            LTE_NanosemiDPD_EVM_ACP_UL lteNanosemiDpdAmPmEvmAcpUl = new LTE_NanosemiDPD_EVM_ACP_UL();
            try
            {
                lteNanosemiDpdAmPmEvmAcpUl.Run();

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