using NationalInstruments.RFmx.InstrMX;


namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    public static class RFmxInstr
    {
        #region Type Definitions
        public struct InstrumentConfiguration
        {
            public bool AutoLoShared;
            public string LoOffsetMode; //{"NoOffset","Automatic","UserDefined"}
            public double LoOffset;

            public static InstrumentConfiguration GetDefault()
            {
                return new InstrumentConfiguration
                {
                    AutoLoShared = true,
                    LoOffsetMode = "Automatic",
                    LoOffset     = 0                                   
                };
            }
        }

        #endregion
        #region Instrument Configurations
        public static void ConfigureInstrument(RFmxInstrMX sessionHandle, InstrumentConfiguration instrConfig, string selectorString = "")
        {
            if (instrConfig.AutoLoShared)
            {
                sessionHandle.SetAutomaticSGSASharedLO("", RFmxInstrMXAutomaticSGSASharedLO.Enabled);
            }
            else
            {
                sessionHandle.SetAutomaticSGSASharedLO("", RFmxInstrMXAutomaticSGSASharedLO.Disabled);
            }

            switch (instrConfig.LoOffsetMode.ToLower())
            {
                case "nooffset": default:
                    sessionHandle.SetLOLeakageAvoidanceEnabled(selectorString,RFmxInstrMXLOLeakageAvoidanceEnabled.False);
                    sessionHandle.SetDownconverterFrequencyOffset(selectorString, 0);
                    return;
                case "automatic":
                    sessionHandle.SetLOLeakageAvoidanceEnabled(selectorString, RFmxInstrMXLOLeakageAvoidanceEnabled.True);
                    sessionHandle.ResetAttribute(selectorString, RFmxInstrMXPropertyId.DownconverterFrequencyOffset);
                    return;
                case "userdefined":
                    sessionHandle.SetLOLeakageAvoidanceEnabled(selectorString, RFmxInstrMXLOLeakageAvoidanceEnabled.False);
                    sessionHandle.SetDownconverterFrequencyOffset(selectorString, instrConfig.LoOffset);
                    return;
            }


        }
        #endregion
    }
}

