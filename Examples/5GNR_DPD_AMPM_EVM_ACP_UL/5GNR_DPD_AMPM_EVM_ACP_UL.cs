using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.NRMX;
using NationalInstruments.RFmx.SpecAnMX;
using System;
using System.IO;
using static NationalInstruments.ReferenceDesignLibraries.SA.RFmxNR;
using static NationalInstruments.ReferenceDesignLibraries.SG;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    public class NR5G_DPD_AMPM_EVM_ACP_UL
    {
        #region Declaring Parameters
        //shared Parameters
        public string resourceName;
        public double centerFrequency;
        public string signalStringSpecan, signalStringNr;
        public string resultStringSpecan, resultStringNr;

        //Generator Configuration
        public SG.InstrumentConfiguration SgInstrConfig;
        public string filePath;

        //Analyzer Configuration 
        public SA.RFmxInstr.InstrumentConfiguration saInstrConfig;
        public SA.CommonConfiguration saCommonConfig;
        public SA.AutoLevelConfiguration saAutolevelConfig;

        //Measurements Configuration
        public SA.RFmxSpecAn.AmpmConfiguration AmpmConfigurationSpecAn;
        public SA.RFmxSpecAn.AmpmResults AmpmResultsSpecAn;

        public SA.RFmxNR.StandardConfiguration StandardConfigNR = new SA.RFmxNR.StandardConfiguration();

        public SA.RFmxNR.AcpConfiguration AcpConfigNR;
        public SA.RFmxNR.AcpResults AcpResultsNR = new AcpResults();

        public SA.RFmxNR.ModAccConfiguration ModaccConfigNR;
        public SA.RFmxNR.ModAccResults ModaccResultsNR = new ModAccResults();

        //Methods Configuration 
        public Methods.RFmxDPD.CommonConfiguration CommonConfigurationDpd;
        public Methods.RFmxDPD.MemoryPolynomialConfiguration MemoryPolynomialConfiguration;
        public Methods.RFmxDPD.PreDpdCrestFactorReductionConfiguration preDpdCrestFactorReductionConfig;
        public Methods.RFmxDPD.ApplyDpdCrestFactorReductionConfiguration applyDpdCrestFactorReductionConfig;
        public bool EnableDpd;
        #endregion

        public NR5G_DPD_AMPM_EVM_ACP_UL()
        {
            InitializeParameters();
        }

        public void InitializeParameters()
        {
            //shared Parameters
            centerFrequency = 3.5e9; //Hz
            resourceName = "5840";
            filePath = @"C:\Users\Public\Documents\National Instruments\RFIC Test Software\Waveforms\NR_FR1_UL_1x100MHz_30kHz-SCS_256QAM_OS4_VST2_1ms.tdms";
            signalStringSpecan = "specanSig0";
            signalStringNr = "nrSig0";
            resultStringSpecan = "specanResult0";
            resultStringNr = "nrResult0";

            //Generator Configuiration
            SgInstrConfig = SG.InstrumentConfiguration.GetDefault();
            SgInstrConfig.CarrierFrequency_Hz = centerFrequency;
            SgInstrConfig.DutAverageInputPower_dBm = -10.0;
            SgInstrConfig.ExternalAttenuation_dB = 0;

            //Analyzer Configuration 
            saInstrConfig = SA.RFmxInstr.InstrumentConfiguration.GetDefault();
            saCommonConfig = SA.CommonConfiguration.GetDefault();
            saCommonConfig.ExternalAttenuation_dB = 0;
            saCommonConfig.CenterFrequency_Hz = centerFrequency;
            saCommonConfig.ReferenceLevel_dBm = 0.0;

            saAutolevelConfig = SA.AutoLevelConfiguration.GetDefault();
            saAutolevelConfig.Enabled = true;

            AmpmConfigurationSpecAn = SA.RFmxSpecAn.AmpmConfiguration.GetDefault();

            StandardConfigNR = SA.RFmxNR.StandardConfiguration.GetDefault();
            StandardConfigNR.ComponentCarrierConfigurations[0].PuschModulationType = RFmxNRMXPuschModulationType.Qam16;

            AcpConfigNR = SA.RFmxNR.AcpConfiguration.GetDefault();
            AcpConfigNR.NumberOfNrOffsets = 2;
            AcpConfigNR.NumberOfUtraOffsets = 2;
            AcpConfigNR.NumberOfEutraOffsets = 0;

            ModaccConfigNR = SA.RFmxNR.ModAccConfiguration.GetDefault();
            
            //Methods Configuration
            CommonConfigurationDpd = Methods.RFmxDPD.CommonConfiguration.GetDefault();
            CommonConfigurationDpd.DutAverageInputPower_dBm = SgInstrConfig.DutAverageInputPower_dBm;
            MemoryPolynomialConfiguration = Methods.RFmxDPD.MemoryPolynomialConfiguration.GetDefault();
            MemoryPolynomialConfiguration.NumberOfIterations = 1;
            EnableDpd = true;
            preDpdCrestFactorReductionConfig = Methods.RFmxDPD.PreDpdCrestFactorReductionConfiguration.GetDefault();
            preDpdCrestFactorReductionConfig.Enabled = RFmxSpecAnMXDpdPreDpdCfrEnabled.False;
            applyDpdCrestFactorReductionConfig = Methods.RFmxDPD.ApplyDpdCrestFactorReductionConfiguration.GetDefault();
            applyDpdCrestFactorReductionConfig.Enabled = RFmxSpecAnMXDpdApplyDpdCfrEnabled.False;
        }

        public void Run()
        {
            #region Create Sessions
            NIRfsg nIRfsg = new NIRfsg(resourceName, false, false);
            RFmxInstrMX instr = new RFmxInstrMX(resourceName, "");
            RFmxSpecAnMX specAn = instr.GetSpecAnSignalConfiguration(signalStringSpecan);
            RFmxNRMX nr = instr.GetNRSignalConfiguration(signalStringNr);
            #endregion

            #region Configure Generation
            ConfigureInstrument(nIRfsg, SgInstrConfig);
            Waveform waveform = LoadWaveformFromTDMS(filePath);

            // Apply CRF to the waveform if it is enabled
            waveform = Methods.RFmxDPD.ConfigurePreDpdCrestFactorReduction(specAn, waveform, preDpdCrestFactorReductionConfig);

            DownloadWaveform(nIRfsg, waveform);
            ConfigureContinuousGeneration(nIRfsg, waveform);
            nIRfsg.Initiate();
            #endregion

            #region Configure Analyzer
            saAutolevelConfig.MeasurementInterval_s = waveform.BurstLength_s;
            SA.RFmxInstr.ConfigureInstrument(instr, saInstrConfig);

            SA.RFmxSpecAn.ConfigureCommon(specAn, saCommonConfig);

            AmpmConfigurationSpecAn.ReferenceWaveform = waveform;
            AmpmConfigurationSpecAn.DutAverageInputPower_dBm = SgInstrConfig.DutAverageInputPower_dBm;
            SA.RFmxSpecAn.ConfigureAmpm(specAn, AmpmConfigurationSpecAn);

            SA.RFmxNR.ConfigureCommon(nr, saCommonConfig);
            SA.RFmxNR.ConfigureStandard(nr, StandardConfigNR);
            #endregion

            #region Configure and Measure DPD
            if (EnableDpd)
            {
                Methods.RFmxDPD.ConfigureCommon(specAn, CommonConfigurationDpd, waveform);
                Methods.RFmxDPD.ConfigureMemoryPolynomial(specAn, MemoryPolynomialConfiguration);
                Methods.RFmxDPD.ConfigureApplyDpdCrestFactorReduction(specAn, applyDpdCrestFactorReductionConfig);

                Console.WriteLine("\n--------------- Measurement Results with DPD --------------\n");
                specAn.SelectMeasurements("", RFmxSpecAnMXMeasurementTypes.Dpd, true);
                Methods.RFmxDPD.PerformMemoryPolynomial(specAn, nIRfsg, MemoryPolynomialConfiguration, waveform);
            }
            else
            {
                Console.WriteLine("\n------------- Measurement Results without DPD -------------\n");
            }
            #endregion

            #region Measure
            RFmxSpecAnMXMeasurementTypes[] specanMeasurements = new RFmxSpecAnMXMeasurementTypes[1] { RFmxSpecAnMXMeasurementTypes.Ampm };
            SA.RFmxSpecAn.SelectAndInitiateMeasurements(specAn, specanMeasurements, saAutolevelConfig, waveform.SignalBandwidth_Hz, false, "", resultStringSpecan);
            AmpmResultsSpecAn = SA.RFmxSpecAn.FetchAmpm(specAn, RFmxSpecAnMX.BuildResultString(resultStringSpecan));
            PrintAMPMResults();

            ConfigureAcp(nr, AcpConfigNR);
            RFmxNRMXMeasurementTypes[] nrMeasurements = new RFmxNRMXMeasurementTypes[1] { RFmxNRMXMeasurementTypes.Acp };
            SA.RFmxNR.SelectAndInitiateMeasurements(nr, nrMeasurements, saAutolevelConfig, false, "", resultStringNr);
            AcpResultsNR = FetchAcp(nr, RFmxNRMX.BuildResultString(resultStringNr));
            PrintACPResults();
            
            ConfigureModacc(nr, ModaccConfigNR);
            nrMeasurements[0] = RFmxNRMXMeasurementTypes.ModAcc;
            SA.RFmxNR.SelectAndInitiateMeasurements(nr, nrMeasurements, saAutolevelConfig, false, "", resultStringNr);
            ModaccResultsNR = FetchModAcc(nr, RFmxNRMX.BuildResultString(resultStringNr));
            PrintModAccResults();
            #endregion

            specAn.Dispose();
            specAn = null;
            nr.Dispose();
            instr.Close();

            AbortGeneration(nIRfsg);
            CloseInstrument(nIRfsg);
        }

        #region Utilities
        private void PrintACPResults()
        {
            Console.WriteLine("\n----------------------- ACP Results -----------------------\n");
            Console.WriteLine("Carrier Absolute Power (dBm): {0:0.00}\n", AcpResultsNR.ComponentCarrierResults[0].AbsolutePower_dBm);
            Console.WriteLine("\n-----------Offset Channel Measurements----------- \n");

            for (int i = 0; i < AcpResultsNR.OffsetResults.Length; i++)
            {
                Console.WriteLine("Offset  {0}", i);
                Console.WriteLine("Lower Absolute Power (dBm)   : {0:0.000}", AcpResultsNR.OffsetResults[i].LowerAbsolutePower_dBm);
                Console.WriteLine("Upper Absolute Power (dBm)   : {0:0.000}", AcpResultsNR.OffsetResults[i].UpperAbsolutePower_dBm);
                Console.WriteLine("Lower Relative Power (dB)    : {0:0.000}", AcpResultsNR.OffsetResults[i].LowerRelativePower_dB);
                Console.WriteLine("Upper Relative Power (dB)    : {0:0.000}", AcpResultsNR.OffsetResults[i].UpperRelativePower_dB);
                Console.WriteLine("-------------------------------------------------\n");
            }
        }
        private void PrintModAccResults()
        {
            for(int i = 0; i < ModaccResultsNR.ComponentCarrierResults.Length; i++)
            { 
                Console.WriteLine("\n----------------------- EVM Results CC {0} -----------------------\n", i);
                Console.WriteLine("Composite RMS EVM Mean (% or dB)               : {0:0.000}", ModaccResultsNR.ComponentCarrierResults[i].MeanRmsCompositeEvm);
                Console.WriteLine("Composite Peak EVM Maximum (% or dB)           : {0:0.000}", ModaccResultsNR.ComponentCarrierResults[i].MaxPeakCompositeEvm);
                Console.WriteLine("Composite Peak EVM Slot Index                  : {0}",       ModaccResultsNR.ComponentCarrierResults[i].PeakCompositeEvmSlotIndex);
                Console.WriteLine("Composite Peak EVM Symbol Index                : {0}",       ModaccResultsNR.ComponentCarrierResults[i].PeakCompositeEvmSymbolIndex);
                Console.WriteLine("Composite Peak EVM Subcarrier Index            : {0}",       ModaccResultsNR.ComponentCarrierResults[i].PeakCompositeEvmSubcarrierIndex);
                Console.WriteLine("Component Carrier Frequency Error Mean (Hz)    : {0:0.000}", ModaccResultsNR.ComponentCarrierResults[i].MeanFrequencyError_Hz);
            }
        }
        private void PrintAMPMResults()
        {
            Console.WriteLine("\n----------------------- AMPM Results ----------------------\n");
            Console.WriteLine("Mean Linear Gain (dB):                       {0:0.00}", AmpmResultsSpecAn.MeanLinearGain_dB);
            Console.WriteLine("Mean RMS EVM (%):                            {0:0.00}", AmpmResultsSpecAn.MeanRmsEvm_percent);
            Console.WriteLine("AM to AM Residual (dB):                      {0:0.00}", AmpmResultsSpecAn.AmToAMResidual_dB);
            Console.WriteLine("AM to PM Residual (dB):                      {0:0.00}", AmpmResultsSpecAn.AmToPMResidual_deg);
            Console.WriteLine("1 dB Compression Point (dBm):                {0:0.00}", AmpmResultsSpecAn.OnedBCompressionPoint_dBm);
        }
        #endregion
    }
}
