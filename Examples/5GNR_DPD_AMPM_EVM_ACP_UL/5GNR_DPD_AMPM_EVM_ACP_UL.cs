using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.NRMX;
using NationalInstruments.RFmx.SpecAnMX;
using System;
using System.IO;
using static NationalInstruments.ReferenceDesignLibraries.SA.RFmxNR;
using static NationalInstruments.ReferenceDesignLibraries.SG;
using static NationalInstruments.ReferenceDesignLibraries.SA.RFmxInstr;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    public class NR5G_DPD_AMPM_EVM_ACP_UL
    {
        public string resourceName;
        public double centerFrequency;
        public bool AutoLoSharing;

        //Generator configuration
        SG.InstrumentConfiguration SgInstrConfig;
        LoConfiguration SgLoConfig;
        SignalConfiguration signal = new SignalConfiguration();
        public string filePath;
        //Analyzer Configuration 
        SA.RFmxInstr.InstrumentConfiguration SaInstrConfig;
        SA.RFmxNR.CommonConfiguration commonConfigurationNR;
        //Measurements Configuration
        public AcpConfiguration acpConfig;
        public ModAccConfiguration modaccConfig;
        public AcpResults acpResults = new AcpResults();
        public ModAccResults modaccResults = new ModAccResults();
        

        public NR5G_DPD_AMPM_EVM_ACP_UL()
        {
            InitializeParameters();
        }

        public void InitializeParameters()
        {
            centerFrequency = 3.5e9; //Hz
            resourceName="VST2";
            AutoLoSharing = true;
            filePath = @"C:\Users\Public\Documents\National Instruments\RFIC Test Software\Waveforms\NR_FR1_UL_1x100MHz_30kHz-SCS_256QAM_OS4_VST2_1ms.tdms";
            //Generator Configuiration
            SgInstrConfig = SG.InstrumentConfiguration.GetDefault();
            SgInstrConfig.CarrierFrequency_Hz = centerFrequency;
            SgInstrConfig.DutAverageInputPower_dBm = -10.0;
            SgInstrConfig.ExternalAttenuation_dBm = 0;
            SgLoConfig = SG.LoConfiguration.GetDefault();
            SgLoConfig.AutoLoShared = AutoLoSharing;


            //Analyzer Configuration 
            SaInstrConfig = SA.RFmxInstr.InstrumentConfiguration.GetDefault();
            SaInstrConfig.AutoLoShared = AutoLoSharing;
            //Analyzer Configuration (NR)
            commonConfigurationNR = SA.RFmxNR.CommonConfiguration.GetDefault();
            commonConfigurationNR.CenterFrequency_Hz = centerFrequency;
            commonConfigurationNR.ReferenceLevel_dBm = 0;
            commonConfigurationNR.ExternalAttenuation_dB = 0;

            signal = SignalConfiguration.GetDefault();
            signal.ComponentCarrierConfigurations[0].PuschModulationType = RFmxNRMXPuschModulationType.Qam256;

            acpConfig = SA.RFmxNR.AcpConfiguration.GetDefault();
            acpConfig.NumberOfNrOffsets = 2;
            acpConfig.NumberOfUtraOffsets = 2;
            acpConfig.NumberOfEutraOffsets = 0;

            modaccConfig = SA.RFmxNR.ModAccConfiguration.GetDefault();
           
           
        }

        public void Run()
        {
            
            #region Configure Generation

            NIRfsg nIRfsg = new NIRfsg(resourceName, false, false);
            ConfigureInstrument(nIRfsg, SgInstrConfig);
            Waveform waveform = LoadWaveformFromTDMS(filePath);
            
            DownloadWaveform(nIRfsg, waveform, SgLoConfig);
            ConfigureContinuousGeneration(nIRfsg, waveform);
            nIRfsg.Initiate();
            #endregion

            #region Configure Analyzer
            RFmxInstrMX instr = new RFmxInstrMX(resourceName, "");
            SA.RFmxInstr.ConfigureInstrument(instr, SaInstrConfig);

            RFmxNRMX nr = instr.GetNRSignalConfiguration();
            SA.RFmxNR.ConfigureCommon(instr, nr, commonConfigurationNR);
       
            SA.RFmxNR.ConfigureSignal(nr, signal);
            
            #endregion

            ConfigureAcp(nr, acpConfig);
            nr.SelectMeasurements("", RFmxNRMXMeasurementTypes.Acp, true);
            nr.Initiate("", "");
            acpResults = FetchAcp(nr);
            PrintACPResults();
            
            ConfigureModacc(nr, modaccConfig);
            nr.SelectMeasurements("", RFmxNRMXMeasurementTypes.ModAcc, true);
            nr.Initiate("", "");
            modaccResults = FetchModAcc(nr);
            PrintModAccResults();
            
            
            ///TEST///
            AbortGeneration(nIRfsg);
            SgLoConfig.AutoLoShared = false;
            DownloadWaveform(nIRfsg, waveform, SgLoConfig);
            nIRfsg.Initiate();
            SaInstrConfig.AutoLoShared = false;
            SA.RFmxInstr.ConfigureInstrument(instr, SaInstrConfig);
            nr.Initiate("", "");
            modaccResults = FetchModAcc(nr);
            PrintModAccResults();
            //////


            nr.Dispose();
            instr.Close();

            AbortGeneration(nIRfsg);
            CloseInstrument(nIRfsg);
        }

        #region Utilities
        private void PrintACPResults()
        {
            Console.WriteLine("\n----------------------- ACP Results -----------------------\n");
            Console.WriteLine("Carrier Absolute Power (dBm): {0:0.00}\n", acpResults.ComponentCarrierResults[0].AbsolutePower_dBm);
            Console.WriteLine("\n-----------Offset Channel Measurements----------- \n");

            for (int i = 0; i < acpResults.OffsetResults.Length; i++)
            {
                Console.WriteLine("Offset  {0}", i);
                Console.WriteLine("Lower Relative Power (dB)    : {0:0.000}", acpResults.OffsetResults[i].LowerRelativePower_dB);
                Console.WriteLine("Upper Relative Power (dB)    : {0:0.000}", acpResults.OffsetResults[i].UpperRelativePower_dB);
                Console.WriteLine("Lower Absolute Power (dBm)   : {0:0.000}", acpResults.OffsetResults[i].LowerAbsolutePower_dBm);
                Console.WriteLine("Upper Absolute Power (dBm)   : {0:0.000}", acpResults.OffsetResults[i].UpperAbsolutePower_dBm);
                Console.WriteLine("-------------------------------------------------\n");
            }
        }
        private void PrintModAccResults()
        {
            for(int i = 0; i < modaccResults.ComponentCarrierResults.Length; i++)
            { 
                Console.WriteLine("\n----------------------- EVM Results CC {0} -----------------------\n", i);
                Console.WriteLine("Composite RMS EVM Mean (% or dB)               : {0:0.000}", modaccResults.ComponentCarrierResults[i].MeanRmsCompositeEvm);
                Console.WriteLine("Composite Peak EVM Maximum (% or dB)           : {0:0.000}", modaccResults.ComponentCarrierResults[i].MaxPeakCompositeEvm);
                Console.WriteLine("Composite Peak EVM Slot Index                  : {0}",       modaccResults.ComponentCarrierResults[i].PeakCompositeEvmSlotIndex);
                Console.WriteLine("Composite Peak EVM Symbol Index                : {0}",       modaccResults.ComponentCarrierResults[i].PeakCompositeEvmSymbolIndex);
                Console.WriteLine("Composite Peak EVM Subcarrier Index            : {0}",       modaccResults.ComponentCarrierResults[i].PeakCompositeEvmSubcarrierIndex);
                Console.WriteLine("Component Carrier Frequency Error Mean (Hz)    : {0:0.000}", modaccResults.ComponentCarrierResults[i].MeanFrequencyError_Hz);
            }
        }
        #endregion
    }
}
