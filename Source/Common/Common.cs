using NationalInstruments.DataInfrastructure;

namespace NationalInstruments.ReferenceDesignLibraries
{
    public struct Waveform
    {
        public string Name;
        public ComplexWaveform<ComplexSingle> Data;
        public double SignalBandwidth_Hz;
        public double PAPR_dB;
        public double BurstLength_s;
        public double SampleRate;
        public int[] BurstStartLocations;
        public int[] BurstStopLocations;
        public bool IdleDurationPresent;
        public double RuntimeScaling;
        public string Script;
    }
    public enum LocalOscillatorSharingMode
    {
        Automatic,
        None
    }
    // SA Specific Common Properties
    namespace SA
    {
        public struct CommonConfiguration
        {
            public double CenterFrequency_Hz;
            public double ReferenceLevel_dBm;
            public double ExternalAttenuation_dB;
            public string SelectedPorts;
            public bool TriggerEnabled;
            public string DigitalTriggerSource;
            public double TriggerDelay_s;
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
        public struct AutoLevelConfiguration
        {
            public bool Enabled;
            public double MeasurementInterval_s;
            public double Bandwidth_Hz;

            public static AutoLevelConfiguration GetDefault()
            {
                return new AutoLevelConfiguration
                {
                    Enabled = false,
                    MeasurementInterval_s = 10e-3,
                    Bandwidth_Hz = 20e6
                };
            }
        }
    }
}
