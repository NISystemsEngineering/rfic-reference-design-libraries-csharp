using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.WlanMX;
using static NationalInstruments.ReferenceDesignLibraries.SA.RFmxWLAN;
using System;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class Program
    {
        /// <summary>
        /// This example illustrates how to use the RFmx and RFSG drivers and the WLAN toolkit to perform EVM measurements.
        /// </summary>
        static void Main(string[] args)
        {
            Console.WriteLine("\n----------------------- WLAN Analyzer Example -----------------------\n");
            double centerFrequency = 3.5e9; //Hz
            string resourceName = "5840";
            string signalString = "Signal0";
            string resultString = "Result0";
            SA.RFmxInstr.InstrumentConfiguration saInstrConfig;
            SA.CommonConfiguration saCommonConfig;
            SA.AutoLevelConfiguration saAutolevelConfig;
            SA.RFmxWLAN.StandardConfiguration wlanStandardConfig;
            SA.RFmxWLAN.OFDMModAccConfiguration modaccConfig;
            SA.RFmxWLAN.OFDMModAccResults modAccResults = new OFDMModAccResults();

            //Analyzer Configuration 
            Console.WriteLine("Configure...\n");
            saInstrConfig = SA.RFmxInstr.InstrumentConfiguration.GetDefault();
            saCommonConfig = saCommonConfig = SA.CommonConfiguration.GetDefault();
            saCommonConfig.ExternalAttenuation_dB = 0;
            saCommonConfig.CenterFrequency_Hz = centerFrequency;
            saCommonConfig.ReferenceLevel_dBm = 0.0;
            saAutolevelConfig = SA.AutoLevelConfiguration.GetDefault();
            saAutolevelConfig.Enabled = true;
            saAutolevelConfig.MeasurementInterval_s = 10e-3;
            wlanStandardConfig = SA.RFmxWLAN.StandardConfiguration.GetDefault();
            wlanStandardConfig.ChannelBandwidth_Hz = 80.0e6;
            wlanStandardConfig.Standard = RFmxWlanMXStandard.Standard802_11ax;
            modaccConfig = SA.RFmxWLAN.OFDMModAccConfiguration.GetDefault();

            #region Configure Analyzer
            RFmxInstrMX instr = new RFmxInstrMX(resourceName, "");
            SA.RFmxInstr.ConfigureInstrument(instr, saInstrConfig);
            RFmxWlanMX wlan = instr.GetWlanSignalConfiguration(signalString);
            SA.RFmxWLAN.ConfigureCommon(wlan, saCommonConfig);
            SA.RFmxWLAN.ConfigureStandard(wlan, wlanStandardConfig);
            #endregion

            #region Measure
            Console.WriteLine("Measure...\n");
            ConfigureOFDMModAcc(wlan, modaccConfig);
            RFmxWlanMXMeasurementTypes[] lteMeasurements = new RFmxWlanMXMeasurementTypes[1] { RFmxWlanMXMeasurementTypes.OfdmModAcc };
            SA.RFmxWLAN.SelectAndInitiateMeasurements(wlan, lteMeasurements, saAutolevelConfig, false, "", resultString);
            modAccResults = FetchOFDMModAcc(wlan,RFmxWlanMX.BuildResultString(resultString));
            //print Results
            Console.WriteLine("\n---------------------- ModAcc Results ---------------------\n");
            Console.WriteLine("Composite RMS EVM (dB): {0:N}", modAccResults.CompositeRMSEVMMean_dB);
            #endregion
            wlan.Dispose();
            instr.Close();
            Console.WriteLine("Please press any key to close the application.\n");
            Console.ReadKey();
        }
    }
}

