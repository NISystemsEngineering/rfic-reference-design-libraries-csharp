using NationalInstruments.ModularInstruments;
using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.ModularInstruments.SystemServices.TimingServices;
using System;
using NationalInstruments;
using NationalInstruments.DataInfrastructure;
using System.Linq;

namespace NationalInstruments.ReferenceDesignLibraries
{
    public static class EnvelopeTracking
    {
        public struct EnvelopeGeneratorConfiguration
        {
            public RfsgTerminalConfiguration TerminalConfiguration;
            public string MarkerEventOutputTerminal;

            public static EnvelopeGeneratorConfiguration GetDefault()
            {
                return new EnvelopeGeneratorConfiguration()
                {
                    TerminalConfiguration = RfsgTerminalConfiguration.Differential,
                };
            }
        }

        public struct TrackerConfiguration
        {
            public double InputImpedance_Ohms;
            public double CommonModeOffset_V;
            public double LinearGain;
            public double OutputOffset_V;

            public static TrackerConfiguration GetDefault()
            {
                return new TrackerConfiguration()
                {
                    InputImpedance_Ohms = 1e6,
                    CommonModeOffset_V = 1.0,
                    LinearGain = 2.5,
                    OutputOffset_V = 2.55
                };
            }
        }

        public enum DetroughType { Exponential, Cosine, Power};

        public struct DetroughConfiguration
        {
            public DetroughType Type;
            public double MinimumVoltage;
            public double MaximumVoltage;
            public double Exponent;

