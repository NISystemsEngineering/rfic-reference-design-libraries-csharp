using System;
using System.IO;
using NationalInstruments.ModularInstruments.NIRfsg;
using static NationalInstruments.ReferenceDesignLibraries.SG;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class SGExample
    {
        public enum GenerationType { Bursted, Continuous };
        static void Main()
        {
            string resourceName = "VST2";
            string filePath = Path.GetFullPath(@"Support Files\80211a_20M_48Mbps.tdms");
            GenerationType genType = GenerationType.Continuous;

            NIRfsg nIRfsg = new NIRfsg(resourceName, false, false);

            InstrumentConfiguration instrConfig = GetDefaultInstrumentConfiguration();
            instrConfig.CarrierFrequency_Hz = 2e9;

            ConfigureInstrument(nIRfsg, instrConfig);
            Waveform waveform = LoadWaveformFromTDMS(nIRfsg, filePath);

            DownloadWaveform(nIRfsg, waveform);

            switch (genType)
            {
                // For continous generation, we can simply call this function to begin the generation
                case GenerationType.Continuous:
                    ConfigureContinuousGeneration(nIRfsg, waveform);
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
