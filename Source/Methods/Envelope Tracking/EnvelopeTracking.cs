using NationalInstruments.DataInfrastructure;
using NationalInstruments.ModularInstruments;
using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.ModularInstruments.SystemServices.TimingServices;
using System;
using System.Linq;

/// <summary>Defines common types and methods for implementing Envelope Tracking.</summary>
namespace NationalInstruments.ReferenceDesignLibraries.Methods
{
    public static class EnvelopeTracking
    {
        #region Type Definitions

        /// <summary>Defines common settings for a configuring the envelope generator.</summary>
        public struct EnvelopeGeneratorConfiguration
        {
            /// <summary>Specifies the source of the Reference Clock signal. For envelope tracking, the RF generator and envelope generator must
            /// use the same reference clock. See the NI-RFSG help for more documentation of this parameter.</summary>
            public string ReferenceClockSource;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static EnvelopeGeneratorConfiguration GetDefault()
            {
                return new EnvelopeGeneratorConfiguration()
                {
                    ReferenceClockSource = "PXI_CLK"
                };
            }
        }

        /// <summary>Defines common settings about the tracker used for modulating the power supply voltage.</summary>
        public struct TrackerConfiguration
        {
            /// <summary>Specifies the input impedance of the power modulator (tracker) being used. This value is expressed in Ohms.</summary>
            public double InputImpedance_Ohms;
            /// <summary>Specifies the common mode offset of the power modulator (tracker) being used. This value is expressed in Volts.</summary>
            public double CommonModeOffset_V;
            /// <summary>Specifies the linear gain that will be applied by the power modulator (tracker) to the envelope waveform.
            /// This value is expressed in Volts per Volts (V/V).</summary>
            public double Gain_VperV;
            /// <summary>Specifies the ouptut offset that will be applied by the power modulator (tracker) to the envelope waveform.
            /// This value is expressed in Volts.</summary>
            public double OutputOffset_V;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static TrackerConfiguration GetDefault()
            {
                return new TrackerConfiguration()
                {
                    InputImpedance_Ohms = 1e6,
                    CommonModeOffset_V = 1.0,
                    Gain_VperV = 2.5,
                    OutputOffset_V = 2.55
                };
            }
        }

        /// <summary>Defines different mathemtical methods to use for creating the detrough envelope waveform.</summary>
        public enum DetroughType {
            /// <summary>Specifies an exponential detrough function: F(x) = x+d*e^(-x/d), where d = Vmin/Vmax</summary>
            Exponential,
            /// <summary>Specifies a cosine detrough function: F(x) = 1-(1-d)*cos(x*pi/2), where d = Vmin/Vmax</summary>
            Cosine,
            /// <summary>Specifies a power detrough function: F(x) = d+(1-d)*x^a, where d = Vmin/Vmax</summary>
            Power
        };

        /// <summary>Defines common settings used for creating the detrough envelope waveform.</summary>
        public struct DetroughConfiguration
        {
            /// <summary>Specifies the detrough function to use.</summary>
            public DetroughType Type;
            /// <summary>Specifies the minimum voltage that should be maintained by the power modulator (tracker).</summary>
            public double MinimumVoltage_V;
            /// <summary>Specifies the maximum voltage that should be maintained by the power modulator (tracker).</summary>
            public double MaximumVoltage_V;
            /// <summary>Specifies the exponent 'a' when <see cref="Type"/> is set to <see cref="DetroughType.Power"/>.</summary>
            public double Exponent;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static DetroughConfiguration GetDefault()
            {
                return new DetroughConfiguration()
                {
                    Type = DetroughType.Exponential,
                    MinimumVoltage_V = 1.5,
                    MaximumVoltage_V = 3.5,
                    Exponent = 1.2
                };
            }
        }

        /// <summary>Defines common settings used for creating the lookup table envelope waveform.</summary>
        public struct LookUpTableConfiguration
        {
            /// <summary>Specifies the desired average input power in dBm that the DUT should receive from the generator.</summary>
            public double DutAverageInputPower_dBm;
            /// <summary>Specifies the array of DUT input powers which will be paired with the supply voltage values to create the lookup table.</summary>
            public float[] DutInputPower_dBm;
            /// <summary>Specifies the array of supply voltages which will be paired with the DUT input powers to create the lookup table.</summary>
            public float[] SupplyVoltage_V;
        }

