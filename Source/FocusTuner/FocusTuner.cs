using NationalInstruments.ReferenceDesignLibraries.FocusITuner;
using System;
using System.Collections.Generic;

namespace NationalInstruments.ReferenceDesignLibraries
{
    public static class FocusTuner
    {
        #region Type Definitions
        public struct SParameter
        {
            public Complex S11;
            public Complex S21;
            public Complex S12;
            public Complex S22;

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

        public struct CommonConfiguration
        {
            public TunerMode TunerMode;
            public int CalibrationID;
            public SParameter[] DUTtoTunerSParameters;
            public Complex[] LoadTermination;

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
        public static void CloseTuner(FocusITunerBroker iTuner)
        {
            iTuner.InitializeTunerAllAxises();
            iTuner.Close();
        }

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

        public static Complex[] MoveTunerPerGamma(FocusITunerBroker iTuner, Complex gamma, short timeout = 60)
        {
            iTuner.MoveTunerPerReflectionCoefficient(gamma);
            iTuner.WaitForOperationComplete(timeout);
            return iTuner.QueryReflectionCoefficientAllFrequencies();
        }

        public static PhaseVSWR[] MoveTunerPerVSWR(FocusITunerBroker iTuner, PhaseVSWR vswr, short timeout = 60)
        {
            iTuner.MoveTunerPerVSWR(vswr);
            iTuner.WaitForOperationComplete(timeout);
            return iTuner.QueryVSWRAllFrequencies();
        }

        public static MotorPosition[] MoveTunerMotorPosition(FocusITunerBroker iTuner, MotorPosition[] motorPositions, short timeout = 60)
        {
            iTuner.MoveTunerPerMotorPosition(motorPositions);
            iTuner.WaitForOperationComplete(timeout);
            return iTuner.QueryCurrentMotorPositionAll();
        }
        #endregion
    }
}
