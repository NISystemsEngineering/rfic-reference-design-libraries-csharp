using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.ReferenceDesignLibraries.FocusITuner;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.SpecAnMX;
using System;
using System.Collections.Generic;
using static NationalInstruments.ReferenceDesignLibraries.SG;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    public class SPECAN_TXP_LOADPULL
    {
        #region Declaring Parameters
        //shared Parameters
        public string resourceName;
        public double centerFrequency;
        public string signalStringSpecan;
        public string resultStringSpecan;

        //Tuner Configration
        public string tunerAddress;
        public FocusTuner.CommonConfiguration commonConfiguration;
        Complex[] gammaSweep;

        //Waveform
        public string filePath;

        //Generator Configuration
        SG.InstrumentConfiguration sgInstrConfig;

        //Analyzer Configuration 
        public SA.RFmxInstr.InstrumentConfiguration saInstrConfig;
        public SA.CommonConfiguration saCommonConfig;
        public SA.AutoLevelConfiguration saAutolevelConfig;

        //Measurements Configuration
        public Complex currentGamma;
        public SA.RFmxSpecAn.TxpConfiguration txpConfigurationSpecAn;
        public SA.RFmxSpecAn.TxpResults txpResultsSpecAn;
        #endregion

        public SPECAN_TXP_LOADPULL()
        {
            InitializeParameters();
        }

        public void InitializeParameters()
        {
            //shared Parameters
            centerFrequency = 3e9; //Hz
            resourceName="5841";
            filePath = @"C:\Users\Public\Documents\National Instruments\RFIC Test Software\Waveforms\LTE_FDD_UL_1x20MHz_256QAM_OS4.tdms";
            signalStringSpecan = "specanSig0";
            resultStringSpecan = "specanResult0";

            //Tuner Configration
            tunerAddress = "10.0.0.2";
            commonConfiguration = FocusTuner.CommonConfiguration.GetDefault();
            commonConfiguration.CalibrationID = 1;
            gammaSweep = GetConstantVSWR(2, 5);

            //Generator Configuiration
            sgInstrConfig = SG.InstrumentConfiguration.GetDefault();
            sgInstrConfig.CarrierFrequency_Hz = centerFrequency;
            sgInstrConfig.DutAverageInputPower_dBm = -10.0;
            sgInstrConfig.ExternalAttenuation_dB = 0;

            //Analyzer Configuration
            saInstrConfig = SA.RFmxInstr.InstrumentConfiguration.GetDefault();
            saCommonConfig = SA.CommonConfiguration.GetDefault();
            saCommonConfig.ExternalAttenuation_dB = 0;
            saCommonConfig.CenterFrequency_Hz = centerFrequency;
            saCommonConfig.ReferenceLevel_dBm = 0.0;

            saAutolevelConfig = SA.AutoLevelConfiguration.GetDefault();
            saAutolevelConfig.Enabled = true;
            
            txpConfigurationSpecAn = SA.RFmxSpecAn.TxpConfiguration.GetDefault();
            txpConfigurationSpecAn.Rbw_Hz = 20e6;
        }

        public void Run()
        {
            #region Create Sessions
            FocusITunerBroker iTuner = new FocusITunerBroker();
            iTuner.Initialize(tunerAddress, false, true);
            NIRfsg nIRfsg = new NIRfsg(resourceName, false, false);
            RFmxInstrMX instr = new RFmxInstrMX(resourceName, "");
            RFmxSpecAnMX specAn = instr.GetSpecAnSignalConfiguration(signalStringSpecan);
            #endregion

            #region Configure Tuner
            FocusTuner.ConfigCommon(iTuner, commonConfiguration);
            #endregion

            #region Configure Generation
            ConfigureInstrument(nIRfsg, sgInstrConfig);
            Waveform waveform = LoadWaveformFromTDMS(filePath);
            DownloadWaveform(nIRfsg, waveform);
            ConfigureContinuousGeneration(nIRfsg, waveform);
            nIRfsg.Initiate();
            #endregion

            #region Configure Analyzer
            saAutolevelConfig.MeasurementInterval_s = waveform.BurstLength_s;
            SA.RFmxInstr.ConfigureInstrument(instr, saInstrConfig);
            SA.RFmxSpecAn.ConfigureCommon(specAn, saCommonConfig);
            SA.RFmxSpecAn.ConfigureTxp(specAn, txpConfigurationSpecAn);
            #endregion

            #region Measure
            foreach (var gamma in gammaSweep)
            {
                Console.WriteLine("\n--------------------- Tuning --------------------\n");
                currentGamma = FocusTuner.MoveTunerPerGamma(iTuner, gamma)[0];
                PrintTuneResults();

                Console.WriteLine("\n--------------------- Results --------------------\n");
                RFmxSpecAnMXMeasurementTypes[] specanMeasurements = new RFmxSpecAnMXMeasurementTypes[1] { RFmxSpecAnMXMeasurementTypes.Txp };
                SA.RFmxSpecAn.SelectAndInitiateMeasurements(specAn, specanMeasurements, saAutolevelConfig, waveform.SignalBandwidth_Hz, false, "", resultStringSpecan);
                txpResultsSpecAn = SA.RFmxSpecAn.FetchTxp(specAn, RFmxSpecAnMX.BuildResultString(resultStringSpecan));
                PrintTxPResults();
            }
            #endregion
            
            AbortGeneration(nIRfsg);
            CloseInstrument(nIRfsg);
            FocusTuner.CloseTuner(iTuner);
            specAn.Dispose();
            specAn = null;
            instr.Close();
            instr = null;
        }

        #region Utilities
        private Complex[] GetConstantVSWR(double vswr, int numberOfPoints)
        {
            List<Complex> gammaArray = new List<Complex>();
            double magnitude = (vswr - 1) / (vswr + 1);
            for(int i = 0; i<numberOfPoints; i++)
            {
                double theta = Math.PI * 2 * i / numberOfPoints;
                gammaArray.Add(new Complex { Real = magnitude * Math.Cos(theta), Imaginary = magnitude * Math.Sin(theta) });
            }
            return gammaArray.ToArray();
        }

        private void PrintTuneResults()
        {
            Console.WriteLine("\n----------------------- Tune Results -----------------------\n");
            if (currentGamma.Imaginary >= 0)
            {
                Console.WriteLine("ReflectionCoefficient    : {0:0.000} + {1:0.000}i", currentGamma.Real, currentGamma.Imaginary);
            }
            else
            {
                Console.WriteLine("ReflectionCoefficient    : {0:0.000} - {1:0.000}i", currentGamma.Real, Math.Abs(currentGamma.Imaginary));
            }
            Console.WriteLine("-------------------------------------------------\n");
        }

        private void PrintTxPResults()
        {
            Console.WriteLine("\n----------------------- TXP Results -----------------------\n");
            Console.WriteLine("AverageMeanPower_dBm    : {0:0.000}", txpResultsSpecAn.AverageMeanPower_dBm);
            Console.WriteLine("PeakToAverageRatio_dB    : {0:0.000}", txpResultsSpecAn.PeakToAverageRatio_dB);
            Console.WriteLine("MaximumPower_dBm   : {0:0.000}", txpResultsSpecAn.MaximumPower_dBm);
            Console.WriteLine("MinimumPower_dBm   : {0:0.000}", txpResultsSpecAn.MinimumPower_dBm);
            Console.WriteLine("-------------------------------------------------\n");
        }
        #endregion
    }
}
