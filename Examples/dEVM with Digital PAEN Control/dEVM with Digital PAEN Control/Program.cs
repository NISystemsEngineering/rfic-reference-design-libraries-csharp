using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.ReferenceDesignLibraries;
using System;
using System.IO;
using static NationalInstruments.ReferenceDesignLibraries.Digital;
using static NationalInstruments.ReferenceDesignLibraries.SG;

namespace Digital_Dynamic_PAEN_Example
{
    class Program
    {
        enum DigitalMode { RFFE_MIPI, Digital_Enable };
        static void Main(string[] args)
        {
            #region Station Configuration
            string sgName = "VST2";
            string digitalName = "PXIe-6570";
            DigitalMode desiredMode = DigitalMode.Digital_Enable;
            //To calculate this delay, use the TDR feature of the Digital Pattern Editor (DPE)
            //In the DPE, navigate to Instruments » TDR...
            double PFI0CableDelay_s = 11.4e-9;
            #endregion

            #region SG Configuration
            NIRfsg rfsgSession = new NIRfsg(sgName, false, false);

            InstrumentConfiguration instrConfig = InstrumentConfiguration.GetDefault(rfsgSession);
            instrConfig.ReferenceClockSource = RfsgFrequencyReferenceSource.PxiClock;
            instrConfig.CarrierFrequency_Hz = 2.402e9;
            instrConfig.DutAverageInputPower_dBm = 0;

            ConfigureInstrument(rfsgSession, instrConfig);

            string waveformPath = Path.GetFullPath(@"TDMS Files\11AC_MCS8_40M.tdms");

            Waveform wave = LoadWaveformFromTDMS(waveformPath, "wave");
            DownloadWaveform(rfsgSession, wave);

            WaveformTimingConfiguration waveTiming = new WaveformTimingConfiguration
            {
                DutyCycle_Percent = 20,
                PreBurstTime_s = 2000e-9,
                PostBurstTime_s = 500e-9,
                BurstStartTriggerExport = "PXI_Trig0"
            };
            PAENConfiguration paenConfig = new PAENConfiguration
            {
                PAEnableMode = PAENMode.Dynamic,
                PAEnableTriggerExportTerminal = "PFI0",
                PAEnableTriggerMode = RfsgMarkerEventOutputBehaviour.Toggle,
            };

            switch (desiredMode)
            {
                case DigitalMode.RFFE_MIPI:
                    //Calculate the command length by multiplying the number of vector cycles (18 for Reg0Write)
                    //by the clock rate (currently 52 MHz)
                    paenConfig.CommandEnableTime_s = 18 * (1 / 52e6);
                    paenConfig.CommandDisableTime_s = 18 * (1 / 52e6);
                    break;
                case DigitalMode.Digital_Enable:
                    //For the digital enable case, the command time can be considered to be 0 since the digital
                    //line is simply being toggled high/low
                    paenConfig.CommandDisableTime_s = 0;
                    paenConfig.CommandEnableTime_s = 0;
                    break;
            }
            //Until NI-DIGITAL 19.0 is released, the triggering mechanism used is to trigger using the PFI line
            //and detecting the change with a match opcode. There are a total of 830 cycles of the instrument required
            //before the command is sent from the pattern. Hence, an 830ns delay is added to the command time to account
            //for this.
            paenConfig.CommandEnableTime_s += 830e-9;
            paenConfig.CommandDisableTime_s += 830e-9;

            ConfigureBurstedGeneration(rfsgSession, wave, waveTiming, paenConfig, out _, out _);
            #endregion

            #region NI Digital Config
            NIDigital digital = new NIDigital(digitalName, false, false);

            ProjectFiles projectFiles = Digital.Utilities.SearchForProjectFiles(Path.GetFullPath(@"Dynamic Digital Control Project"), true);
            LoadProjectFiles(digital, projectFiles);

            ApplyPinTDROffset(digital, "PFI0", PFI0CableDelay_s);

            //Select the appropriate pattern based on the desired control mechanism
            switch (desiredMode)
            {
                case DigitalMode.Digital_Enable:
                    digital.PatternControl.BurstPattern("", "Dynamic_Digital_Enable", true, false, TimeSpan.FromSeconds(10));
                    break;
                case DigitalMode.RFFE_MIPI:
                    digital.PatternControl.BurstPattern("", "Dynamic_RFFE_Control", true, false, TimeSpan.FromSeconds(10));
                    break;
            }

            #endregion

            rfsgSession.Initiate();

            Console.WriteLine("Generation on the signal generator and digital pattern instrument has begun. Press any key to abort generation and exit the program.");
            Console.ReadKey();

            AbortGeneration(rfsgSession);

            digital.PatternControl.Abort();

            DisconnectAndClose(digital);
            rfsgSession.Close();
        }
    }
}