        /// <summary>Defines common settings used for synchronizing the RF and envelope signal generators.</summary>
        public struct SynchronizationConfiguration
        {
            /// <summary>Specifies the absolute value of the largest expected delay range between the two generators. For example, for a maximum delay of
            /// +/-1us, set this value to 2. This value is expressed in seconds.</summary>
            public double RFDelayRange_s;
            /// <summary>Specifies the delay between the RF signal and the modulated power supplly signal at the DUT plane. Even though the two signal 
            /// generators are synchronized with sub-nanosecond accuracy, this delay is necessary to compensate for the inherent differences between the signal chains
            /// from the RF signal generator to the DUT input port, and from the envelope signal generator to the DUT voltage supply pins.
            /// This delay can be characterized using a delay sweep to find the optimal delay.</summary>
            public double RFDelay_s;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static SynchronizationConfiguration GetDefault()
            {
                return new SynchronizationConfiguration()
                {
                    RFDelayRange_s = 2e-6, // +/- 1us
                    RFDelay_s = 0.0
                };
            }
        }
        #endregion

        #region Envelope Creation

        /// <summary>Creates an envelope waveform utilizing the detrough method.</summary>
        /// <param name="referenceWaveform">Specifies the waveform used for RF signal generation.</param>
        /// <param name="detroughConfig">Specifies common settings for creating the detrough envelope waveform.</param>
        /// <returns>The detrough envelope waveform.</returns>
        public static Waveform CreateDetroughEnvelopeWaveform(Waveform referenceWaveform, DetroughConfiguration detroughConfig)
        {
            Waveform envelopeWaveform = CloneAndConditionReferenceWaveform(referenceWaveform);
            WritableBuffer<ComplexSingle> envWfmWriteBuffer = envelopeWaveform.Data.GetWritableBuffer();

            double[] iqMagnitudes = referenceWaveform.Data.GetMagnitudeDataArray(false);
            double detroughRatio = detroughConfig.MinimumVoltage_V / detroughConfig.MaximumVoltage_V;

            // Waveforms are assumed to be normalized in range [0, 1], so no normalization will happen here
            switch (detroughConfig.Type)
            {
                case DetroughType.Exponential:
                    // Formula: e = IQmag + d*exp(-IQmag/d)
                    double expScale = 1 + detroughRatio * Math.Exp(-1.0 / detroughRatio);
                    for (int i = 0; i < envWfmWriteBuffer.Size; i++)
                    {
                        double sampleValue = iqMagnitudes[i] + detroughRatio * Math.Exp(-iqMagnitudes[i] / detroughRatio);
                        // Scale detrough to Vmax. Divide by detroughed waveform's max value to normalize. IQMagnitude's max value is 1
                        envWfmWriteBuffer[i] = ComplexSingle.FromSingle((float)(detroughConfig.MaximumVoltage_V * sampleValue / expScale));
                    }
                    break;
                case DetroughType.Cosine:
                    // Formula: e = 1 - (1-d)*cos(IQmag*pi/2)
                    double cosScale = 1 - (1 - detroughRatio) * Math.Cos(Math.PI / 2);
                    for (int i = 0; i < envWfmWriteBuffer.Size; i++)
                    {
                        double sampleValue = 1 - (1 - detroughRatio) * Math.Cos(iqMagnitudes[i] * cosScale);
                        // Scale detrough to Vmax. Divide by detroughed waveform's max value to normalize. IQMagnitude's max value is 1: 1-(1-d) *cos(1*pi/2)
                        envWfmWriteBuffer[i] = ComplexSingle.FromSingle((float)(detroughConfig.MaximumVoltage_V * sampleValue / cosScale));
                    }
                    break;
                case DetroughType.Power:
                    // Formula: e = (1-d) + IQmag^(exponent)*(1-d)
                    double powScale = 2 - 2 * detroughRatio;
                    for (int i = 0; i < envWfmWriteBuffer.Size; i++)
                    {
                        double sampleValue = (1 - detroughRatio) + (Math.Pow(iqMagnitudes[i], detroughConfig.Exponent) * (1 - detroughRatio));
                        // Scale detrough to Vmax. Divide by detroughed waveform's max value. IQMagnitude's max value is 1: (1-d) + (1^exponent)*(1-d)
                        envWfmWriteBuffer[i] = ComplexSingle.FromSingle((float)(detroughConfig.MaximumVoltage_V * sampleValue / powScale));
                    }
                    break;
                default:
                    throw new ArgumentException("Detrough type not supported.\n");
            }

            return envelopeWaveform;
        }

