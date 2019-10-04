using Ivi.Driver;

namespace NationalInstruments.ReferenceDesignLibraries
{
    public enum LocalOscillatorSharingMode
    {
        None,
        Automatic,
        Manual
    }

    public enum LocalOscillatorOffsetMode
    {
        NoOffset,
        Automatic,
        UserDefined
    }

    public struct LocalOscillatorConfiguration
    {
        public string ChannelName;
        public LocalOscillatorSharingMode SharingMode;
        public string Source;
        public bool ExportEnabled;
        public LocalOscillatorOffsetMode OffsetMode;
        public double Offset_Hz;

        public static LocalOscillatorConfiguration GetDefault()
        {
            return new LocalOscillatorConfiguration()
            {
                ChannelName = "",
                SharingMode = LocalOscillatorSharingMode.Automatic,
                Source = "Onboard", // since default sharing style is automatic, the initial default value here doesn't matter much
                ExportEnabled = false,
                OffsetMode = LocalOscillatorOffsetMode.Automatic,
                Offset_Hz = 0.0
            };
        }

        public static LocalOscillatorConfiguration[] GetInstrumentDefaults(IIviDriver handle)
        {
            switch (handle.Identity.InstrumentModel)
            {
                case "NI PXIe-5830":
                case "NI PXIe-5831":
                    LocalOscillatorConfiguration lo1Config = GetDefault();
                    lo1Config.ChannelName = "LO1";
                    LocalOscillatorConfiguration lo2Config = GetDefault();
                    lo2Config.ChannelName = "LO2";
                    return new LocalOscillatorConfiguration[] { lo1Config, lo2Config };
                default:
                    return new LocalOscillatorConfiguration[] { GetDefault() };
            }
        }
    }
}
