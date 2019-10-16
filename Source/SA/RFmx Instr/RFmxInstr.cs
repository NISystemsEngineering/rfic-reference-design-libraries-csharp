using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.ModularInstruments.NIRfsa;
using System.Text.RegularExpressions;

namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    public static class RFmxInstr
    {
        #region Type Definitions
        public struct InstrumentConfiguration
        {
            public LocalOscillatorSharingMode LOSharingMode;
            public LocalOscillatorRoutingConfiguration[] LORoutingConfigurations;
            public LocalOscillatorFrequencyOffsetConfiguration LOOffsetConfiguration;

            public static InstrumentConfiguration GetDefault()
            {
                return new InstrumentConfiguration
                {
                    LOSharingMode = LocalOscillatorSharingMode.Automatic,
                    LORoutingConfigurations = new LocalOscillatorRoutingConfiguration[] { LocalOscillatorRoutingConfiguration.GetDefault() },
                    LOOffsetConfiguration = LocalOscillatorFrequencyOffsetConfiguration.GetDefault()
                };
            }

            public static InstrumentConfiguration GetDefault(RFmxInstrMX sessionHandle)
            {
                InstrumentConfiguration instrConfig = GetDefault(); // covers case for sub6 instruments with a single configurable LO
                // lo routing configurations will now be overridden if the instrument has number of LOs != 1
                sessionHandle.GetInstrumentModel("", out string instrumentModel);
                if (instrumentModel.Equals("NI PXIe-5830"))
                    instrConfig.LORoutingConfigurations[0].ChannelName = "LO2";
                else if (Regex.IsMatch(instrumentModel, "NI PXIe-5831"))
                {
                    LocalOscillatorRoutingConfiguration lo1RoutingConfig = LocalOscillatorRoutingConfiguration.GetDefault();
                    lo1RoutingConfig.ChannelName = "LO1";
                    LocalOscillatorRoutingConfiguration lo2RoutingConfig = LocalOscillatorRoutingConfiguration.GetDefault();
                    lo2RoutingConfig.ChannelName = "LO2";
                    instrConfig.LORoutingConfigurations = new LocalOscillatorRoutingConfiguration[] { lo1RoutingConfig, lo2RoutingConfig };
                }
                else if (Regex.IsMatch(instrumentModel, "NI PXIe-5(82.|645R)")) // matches on any baseband instrument without an LO
                {
                    instrConfig.LORoutingConfigurations = new LocalOscillatorRoutingConfiguration[0]; // baseband instruments have no LOs
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

            // Set channel agnostic auto SG SA shared property
            if (Regex.IsMatch(instrumentModel, "NI PXIe-58[34]."))
                switch (instrConfig.LOSharingMode)
                {
                    case LocalOscillatorSharingMode.None:
                    case LocalOscillatorSharingMode.Manual:
                        instrHandle.SetAutomaticSGSASharedLO("", RFmxInstrMXAutomaticSGSASharedLO.Disabled);
                        break;
                    default:
                        instrHandle.SetAutomaticSGSASharedLO("", RFmxInstrMXAutomaticSGSASharedLO.Enabled);
                        break;
                }

            /// Properties to modify related to LO routing:
            /// LOSource
            /// LOOutEnabled
            foreach (LocalOscillatorRoutingConfiguration loRoutingConfig in instrConfig.LORoutingConfigurations)
            {
                switch (instrConfig.LOSharingMode)
                {
                    case LocalOscillatorSharingMode.None:
                        instrHandle.SetLOSource(loRoutingConfig.ChannelName, RFmxInstrMXConstants.LOSourceOnboard);
                        instrHandle.SetLOExportEnabled(loRoutingConfig.ChannelName, false);
                        break;
                    case LocalOscillatorSharingMode.Manual:
                        instrHandle.SetLOSource(loRoutingConfig.ChannelName, loRoutingConfig.Source);
                        instrHandle.SetLOExportEnabled(loRoutingConfig.ChannelName, loRoutingConfig.ExportEnabled);
                        break;
                    default:
                        instrHandle.ResetAttribute(loRoutingConfig.ChannelName, RFmxInstrMXPropertyId.LOSource);
                        instrHandle.ResetAttribute(loRoutingConfig.ChannelName, RFmxInstrMXPropertyId.LOExportEnabled);
                        break;
                }
            }

            switch (instrConfig.LOOffsetConfiguration.Mode)
            {
                case LocalOscillatorFrequencyOffsetMode.NoOffset:
                    instrHandle.SetLOLeakageAvoidanceEnabled("", RFmxInstrMXLOLeakageAvoidanceEnabled.False);
                    instrHandle.SetDownconverterFrequencyOffset("", 0.0);
                    break;
                case LocalOscillatorFrequencyOffsetMode.UserDefined:
                    instrHandle.SetLOLeakageAvoidanceEnabled("", RFmxInstrMXLOLeakageAvoidanceEnabled.False);
                    instrHandle.SetDownconverterFrequencyOffset("", instrConfig.LOOffsetConfiguration.Offset_Hz);
                    break;
                default:
                    instrHandle.ResetAttribute("", RFmxInstrMXPropertyId.DownconverterFrequencyOffset);
                    instrHandle.SetLOLeakageAvoidanceEnabled("", RFmxInstrMXLOLeakageAvoidanceEnabled.True);
                    break;
            }
        }
        #endregion
    }
}