            public static DetroughConfiguration GetDefault()
            {
                return new DetroughConfiguration()
                {
                    Type = DetroughType.Exponential,
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

        public struct SynchronizationConfiguration
        {
            public double RFDelayRange_s;
            public double RFDelay_s;

            public static SynchronizationConfiguration GetDefault()
            {
                return new SynchronizationConfiguration()
                {
                    RFDelayRange_s = 1e-6,
                    RFDelay_s = 0.0
                };
            }
        }

        public static SG.Waveform CreateEnvelopeWaveform(SG.Waveform referenceWaveform, DetroughConfiguration detroughConfig)
        {
            SG.Waveform envWfm = new SG.Waveform()
            {
                WaveformName = referenceWaveform.WaveformName + "Envelope",
                WaveformData = referenceWaveform.WaveformData.Clone(),
                SignalBandwidth_Hz = 0.8 * referenceWaveform.SampleRate,
                PAPR_dB = 0.0, // unnecessary to calculate as it won't be used
                BurstLength_s = referenceWaveform.BurstLength_s,
                SampleRate = referenceWaveform.SampleRate,
                BurstStartLocations = (int[])referenceWaveform.BurstStartLocations.Clone(),
                BurstStopLocations = (int[])referenceWaveform.BurstStopLocations.Clone(),
                IdleDurationPresent = referenceWaveform.IdleDurationPresent,
                RuntimeScaling = 10 * Math.Log10(0.9) // applies 10% headroom
            };

            double[] IqMagnitudes = referenceWaveform.WaveformData.GetMagnitudeDataArray(false);
            WritableBuffer<ComplexSingle> envWfmWriteBuffer = envWfm.WaveformData.GetWritableBuffer();
            double detroughRatio = detroughConfig.MinimumVoltage / detroughConfig.MaximumVoltage;

            // SG.Waveforms are assumed to be normalized in range [0, 1], so no normalization will happen here
            switch (detroughConfig.Type)
            {
                case DetroughType.Exponential:
                    // Formula: e = IQmag + d*exp(-IQmag/d)
                    double expScale = 1 + detroughRatio * Math.Exp(-1.0 / detroughRatio);
                    for (int i = 0; i < envWfmWriteBuffer.Size; i++)
                    {
                        double sampleValue = IqMagnitudes[i] + detroughRatio * Math.Exp(-IqMagnitudes[i] / detroughRatio);
                        envWfmWriteBuffer[i] = ComplexSingle.FromSingle((float)(detroughConfig.MaximumVoltage * sampleValue / expScale)); // Scale detrough to Vmax. Divide by detroughed waveform's max value to normalize. IQMagnitude's max value is 1
                    }
                    break;
                case DetroughType.Cosine:
                    // Formula: e = 1 - (1-d)*cos(IQmag*pi/2)
                    double cosScale = 1 - (1 - detroughRatio) * Math.Cos(Math.PI / 2);
                    for (int i = 0; i < envWfmWriteBuffer.Size; i++)
                    {
                        double sampleValue = 1 - (1 - detroughRatio) * Math.Cos(IqMagnitudes[i] * cosScale);
                        envWfmWriteBuffer[i] = ComplexSingle.FromSingle((float)(detroughConfig.MaximumVoltage * sampleValue / cosScale)); // Scale detrough to Vmax. Divide by detroughed waveform's max value to normalize. IQMagnitude's max value is 1: 1-(1-d) *cos(1*pi/2)
                    }
                    break;
                case DetroughType.Power:
                    // Formula: e = (1-d) + IQmag^(exponent)*(1-d)
                    double powScale = 2 - 2 * detroughRatio;
                    for (int i = 0; i < envWfmWriteBuffer.Size; i++)
                    {
                        double sampleValue = (1 - detroughRatio) + (Math.Pow(IqMagnitudes[i], detroughConfig.Exponent) * (1 - detroughRatio));
                        envWfmWriteBuffer[i] = ComplexSingle.FromSingle((float)(detroughConfig.MaximumVoltage * sampleValue / powScale)); // Scale detrough to Vmax. Divide by detroughed waveform's max value. IQMagnitude's max value is 1: (1-d) + (1^exponent)*(1-d)
                    }
                    break;
                default:
                    throw new InvalidOperationException("Detrough type not supported.\n");
            }

            return envWfm;
        }

        public static void ConfigureEnvelopeGenerator(NIRfsg envVsg, EnvelopeGeneratorConfiguration envVsgConfig, SG.Waveform envelopeWaveform, TrackerConfiguration trackerConfig)
        {
            envVsg.IQOutPort[""].CommonModeOffset = trackerConfig.CommonModeOffset_V;
            envVsg.IQOutPort[""].TerminalConfiguration = envVsgConfig.TerminalConfiguration;
            if (envVsgConfig.TerminalConfiguration == RfsgTerminalConfiguration.Differential)
                envVsg.IQOutPort[""].LoadImpedance = trackerConfig.InputImpedance_Ohms == 50.0 ? 100.0 : trackerConfig.InputImpedance_Ohms;
            else
                envVsg.IQOutPort[""].LoadImpedance = trackerConfig.InputImpedance_Ohms;

            WritableBuffer<ComplexSingle> envWfm = envelopeWaveform.WaveformData.GetWritableBuffer();
            
            // Scale waveform to adjust for tracker
            for (int i = 0; i < envWfm.Size; i++)
                envWfm[i] = ComplexSingle.FromSingle((float)((envWfm[i].Real - trackerConfig.OutputOffset_V) / trackerConfig.LinearGain));

            // Scale waveform to optimize instrument dynamic range
            ComplexSingle.DecomposeArray(envelopeWaveform.WaveformData.GetRawData(), out float[] rawEnvelope, out _);
            float min = rawEnvelope.Min();
            float amplitude = (rawEnvelope.Max() - min) * 0.5f;
            float offset = min + amplitude;
            for (int i = 0; i < envWfm.Size; i++)
                envWfm[i] = ComplexSingle.FromSingle((envWfm[i].Real - offset) / amplitude);

            envVsg.IQOutPort[""].Level = 2 * amplitude; // gain is interpreted as peak-to-peak
            envVsg.IQOutPort[""].Offset = 0.0;

            SG.DownloadWaveform(envVsg, envelopeWaveform);
            SG.ConfigureContinuousGeneration(envVsg, envelopeWaveform, envVsgConfig.MarkerEventOutputTerminal);
        }

        public static void Initiate(NIRfsg rfVsg, NIRfsg envVsg, SynchronizationConfiguration syncConfig)
        {   
            TClock tclk = new TClock(new ITClockSynchronizableDevice[] { rfVsg, envVsg });
            tclk.DevicesToSynchronize[0].SampleClockDelay = -syncConfig.RFDelayRange_s; // The VST2 can only apply positive delays so we have to establish an inital delay of -RFDelayRange to TCLK to handle negative shifts as well
            rfVsg.Arb.RelativeDelay = syncConfig.RFDelayRange_s + syncConfig.RFDelay_s;
            tclk.ConfigureForHomogeneousTriggers();
            tclk.Synchronize(); // #todo: allow drift?
            tclk.Initiate();
        }

        public struct DelaySweepConfiguration
        {
            public double Start_s;
            public double Stop_s;
            public double[] DeltaSchedule_s; 

            public static DelaySweepConfiguration GetDefault()
            {
                return new DelaySweepConfiguration()
                {
                    Start_s = -1e-6,
                    Stop_s = 1e-6,
                    DeltaSchedule_s = new double[] { 100e-9, 10e-9, 1e-9 }
                };
            }
        }
            
        public static double SweepRFDelay(NIRfsg rfVsg, DelaySweepConfiguration delaySweepConfig)
        {
            double optimalDelay = double.NaN;
            double optimalEvm = double.NegativeInfinity; //#todo: change

            double startDelay = delaySweepConfig.Start_s;
            double stopDelay = delaySweepConfig.Stop_s;

            foreach (double delta in delaySweepConfig.DeltaSchedule_s)
            {
                for (double delay = startDelay; delay <= stopDelay; delay += delta)
                {
                    rfVsg.Arb.RelativeDelay = delay;

                    double evm = double.NaN;
                    if (evm < optimalEvm)
                    {
                        optimalDelay = delay;
                        optimalEvm = evm;
                    }
                }
                startDelay = optimalDelay - delta;
                stopDelay = optimalDelay + delta;
            }

            return optimalDelay;
        }
    }
}
