using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.ReferenceDesignLibraries;
using System;
using static NationalInstruments.ReferenceDesignLibraries.Methods.EnvelopeTracking;
using static NationalInstruments.ReferenceDesignLibraries.SG;

namespace EnvelopeTrackingExample
{
    class Program
    {
        internal enum EnvelopeMode { Detrough, LUT };
        static void Main(string[] args)
        {
            #region Example Settings
            // Select mode for use in the example
            EnvelopeMode mode = EnvelopeMode.Detrough;
            string waveformPath = @"C:\Users\Public\Documents\National Instruments\RFIC Test Software\Waveforms\LTE_FDD_DL_1x20MHz_TM11_OS4.tdms";
            #endregion

            #region Configure RF Generator
            // Initialize instrument sessions
            NIRfsg rfVsg = new NIRfsg("5840", true, false);

            // Load up waveform
            Waveform rfWfm = LoadWaveformFromTDMS(waveformPath);

            // Configure RF generator
            InstrumentConfiguration rfInstrConfig = InstrumentConfiguration.GetDefault();
            ConfigureInstrument(rfVsg, rfInstrConfig);
            DownloadWaveform(rfVsg, rfWfm);
            ConfigureContinuousGeneration(rfVsg, rfWfm);
            #endregion

            #region Configure Tracker Generator
            NIRfsg envVsg = new NIRfsg("5820", true, false);

            // Configure envelope generator
            EnvelopeGeneratorConfiguration envInstrConfig = EnvelopeGeneratorConfiguration.GetDefault();
            TrackerConfiguration trackerConfig = TrackerConfiguration.GetDefault();
            ConfigureEnvelopeGenerator(envVsg, envInstrConfig, trackerConfig);

            Waveform envWfm = new Waveform();
            switch (mode)
            {
                case EnvelopeMode.Detrough:
                    // Create envelope waveform
                    DetroughConfiguration detroughConfig = DetroughConfiguration.GetDefault();
                    detroughConfig.MinimumVoltage_V = 1.5;
                    detroughConfig.MaximumVoltage_V = 3.5;
                    detroughConfig.Exponent = 1.2;
                    detroughConfig.Type = DetroughType.Exponential;
                    envWfm = CreateDetroughEnvelopeWaveform(rfWfm, detroughConfig);
                    break;
                case EnvelopeMode.LUT:
                    LookUpTableConfiguration lutConfig = new LookUpTableConfiguration
                    {
                        DutAverageInputPower_dBm = rfInstrConfig.DutAverageInputPower_dBm
                    };
                    // Todo - initialize lookup table
                    envWfm = CreateLookUpTableEnvelopeWaveform(rfWfm, lutConfig);
                    break;
            }

            ScaleAndDownloadEnvelopeWaveform(envVsg, envWfm, trackerConfig);
            ConfigureContinuousGeneration(envVsg, envWfm, "PFI0");
            #endregion

            // Start envelope tracking
            SynchronizationConfiguration syncConfig = SynchronizationConfiguration.GetDefault();
            InitiateSynchronousGeneration(rfVsg, envVsg, syncConfig);

            // Wait until user presses a button to stop
            Console.WriteLine("Press any key to abort envelope tracking..");
            Console.ReadKey();

            AbortGeneration(envVsg);
            AbortGeneration(rfVsg);

            // Close instruments
            rfVsg.Close();
            envVsg.Close();
        }
    }
}
