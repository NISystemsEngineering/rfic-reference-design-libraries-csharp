using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.LteMX;
using static NationalInstruments.ReferenceDesignLibraries.SA.RFmxLTE;
using System;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class Program
    {
        /// <summary>
        /// This example illustrates how to use the RFmxLTE APIs to configure the analyzer to perform a ModAcc measurement. 
        /// You can use the Generator Basic example to generate the LTE signal before running this example.
        /// </summary>
        static void Main(string[] args)
        {
            Console.WriteLine("\n----------------------- LTE Analyzer Example -----------------------\n");
            double centerFrequency = 3.5e9; //Hz
            string resourceName = "5840";
            string signalString = "Signal0";
            string resultString = "Result0";
            SA.RFmxInstr.InstrumentConfiguration saInstrConfig;
            SA.CommonConfiguration saCommonConfig;
            SA.AutoLevelConfiguration saAutolevelConfig;
            SA.RFmxLTE.StandardConfiguration StandardConfigLte;
            SA.RFmxLTE.ModAccConfiguration ModaccConfigLte;
            SA.RFmxLTE.ModAccResults ModaccResultsLte = new ModAccResults();

            //Analyzer Configuration 
            Console.WriteLine("Configure...\n");
            saInstrConfig = SA.RFmxInstr.InstrumentConfiguration.GetDefault();
            saCommonConfig = SA.CommonConfiguration.GetDefault();
            saCommonConfig.ExternalAttenuation_dB = 0;
            saCommonConfig.CenterFrequency_Hz = centerFrequency;
            saCommonConfig.ReferenceLevel_dBm = 0.0;
            saAutolevelConfig = SA.AutoLevelConfiguration.GetDefault();
            saAutolevelConfig.Enabled = true;
            StandardConfigLte = SA.RFmxLTE.StandardConfiguration.GetDefault();
            StandardConfigLte.AutoDmrsDetectionEnabled = RFmxLteMXAutoDmrsDetectionEnabled.True;
            StandardConfigLte.ComponentCarrierConfigurations[0].Bandwidth_Hz = 20.0e6;
            ModaccConfigLte = SA.RFmxLTE.ModAccConfiguration.GetDefault();

            #region Configure Analyzer
            saAutolevelConfig.MeasurementInterval_s = 0.001;
            RFmxInstrMX instr = new RFmxInstrMX(resourceName, "");
            SA.RFmxInstr.ConfigureInstrument(instr, saInstrConfig);
            RFmxLteMX lte = instr.GetLteSignalConfiguration(signalString);
            SA.RFmxLTE.ConfigureCommon(lte, saCommonConfig);
            SA.RFmxLTE.ConfigureStandard(lte, StandardConfigLte);
            #endregion

            #region Measure
            Console.WriteLine("Measure...\n");
            ConfigureModAcc(lte, ModaccConfigLte);
            RFmxLteMXMeasurementTypes[] lteMeasurements = new RFmxLteMXMeasurementTypes[1] { RFmxLteMXMeasurementTypes.ModAcc };
            SA.RFmxLTE.SelectAndInitiateMeasurements(lte, lteMeasurements, saAutolevelConfig, false, "", resultString);
            ModaccResultsLte = FetchModAcc(lte,RFmxLteMX.BuildResultString(resultString));
            //print Results
            for (int i = 0; i < ModaccResultsLte.ComponentCarrierResults.Length; i++)
            {
                Console.WriteLine("----------------------- EVM Results CC {0} -----------------------\n", i);
                Console.WriteLine("Composite RMS EVM Mean (% or dB)               : {0:0.000}", ModaccResultsLte.ComponentCarrierResults[i].MeanRmsCompositeEvm);
                Console.WriteLine("Composite Peak EVM Maximum (% or dB)           : {0:0.000}", ModaccResultsLte.ComponentCarrierResults[i].MaxPeakCompositeEvm);
                Console.WriteLine("Composite Peak EVM Slot Index                  : {0}", ModaccResultsLte.ComponentCarrierResults[i].PeakCompositeEvmSlotIndex);
                Console.WriteLine("Composite Peak EVM Symbol Index                : {0}", ModaccResultsLte.ComponentCarrierResults[i].PeakCompositeEvmSymbolIndex);
                Console.WriteLine("Composite Peak EVM Subcarrier Index            : {0}", ModaccResultsLte.ComponentCarrierResults[i].PeakCompositeEvmSubcarrierIndex);
                Console.WriteLine("Component Carrier Frequency Error Mean (Hz)    : {0:0.000}", ModaccResultsLte.ComponentCarrierResults[i].MeanFrequencyError_Hz);
            }
            #endregion
            lte.Dispose();
            instr.Close();
            Console.WriteLine("Please press any key to close the application.\n");
            Console.ReadKey();
        }

    }
}