        /// <summary>Creates an envelope waveform utilizing the lookup table method.</summary>
        /// <param name="referenceWaveform">Specifies the waveform used for RF signal generation.</param>
        /// <param name="lookUpTableConfig">Specifies common settings for creating the lookup table envelope waveform.</param>
        /// <returns>The lookup table envelope waveform.</returns>
        public static Waveform CreateLookUpTableEnvelopeWaveform(Waveform referenceWaveform, LookUpTableConfiguration lookUpTableConfig)
        {
            ComplexSingle[] iq = referenceWaveform.Data.GetRawData(); // get copy of iq samples
            
            /// power conversions needed to understand following scaling:
            /// power_dBW = 20log(V) - 10log(R) - since R is 100 we get power_dBW = 20log(V) - 20
            /// if we want to normalize to dbW 1ohm, we would have 20log(V) - 10log(1) = 20log(V)
            /// power_dBm = power_dBW + 30  therefore power_dBm = 20log(V) + 10
            /// therefore, to convert from dBm to dBW 1ohm we subtract 10 from dBm value

            // scale waveform to have average dBW 1ohm power equal to dut input power normalized to dBW 1ohm
            ComplexSingle scale = ComplexSingle.FromSingle((float)Math.Pow(10.0, (referenceWaveform.PAPR_dB + lookUpTableConfig.DutAverageInputPower_dBm - 10.0) / 20.0));
            for (int i = 0; i < iq.Length; i++)
                iq[i] *= scale;

            // get 1 ohm power trace in watts of scaled iq data
            float[] powerTrace_W = new float[iq.Length];
            for (int i = 0; i < iq.Length; i++)
                powerTrace_W[i] = iq[i].Real * iq[i].Real + iq[i].Imaginary * iq[i].Imaginary;

            // get lookup table input power trace in 1ohm watts
            float[] lutDutInputPowerWattOneOhm = new float[lookUpTableConfig.DutInputPower_dBm.Length];
            for (int i = 0; i < lookUpTableConfig.DutInputPower_dBm.Length; i++)
                lutDutInputPowerWattOneOhm[i] = (float)Math.Pow(10.0, (lookUpTableConfig.DutInputPower_dBm[i] - 10.0) / 10.0); // V^2 = 10^((Pin - 10.0)/10)

            // run the trace through 1D interpolation
            float[] rawEnvelope = LinearInterpolation1D(lutDutInputPowerWattOneOhm, lookUpTableConfig.SupplyVoltage_V, powerTrace_W);

            // create waveform to return to the user
            Waveform envelopeWaveform = CloneAndConditionReferenceWaveform(referenceWaveform);

            // copy raw envelope data into cloned envelope waveform
            WritableBuffer<ComplexSingle> envWfmWriteBuffer = envelopeWaveform.Data.GetWritableBuffer();
            for (int i = 0; i < rawEnvelope.Length; i++)
                envWfmWriteBuffer[i] = ComplexSingle.FromSingle(rawEnvelope[i]);

            return envelopeWaveform;
        }
        #endregion

        #region Instrument Configuration

        /// <summary>Configures common instrument settings for the envelope generator.</summary>
        /// <param name="envVsg">The open RFSG session to configure.</param>
        /// <param name="envVsgConfig">The common settings to apply to the envelope generator.</param>
        /// <param name="trackerConfig">The common settings pertaining to the tracker that is used to modulate the power supply voltage.</param>
        public static void ConfigureEnvelopeGenerator(NIRfsg envVsg, EnvelopeGeneratorConfiguration envVsgConfig, TrackerConfiguration trackerConfig)
        {
            // all function calls assume a differential terminal configuration since that is the only option supported by the PXIe-5820
            envVsg.FrequencyReference.Source = RfsgFrequencyReferenceSource.FromString(envVsgConfig.ReferenceClockSource);
            envVsg.IQOutPort[""].LoadImpedance = trackerConfig.InputImpedance_Ohms == 50.0 ? 100.0 : trackerConfig.InputImpedance_Ohms;
            envVsg.IQOutPort[""].TerminalConfiguration = RfsgTerminalConfiguration.Differential;
            envVsg.IQOutPort[""].CommonModeOffset = trackerConfig.CommonModeOffset_V;
        }

