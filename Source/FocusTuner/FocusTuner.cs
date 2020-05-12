using NationalInstruments.ReferenceDesignLibraries.FocusITuner;
using System.Collections.Generic;

namespace NationalInstruments.ReferenceDesignLibraries
{
    /// <summary>Defines common types and methods for configuring a Focus tuner.</summary>
    public static class FocusTuner
    {
        #region Type Definitions
        /// <summary>Defines 2 ports S-parameter.</summary>
        public struct SParameter
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
            public static SParameter GetDefault()
            {
                return new SParameter 
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
            /// <summary>Specifies the calibration ID of the calibration data set to load. Use the Query Calibration List VI to retrieve a list of valid calibration IDs.</summary>
            public int CalibrationID;
            /// <summary>Specifies the S-parameters of the adapter section between the DUT and the tuner at the primary frequency and up to four secondary frequencies.</summary>
            public SParameter[] DUTtoTunerSParameters;
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
                    DUTtoTunerSParameters = new SParameter[1],
                    LoadTermination = new Complex[1]
                };
                commonConfiguration.DUTtoTunerSParameters[0] = SParameter.GetDefault();
                commonConfiguration.LoadTermination[0] = new Complex { Real = 0, Imaginary = 0 };

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
        public static double ConfigCommon(FocusITunerBroker iTuner, CommonConfiguration commonConfiguration)
        {
            List<Complex[]> sParameters = new List<Complex[]>();
            foreach (var element in commonConfiguration.DUTtoTunerSParameters)
            {
                Complex[] sParameter = new Complex[4];
                //The order in the Focus driver is S11, S12, S21 and S22.
                sParameter[0] = element.S11;
                sParameter[1] = element.S12;
                sParameter[2] = element.S21;
                sParameter[3] = element.S22;
                sParameters.Add(sParameter);
            }
            iTuner.ConfigureTunerMode(commonConfiguration.TunerMode);
            iTuner.ConfigureActiveCalibration(commonConfiguration.CalibrationID);
            iTuner.ConfigureAdapter(sParameters.ToArray());
            iTuner.ConfigureTermination(commonConfiguration.LoadTermination);
            double[] frequencies = iTuner.QueryActiveFrequency();
            return frequencies[0];
        }

        /// <summary>Sets the specified reflection coefficient in the device reference plane at fundamental frequency with timeout.</summary>
        /// <param name="iTuner">Specifies a reference to the instrument.</param>
        /// <param name="gamma">Specifies the desired reflection coefficient in the device reference plane. Valid Range is Real part: [-1, 1] and Imaginary part: [-1, 1].</param>
        /// <param name="timeout">specifies the maximum length of time, in seconds, to allow the operation to complete. The default is 60.</param>
        /// <returns>returns the estimated reflection coefficient at the device reference plane for the current tuner position at all frequencies of the active tuner calibration data set.</returns>
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
        /// <returns>the estimated voltage standing wave ratio (VSWR) and the associated phase at the device reference plane for the current tuner position at all frequencies of the active tuner calibration data set.</returns>
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
        public static MotorPosition[] MoveTunerMotorPosition(FocusITunerBroker iTuner, MotorPosition[] motorPositions, short timeout = 60)
        {
            iTuner.MoveTunerPerMotorPosition(motorPositions);
            iTuner.WaitForOperationComplete(timeout);
            return iTuner.QueryCurrentMotorPositionAll();
        }
        #endregion
    }
}
