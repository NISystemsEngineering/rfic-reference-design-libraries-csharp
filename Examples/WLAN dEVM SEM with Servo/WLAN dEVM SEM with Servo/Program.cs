using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.WlanMX;
using System;
using System.IO;
using NationalInstruments.ReferenceDesignLibraries;
using NationalInstruments.ReferenceDesignLibraries.SA;
using NationalInstruments.ReferenceDesignLibraries.Methods;
using NationalInstruments.ModularInstruments;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class RFmxWLANExample
    {
        public static string VSTName = "VST2";
        public static string WaveformPath = @"Support Files\80211a_20M_48Mbps.tdms";

        public static double WaveformBandwidth = 20e6;
        public static RFmxWlanMXStandard WaveformStandard = RFmxWlanMXStandard.Standard802_11ag;
        public static double CarrierFrequency = 2.412e9;
        public static double SAExternalAttenuation_dB = 0;

        public static double DesiredDutOutputPower_dBm = -10;
        public static double ExpectedDutGain_dBm = 0;

        static void Main()
        {
            PowerServo.InitializeVSTForServo(VSTName, out NIRfsg rfsgSession, out RFmxInstrMX instrSession,
                out niPowerServo servoSession);
            RFmxWlanMX wlan = instrSession.GetWlanSignalConfiguration();
            #region Configure Generator
            SG.InstrumentConfiguration sgConfig = SG.InstrumentConfiguration.GetDefault();
            sgConfig.CarrierFrequency_Hz = CarrierFrequency;
            SG.ConfigureInstrument(rfsgSession, sgConfig);

            SG.Waveform wave = SG.LoadWaveformFromTDMS(WaveformPath);
            SG.DownloadWaveform(rfsgSession, wave);

            SG.WaveformTimingConfiguration waveTiming = SG.WaveformTimingConfiguration.GetDefault();
            waveTiming.DutyCycle_Percent = 20;
            SG.PAENConfiguration paenConfig = SG.PAENConfiguration.GetDefault();
            SG.ConfigureBurstedGeneration(rfsgSession, wave, waveTiming, paenConfig, out double _, out double _);
            rfsgSession.Utility.Commit();
            #endregion
            #region Configure Analyzer

            RFmxWLAN.CommonConfiguration saConfig = RFmxWLAN.CommonConfiguration.GetDefault();
            saConfig.CenterFrequency_Hz = CarrierFrequency;
            saConfig.ExternalAttenuation_dB = SAExternalAttenuation_dB;
            RFmxWLAN.ConfigureCommon(instrSession, wlan, saConfig, RFmxWLAN.AutoLevelConfiguration.GetDefault());

            RFmxWLAN.SignalConfiguration signalConfig = new RFmxWLAN.SignalConfiguration
            {
                AutoDetectSignal = false,
                ChannelBandwidth_Hz = WaveformBandwidth,
                Standard = WaveformStandard
            };
            RFmxWLAN.ConfigureSignal(wlan, signalConfig);

            RFmxWLAN.TxPConfiguration txpConfig = RFmxWLAN.TxPConfiguration.GetDefault();
            txpConfig.MaximumMeasurementInterval_s = wave.BurstLength_s;
            RFmxWLAN.ConfigureTxP(wlan, txpConfig);

            RFmxWLAN.OFDMModAccConfiguration modAccConfig = RFmxWLAN.OFDMModAccConfiguration.GetDefault();
            modAccConfig.OptimizeDynamicRangeForEvmEnabled = RFmxWlanMXOfdmModAccOptimizeDynamicRangeForEvmEnabled.False;
            modAccConfig.AveragingEnabled = RFmxWlanMXOfdmModAccAveragingEnabled.True;
            RFmxWLAN.ConfigureOFDMModAcc(wlan, modAccConfig);


            RFmxWLAN.SEMConfiguration semConfig = RFmxWLAN.SEMConfiguration.GetDefault();
            RFmxWLAN.ConfigureSEM(wlan, semConfig);
            wlan.Commit("");
            #endregion
            #region Configure Servo
            PowerServo.DutConfiguration dutConfig = PowerServo.DutConfiguration.GetDefault();
            dutConfig.DutDesiredOutputPower_dBm = DesiredDutOutputPower_dBm;
            dutConfig.DutEstimatedGain_dBm = ExpectedDutGain_dBm;
            PowerServo.CalculatedVSTLevels calcLevels = PowerServo.CalculateVSTLevels(servoSession, dutConfig, wave);

            rfsgSession.RF.PowerLevel = calcLevels.VSGAveragePowerLevel_dBm;
            rfsgSession.Utility.Commit();
            wlan.ConfigureReferenceLevel("", calcLevels.VSAReferenceLevel_dBm);
            wlan.Commit("");

            PowerServo.ServoConfiguration servoConfig = PowerServo.ServoConfiguration.GetDefault();
            PowerServo.ConfigureServo(servoSession, servoConfig, wave);
            #endregion
            #region Initiate and Measure
            rfsgSession.Initiate();
            wlan.Initiate("", "");
            PowerServo.ServoResults servoResults = PowerServo.InitiateServo(servoSession, rfsgSession);

            RFmxWLAN.TxPResults txpRes = RFmxWLAN.FetchTxP(wlan);
            RFmxWLAN.OFDMModAccResults modAccResults = RFmxWLAN.FetchOFDMModAcc(wlan);
            RFmxWLAN.SEMResults semResults = RFmxWLAN.FetchSEM(wlan);

            SG.AbortGeneration(rfsgSession);
            PowerServo.AbortServo(servoSession);

            Console.WriteLine($"Final DUT Input Power: {servoResults.FinalInputPower_dBm}");
            Console.WriteLine($"Final DUT Output Power: {servoResults.FinalOutputPower_dBm}");
            Console.WriteLine($"Calculated DUT Gain: {servoResults.CalculatedDutGain_dB}");
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
            instrSession.Close();
            rfsgSession.Close();
            servoSession.Close();
            #endregion
        }

    }
}
