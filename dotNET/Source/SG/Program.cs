using System;
using NationalInstruments.ModularInstruments.NIRfsg;
using static NationalInstruments.ReferenceDesignLibraries.SG;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class Program
    {
        
        static void Main()
        {
            string resourceName = "VST2";
            string filePath = @"C:\P4\Sales\projects\RefApps\ReferenceDesignLibraries\LabVIEW\Export\pSemi WLAN FEM Test\1.0\TDMS Files\MMC_MCS9HT40_FS_250M_v3.tdms";

            NIRfsg nIRfsg = new NIRfsg(resourceName, false, false);
            InstrumentConfiguration instrConfig = new InstrumentConfiguration();
            instrConfig.SetDefaults();

            ConfigureInstrument(ref nIRfsg, instrConfig);
            Waveform waveform = LoadWaveformFromTDMS(ref nIRfsg, filePath);

            DownloadWaveform(ref nIRfsg, ref waveform);

            WaveformGenerationTiming timing = new WaveformGenerationTiming
            {
                DutyCycle_Percent = 60,
                PFIPortMode = WaveformGenerationTiming.PFIMode.Dynamic,
                PreBurstTime_s = 1e-9,
                PostBurstTime_s = 1e-9,
            };

            CreatedAndDownloadScript(ref nIRfsg, ref waveform, timing, out _, out _);

            waveform = GetWaveformParametersByName(ref nIRfsg, waveform.WaveformName);

            TogglePFILine(ref nIRfsg, RfsgMarkerEventToggleInitialState.DigitalLow);
            CloseInstrument(ref nIRfsg);
        }
    }
}
