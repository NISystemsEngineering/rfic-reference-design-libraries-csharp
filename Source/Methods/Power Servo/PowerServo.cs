using System;
using NationalInstruments.ModularInstruments;
using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.RFmx.InstrMX;

namespace NationalInstruments.ReferenceDesignLibraries.Methods
{
    public static class PowerServo
    {
        #region Type Definitions
        internal static string FGPABitfileName = "NI-RFIC.lvbitx";
        internal static ushort numAverages = 0;
        public struct DutConfiguration
        {
            public double DutDesiredOutputPower_dBm;
            public double DutEstimatedGain_dBm;
            public double DutGainAccuaracy_dB;
            public static DutConfiguration GetDefault()
            {
                return new DutConfiguration
                {
                    DutDesiredOutputPower_dBm = -10,
                    DutEstimatedGain_dBm = 0,
                    DutGainAccuaracy_dB = 3
                };
            }
        }
        //Defines the parameters necessary for configuring the power servo
        public struct ServoConfiguration
        {
            public ushort ServoMaxSteps;
            public ushort ServoMinSteps;
            public double ServoInitialAveragingTime_S;
            public double ServoFinalAveragingTime_S;
            public double ServoActiveServoWindow;
            public double ServoTolerance_dB;
            //public bool ServoContinuousLeveling;
            public static ServoConfiguration GetDefault()
            {
                return new ServoConfiguration
                {
                    ServoMinSteps = 2,
                    ServoMaxSteps = 10,
                    ServoInitialAveragingTime_S = 200e-6,
                    ServoFinalAveragingTime_S = 400e-6,
                    ServoActiveServoWindow = 1e-3,
                    ServoTolerance_dB = 0.05,
                    //ServoContinuousLeveling = false
                };
            }
        }
        public struct CalculatedVSTLevels
        {
            public double VSAReferenceLevel_dBm;
            public double VSGAveragePowerLevel_dBm;
        }
        public struct ServoResults
        {
            public double FinalInputPower_dBm;
            public double FinalOutputPower_dBm;
            public double CalculatedDutGain_dB;
            public double[] ServoTrace;
        }
        #endregion
        /* 
         * Initialize the RFSG and RFmx drivers for the RF generator and the RF analyzer,
         * respectively. Also create a new servo session based on the RFSA handle. 
         */
        public static void InitializeVSTForServo(string VSTResourceName, out NIRfsg niRFSGSession, 
                                                 out RFmxInstrMX niRFmxInstrSession, out niPowerServo powerServoSession)
        {
            niRFSGSession = new NIRfsg(VSTResourceName, false, false, $"DriverSetup=Bitfile:{FGPABitfileName}");
            niRFmxInstrSession = new RFmxInstrMX(VSTResourceName, $"RFmxSetup=Bitfile:{FGPABitfileName}");

            niRFmxInstrSession.DangerousGetNIRfsaHandle(out IntPtr rfsaSession);
            powerServoSession = new niPowerServo(rfsaSession, false);
            powerServoSession.Reset();
        }
        public static CalculatedVSTLevels CalculateVSTLevels(niPowerServo servoSession, DutConfiguration dutConfig, 
                                                             SG.Waveform waveform)
        {
            CalculatedVSTLevels calcLevels;
            servoSession.CalculateServoParams(dutConfig.DutDesiredOutputPower_dBm, dutConfig.DutEstimatedGain_dBm,
                                              dutConfig.DutGainAccuaracy_dB, waveform.PAPR_dB,
                                              out calcLevels.VSAReferenceLevel_dBm, out calcLevels.VSGAveragePowerLevel_dBm);
            return calcLevels;
        }
        public static void ConfigureServo(niPowerServo servoSession, ServoConfiguration servoConfig)
        {
            servoSession.ResetDigitalGainOnFailureEnabled(true);
            servoSession.DigitalGainStepLimitEnabled(false);
            servoSession.FailServoOnDigitalSaturationEnabled(false);
            servoSession.ConfigureVSAReferenceTriggerOverride(ReferenceTriggerOverride.Arm_Ref_Trig_On_Servo_Done);

            servoSession.Enable();

            /* BEGIN COMMENT
            Regardless of whether the waveform has 1 or more bursts, we will instruct the servo that the waveform is bursted
            and specify the first burst (potentially first of multiple) that this burst length is to be used. The rationale
            is as follows:
            1) Setting all waveforms to be bursted and specifying the duration reduces code complexity, while maintaining
               the same result for continuous waveforms
            2) Regardless of whether or not the waveform itself contains a burst, a user could use the code in the SG module
               to configure the generator for bursted generation. Since we cannot be aware of this enabling bursted analysis
               is the safest choice
            3) The other code modules configure triggers for the first sample of waveform. Hence, for waveforms with multiple 
               bursts only the first burst can be analyzed anyway unless an IQ edge trigger was generated, or the script
               was modified to send a trigger on each burst location of the waveform. Since this complex generation scenario
               is not practical, the first burst for all waveforms will be used instead.
            END COMMENT*/

            /* BEGIN COMMENT
             * 
             * bbachman: The user should define the servo schedule which also depends
             * on the use case, instead of having it hard-coded.
             * Using only the bursted setting is ok which forces the servo to work only on trigger events.
             * 
               END COMMENT*/


            servoSession.Setup(servoConfig.ServoTolerance_dB, servoConfig.ServoInitialAveragingTime_S,
                               servoConfig.ServoFinalAveragingTime_S, servoConfig.ServoMinSteps, 
                               servoConfig.ServoMaxSteps, false, 0);
            servoSession.ConfigureActiveServoWindow(true, servoConfig.ServoActiveServoWindow);
        }
        public static void InitiateServo(niPowerServo servoSession, double timeOut_s = 1.0)
        {
            // Initiate the power servo
            servoSession.Start();
            servoSession.Wait(timeOut_s, out numAverages, out bool done, out bool failed);
            if (failed)
            {
                throw new System.OperationCanceledException("FPGA power servo failed to complete. Either the maximum " + 
                                                            "number of iterations were exceeded, the digital gain limit " +
                                                            "was reached, or a timeout occured. Try increasing the maximum " +
                                                            "number of servo steps, decreasing the expected gain, or increasing " +
                                                            "the gain accuracy parameter. The digital gain has been reset to " +
                                                            "the default value.");
            }
        }

        public static ServoResults FetchServoResults(niPowerServo servoSession, NIRfsg rfsgSession)
        {
            ServoResults results = new ServoResults();

            servoSession.GetDigitalGain(out double _, out double servoDigitalGain_dB);
            servoSession.GetServoSteps(numAverages, false, false, 0, out double[] _, out results.ServoTrace);
            // The final input power is calculated by the input power level plus the final servo gain
            results.FinalInputPower_dBm = rfsgSession.RF.PowerLevel + servoDigitalGain_dB;
            results.FinalOutputPower_dBm = results.ServoTrace[results.ServoTrace.Length - 1];
            results.CalculatedDutGain_dB = results.FinalOutputPower_dBm - results.FinalInputPower_dBm;

            return results;
        }

        public static void AbortServo(niPowerServo servoSession)
        {
            // NOTE: Only call after disabling  RF generation to avoid a power step in the PA input 
            // that occurs when the servo IP is disabled. This is due to the digital gain being reset to 1.
            
            servoSession.Disable();
            servoSession.Reset();
        }
    }
}
