using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.LteMX;
using NationalInstruments.RFmx.SpecAnMX;
using System;
using System.IO;
using static NationalInstruments.ReferenceDesignLibraries.SA.RFmxLTE;
using static NationalInstruments.ReferenceDesignLibraries.SG;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    public class LTE_DPD_AMPM_EVM_ACP_DL
    {
        #region Declaring Parameters
        //shared Parameters
        public string resourceName;
        public double centerFrequency;
        public string signalStringSpecan, signalStringLte;
        public string resultStringSpecan, resultStringLte;

        //Generator Configuration
        public SG.InstrumentConfiguration SgInstrConfig;
       
        public string filePath;

        //Analyzer Configuration 
        public SA.RFmxInstr.InstrumentConfiguration saInstrConfig;
        public SA.CommonConfiguration saCommonConfig;
        public SA.AutoLevelConfiguration saAutolevelConfig;

        public SA.RFmxLTE.StandardConfiguration StandardConfigLte = new SA.RFmxLTE.StandardConfiguration();
        
        //Measurements Configuration
        public SA.RFmxSpecAn.AmpmConfiguration AmpmConfigurationSpecAn;
        public SA.RFmxSpecAn.AmpmResults AmpmResultsSpecAn;

        public SA.RFmxLTE.AcpConfiguration AcpConfigLte;
        public SA.RFmxLTE.AcpResults AcpResultsLte = new SA.RFmxLTE.AcpResults();

        public SA.RFmxLTE.ModAccConfiguration ModaccConfigLte;
        public SA.RFmxLTE.ModAccResults ModaccResultsLte = new SA.RFmxLTE.ModAccResults();

        //Methods Configuration 
        public Methods.RFmxDPD.CommonConfiguration CommonConfigurationDpd;
        public Methods.RFmxDPD.MemoryPolynomialConfiguration MemoryPolynomialConfiguration;
        public Methods.RFmxDPD.PreDpdCrestFactorReductionConfiguration preDpdCrestFactorReductionConfig;
        public Methods.RFmxDPD.ApplyDpdCrestFactorReductionConfiguration applyDpdCrestFactorReductionConfig;
        public bool EnableDpd;
        #endregion

        public LTE_DPD_AMPM_EVM_ACP_DL()
        {
            InitializeParameters();
        }

        public void InitializeParameters()
        {
            //shared Parameters
            centerFrequency = 1.95e9; //Hz
            resourceName = "5840";
            filePath = @"C:\Users\Public\Documents\National Instruments\RFIC Test Software\Waveforms\LTE_FDD_DL_1x20MHz_TM11_OS4.tdms";
            signalStringSpecan = "specanSig0";
            signalStringLte = "lteSig0";
            resultStringSpecan = "specanResult0";
            resultStringLte = "lteResult0";

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

            StandardConfigLte = SA.RFmxLTE.StandardConfiguration.GetDefault();
            StandardConfigLte.LinkDirection = RFmxLteMXLinkDirection.Downlink;
            StandardConfigLte.DownlinkAutoCellIDDetectionEnabled = RFmxLteMXDownlinkAutoCellIDDetectionEnabled.True;
            StandardConfigLte.ComponentCarrierConfigurations[0].Bandwidth_Hz = 20.0e6;
            StandardConfigLte.ComponentCarrierConfigurations[0].DownlinkTestModel = RFmxLteMXDownlinkTestModel.TM1_1;

            AcpConfigLte = SA.RFmxLTE.AcpConfiguration.GetDefault();

            ModaccConfigLte = SA.RFmxLTE.ModAccConfiguration.GetDefault();
            ModaccConfigLte.SynchronizationMode = RFmxLteMXModAccSynchronizationMode.Frame;

            //Methods Configuration
            CommonConfigurationDpd = Methods.RFmxDPD.CommonConfiguration.GetDefault();
            CommonConfigurationDpd.DutAverageInputPower_dBm = SgInstrConfig.DutAverageInputPower_dBm;
            MemoryPolynomialConfiguration = Methods.RFmxDPD.MemoryPolynomialConfiguration.GetDefault();
            MemoryPolynomialConfiguration.Order = 5;
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
            RFmxLteMX lte = instr.GetLteSignalConfiguration(signalStringLte);
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

            SA.RFmxLTE.ConfigureCommon(lte, saCommonConfig);
            SA.RFmxLTE.ConfigureStandard(lte, StandardConfigLte);
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

            ConfigureAcp(lte, AcpConfigLte);
            RFmxLteMXMeasurementTypes[] lteMeasurements = new RFmxLteMXMeasurementTypes[1] { RFmxLteMXMeasurementTypes.Acp };
            SA.RFmxLTE.SelectAndInitiateMeasurements(lte, lteMeasurements, saAutolevelConfig, false, "", resultStringLte);
            AcpResultsLte = FetchAcp(lte, RFmxLteMX.BuildResultString(resultStringLte));
            PrintACPResults();

            ConfigureModAcc(lte, ModaccConfigLte);
            lteMeasurements[0] = RFmxLteMXMeasurementTypes.ModAcc;
            SA.RFmxLTE.SelectAndInitiateMeasurements(lte, lteMeasurements, saAutolevelConfig, false, "", resultStringLte);
            ModaccResultsLte = FetchModAcc(lte, RFmxLteMX.BuildResultString(resultStringLte));
            PrintModAccResults();
            #endregion

            specAn.Dispose();
            specAn = null;
            lte.Dispose();
            instr.Close();

            AbortGeneration(nIRfsg);
            CloseInstrument(nIRfsg);
        }

        #region Utilities
        private void PrintACPResults()
        {
            Console.WriteLine("\n----------------------- ACP Results -----------------------\n");
            Console.WriteLine("Carrier Absolute Power (dBm): {0:0.00}\n", AcpResultsLte.ComponentCarrierResults[0].AbsolutePower_dBm);
            Console.WriteLine("\n-----------Offset Channel Measurements----------- \n");

            for (int i = 0; i < AcpResultsLte.OffsetResults.Length; i++)
            {
                Console.WriteLine("Offset  {0}", i);
                Console.WriteLine("Lower Absolute Power         (dBm) : {0:0.000}", AcpResultsLte.OffsetResults[i].LowerAbsolutePower_dBm);
                Console.WriteLine("Upper Absolute Power         (dBm) : {0:0.000}", AcpResultsLte.OffsetResults[i].UpperAbsolutePower_dBm);
                Console.WriteLine("Lower Relative Power         (dB)  : {0:0.000}", AcpResultsLte.OffsetResults[i].LowerRelativePower_dB);
                Console.WriteLine("Upper Relative Power         (dB)  : {0:0.000}", AcpResultsLte.OffsetResults[i].UpperRelativePower_dB);
                Console.WriteLine("Offset Frequency             (Hz)  : {0:0.000}", AcpResultsLte.OffsetResults[i].Frequency_Hz);
                Console.WriteLine("Offset Integration Bandwidth (Hz)  : {0:0.000}", AcpResultsLte.OffsetResults[i].IntegrationBandwidth_Hz);
                Console.WriteLine("-------------------------------------------------\n");
            }
        }
        private void PrintModAccResults()
        {
            for(int i = 0; i < ModaccResultsLte.ComponentCarrierResults.Length; i++)
            { 
                Console.WriteLine("\n----------------------- EVM Results CC {0} -----------------------\n", i);
                Console.WriteLine("Composite RMS EVM Mean (% or dB)               : {0:0.000}", ModaccResultsLte.ComponentCarrierResults[i].MeanRmsCompositeEvm);
                Console.WriteLine("Composite Peak EVM Maximum (% or dB)           : {0:0.000}", ModaccResultsLte.ComponentCarrierResults[i].MaxPeakCompositeEvm);
                Console.WriteLine("Composite Peak EVM Slot Index                  : {0}",       ModaccResultsLte.ComponentCarrierResults[i].PeakCompositeEvmSlotIndex);
                Console.WriteLine("Composite Peak EVM Symbol Index                : {0}",       ModaccResultsLte.ComponentCarrierResults[i].PeakCompositeEvmSymbolIndex);
                Console.WriteLine("Composite Peak EVM Subcarrier Index            : {0}",       ModaccResultsLte.ComponentCarrierResults[i].PeakCompositeEvmSubcarrierIndex);
                Console.WriteLine("Component Carrier Frequency Error Mean (Hz)    : {0:0.000}", ModaccResultsLte.ComponentCarrierResults[i].MeanFrequencyError_Hz);
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
