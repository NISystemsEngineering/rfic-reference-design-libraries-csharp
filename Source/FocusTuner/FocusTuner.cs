using NationalInstruments.ReferenceDesignLibraries.FocusITuner;
using System;
using System.Collections.Generic;

namespace NationalInstruments.ReferenceDesignLibraries
{
    public static class FocusTuner
    {
        #region Type Definitions
        public struct CommonConfiguration
        {
            public TunerMode TunerMode;
            public int CalibrationID;
            public Complex[][] DUTtoTunerSParameters;
            public Complex[] LoadTermination;

            public static CommonConfiguration GetDefault()
            {
                var commonConfiguration = new CommonConfiguration();

                commonConfiguration.TunerMode = TunerMode.Load;
                commonConfiguration.CalibrationID = 1;

                // Ideal S-Parameters with S11 = 0 + 0i, S12 = 1 + 0i, S21 = 1 + 0i, S22 = 0 + 0i
                commonConfiguration.DUTtoTunerSParameters = new Complex[1][];
                commonConfiguration.DUTtoTunerSParameters[0] = new Complex[4];
                commonConfiguration.DUTtoTunerSParameters[0][0] = new Complex(0.0, 0.0);
                commonConfiguration.DUTtoTunerSParameters[0][1] = new Complex(1.0, 0.0);
                commonConfiguration.DUTtoTunerSParameters[0][2] = new Complex(1.0, 0.0);
                commonConfiguration.DUTtoTunerSParameters[0][3] = new Complex(0.0, 0.0);

                // Ideal Termination 0 + 0i
                commonConfiguration.LoadTermination = new Complex[1];
                commonConfiguration.LoadTermination[0] = new Complex(0.0, 0.0);

                return commonConfiguration;
            }
        }
        #endregion

        #region Configuration
        public static void CloseTuner(FocusITunerBroker iTuner)
        {
            iTuner.InitializeTunerAllAxises();
            iTuner.Close();
        }

        public static double ConfigCommon(FocusITunerBroker iTuner, CommonConfiguration commonConfiguration)
        {
            iTuner.ConfigureActiveCalibration(commonConfiguration.CalibrationID);
            iTuner.ConfigureAdapter(commonConfiguration.DUTtoTunerSParameters);
            iTuner.ConfigureTermination(commonConfiguration.LoadTermination);
            List<double> frequencies = iTuner.QueryActiveFrequency();
            return frequencies[0];
        }

        public static List<Complex> MoveTunerPerGamma(FocusITunerBroker iTuner, Complex gamma, short timeout = 60)
        {
            iTuner.MoveTunerPerReflectionCoefficient(gamma);
            iTuner.WaitForOperationComplete(timeout);
            return iTuner.QueryReflectionCoefficientAllFrequencies();
        }

        public static List<PhaseVSWR> MoveTunerPerVSWR(FocusITunerBroker iTuner, PhaseVSWR vswr, short timeout = 60)
        {
            iTuner.MoveTunerPerVSWR(vswr);
            iTuner.WaitForOperationComplete(timeout);
            return iTuner.QueryVSWRAllFrequencies();
        }

        public static List<MotorPosition> MoveTunerMotorPosition(FocusITunerBroker iTuner, MotorPosition[] motorPositions, short timeout = 60)
        {
            iTuner.MoveTunerPerMotorPosition(motorPositions);
            iTuner.WaitForOperationComplete(timeout);
            return iTuner.QueryCurrentMotorPositionAll();
        }
        #endregion
    }
}
