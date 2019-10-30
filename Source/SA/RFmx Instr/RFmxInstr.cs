using NationalInstruments.RFmx.InstrMX;

namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    public static class RFmxInstr
    {
        #region Type Definitions
        public struct InstrumentConfiguration
        {
            public LocalOscillatorSharingMode LOSharingMode;
            public LocalOscillatorFrequencyOffsetMode LOOffsetMode;

            public static InstrumentConfiguration GetDefault()
            {
                return new InstrumentConfiguration
                {
                    LOSharingMode = LocalOscillatorSharingMode.Automatic,
                };
            }
        }
        #endregion

        #region Instrument Configurations
        public static void ConfigureInstrument(RFmxInstrMX instrHandle, InstrumentConfiguration instrConfig)
        {
            if (instrConfig.LOSharingMode == LocalOscillatorSharingMode.None)
                instrHandle.ConfigureAutomaticSGSASharedLO("", RFmxInstrMXAutomaticSGSASharedLO.Disabled);
            else
                instrHandle.ConfigureAutomaticSGSASharedLO("", RFmxInstrMXAutomaticSGSASharedLO.Enabled);

            if (instrConfig.LOOffsetMode == LocalOscillatorFrequencyOffsetMode.NoOffset)
                instrHandle.SetLOLeakageAvoidanceEnabled("", RFmxInstrMXLOLeakageAvoidanceEnabled.False);
            else
                instrHandle.ResetAttribute("", RFmxInstrMXPropertyId.LOLeakageAvoidanceEnabled);
        }
        #endregion
    }
}
