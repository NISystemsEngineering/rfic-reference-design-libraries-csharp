using NationalInstruments.DataInfrastructure;

namespace NationalInstruments.ReferenceDesignLibraries
{
    public struct SACommonConfiguration
    {
        public double CenterFrequency_Hz;
        public double ReferenceLevel_dBm;
        public double ExternalAttenuation_dB;
        public string SelectedPorts;
        public bool EnableTrigger;
        public string DigitalTriggerSource;
        public double TriggerDelay_s;
        public static SACommonConfiguration GetDefault()
        {
            return new SACommonConfiguration
            {
                SelectedPorts = "",
                CenterFrequency_Hz = 1e9,
                ReferenceLevel_dBm = 0,
                ExternalAttenuation_dB = 0,
                EnableTrigger = true,
                DigitalTriggerSource = "PXI_Trig0",
                TriggerDelay_s = 0,
            };
        }
    }
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

}
