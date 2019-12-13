namespace NationalInstruments.ReferenceDesignLibraries
{
    /// <summary>
    /// Defines a waveform and associated parameters necessary for generation or proper measurement.
    /// </summary>
    public struct Waveform
    {
        /// <summary>Defines the name of the waveform that will be used to identify it on the generator.</summary>
        public string Name;
        /// <summary>Defines the complex samples that comprise the waveform.</summary>
        public ComplexWaveform<ComplexSingle> Data;
        /// <summary>Defines the bandwidth of the waveform data in Hz.</summary>
        public double SignalBandwidth_Hz;
        /// <summary>Defines the peak-to-average power ratio (PAPR) of the waveform data.</summary>
        public double PAPR_dB;
        /// <summary>Defines the length in seconds from the beginning of the first sample of the first burst and the last sample of the final burst.</summary>
        public double BurstLength_s;
        /// <summary>Defines the sample rate of the waveform data in samples/second.</summary>
        public double SampleRate;
        /// <summary>Defines the sample location where each burst within the waveform data begins. If no burst is present the first sample of the waveform will be used.</summary>
        public int[] BurstStartLocations;
        /// <summary>Defines the sample location where each burst within the waveform data ends. If no burst is present the last sample of the waveform will be used.</summary>
        public int[] BurstStopLocations;
        /// <summary>Specifies whether there is any idle (off or null) time within the waveform data. This will be true if the burst start/stop locations are not the first/last 
        /// samples of the waveform.</summary>
        public bool IdleDurationPresent;
        /// <summary>Specifies a digital scaling factor in dB that will be applied to the waveform data during generation to avoid any DAC overflow issues. The default value is -1.5 dB.</summary>
        public double RuntimeScaling;
        /// <summary>Specifies the script that will be used in order to generate the waveform.</summary>
        public string Script;

        /// <summary>Updates the <see cref="Name"/> value with <paramref name="newName"/> and replaces any reference to it in <see cref="Script"/>.</summary>
        /// <param name="newName"></param>
        public void UpdateNameAndScript(string newName)
        {
            // Update the script with the new waveform name
            Script = Script?.Replace(Name, newName);
            Name = newName;
        }
    }

    /// <summary>Defines the local oscillator (LO) sharing behavior for VST devices.</summary>
    public enum LocalOscillatorSharingMode
    {
        /// <summary>Automatically share the local oscillator from the generator to the analyzer for measurements that require it to be shared.</summary>
        Automatic,
        /// <summary>Do not automatically share the local oscillator from the generator to the analyzer.</summary>
        None
    }

    // SA Specific Common Properties
    namespace SA
    {
        /// <summary>Defines parameters common to all analyzer configurations and each RFmx personality.</summary>
        public struct CommonConfiguration
        {
            /// <summary>Specifies the carrier frequency of the RF signal to acquire. The signal analyzer tunes to this frequency. This value is expressed in Hz.</summary>
            public double CenterFrequency_Hz;
            /// <summary>Specifies the reference level that represents the maximum expected power of the RF input signal. This value is expressed in dBm.</summary>
            public double ReferenceLevel_dBm;
            /// <summary>Specifies the attenuation of a switch or cable connected to the RF IN connector of the signal analyzer. This value is expressed in dB.</summary>
            public double ExternalAttenuation_dB;
            /// <summary>Specifies the instrument port to be used by the measurement.</summary>
            public string SelectedPorts;
            /// <summary>Specifies whether to enable the trigger.</summary>
            public bool TriggerEnabled;
            /// <summary>Specifies the source terminal for the digital edge trigger.</summary>
            public string DigitalTriggerSource;
            /// <summary>Specifies the trigger delay time, in seconds.</summary>
            public double TriggerDelay_s;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static CommonConfiguration GetDefault()
            {
                return new CommonConfiguration
                {
                    SelectedPorts = "",
                    CenterFrequency_Hz = 1e9,
                    ReferenceLevel_dBm = 0,
                    ExternalAttenuation_dB = 0,
                    TriggerEnabled = true,
                    DigitalTriggerSource = "PXI_Trig0",
                    TriggerDelay_s = 0,
                };
            }
        }

        /// <summary>Defines configuration parameters used for invoking RFmx AutoLevel. This examines the incoming signal to calculate the peak power level and sets it as 
        /// the value of the Reference Level. This is used to help calculate an approximate setting for the power level for measurements.</summary>
        public struct AutoLevelConfiguration
        {
            /// <summary>Specifies whether or not autolevel should be enabled.</summary>
            public bool Enabled;
            /// <summary>Specifies the acquisition length, in seconds. Auto Level ignores trigger settings, so this value should be set to a single period of a waveform including any idle time.</summary>
            public double MeasurementInterval_s;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static AutoLevelConfiguration GetDefault()
            {
                return new AutoLevelConfiguration
                {
                    Enabled = false,
                    MeasurementInterval_s = 10e-3,
                };
            }
        }
    }
}
