using System;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class Program
    {
        /// <summary>
        /// This example illustrates how to use power meter to do power servoing. 
        /// </summary>
        static void Main()
        {
            Console.WriteLine("Example Generation Basic with Power Meter Servoing.\n");
            SG_PowerMeter_Servoing sg_powerMeter_servoing = new SG_PowerMeter_Servoing();

            try
            {
                sg_powerMeter_servoing.Run();
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
