using NationalInstruments.ModularInstruments.NIRfsg;
using System;
using System.IO;
using NationalInstruments.ModularInstruments.Interop;
using static NationalInstruments.ReferenceDesignLibraries.SG;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class SG_PowerMeter_Servoing
    {
        #region Declaring Parameters
        public string resourceName;
        public string filePath;
        public InstrumentConfiguration instrConfig;
        public double centerFrequency;
        public double Coupling_dB;

        public string powerMeterResourceName;
        public PowerMeter.CommonConfiguration commonConfiguration;

        public double paOutputPower_dBm;
        public double designedAccuracy_dB;
        public double paGain_dB;
        public int minimumSteps;
        public int maximumSteps;
        public bool servoFailed;        
        #endregion

        public SG_PowerMeter_Servoing()
        {
            InitializeParameters();
        }

        public void InitializeParameters()
        {
            resourceName = "5840";
            filePath = Path.GetFullPath(@"C:\Users\Public\Documents\National Instruments\RFIC Test Software\Waveforms\80211ax_80M_MCS11.tdms");
            centerFrequency = 3.5e9;
            Coupling_dB = 33;

            paOutputPower_dBm = 5;
            designedAccuracy_dB = 0.1;
            paGain_dB = 0;
            minimumSteps = 1;
            maximumSteps = 5;
            servoFailed = true;

            instrConfig = InstrumentConfiguration.GetDefault();
            instrConfig.CarrierFrequency_Hz = centerFrequency;
            instrConfig.DutAverageInputPower_dBm = paOutputPower_dBm - paGain_dB;
            

            powerMeterResourceName = "";
            commonConfiguration = PowerMeter.CommonConfiguration.GetDefault();
            commonConfiguration.Frequency = centerFrequency;
        }

        public void Run()
        {
            NIRfsg nIRfsg = new NIRfsg(resourceName, false, false);
            ConfigureInstrument(nIRfsg, instrConfig);
            Waveform waveform = LoadWaveformFromTDMS(filePath);
            DownloadWaveform(nIRfsg, waveform);
            ConfigureContinuousGeneration(nIRfsg, waveform, "PXI_Trig0");

            ni568x sensor = new ni568x(powerMeterResourceName, true, true);
            PowerMeter.ConfigureCommon(sensor, commonConfiguration);

            bool measuredPowerInRange;
            double measuredPower = 0.0;
            double maximumDutOutputPower = paOutputPower_dBm + designedAccuracy_dB;
            double minimumDutOutputPower = paOutputPower_dBm - designedAccuracy_dB;
            Console.WriteLine("\n------------------------Servo Started ---------------------\n");

            for (int currentServoStep = 1; currentServoStep <= maximumSteps; currentServoStep++)
            {
                nIRfsg.Initiate();
                
                measuredPower = PowerMeter.ReadMeasurement(sensor, Coupling_dB, 10000);
                Console.WriteLine("Servo step:                               {0}", currentServoStep);
                Console.WriteLine("Measured power (dBm):                     {0:0.00}", measuredPower);

                AbortGeneration(nIRfsg);
                
                measuredPowerInRange = (measuredPower >= minimumDutOutputPower) && (measuredPower <= maximumDutOutputPower);
                if (measuredPowerInRange && (currentServoStep >= minimumSteps))
                {
                    servoFailed = false;
                    break;
                }
                else
                {
                    instrConfig.DutAverageInputPower_dBm += ((paOutputPower_dBm - measuredPower) * 0.95);
                    ConfigureInstrument(nIRfsg, instrConfig);
                }
            }
            PrintServoResults(measuredPower);

            CloseInstrument(nIRfsg);
            sensor.Dispose();
        }

        private void PrintServoResults(double dutOutputPower)
        {
            string servoStatus = servoFailed ? "Failed" : "Successful";
            Console.WriteLine("\n------------------------Servo Results ---------------------\n");
            Console.WriteLine("Servo Outcome:                               {0}", servoStatus);
            Console.WriteLine("DUT Output Power (dBm):                      {0:0.00}", dutOutputPower);
        }
    }
}

