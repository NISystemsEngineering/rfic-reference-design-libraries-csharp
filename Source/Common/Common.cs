using Ivi.Driver;

namespace NationalInstruments.ReferenceDesignLibraries
{
    public enum LocalOscillatorSharingMode
    {
        None,
        Automatic,
        Manual
    }

    public enum LocalOscillatorFrequencyOffsetMode
    {
        NoOffset,
        Automatic,
        UserDefined
    }

    public struct LocalOscillatorRoutingConfiguration
    {
        public string ChannelName;
        public string Source;
        public bool ExportEnabled;

        public static LocalOscillatorRoutingConfiguration GetDefault()
        {
            return new LocalOscillatorRoutingConfiguration()
            {
                ChannelName = "",
                Source = "Onboard", // since default sharing style is automatic, the initial default value here doesn't matter much
                ExportEnabled = false
            };
        }

        public static LocalOscillatorRoutingConfiguration[] GetInstrumentDefaults(IIviDriver handle)
        {
            switch (handle.Identity.InstrumentModel)
            {
                case "NI PXIe-5830":
                case "NI PXIe-5831":
                    LocalOscillatorRoutingConfiguration lo1RoutingConfig = GetDefault();
                    lo1RoutingConfig.ChannelName = "LO1";
                    LocalOscillatorRoutingConfiguration lo2RoutingConfig = GetDefault();
                    lo2RoutingConfig.ChannelName = "LO2";
                    return new LocalOscillatorRoutingConfiguration[] { lo1RoutingConfig, lo2RoutingConfig };
                default:
                    return new LocalOscillatorRoutingConfiguration[] { GetDefault() };
            }
        }
    }

    public struct LocalOscillatorFrequencyOffsetConfiguration
    {
        public LocalOscillatorFrequencyOffsetMode Mode;
        public double Offset_Hz;

        public static LocalOscillatorFrequencyOffsetConfiguration GetDefault()
        {
            return new LocalOscillatorFrequencyOffsetConfiguration()
            {
                Mode = LocalOscillatorFrequencyOffsetMode.Automatic,
                Offset_Hz = 0.0
            };
        }
    }
}
