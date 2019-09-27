using NationalInstruments.RFmx.InstrMX;


namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    public static class RFmxInstr
    {
        #region Type Definitions
        public struct LoConfiguration
        {
            public string LoSharingMode; //{"None","Automatic","Manual"}
            public string LoSource;
            public string LoOffsetMode; //{"NoOffset","Automatic","UserDefined"}
            public double LoOffset_Hz;

            public static LoConfiguration GetDefault()
            {
                return new LoConfiguration
                {
                    LoSharingMode = "Automatic",
                    LoSource = RFmxInstrMXConstants.LOSourceLOIn,
                    LoOffsetMode = "Automatic",
                    LoOffset_Hz     = 0                                   
                };
            }
        }

        #endregion
        #region Instrument Configurations
        public static void ConfigureInstrument(RFmxInstrMX sessionHandle, LoConfiguration loConfig, string selectorString = "")
        {
            switch (loConfig.LoSharingMode.ToLower())
            {

                default:
                    sessionHandle.SetAutomaticSGSASharedLO(selectorString, RFmxInstrMXAutomaticSGSASharedLO.Disabled);
                    sessionHandle.SetLOSource("", RFmxInstrMXConstants.LOSourceOnboard);
                    break;
                case "automatic":
                    sessionHandle.SetAutomaticSGSASharedLO(selectorString, RFmxInstrMXAutomaticSGSASharedLO.Enabled);
                    sessionHandle.ResetAttribute("", RFmxInstrMXPropertyId.LOSource);
                    break;
                case "manual":
                    sessionHandle.SetAutomaticSGSASharedLO(selectorString, RFmxInstrMXAutomaticSGSASharedLO.Disabled);
                    sessionHandle.SetLOSource(selectorString, RFmxInstrMXConstants.LOSourceLOIn);
                    break;
                
            }

            switch (loConfig.LoOffsetMode.ToLower())
            {
                case "nooffset": default:
                    sessionHandle.SetLOLeakageAvoidanceEnabled(selectorString,RFmxInstrMXLOLeakageAvoidanceEnabled.False);
                    sessionHandle.SetDownconverterFrequencyOffset(selectorString, 0);
                    break;
                case "automatic":
                    sessionHandle.SetLOLeakageAvoidanceEnabled(selectorString, RFmxInstrMXLOLeakageAvoidanceEnabled.True);
                    sessionHandle.ResetAttribute(selectorString, RFmxInstrMXPropertyId.DownconverterFrequencyOffset);
                    break;
                case "userdefined":
                    sessionHandle.SetLOLeakageAvoidanceEnabled(selectorString, RFmxInstrMXLOLeakageAvoidanceEnabled.False);
                    sessionHandle.SetDownconverterFrequencyOffset(selectorString, loConfig.LoOffset_Hz);
                    break;
            }
        }
        #endregion
    }
}

