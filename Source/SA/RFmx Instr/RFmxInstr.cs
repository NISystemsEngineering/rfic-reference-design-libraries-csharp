using NationalInstruments.RFmx.InstrMX;
using System.Text.RegularExpressions;

namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    /// <summary>Defines common types and methods for configuring instruments with NI-RFmx.</summary>
    public static class RFmxInstr
    {
        #region Type Definitions
        /// <summary>Defines common instrument configurations to apply to the analyzer.</summary>
        public struct InstrumentConfiguration
        {
            /// <summary>Defines the local oscillator sharing behavior for VST devices.</summary>
            public LocalOscillatorSharingMode LOSharingMode;
            /// <summary>Specifies the frequency reference source.</summary>
            public string FrequencyReferenceSource;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static InstrumentConfiguration GetDefault()
            {
                return new InstrumentConfiguration
                {
                    LOSharingMode = LocalOscillatorSharingMode.Automatic,
                    FrequencyReferenceSource = RFmxInstrMXConstants.PxiClock
                };
            }
        }
        #endregion

        #region Instrument Configurations
        /// <summary>Applies common instrument settings to the analyzer.</summary>
        /// <param name="instrHandle">The open RFmx Instr session to configure.</param>
        /// <param name="instrConfig">The instrument configuration properties to apply.</param>
        public static void ConfigureInstrument(RFmxInstrMX instrHandle, InstrumentConfiguration instrConfig)
        {
            instrHandle.SetFrequencyReferenceSource("", instrConfig.FrequencyReferenceSource);
            instrHandle.GetInstrumentModel("", out string model);
            // Only configure LO settings on supported VSTs
            if (Regex.IsMatch(model, "NI PXIe-58[34].")) // Matches 583x and 584x VST families
            {
                if (instrConfig.LOSharingMode == LocalOscillatorSharingMode.None)
                    instrHandle.ConfigureAutomaticSGSASharedLO("", RFmxInstrMXAutomaticSGSASharedLO.Disabled);
                else
                {
                    instrHandle.ConfigureAutomaticSGSASharedLO("", RFmxInstrMXAutomaticSGSASharedLO.Enabled);
                    // Configure automatic LO offsetting
                    instrHandle.SetLOLeakageAvoidanceEnabled("", RFmxInstrMXLOLeakageAvoidanceEnabled.True);
                    instrHandle.ResetAttribute("", RFmxInstrMXPropertyId.DownconverterFrequencyOffset);
                }
            }
        }
        #endregion
    }
}
