using System;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class Program
    {
        /// <summary>
        /// This example illustrates how to use the RFmx and RFSG drivers and the WLAN toolkit to perform SEM and EVM measurements with or without digital predistortion (DPD) of the waveform input to the DUT.
        /// Before executing this application, please check and ensure you have the right configurations in the InitializeParameters() function.
        /// 
        /// DPD is a technique to optimize the operating point of power amplifiers and other non-linear RFIC devices.
        /// Power amplifiers are much more efficient if they operate near compression.However, compressing signal amplitudes can significantly deteriorate
        /// the amplified signal's quality both, in-band (e.g., EVM increases due to non-linear distortion of the signal's I/Q constellation) and 
        /// out-of-band(e.g., SEM test fails due to spectral regrowth). With DPD, the original waveform is first pre-distorted in a way that matches
        /// the non-linear effects of the RFIC device before being fed through the device.The resulting output is a much cleaner amplified and/or filtered version of the original waveform.
        /// 
        /// Different DPD models can be used to trade off performance with computational ease, with the Lookup-Table (LUT) model the simplest and the Generalized Memory Polynomial Model (GMP) the most complex. 
        /// With a LUT, the DPD algorithm achieves gain and phase linearization on average. If you want to correct for memory effects - i.e., an output sample not only depends on the current input but also
        /// previous input samples; you will notice that a specific input power value translates into a range of gain and phase values  - you would use one of the memory polynomial models.
        /// For these models, several parameters such as polynomial order and memory depth are available to fine-tune the model to the DUT behavior.
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
