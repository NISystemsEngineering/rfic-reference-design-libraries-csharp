using NationalInstruments.ModularInstruments.Interop;

namespace NationalInstruments.ReferenceDesignLibraries
{
    public static class PowerMeter
    {
        #region Type Definition  
        /// <summary>
        /// Defines common settings for power meter.
        /// </summary>
        public struct CommonConfiguration
        {
            /// <summary>
            /// Specifies the name of the channel you want to configure.
            /// </summary>
            public string Channel_Name;

            /// <summary>
            /// Specifies the measurement mode to which sensor should be switched.
            /// </summary>
            public int Measurement_Mode;

            /// <summary>
            /// Specifies the units NI-568x uses to return measured values. NI-568x sets the NI568X_ATTR_UNITS attribute to this value.
            /// </summary>
            public int Units;

            /// <summary>
            /// Specifies the trigger source.
            /// </summary>
            public int Trigger_Source;

            /// <summary>
            /// Specifies the polarity of the internal trigger slope.
            /// </summary>
            public int Slope;

            /// <summary>
            /// Specifies the trigger level for the measurement signal.
            /// </summary>
            public double Trigger_Level;

            /// <summary>
            /// Specifies whether auto-averaging mode is enabled or not.
            /// </summary>
            public bool Averaging_Auto_Enabled;

            /// <summary>
            /// Specifies the frequency in Hz of the power to be measured since this frequency is not automatically determined.
            /// </summary>
            public double Frequency;

            /// <summary>
            /// Specifies the ContAv Aperture in seconds.
            /// </summary>
            public double ApertureTime;

            /// <summary>
            /// Specifies the filter bandwidth.
            /// </summary>
            public int Count;

            /// <summary>
            /// Returns the struct with default values set.
            /// </summary>
            public static CommonConfiguration GetDefault()
            {
                var commonConfiguration = new CommonConfiguration
                {
                    Channel_Name = "0",
                    Frequency = 2E9,
                    Units = 1,

                    Measurement_Mode = ni568xConstants.ContinuousMode,
                    Trigger_Source = ni568xConstants.Immediate,
                    Slope = ni568xConstants.Positive,
                    Trigger_Level = -20,
                    Averaging_Auto_Enabled = true,
                    ApertureTime = 0.02,
                    Count = 4
                };
                return commonConfiguration;
            }
        }
        #endregion

        #region Configuration
        /// <summary>
        /// Configures common settings for power meter.
        /// </summary>
        /// <param name="sensor">Specifies a reference to the instrument.</param>
        /// <param name="commonConfiguration">Specifies the settings for power meter.</param>
        public static void ConfigureCommon(ni568x sensor, CommonConfiguration commonConfiguration)
        {
            sensor.ConfigureAcquisitionMode(commonConfiguration.Channel_Name, commonConfiguration.Measurement_Mode);
            sensor.ConfigureUnits(commonConfiguration.Units);
            sensor.ConfigureTriggerSource(commonConfiguration.Trigger_Source);

            if (commonConfiguration.Trigger_Source == ni568xConstants.Internal)
            {
                sensor.ConfigureInternalTrigger(commonConfiguration.Channel_Name, commonConfiguration.Slope);
                sensor.ConfigureInternalTriggerLevel(commonConfiguration.Trigger_Level);
            }

            sensor.ConfigureRangeAutoEnabled(commonConfiguration.Channel_Name, true);
            sensor.ConfigureCorrectionFrequency(commonConfiguration.Channel_Name, commonConfiguration.Frequency);
            if (commonConfiguration.Averaging_Auto_Enabled)
            {
                sensor.ConfigureAveragingAutoEnabled(commonConfiguration.Channel_Name, commonConfiguration.Averaging_Auto_Enabled);
            }
            else
            {
                sensor.ConfigureAveragingCount(commonConfiguration.Channel_Name, commonConfiguration.Count);
            }

            sensor.SetDouble(ni568xProperties.ApertureTime, commonConfiguration.ApertureTime);
        }

        /// <summary>
        /// Initiates a measurement, waits until the measurement is complete and NI-568x returns to the Idle state, and then returns the measurement result in the result parameter.
        /// </summary>
        /// <param name="sensor">Specifies a reference to the instrument.</param>
        /// <param name="timeout">Specifies the maximum length of time, in milliseconds, to allow the read measurement operation to complete.</param>
        /// <returns>The measurement results, in dBm.</returns>
        public static double ReadMeasurement(ni568x sensor, int timeout)
        {
            double result = 0.0;
            sensor.Read(timeout, out result);
            return result;
        }
        #endregion
    }
}
