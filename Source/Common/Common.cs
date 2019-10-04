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
        public LocalOscillatorOffsetMode OffsetMode;
        public double Offset_Hz;

        public static LocalOscillatorConfiguration GetDefault()
        {
            return new LocalOscillatorConfiguration()
            {
                ChannelName = "",
                SharingMode = LocalOscillatorSharingMode.Automatic,
                Source = "Onboard", // since default sharing style is automatic, the initial default value here doesn't matter much
                OffsetMode = LocalOscillatorOffsetMode.Automatic,
                Offset_Hz = 0.0
            };
        }
    }
}
