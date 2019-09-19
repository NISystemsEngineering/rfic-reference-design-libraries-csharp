using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.ModularInstruments.SystemServices.TimingServices;
using NationalInstruments.ModularInstruments;

namespace NationalInstruments.ReferenceDesignLibraries
{
    public static class EnvelopeTracking
    {
        public enum EnvelopeShapingSource
        {
            Detrough,
            LookUpTable
        }

        public struct TrackerConfiguration
        {
            public double InputImpedance_Ohms;
            public double CommonModeOffset_V;
            public double Gain_Linear;
            public double OutputOffset_V;

            public static TrackerConfiguration GetDefault()
            {
                return new TrackerConfiguration()
                {
                    InputImpedance_Ohms = 1e6,
                    CommonModeOffset_V = 1.0,
                    Gain_Linear = 2.5,
                    OutputOffset_V = 2.55
                };
            }
        }

        public struct DetroughConfiguration
        {
            public niETUtil.DetroughType Type;
            public double MinimumVoltage;
            public double MaximumVoltage;
            public double Exponent;

            public static DetroughConfiguration GetDefault()
            {
                return new DetroughConfiguration()
                {
                    Type = niETUtil.DetroughType.Exp,
                    MinimumVoltage = 1.5,
                    MaximumVoltage = 3.5,
                    Exponent = 1.2
                };
            }
        }

        public struct LookUpTableConfiguration
        {
            niETUtil.EtLookUpTable LookUpTable;
        }

        public struct EnvelopeInstrumentConfiguration
        {
            public RfsgTerminalConfiguration TerminalConfiguration;
            public string MarkerExportTerminal;
                        
            public static EnvelopeInstrumentConfiguration GetDefault()
            {
                return new EnvelopeInstrumentConfiguration()
                {
                    TerminalConfiguration = RfsgTerminalConfiguration.Differential,
                    MarkerExportTerminal = "PFI0"                    
                };
            }
        }

        public static SG.Waveform LoadWaveformFromTDMS(string filePath, string waveformName = "")
        {
            return SG.LoadWaveformFromTDMS(filePath, waveformName);
        }

        public static void ConfigureInstruments(NIRfsg rfVsg, NIRfsg envVsg)
        {
            TClock tclk = new TClock(new ITClockSynchronizableDevice[] { rfVsg, envVsg });
            tclk.ConfigureForHomogeneousTriggers();
        }

        public static void DownloadWaveforms(NIRfsg rfVsg, NIRfsg envVsg, SG.Waveform referenceWaveform, DetroughConfiguration detroughConfig, 
            TrackerConfiguration trackerConfig)
        {
            SG.DownloadWaveform(rfVsg, referenceWaveform);
            niETUtil.CreateEnvelopeWaveform(ref referenceWaveform.WaveformData, detroughConfig.Type, detroughConfig.Exponent, detroughConfig.MaximumVoltage,
                detroughConfig.MinimumVoltage, out double[] envelopeWaveform);
            niETUtil.ScaleEnvelopeWaveform(ref envelopeWaveform, trackerConfig.OutputOffset_V, trackerConfig.Gain_Linear,
                out double arbOffset, out double arbGain);
            envVsg.Arb.IQRate = referenceWaveform.SampleRate;
            envVsg.IQOutPort[""].Level = 2 * (arbGain + Math.Abs(arbOffset));
            envVsg.IQOutPort[""].Offset = 0;
            for (int i = 0; i < envelopeWaveform.Length; i++)
                envelopeWaveform[i] = (arbGain * envelopeWaveform[i] + arbOffset) / (arbGain + Math.Abs(arbOffset));
        }

        public static void DownloadWaveforms(NIRfsg rfVsg, NIRfsg envVsg, SG.Waveform referenceWaveform, LookUpTableConfiguration lutConfig)
        {

        }
    }
}
