using NationalInstruments.ModularInstruments.NIRfsg;
using System;
using System.IO;
using static NationalInstruments.ReferenceDesignLibraries.SG;


namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class SGExample
    {
        public enum GenerationType { Bursted, Continuous };

        /// <summary>
        /// This example illustrates how to use RFSG drivers to generate burst or continuous waveform. 
        /// </summary>
        static void Main()
        {
            string resourceName = "5840";
            string filePath = Path.GetFullPath(@"C:\Users\Public\Documents\National Instruments\RFIC Test Software\Waveforms\NR_FR1_UL_1x100MHz_30kHz-SCS_256QAM_OS4_VST2_1ms.tdms");
            //string filePath = Path.GetFullPath(@"C:\Users\Public\Documents\National Instruments\RFIC Test Software\Waveforms\LTE_FDD_UL_1x20MHz_256QAM_OS4.tdms");
            //string filePath = Path.GetFullPath(@"C:\Users\Public\Documents\National Instruments\RFIC Test Software\Waveforms\80211ax_80M_MCS11.tdms");
            GenerationType genType = GenerationType.Continuous;

            NIRfsg nIRfsg = new NIRfsg(resourceName, false, false);

            InstrumentConfiguration instrConfig = InstrumentConfiguration.GetDefault();
            instrConfig.CarrierFrequency_Hz = 3.5e9;

            ConfigureInstrument(nIRfsg, instrConfig);
            Waveform waveform = LoadWaveformFromTDMS(filePath);

            DownloadWaveform(nIRfsg, waveform);

            switch (genType)
            {
                // For continous generation, we can simply call this function to begin the generation
                case GenerationType.Continuous:
                    ConfigureContinuousGeneration(nIRfsg, waveform, "PXI_Trig0");
                    break;
                // For bursted generation, we need to configure the duty cycle and PA control
                case GenerationType.Bursted:
                    WaveformTimingConfiguration dynamicConfig = WaveformTimingConfiguration.GetDefault();
                    PAENConfiguration paenConfig = PAENConfiguration.GetDefault();
                    ConfigureBurstedGeneration(nIRfsg, waveform, dynamicConfig, paenConfig, out _, out _);
                    break;
            }

            nIRfsg.Initiate();

            Console.WriteLine("Generation has now begun. Press any key to abort generation and close the example.");
            Console.ReadKey();

            AbortGeneration(nIRfsg);

            CloseInstrument(nIRfsg);
        }
    }
}
