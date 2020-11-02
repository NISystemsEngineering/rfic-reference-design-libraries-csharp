using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.NRMX;
using static NationalInstruments.ReferenceDesignLibraries.SA.RFmxNR;
using System;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class Program
    {
        /// <summary>
        /// This example illustrates how to use the RFmxNR APIs to configure the analyzer to perform a ModAcc measurement. 
        /// You can use the Generator Basic example to generate the NR signal before running this example.
        /// </summary>
        static void Main(string[] args)
        {
            Console.WriteLine("\n----------------------- 5GNR Analyzer Example -----------------------\n");
            double centerFrequency = 3.5e9; //Hz
            string resourceName = "5840";
            string signalString = "Signal0";
            string resultString = "Result0";
            SA.RFmxInstr.InstrumentConfiguration saInstrConfig;
            SA.CommonConfiguration saCommonConfig;
            SA.AutoLevelConfiguration saAutolevelConfig;
            SA.RFmxNR.StandardConfiguration StandardConfigNR;
            SA.RFmxNR.ModAccConfiguration ModaccConfigNR;
            SA.RFmxNR.ModAccResults ModaccResultsNR = new ModAccResults();

            //Analyzer Configuration 
            Console.WriteLine("Configure...\n");
            saInstrConfig = SA.RFmxInstr.InstrumentConfiguration.GetDefault();
            saCommonConfig = saCommonConfig = SA.CommonConfiguration.GetDefault();
            saCommonConfig.ExternalAttenuation_dB = 0;
            saCommonConfig.CenterFrequency_Hz = centerFrequency;
            saCommonConfig.ReferenceLevel_dBm = 0.0;
            saAutolevelConfig = SA.AutoLevelConfiguration.GetDefault();
            saAutolevelConfig.Enabled = true;
            StandardConfigNR = SA.RFmxNR.StandardConfiguration.GetDefault();
            StandardConfigNR.ComponentCarrierConfigurations[0].PuschModulationType = RFmxNRMXPuschModulationType.Qpsk;
            ModaccConfigNR = SA.RFmxNR.ModAccConfiguration.GetDefault();

            #region Configure Analyzer
            saAutolevelConfig.MeasurementInterval_s = 0.001;
            RFmxInstrMX instr = new RFmxInstrMX(resourceName, "");
            SA.RFmxInstr.ConfigureInstrument(instr, saInstrConfig);
            RFmxNRMX nr = instr.GetNRSignalConfiguration(signalString);
            SA.RFmxNR.ConfigureCommon(nr, saCommonConfig);
            SA.RFmxNR.ConfigureStandard(nr, StandardConfigNR);
            #endregion

            #region Measure
            Console.WriteLine("Measure...\n");
            ConfigureModacc(nr, ModaccConfigNR);
            RFmxNRMXMeasurementTypes[] nrMeasurements = new RFmxNRMXMeasurementTypes[1] { RFmxNRMXMeasurementTypes.ModAcc };
            SA.RFmxNR.SelectAndInitiateMeasurements(nr, nrMeasurements, saAutolevelConfig, false, "", resultString);
            ModaccResultsNR = FetchModAcc(nr,RFmxNRMX.BuildResultString(resultString));
            //print Results
            for (int i = 0; i < ModaccResultsNR.ComponentCarrierResults.Length; i++)
            {
                Console.WriteLine("----------------------- EVM Results CC {0} -----------------------\n", i);
                Console.WriteLine("Composite RMS EVM Mean (% or dB)               : {0:0.000}", ModaccResultsNR.ComponentCarrierResults[i].MeanRmsCompositeEvm);
                Console.WriteLine("Composite Peak EVM Maximum (% or dB)           : {0:0.000}", ModaccResultsNR.ComponentCarrierResults[i].MaxPeakCompositeEvm);
                Console.WriteLine("Composite Peak EVM Slot Index                  : {0}", ModaccResultsNR.ComponentCarrierResults[i].PeakCompositeEvmSlotIndex);
                Console.WriteLine("Composite Peak EVM Symbol Index                : {0}", ModaccResultsNR.ComponentCarrierResults[i].PeakCompositeEvmSymbolIndex);
                Console.WriteLine("Composite Peak EVM Subcarrier Index            : {0}", ModaccResultsNR.ComponentCarrierResults[i].PeakCompositeEvmSubcarrierIndex);
                Console.WriteLine("Component Carrier Frequency Error Mean (Hz)    : {0:0.000}", ModaccResultsNR.ComponentCarrierResults[i].MeanFrequencyError_Hz);
            }
            #endregion
            nr.Dispose();
            instr.Close();
            Console.WriteLine("Please press any key to close the application.\n");
            Console.ReadKey();
        }

    }
}

