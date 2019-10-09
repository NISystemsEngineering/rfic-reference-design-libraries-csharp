using NationalInstruments.ModularInstruments.NIRfsg;
using System;
using System.IO;
using static NationalInstruments.ReferenceDesignLibraries.SG;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class SGExample
    {
        public enum GenerationType { Bursted, Continuous };

        static void Main()
        {
            // Initialize variables
            string resourceName = "VST2";
            string filePath = Path.GetFullPath(@"Support Files\80211a_20M_48Mbps.tdms");
            GenerationType genType = GenerationType.Continuous;

            // Initialize session
            NIRfsg niRfsg = new NIRfsg(resourceName, false, false);

            // Configure instrument
            InstrumentConfiguration instrConfig = InstrumentConfiguration.GetDefault(niRfsg);
            ConfigureInstrument(niRfsg, instrConfig);

            // Download waveform
            Waveform waveform = LoadWaveformFromTDMS(filePath);
            DownloadWaveform(niRfsg, waveform);

            switch (genType)
            {
                // For continous generation, we can simply call this function to begin the generation
                case GenerationType.Continuous:
                    ConfigureContinuousGeneration(niRfsg, waveform);
                    break;
                // For bursted generation, we need to configure the duty cycle and PA control
                case GenerationType.Bursted:
                    WaveformTimingConfiguration dynamicConfig = new WaveformTimingConfiguration
                    {
                        DutyCycle_Percent = 20,
                        PreBurstTime_s = 500e-9,
                        PostBurstTime_s = 500e-9,
                        BurstStartTriggerExport = "PXI_Trig0"
                    };
                    PAENConfiguration paenConfig = new PAENConfiguration
                    {
                        PAEnableMode = PAENMode.Dynamic,
                        PAEnableTriggerExportTerminal = "PFI0",
                        PAEnableTriggerMode = RfsgMarkerEventOutputBehaviour.Toggle,
                        CommandEnableTime_s = 0,
                        CommandDisableTime_s = 0,
                    };
                    ConfigureBurstedGeneration(niRfsg, waveform, dynamicConfig, paenConfig, out _, out _);
                    break;
            }

            // Initiate generation
            niRfsg.Initiate();

            // Wait on user
            Console.WriteLine("Generation has now begun. Press any key to abort generation and close the example.");
            Console.ReadKey();

            // Stop generation
            AbortGeneration(niRfsg);

            // Close instrument session
            CloseInstrument(niRfsg);
        }
    }
}
