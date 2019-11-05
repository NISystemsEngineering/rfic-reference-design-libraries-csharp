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

    public enum LocalOscillatorFrequencyOffsetMode
    {
        Automatic,
        NoOffset
    }
}
