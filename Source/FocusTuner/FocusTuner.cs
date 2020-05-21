using NationalInstruments.ReferenceDesignLibraries.FocusITuner;

namespace NationalInstruments.ReferenceDesignLibraries
{
    /// <summary>Defines common types and methods for configuring a Focus tuner.</summary>
    public static class FocusTuner
    {
        #region Type Definitions
        /// <summary>Defines 2 port S-parameters.</summary>
        public struct SParameters
        {
            /// <summary>Specifies the reflected power ratio from tuner input.</summary>
            public Complex S11;
            /// <summary>Specifies the transferred power ratio from tuner input to output.</summary>
            public Complex S21;
            /// <summary>Specifies the transferred power ratio from tuner output to input.</summary>
            public Complex S12;
            /// <summary>Specifies the reflected power ratio from tuner output.</summary>
            public Complex S22;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static SParameters GetDefault()
            {
                return new SParameters 
                {
                    S11 = new Complex { Real = 0, Imaginary = 0 }, 
                    S21 = new Complex { Real = 1, Imaginary = 0 },
                    S12 = new Complex { Real = 1, Imaginary = 0 },
                    S22 = new Complex { Real = 0, Imaginary = 0 },
                };
            }
        }

        /// <summary>Defines common settings for load pull using the Focus tuner.</summary>
        public struct CommonConfiguration
        {
            /// <summary>Specifies the tuner mode.</summary>
            public TunerMode TunerMode;
            /// <summary>Specifies the calibration ID of the calibration data set to load. See <see cref="FocusITunerBroker.QueryCalibrationList()"/> for a list of valid calibration IDs.</summary>
            public int CalibrationID;
            /// <summary>Specifies the S-parameters of the adapter section between the DUT and the tuner at the primary frequency and up to four secondary frequencies.</summary>
            public SParameters[] DUTtoTunerSParameters;
            /// <summary>Specifies the reflection coefficient of the termination seen by the tuner at load side for the primary frequency and up to four secondary frequencies.</summary>
            public Complex[] LoadTermination;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static CommonConfiguration GetDefault()
            {
                var commonConfiguration = new CommonConfiguration
                {
                    TunerMode = TunerMode.Load,
                    CalibrationID = 1,
                    DUTtoTunerSParameters = new SParameters[] { SParameters.GetDefault() },
                    LoadTermination = new Complex[] { new Complex { Real = 0, Imaginary = 0 } }
                };
                return commonConfiguration;
            }
        }
        #endregion

        #region Configuration
        /// <summary>Reinitializes all the axes of the tuner to its home position before terminating the software connection to the instrument.</summary>
        /// <param name="iTuner">Specifies a reference to the instrument.</param>
        public static void CloseTuner(FocusITunerBroker iTuner)
        {
            iTuner.InitializeTunerAllAxises();
            iTuner.Close();
        }

        /// <summary>Configures common settings for load pull using the Focus tuner.</summary>
        /// <param name="iTuner">Specifies a reference to the instrument.</param>
        /// <param name="commonConfiguration">Specifies the settings for load pull using the Focus tuner.</param>
        /// <returns>The primary frequency in GHz, of the active tuner calibration data set.</returns>
        public static double ConfigureCommon(FocusITunerBroker iTuner, CommonConfiguration commonConfiguration)
        {
            Complex[][] sParameters = new Complex[commonConfiguration.DUTtoTunerSParameters.Length][];
            for (int i = 0; i < sParameters.Length; i++)
            {
                SParameters element = commonConfiguration.DUTtoTunerSParameters[i];
                sParameters[i] = new Complex[] { element.S11, element.S12, element.S21, element.S22 };
            }
            iTuner.ConfigureTunerMode(commonConfiguration.TunerMode);
            iTuner.ConfigureActiveCalibration(commonConfiguration.CalibrationID);
            iTuner.ConfigureAdapter(sParameters);
            iTuner.ConfigureTermination(commonConfiguration.LoadTermination);
            double[] frequencies = iTuner.QueryActiveFrequency();
            return frequencies[0];
        }

        /// <summary>Sets the specified reflection coefficient in the device reference plane at fundamental frequency with timeout.</summary>
        /// <param name="iTuner">Specifies a reference to the instrument.</param>
        /// <param name="gamma">Specifies the desired reflection coefficient in the device reference plane. Valid Range is Real part: [-1, 1] and Imaginary part: [-1, 1].</param>
        /// <param name="timeout">Specifies the maximum length of time, in seconds, to allow the operation to complete. The default is 60.</param>
        /// <returns>The estimated reflection coefficient at the device reference plane for the current tuner position at all frequencies of the active tuner calibration data set.</returns>
        public static Complex[] MoveTunerPerGamma(FocusITunerBroker iTuner, Complex gamma, short timeout = 60)
        {
            iTuner.MoveTunerPerReflectionCoefficient(gamma);
            iTuner.WaitForOperationComplete(timeout);
            return iTuner.QueryReflectionCoefficientAllFrequencies();
        }

        /// <summary>Adjusts the positions of the tuner axes to present the specified voltage standing wave ratio (VSWR) in magnitude and phase in the device reference plane with timeout.</summary>
        /// <param name="iTuner">Specifies a reference to the instrument.</param>
        /// <param name="phaseVswr">Specifies the desired voltage standing wave ratio (VSWR) and phase of the reflection coefficient.</param>
        /// <param name="timeout">Specifies the maximum length of time, in seconds, to allow the operation to complete. The default is 60.</param>
        /// <returns>The estimated voltage standing wave ratio (VSWR) and the associated phase at the device reference plane for the current tuner position at all frequencies of the active tuner calibration data set.</returns>
        public static PhaseVSWR[] MoveTunerPerVSWR(FocusITunerBroker iTuner, PhaseVSWR phaseVswr, short timeout = 60)
        {
            iTuner.MoveTunerPerVSWR(phaseVswr);
            iTuner.WaitForOperationComplete(timeout);
            return iTuner.QueryVSWRAllFrequencies();
        }

        /// <summary>Moves the specified tuner axis to the given positions with timeout.</summary>
        /// <param name="iTuner">Specifies a reference to the instrument.</param>
        /// <param name="motorPositions">Specifies the positions to which one or more motors should move.</param>
        /// <param name="timeout">Specifies the maximum length of time, in seconds, to allow the operation to complete. The default is 60.</param>
        /// <returns>The current position of each tuner motor axis.</returns>
        public static MotorPosition[] MoveTunerPerMotorPosition(FocusITunerBroker iTuner, MotorPosition[] motorPositions, short timeout = 60)
        {
            iTuner.MoveTunerPerMotorPosition(motorPositions);
            iTuner.WaitForOperationComplete(timeout);
            return iTuner.QueryCurrentMotorPositionAll();
        }
        #endregion
    }
}
