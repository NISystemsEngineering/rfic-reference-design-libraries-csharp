using System;
using System.IO;
using NationalInstruments.ModularInstruments.NIRfsg;
using static NationalInstruments.ReferenceDesignLibraries.SG;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class SGExample
    {
        static void Main()
        {
            string resourceName = "VST2";
            string filePath = Path.GetFullPath(@"Support Files\80211a_20M_48Mbps.tdms");

            NIRfsg nIRfsg = new NIRfsg(resourceName, false, false);
            InstrumentConfiguration instrConfig = new InstrumentConfiguration();
            instrConfig.SetDefaults();
            instrConfig.CarrierFrequency_Hz = 2e9;

            ConfigureInstrument(nIRfsg, instrConfig);
            Waveform waveform = LoadWaveformFromTDMS(nIRfsg, filePath);

            DownloadWaveform(nIRfsg, ref waveform);

            WaveformTimingConfiguration dynamicConfig = new WaveformTimingConfiguration
            {
                DutyCycle_Percent = 20,
                PreBurstTime_s = 500e-9,
                PostBurstTime_s = 500e-9,
            };
            PAENConfiguration paenConfig = new PAENConfiguration
            {
                PAEnableMode = PAENMode.Dynamic,
                PAEnableTriggerExportTerminal = "PFI0",
                PAEnableTriggerMode = RfsgMarkerEventOutputBehaviour.Toggle,
                CommandEnableTime_s = 0,
                CommandDisableTime_s = 0,
            };

            ConfigureWaveformTimingAndPAControl(nIRfsg, ref waveform, dynamicConfig, paenConfig, out _, out _);

            nIRfsg.Initiate();

            Console.ReadKey();

            AbortDynamicGeneration(nIRfsg);
            CloseInstrument(nIRfsg);
        }
    }
}