        /// <summary>Scales the envelope waveform data based on the settings in <paramref name="trackerConfig"/>, and downloads the waveform to the envelope generator.</summary>
        /// <param name="envVsg">The open RFSG session to configure.</param>
        /// <param name="envelopeWaveform">The envelope waveform created by <see cref="CreateDetroughEnvelopeWaveform(Waveform, DetroughConfiguration)"/> or 
        /// <see cref="CreateLookUpTableEnvelopeWaveform(Waveform, LookUpTableConfiguration)"/> that is to be generated.</param>
        /// <param name="trackerConfig">The common settings pertaining to the tracker that is used to modulate the power supply voltage.</param>
        /// <returns>The envelope waveform with data scaled according to the tracker configuration.</returns>
        public static Waveform ScaleAndDownloadEnvelopeWaveform(NIRfsg envVsg, Waveform envelopeWaveform, TrackerConfiguration trackerConfig)
        {
            // grab the raw envelope so we can use linq to get statistics on it
            ComplexSingle.DecomposeArray(envelopeWaveform.Data.GetRawData(), out float[] envelope, out _);

            // scale envelope to adjust for tracker gain and offset
            for (int i = 0; i < envelope.Length; i++)
                envelope[i] = (float)((envelope[i] - trackerConfig.OutputOffset_V) / trackerConfig.Gain_VperV);

            // clone an envelope waveform to return to the user - want unique waveforms per tracker configuration
            Waveform scaledEnvelopeWaveform = envelopeWaveform;
            scaledEnvelopeWaveform.Data = envelopeWaveform.Data.Clone();
            WritableBuffer<ComplexSingle> scaledEnvelopeWaveformBuffer = scaledEnvelopeWaveform.Data.GetWritableBuffer();

            // populate cloned waveform with scaled waveform data
            for (int i = 0; i < envelope.Length; i++)
                scaledEnvelopeWaveformBuffer[i] = ComplexSingle.FromSingle(envelope[i]);

            // get peak of the waveform
            float absolutePeak = envelope.Max(i => Math.Abs(i)); // applies the absolute value function to each element and returns the max

            // scale waveform to peak voltage
            for (int i = 0; i < envelope.Length; i++)
                envelope[i] = envelope[i] / (absolutePeak); // brings waveform down to +/- 1 magnitude

            // set instrument properties
            envVsg.IQOutPort[""].Level = 2.0 * absolutePeak; // gain is interpreted as peak-to-peak
            envVsg.IQOutPort[""].Offset = 0.0; // set offset to 0 since this is done in DSP not in HW on the 5820 and only clips the waveform further

            // create another waveform that we can use to download the scaled envelope to the instrument
            Waveform instrEnvelopeWaveform = envelopeWaveform;
            instrEnvelopeWaveform.Data = envelopeWaveform.Data.Clone();
            WritableBuffer<ComplexSingle> instrEnvelopeWaveformBuffer = instrEnvelopeWaveform.Data.GetWritableBuffer();

            // populate cloned waveform with scaled waveform data
            for (int i = 0; i < envelope.Length; i++)
                instrEnvelopeWaveformBuffer[i] = ComplexSingle.FromSingle(envelope[i]);

            SG.DownloadWaveform(envVsg, instrEnvelopeWaveform); // download optimized waveform

            return scaledEnvelopeWaveform; // return the waveform as it will appear coming out of the front end of the envelope generator
        }

