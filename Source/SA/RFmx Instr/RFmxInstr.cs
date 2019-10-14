using NationalInstruments.RFmx.InstrMX;
using System.Text.RegularExpressions;

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

            public static InstrumentConfiguration GetDefault(RFmxInstrMX sessionHandle)
            {
                InstrumentConfiguration instrConfig = GetDefault(); // covers case for sub6 instruments with a single configurable LO
                // lo configuration will now be overridden if the instrument has number of LOs != 1
                sessionHandle.GetInstrumentModel("", out string instrumentModel);
                if (instrumentModel.Equals("NI PXIe-5830"))
                    instrConfig.LoConfigurations[0].ChannelName = "LO2";
                else if (instrumentModel.Equals("NI PXIe-5831"))
                {
                    LocalOscillatorConfiguration lo1Config = LocalOscillatorConfiguration.GetDefault();
                    lo1Config.ChannelName = "LO1";
                    LocalOscillatorConfiguration lo2Config = LocalOscillatorConfiguration.GetDefault();
                    lo2Config.ChannelName = "LO2";
                    instrConfig.LoConfigurations = new LocalOscillatorConfiguration[] { lo1Config, lo2Config };
                }
                else if (Regex.IsMatch(instrumentModel, "NI PXIe-5(82.|645R)")) // matches on any baseband instrument without an LO
                {
                    instrConfig.LoConfigurations = new LocalOscillatorConfiguration[0]; // baseband instruments have no LOs
                }
                return instrConfig;
            }
        }
        #endregion

        #region Instrument Configurations
        public static void ConfigureInstrument(RFmxInstrMX instrHandle, InstrumentConfiguration instrConfig)
        {
            instrHandle.GetInstrumentModel("", out string instrumentModel);

            if (Regex.IsMatch(instrumentModel, "NI PXIe-5(82.|645R)")) // matches on any baseband instrument without an LO
                return; // return early since the instrument doesn't have any LOs that can be configured

            /// Properties to modify related to LO:
            /// AutomaticSGSASharedLO
            /// LOSource
            /// LOOutEnabled
            foreach (LocalOscillatorConfiguration loConfig in instrConfig.LoConfigurations)
            {
                switch (loConfig.SharingMode)
                {
                    case LocalOscillatorSharingMode.None:
                        if (Regex.IsMatch(instrumentModel, "NI PXIe-58[34].")) 
                            instrHandle.SetAutomaticSGSASharedLO(loConfig.ChannelName, RFmxInstrMXAutomaticSGSASharedLO.Disabled);
                        instrHandle.SetLOSource(loConfig.ChannelName, RFmxInstrMXConstants.LOSourceOnboard);
                        instrHandle.SetLOExportEnabled(loConfig.ChannelName, false);
                        break;
                    case LocalOscillatorSharingMode.Manual:
                        if (Regex.IsMatch(instrumentModel, "NI PXIe-58[34]."))
                            instrHandle.SetAutomaticSGSASharedLO(loConfig.ChannelName, RFmxInstrMXAutomaticSGSASharedLO.Disabled); 
                        instrHandle.SetLOSource(loConfig.ChannelName, loConfig.Source);
                        instrHandle.SetLOExportEnabled(loConfig.ChannelName, loConfig.ExportEnabled);
                        break;
                    default:
                        instrHandle.ResetAttribute(loConfig.ChannelName, RFmxInstrMXPropertyId.LOSource);
                        instrHandle.ResetAttribute(loConfig.ChannelName, RFmxInstrMXPropertyId.LOExportEnabled);
                        if (Regex.IsMatch(instrumentModel, "NI PXIe-58[34]."))
                            instrHandle.SetAutomaticSGSASharedLO(loConfig.ChannelName, RFmxInstrMXAutomaticSGSASharedLO.Enabled);
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
                    default:
                        instrHandle.ResetAttribute(loConfig.ChannelName, RFmxInstrMXPropertyId.DownconverterFrequencyOffset);
                        instrHandle.SetLOLeakageAvoidanceEnabled(loConfig.ChannelName, RFmxInstrMXLOLeakageAvoidanceEnabled.True);
                        break;
                }
            }
        }
        #endregion
    }
}

