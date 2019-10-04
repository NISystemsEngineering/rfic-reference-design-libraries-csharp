using NationalInstruments.RFmx.InstrMX;

namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    public static class RFmxInstr
    {
        #region Type Definitions
        public struct InstrumentConfiguration
        {
            public LocalOscillatorConfiguration[] LoConfigurations;

            public static InstrumentConfiguration GetDefault()
            {
                return new InstrumentConfiguration
                {
                    LoConfigurations = new LocalOscillatorConfiguration[] { LocalOscillatorConfiguration.GetDefault() }       
                };
            }
        }
        #endregion

        #region Instrument Configurations
        public static void ConfigureInstrument(RFmxInstrMX instrHandle, InstrumentConfiguration instrConfig)
        {
            foreach (LocalOscillatorConfiguration loConfig in instrConfig.LoConfigurations)
            {
                switch (loConfig.SharingMode)
                {
                    case LocalOscillatorSharingMode.None:
                        instrHandle.SetAutomaticSGSASharedLO(loConfig.ChannelName, RFmxInstrMXAutomaticSGSASharedLO.Disabled);
                        instrHandle.SetLOExportEnabled(loConfig.ChannelName, false);
                        instrHandle.SetLOSource(loConfig.ChannelName, RFmxInstrMXConstants.LOSourceOnboard);
                        break;
                    case LocalOscillatorSharingMode.Manual:
                        instrHandle.SetAutomaticSGSASharedLO(loConfig.ChannelName, RFmxInstrMXAutomaticSGSASharedLO.Disabled);
                        instrHandle.SetLOExportEnabled(loConfig.ChannelName, loConfig.ExportEnabled);
                        instrHandle.SetLOSource(loConfig.ChannelName, loConfig.Source);
                        break;
                    default: // default to automatic case
                        instrHandle.SetAutomaticSGSASharedLO(loConfig.ChannelName, RFmxInstrMXAutomaticSGSASharedLO.Enabled);
                        instrHandle.ResetAttribute(loConfig.ChannelName, RFmxInstrMXPropertyId.LOExportEnabled);
                        instrHandle.ResetAttribute(loConfig.ChannelName, RFmxInstrMXPropertyId.LOSource);
                        break;
                }

                switch (loConfig.OffsetMode)
                {
                    case LocalOscillatorOffsetMode.NoOffset:
                        instrHandle.SetLOLeakageAvoidanceEnabled(loConfig.ChannelName, RFmxInstrMXLOLeakageAvoidanceEnabled.False);
                        instrHandle.SetDownconverterFrequencyOffset(loConfig.ChannelName, 0.0);
                        break;
                    case LocalOscillatorOffsetMode.UserDefined:
                        instrHandle.SetLOLeakageAvoidanceEnabled(loConfig.ChannelName, RFmxInstrMXLOLeakageAvoidanceEnabled.False);
                        instrHandle.SetDownconverterFrequencyOffset(loConfig.ChannelName, loConfig.Offset_Hz);
                        break;
                    default: // default to automatic case
                        instrHandle.SetLOLeakageAvoidanceEnabled(loConfig.ChannelName, RFmxInstrMXLOLeakageAvoidanceEnabled.True);
                        instrHandle.ResetAttribute(loConfig.ChannelName, RFmxInstrMXPropertyId.DownconverterFrequencyOffset);
                        break;
                }
            }
        }
        #endregion
    }
}