        /// <summary>Synchronizes the RF and envelope signal generators, and initiates generation.</summary>
        /// <param name="rfVsg">The open RFSG session corresponding to the RF signal generator.</param>
        /// <param name="envVsg">The open RFSG session corresponding to the envelope signal generator.</param>
        /// <param name="syncConfig">Specifies common settings used for synchronizing the RF and envelope signal generators.</param>
        public static void InitiateSynchronousGeneration(NIRfsg rfVsg, NIRfsg envVsg, SynchronizationConfiguration syncConfig)
        {   
            TClock tclk = new TClock(new ITClockSynchronizableDevice[] { rfVsg, envVsg });
            // The PXIe-5840 can only apply positive delays so we have to establish an inital delay of -RFDelayRange/2 for TCLK to handle negative shifts as well
            tclk.DevicesToSynchronize[0].SampleClockDelay = -syncConfig.RFDelayRange_s / 2.0; 
            rfVsg.Arb.RelativeDelay = syncConfig.RFDelayRange_s / 2.0 + syncConfig.RFDelay_s;
            tclk.ConfigureForHomogeneousTriggers();
            tclk.Synchronize();
            tclk.Initiate();
        }
        #endregion

        /// <summary>Duplicates <paramref name="referenceWaveform"/>, updates the name, and sets appropriate values for the waveform.</summary>
        /// <param name="referenceWaveform">Specfies the waveform to clone and condition.</param>
        /// <returns>The cloned and conditioned waveform.</returns>
        private static Waveform CloneAndConditionReferenceWaveform(Waveform referenceWaveform)
        {
            Waveform envelopeWaveform = referenceWaveform;
            envelopeWaveform.UpdateNameAndScript(referenceWaveform.Name + "Envelope");
            envelopeWaveform.Data = referenceWaveform.Data.Clone();
            envelopeWaveform.SignalBandwidth_Hz = 0.8 * referenceWaveform.SampleRate;
            envelopeWaveform.PAPR_dB = double.NaN; // papr does not make sense for envelope waveforms so set to NaN
            // burst length already copied
            // sample rate already copied
            envelopeWaveform.BurstStartLocations = (int[])referenceWaveform.BurstStartLocations?.Clone();
            envelopeWaveform.BurstStopLocations = (int[])referenceWaveform.BurstStopLocations?.Clone();
            envelopeWaveform.RuntimeScaling = 10.0 * Math.Log10(0.9); // applies 10% headroom
            // script was updated already
            return envelopeWaveform;
        }

        /// <summary>Performs one-dimensional interpolation using a selected method based on the lookup table defined by <paramref name="x"/> and <paramref name="y"/>.</summary>
        /// <param name="x">Specifies the array of tabulated values of the independent variable.</param>
        /// <param name="y">Specifies the array of tabulated values of the dependent variable.</param>
        /// <param name="xi">Specifies the array of values of the independent variable at which the interpolated values (yi) of the dependent variable are computed.</param>
        /// <param name="monotonic">Specifies whether the values in <paramref name="x"/> are increasing monotonically with the index.</param>
        /// <returns>The output array of interpolated values (yi) that correspond to the <paramref name="xi"/> independent variable values.</returns>
        private static float[] LinearInterpolation1D(float[] x, float[] y, float[] xi, bool monotonic = false)
        {
            // assumes x and y are the same length and lengths are > 1

            if (!monotonic)
            {
                // clone array so we don't act on user's reference
                x = (float[])x.Clone();
                y = (float[])y.Clone();
                Array.Sort(x, y);
            }

            float[] yi = new float[xi.Length];

            for (int i = 0; i < xi.Length; i++)
            {
                float x1 = xi[i];
                int index = Array.BinarySearch(x, x1);
                if (index < 0) // if the needle falls within two values in the haystack, search returns bitwise compliment of index of next larger value
                    index = ~index; // take bitwise compliment to get index of next larger value
                if (index == 0) // handle edge case where value is less than first element in the lut
                    index = 1; // interpolate on first two elements in the lut
                else if (index == x.Length) // handle edge case where value is greater than last element in the lut
                    index = x.Length - 1; // interpolate on last two elements in the lut
                float x0 = x[index - 1];
                float x2 = x[index];
                float y0 = y[index - 1];
                float y2 = y[index];
                yi[i] = (y2 - y0) / (x2 - x0) * (x1 - x0) + y0;
            }

            return yi;
        }
    }
}
