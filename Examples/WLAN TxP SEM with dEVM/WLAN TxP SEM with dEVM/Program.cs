using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.WlanMX;
using System;
using System.IO;
using static NationalInstruments.ReferenceDesignLibraries.SA.RFmxWLAN;
using static NationalInstruments.ReferenceDesignLibraries.SG;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    static class RFmxWLANExample
    {
        static void Main()
        {
            #region Configure Generation
            string resourceName = "VST2";
            string filePath = Path.GetFullPath(@"Support Files\80211a_20M_48Mbps.tdms");

            NIRfsg nIRfsg = new NIRfsg(resourceName, false, false);
            InstrumentConfiguration instrConfig = InstrumentConfiguration.GetDefault();
            instrConfig.CarrierFrequency_Hz = 2.412e9;

            ConfigureInstrument(nIRfsg, instrConfig);
            Waveform waveform = LoadWaveformFromTDMS(filePath);

            DownloadWaveform(nIRfsg, waveform);

            WaveformTimingConfiguration timing = new WaveformTimingConfiguration
            {
                DutyCycle_Percent = 60,
                PreBurstTime_s = 1e-9,
                PostBurstTime_s = 1e-9,
                BurstStartTriggerExport = "PXI_Trig0"
            };

            PAENConfiguration paenConfig = new PAENConfiguration
            {
                PAEnableMode = PAENMode.Dynamic,
                PAEnableTriggerExportTerminal = "PFI0",
                PAEnableTriggerMode = RfsgMarkerEventOutputBehaviour.Toggle
            };

            ConfigureBurstedGeneration(nIRfsg, waveform, timing, paenConfig, out double period, out _);
            nIRfsg.Initiate();
            #endregion

            RFmxInstrMX instr = new RFmxInstrMX(resourceName, "");
            RFmxWlanMX wlan = instr.GetWlanSignalConfiguration();
            instr.GetWlanSignalConfiguration();


            CommonConfiguration commonConfiguration = CommonConfiguration.GetDefault();
            commonConfiguration.CenterFrequency_Hz = 2.412e9;

            AutoLevelConfiguration autoLevel = new AutoLevelConfiguration
            {
                AutoLevelMeasureTime_s = period,
                AutoLevelReferenceLevel = true
            };

            SA.RFmxWLAN.ConfigureCommon(instr, wlan, commonConfiguration);

            SignalConfiguration signal = SignalConfiguration.GetDefault();
            signal.AutoDetectSignal = false;
            signal.ChannelBandwidth_Hz = 20e6;
            signal.Standard = RFmxWlanMXStandard.Standard802_11ag;

            SA.RFmxWLAN.ConfigureSignal(wlan, signal);

            TxPConfiguration txpConfig = new TxPConfiguration
            {
                AveragingCount = 10,
                MaximumMeasurementInterval_s = waveform.BurstLength_s,
                AveragingEnabled = RFmxWlanMXTxpAveragingEnabled.True
            };

            SA.RFmxWLAN.ConfigureTxP(wlan, txpConfig);

            OFDMModAccConfiguration modAccConfig = OFDMModAccConfiguration.GetDefault();
            modAccConfig.OptimizeDynamicRangeForEvmEnabled = RFmxWlanMXOfdmModAccOptimizeDynamicRangeForEvmEnabled.False;
            modAccConfig.AveragingEnabled = RFmxWlanMXOfdmModAccAveragingEnabled.True;

            SA.RFmxWLAN.ConfigureOFDMModAcc(wlan, modAccConfig);

            TxPServoConfiguration servoConfig = TxPServoConfiguration.GetDefault();
            servoConfig.TargetTxPPower_dBm = 0.5;

            SA.RFmxWLAN.TxPServoPower(wlan, nIRfsg, servoConfig, autoLevel);

            SEMConfiguration semConfig = SEMConfiguration.GetDefault();
            SA.RFmxWLAN.ConfigureSEM(wlan, semConfig);

            wlan.Initiate("", "");

            TxPResults txpRes = SA.RFmxWLAN.FetchTxP(wlan);
            OFDMModAccResults modAccResults = SA.RFmxWLAN.FetchOFDMModAcc(wlan);
            SEMResults semResults = SA.RFmxWLAN.FetchSEM(wlan);

            Console.WriteLine("TXP Avg Power: {0:N}", txpRes.AveragePowerMean_dBm);
            Console.WriteLine("Composite RMS EVM (dB): {0:N}", modAccResults.CompositeRMSEVMMean_dB);
            Console.WriteLine("\n----------Lower Offset Measurements----------\n");
            for (int i = 0; i < semResults.LowerOffsetMargin_dB.Length; i++)
            {
                Console.WriteLine("Offset {0}", i);
                Console.WriteLine("Measurement Status              :{0}",
                     semResults.lowerOffsetMeasurementStatus[i]);
                Console.WriteLine("Margin (dB)                     :{0}", semResults.LowerOffsetMargin_dB[i]);
                Console.WriteLine("Margin Frequency (Hz)           :{0}", semResults.LowerOffsetMarginFrequency_Hz[i]);
                Console.WriteLine("Margin Absolute Power (dBm)     :{0}\n", semResults.LowerOffsetMarginAbsolutePower_dBm[i]);
            }

            Console.WriteLine("\n----------Upper Offset Measurements----------\n");
            for (int i = 0; i < semResults.UpperOffsetMargin_dB.Length; i++)
            {
                Console.WriteLine("Offset {0}", i);
                Console.WriteLine("Measurement Status              :{0}", semResults.upperOffsetMeasurementStatus[i]);
                Console.WriteLine("Margin (dB)                     :{0}", semResults.UpperOffsetMargin_dB[i]);
                Console.WriteLine("Margin Frequency (Hz)           :{0}", semResults.UpperOffsetMarginFrequency_Hz[i]);
                Console.WriteLine("Margin Absolute Power (dBm)     :{0}\n", semResults.UpperOffsetMarginAbsolutePower_dBm[i]);
            }

            Console.WriteLine("\n--------------------\n\nPress any key to exit.");
            Console.ReadKey();

            wlan.Dispose();
            instr.Close();

            AbortGeneration(nIRfsg);
            CloseInstrument(nIRfsg);

        }
    }

}
