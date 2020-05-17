using ITUNERXLib;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;

namespace NationalInstruments.ReferenceDesignLibraries.FocusITuner
{
    #region Type Definitions
    public enum TunerMode
    {
        Source = 0,
        Load = 1,
    }

    /// <summary>Defines the single motor axis and position.</summary>
    public struct MotorPosition
    {
        /// <summary>Specifies the motor axis.</summary>
        public short Axis { get; set; }
        /// <summary>Specifies the motor position.</summary>
        public int Position { get; set; }
    }

    /// <summary>Defines the calibration data set stored in tuner memory.</summary>
    public struct Calibration
    {
        /// <summary>Specifies the calibration ID of the calibration data set.</summary>
        public int CalibrationId { get; set; }
        /// <summary>Specifies the primary frequency (in GHz) of the calibration data set.</summary>
        public double Frequency { get; set; }
        /// <summary>Specifies the number of secondary frequencies included in the calibration data set.</summary>
        public int NumberOfSecondaryFrequencies { get; set; }
        /// <summary>Specifies the number of calibrated points in the calibration data set.</summary>
        public int NumberOfCalibrationPoints { get; set; }
    }

    /// <summary>Defines the impedance by the voltage standing wave ratio (VSWR) and phase of the reflection coefficient.</summary>
    public struct PhaseVSWR
    {
        /// <summary>Specifies the voltage standing wave ratio (VSWR) in the device reference plane.</summary>
        public double VSWR { get; set; }
        /// <summary>Specifies the phase, in degrees, of the reflection coefficient in the device reference plane.</summary>
        public double DegreePhase { get; set; }
    }

    /// <summary>Defines a complex with double-precision, floating-point.</summary>
    public struct Complex
    {
        /// <summary>Specifies the real part of the complex.</summary>
        public double Real { get; set; }
        /// <summary>Specifies the imaginary part of the complex.</summary>
        public double Imaginary { get; set; }
    }
    #endregion

    /// <summary>
    /// Wrapper class for iTunerX ActiveX from Focus Mircowaves. It provides similar functions as "Focus iTuner Series" LabVIEW driver.
    /// </summary>
    public class FocusITunerBroker
    {
        #region PublicMethods
        /// <summary>
        /// Creates a new ITunerXClass.
        /// </summary>
        public FocusITunerBroker()
        {
            _tuner = new ITunerXClass();
        }

        /// <summary>
        /// Establishes communication with the instrument and optionally performs an instrument identification query and/or an instrument reset.
        /// It also places the instrument in a default state needed for other instrument driver operations.
        /// Therefore, call this VI before calling other instrument driver VIs for this instrument.
        /// Generally, you need to call the Initialize VI only once at the beginning of an application.
        /// </summary>
        /// <param name="ipAddress">Specify the IP Address of your Tuner.</param>
        /// <param name="reset">Specify whether you want the instrument driver to reset the instrument during the initialization procedure.</param>
        /// <param name="verifyCommunication">Specify whether you want the instrument driver to perform an ID Query.</param>
        public void Initialize(string ipAddress, bool reset, bool verifyCommunication)
        {
            try
            {
                _tuner.Address = ipAddress;

                if (verifyCommunication)
                {
                    WaitForOperationComplete(2);
                }

                if (reset)
                {
                    Reset();
                }
                else
                {
                    DefaultInstrumentSetup();
                    ErrorQuery();
                }
            }
            catch (Exception)
            {
                _tuner.CloseConnection();
                throw new IOException(string.Format("Error {0}: The ID Query failed. This may mean that you selected the wrong instrument or your instrument did not respond.  " +
                    "You may also be using a model that is not officially supported by this driver.  " +
                    "If you are sure that you have selected the correct instrument and it is responding, try disabling the ID Query.", -1074003951));
            }
        }

        /// <summary>
        /// Performs an instrument error query before terminating the software connection to the instrument.
        /// </summary>
        public void Close()
        {
            ErrorQuery();
            _tuner.CloseConnection();
        }

        /// <summary>
        /// Initializes all axes of the tuner to its home position.
        /// Use <see cref="WaitForOperationComplete"/> to wait until the tuner has moved each of its axes to the new position.
        /// Alternatively, use <see cref="QueryTunerStatus"/> in a loop to monitor the repositioning of each tuner axis.
        /// </summary>
        public void InitializeTunerAllAxises()
        {
            _tuner.Send("INIT");
            WaitForOperationComplete(60);
            ErrorQuery();
        }

        /// <summary>
        /// Gets the list of calibration data sets available in the tuner directory.
        /// </summary>
        /// <returns>The list of calibration data.</returns>
        public Calibration[] QueryCalibrationList()
        {
            string acknowledge = null;
            _tuner.SendCmd("DIR", ref acknowledge);
            return ParseCalibrationListString(acknowledge);
        }

        /// <summary>
        /// Loads the tuner calibration data with the specified calibration ID. The adapter and termination data are reset to default.
        /// Use <see cref="WaitForOperationComplete"/> to wait until the tuner has moved each of its axes to the new position.
        /// Alternatively, use <see cref="QueryTunerStatus"/> in a loop to monitor the repositioning of each tuner axis.
        /// </summary>
        /// <param name="calibrationID">
        /// Specifies the calibration ID of the calibration data set to be loaded.
        /// Use <see cref="QueryCalibrationList"/> to retrieve the list of valid calibration IDs.
        /// </param>
        public void ConfigureActiveCalibration(int calibrationID)
        {
            string cmd = "LOADCAL " + calibrationID.ToString();
            _tuner.Send(cmd);
            ErrorQuery();
        }

        /// <summary>
        /// Specifies the reflection coefficient of the termination seen by the tuner at LOAD side.
        /// </summary>
        /// <param name="terminations">
        /// Specifies the reflection coefficient of the termination seen by the tuner at LOAD side for the primary frequency and up to 4 secondary frequencies.
        /// </param>
        public void ConfigureTermination(Complex[] terminations)
        {
            int termIndex = 1;
            foreach(var termimation in terminations)
            {
                double magnitude = Math.Sqrt(Math.Pow(termimation.Real, 2) + Math.Pow(termimation.Imaginary, 2));
                double phase = (Math.Atan2(termimation.Imaginary, termimation.Real) / Math.PI) * 180;
                string sendCmd = "TERM" + " " + termIndex.ToString() + " " + magnitude.ToString("0.000000") + " " + phase.ToString("0.000000");
                _tuner.Send(sendCmd);
                ErrorQuery();
                termIndex++;
            }
        }

        /// <summary>
        /// Configures common settings for load pull using the Focus tuner.
        /// </summary>
        /// <param name="sParameters">
        /// Specifies the S-parameters of the adapter section between the DUT and the tuner at the primary frequency and up to 4 secondary frequencies.
        /// S-parameters order per first dimension is (S11, S12, S21, S22).
        /// </param>
        public void ConfigureAdapter(Complex[][] sParameters)
        {
            int sIndex = 1;
            foreach(var sParameter in sParameters)
            {
                string sendCmd = "ADAPTER" + " " + sIndex.ToString();
                foreach(var single in sParameter)
                {
                    double magnitude = Math.Sqrt(Math.Pow(single.Real, 2) + Math.Pow(single.Imaginary, 2));
                    double phase = (Math.Atan2(single.Imaginary, single.Real) / Math.PI) * 180;
                    sendCmd = sendCmd + " " + magnitude.ToString("0.000000") + " " + phase.ToString("0.000000");
                }
                _tuner.Send(sendCmd);
                ErrorQuery();
                sIndex++;
            }
        }

        /// <summary>
        /// Gets the primary frequency and all secondary frequencies (in GHz) of the currently active calibration data set. 
        /// Must be preceeded by <see cref="ConfigureActiveCalibration"/>.
        /// </summary>
        /// <returns>
        /// The list of all frequencies (including primary frequency and all secondary frequencies) of the active tuner calibration data set.
        /// </returns>
        public double[] QueryActiveFrequency()
        {
            string acknowledge = null;
            _tuner.SendCmd("FREQ?", ref acknowledge);
            ErrorQuery();
            return ParseFrequencyString(acknowledge);
        }

        /// <summary>
        /// Specifies the tuner mode to determine the tuning reference plane of the tuner and the loss calculation definition.
        /// For Tuning reference plane:
        /// SOURCE: port 1 is at LOAD side, port 2 is at DUT side.
        /// LOAD: port 1 is at DUT side, port 2 is at LOAD side.
        /// For Loss calculation definition:
        /// SOURCE: loss calculated using AVAILABLE GAIN definition.
        /// LOAD: loss calculated using POWER GAIN definition.
        /// </summary>
        /// <param name="mode">Specifies the tuner mode.</param>
        public void ConfigureTunerMode(TunerMode mode)
        {
            if (mode == TunerMode.Load)
            {
                _tuner.Send("MODE Load");
            }
            else if (mode == TunerMode.Source)
            {
                _tuner.Send("MODE Source");
            }

            ErrorQuery();
        }

        /// <summary>
        /// Query the current configured tuner mode.
        /// </summary>
        /// <returns>The configured tuner mode</returns>
        public TunerMode QueryTunerMode()
        {
            string acknowledge = null;
            _tuner.SendCmd("CONFIG?", ref acknowledge);
            ErrorQuery();
            return ParseTunerModeString(acknowledge);
        }

        /// <summary>
        /// Sets the specified reflection coefficient in the device reference plane at fundamental frequency.
        /// The tuner calibration data, the adapter de-embedding data and the termination data are used to determine the positions.
        /// Valid Models: CCMT, CCMT-2C, CCMT-2C-2H, PMT.
        /// Use <see cref="WaitForOperationComplete"/> to wait until the tuner has moved each of its axes to the new position.
        /// Alternatively, use <see cref="QueryTunerStatus"/> in a loop to monitor the repositioning of each tuner axis.
        /// Use <see cref="QueryReflectionCoefficientAllFrequencies"/> to read back the reflection coefficient synthesized by the tuner in device reference plane.
        /// </summary>
        /// <param name="reflectionCoefficient">Specifies the desired reflection coefficient in the device reference plane.
        /// Valid Range for Real part: -1 to 1; for Imaginary part: -1 to 1.
        /// </param>
        public void MoveTunerPerReflectionCoefficient(Complex reflectionCoefficient)
        {
            double magnitude = Math.Sqrt(Math.Pow(reflectionCoefficient.Real, 2) + Math.Pow(reflectionCoefficient.Imaginary, 2));
            double phase = (Math.Atan2(reflectionCoefficient.Imaginary, reflectionCoefficient.Real) / Math.PI) * 180;
            _tuner.SetGamma(magnitude, phase);
            ErrorQuery();
        }

        /// <summary>
        /// Gets the estimated reflection coefficient at the device reference plane for the current tuner position, at all frequencies of the active tuner calibration data set.
        /// The calibration data, the adapter de-embedding data and the termination data are used to determine the reflection coefficient.
        /// </summary>
        /// <returns>
        /// The estimated voltage standing wave ratio and the associated phase at the device reference plane for the current tuner position, at all frequencies of the active tuner calibration data set.
        /// The tuner calibration data, the adapter de-embedding data and the termination data are used to determine the voltage standing wave ratio and the associated phase.
        /// </returns>
        public Complex[] QueryReflectionCoefficientAllFrequencies()
        {
            string acknowledge = null;
            _tuner.SendCmd("GAMMA? 0", ref acknowledge);
            ErrorQuery();
            return ParseGammasString(acknowledge);
        }

        /// <summary>
        /// Adjusts positions of the tuner axes to present the specified voltage standing wave ratio (VSWR) in magnitude and phase in the device reference plane.
        /// The tuner calibration data, the adapter de-embedding data and the termination data are used to determine the positions.
        /// Use <see cref="WaitForOperationComplete"/> to wait until the tuner has moved each of its axes to the new position.
        /// Alternatively, use <see cref="QueryTunerStatus"/> in a loop to monitor the repositioning of each tuner axis.
        /// </summary>
        /// <param name="phaseVSWR">
        /// Specifies the desired voltage standing wave ratio and desired phase of the reflection coefficient in the device reference plane.</param>
        public void MoveTunerPerVSWR(PhaseVSWR phaseVSWR)
        {
            _tuner.SetVSWR(phaseVSWR.VSWR, phaseVSWR.DegreePhase);
            ErrorQuery();
        }

        /// <summary>
        /// Gets the estimated voltage standing wave ratio and associated phase at the device reference plane for the current tuner position, at all frequencies of the active tuner calibration data set.
        /// The calibration data, the adapter de-embedding data and the termination data are used to determine the voltage standing wave ratio and associated phase.
        /// </summary>
        /// <returns>
        /// The estimated voltage standing wave ratio and the associated phase at the device reference plane for the current tuner position, at all frequencies of the active tuner calibration data set.
        /// The tuner calibration data, the adapter de-embedding data and the termination data are used to determine the voltage standing wave ratio and the associated phase.
        /// </returns>
        public PhaseVSWR[] QueryVSWRAllFrequencies()
        {
            string acknowledge = null;
            _tuner.SendCmd("VSWR? 0", ref acknowledge);
            ErrorQuery();
            return ParseVSWRString(acknowledge);
        }

        /// <summary>
        /// Moves the specified tuner axes to the given positions.
        /// Use <see cref="WaitForOperationComplete"/> to wait until the tuner has moved each of its axes to the new position.
        /// Alternatively, use <see cref="QueryTunerStatus"/> in a loop to monitor the repositioning of each tuner axis.
        /// </summary>
        /// <param name="positions">Specifies the positions to which one or more motors should be moved.</param>
        public void MoveTunerPerMotorPosition(MotorPosition[] positions)
        {
            string sendCmd = "POS";
            foreach (MotorPosition pos in positions)
            {
                sendCmd = sendCmd + " " + pos.Axis.ToString() + " " + pos.Position.ToString();
            }
            _tuner.Send(sendCmd);
            ErrorQuery();
        }

        /// <summary>
        /// Gets the current position of all tuner axes.
        /// </summary>
        /// <returns>The current position of each tuner motor axis.</returns>
        public MotorPosition[] QueryCurrentMotorPositionAll()
        {
            string acknowledge = null;
            _tuner.SendCmd("POS? 0", ref acknowledge);
            ErrorQuery();
            return ParsePositionString(acknowledge);
        }

        /// <summary>
        /// Immediately terminates all tuner movements.
        /// </summary>
        public void Abort()
        {
            _tuner.Send("STOP");
            ErrorQuery();
        }

        /// <summary>
        /// Gets the tuner status.
        /// </summary>
        /// <returns>
        /// A boolean indicating whether the tuner is busy or not.
        /// A bit masked status value, the corresponding bit is set if the axis is moving.
        /// Bit 0: Axis 1; Bit 1: Axis 2; Bit 2: Axis 3; Bit 3: Axis 4; Bit 4: Axis 5; Bit 5: Axis 6; Bit 6: Axis 7; Bit 7: Axis 8
        /// </returns>
        public Tuple<bool, short> QueryTunerStatus()
        {
            short status = _tuner.GetStatus();
            bool busy = status != 0;
            ErrorQuery();
            return new Tuple<bool, short>(busy, status);
        }

        /// <summary>
        /// Waits until the tuner has completed its current tasks or until the specified timeout has elapsed.
        /// </summary>
        /// <param name="timeout">The maximum length of time, in seconds, in which to allow the operation to complete. Valid Range: 0 - 60s.</param>
        public void WaitForOperationComplete(short timeout)
        {
            short opc = _tuner.OPC(timeout);
            if (opc < 0)
            {
                throw new TimeoutException(string.Format("Error {0}: Timeout Error", -1073807339));
            }
        }

        /// <summary>
        /// Queries the instrument for any errors in the instrument's error queue. This function is called automatically by most driver VIs so it is not usually necessary to call this in an application.
        /// </summary>
        public void ErrorQuery()
        {
            string lastTunerReply = _tuner.GetLastReply();

            if (lastTunerReply == null || lastTunerReply == "")
            {
                return;
            }

            string[] replyLines = lastTunerReply.Split('\n');
            int errorLineIndex = 0;
            int errorCode = 0;

            foreach (string line in replyLines)
            {
                errorLineIndex++;
                if (line.StartsWith("Result="))
                {
                    string pattern = @"[\+-]?\d+";
                    Match m = Regex.Match(line, pattern);
                    int error = Convert.ToInt32(m.Value);
                    if (error < 0)
                    {
                        errorCode = error;
                        break;
                    }
                }
            }

            string errorMsg = "";
            if (errorCode < 0)
            {
                for (int i = 0; i < errorLineIndex; i++)
                {
                    errorMsg = errorMsg + replyLines[i] + "\n";
                }

                IOException customTunerException = new IOException(string.Format("Error {0}: Instrument reports error code: {1}, error message: {2}", -1074000000, errorCode, errorMsg));
                throw (customTunerException);
            }
        }
        #endregion PublicMethods

        #region PrivateMethods
        private TunerMode ParseTunerModeString(string info)
        {
            try
            {
                string[] returnLines = info.Split('\n');
                string modeLine = null;
                foreach (string line in returnLines)
                {
                    if (line.StartsWith("MODE:"))
                    {
                        modeLine = line;
                        break;
                    }
                }

                if (modeLine.Contains("LOAD"))
                {
                    return TunerMode.Load;
                }
                else if (modeLine.Contains("SOURCE"))
                {
                    return TunerMode.Source;
                }
                else
                {
                    throw new ArgumentException("Unable to parse tuner mode from the command return value.");
                }
            }
            catch (Exception ex) when (ex is IndexOutOfRangeException || ex is FormatException)
            {
                ArgumentException argEx = new ArgumentException("Unable to parse tuner mode from the command return value.", ex);
                throw argEx;
            }
        }

        private double[] ParseFrequencyString(string info)
        {
            try
            {
                List<double> frequencies = new List<double>();
                if (info != null)
                {
                    string[] returnLines = info.Split('\n');
                    foreach (string line in returnLines)
                    {
                        string[] items = Regex.Split(line, @"\s+");
                        if (items[0].StartsWith("#"))
                        {
                            double frequency = Convert.ToDouble(items[1].TrimEnd("MHz".ToCharArray())) / 1000;
                            frequencies.Add(frequency);
                        }
                    }
                }
                return frequencies.ToArray();
            }
            catch (Exception ex) when (ex is IndexOutOfRangeException || ex is FormatException)
            {
                ArgumentException argEx = new ArgumentException("Unable to parse frequencies from the command return value.", ex);
                throw argEx;
            }
        }

        private Calibration[] ParseCalibrationListString(string info)
        {
            try
            {
                List<Calibration> calibrationList = new List<Calibration>();
                if (info != null)
                {
                    string[] returnLines = info.Split('\n');
                    List<int> calibrationId = new List<int>();
                    int calId, index;

                    foreach (string line in returnLines)
                    {
                        if (line.StartsWith("#"))
                        {
                            string[] items = Regex.Split(line, @"\s+");
                            if (items[3] == "0")  //Is standard calibration type
                            {
                                calId = Convert.ToInt32(items[1]);
                                index = calibrationId.IndexOf(calId);
                                if (index < 0)
                                {
                                    calibrationId.Add(calId);
                                    Calibration calibration = new Calibration
                                    {
                                        CalibrationId = calId,
                                        Frequency = Convert.ToDouble(items[2]),
                                        NumberOfSecondaryFrequencies = 1,
                                        NumberOfCalibrationPoints = Convert.ToInt32(items[5])
                                    };
                                    calibrationList.Add(calibration);
                                }
                                else
                                {
                                    Calibration calibration = calibrationList.ElementAt(index);
                                    calibration.NumberOfSecondaryFrequencies += 1;
                                }
                            }
                        }
                    }
                }
                return calibrationList.ToArray();
            }
            catch (Exception ex) when (ex is IndexOutOfRangeException || ex is FormatException)
            {
                ArgumentException argEx = new ArgumentException("Unable to parse CalibrationList from the command return value.", ex);
                throw argEx;
            }
        }

        private Complex[] ParseGammasString(string info)
        {
            try
            {
                List<Complex> reflectionCoefficients = new List<Complex>();
                if (info != null)
                {
                    string[] returnLines = info.Split('\n');
                    foreach (string line in returnLines)
                    {
                        string[] items = Regex.Split(line, @"\s+");
                        if (items[0].Contains("MHz"))
                        {
                            double r = Convert.ToDouble(items[2]);
                            double theta = (Convert.ToDouble(items[3]) / 180) * Math.PI;
                            Complex reflectionCoefficient = new Complex
                            {
                                Real = r * Math.Cos(theta),
                                Imaginary = r * Math.Sin(theta)
                            };
                            reflectionCoefficients.Add(reflectionCoefficient);
                        }
                    }
                }
                return reflectionCoefficients.ToArray();
            }
            catch (Exception ex) when (ex is IndexOutOfRangeException || ex is FormatException)
            {
                ArgumentException argEx = new ArgumentException("Unable to parse Gammas from the command return value.", ex);
                throw argEx;
            }
        }

        private PhaseVSWR[] ParseVSWRString(string info)
        {
            try
            {
                List<PhaseVSWR> phaseVSWRList = new List<PhaseVSWR>();

                if (info != null)
                {
                    string[] returnLines = info.Split('\n');
                    foreach (string line in returnLines)
                    {
                        string[] items = Regex.Split(line, @"\s+");
                        if (items[0].Contains("MHz"))
                        {
                            PhaseVSWR phaseVSWR = new PhaseVSWR
                            {
                                VSWR = Convert.ToDouble(items[2]),
                                DegreePhase = Convert.ToDouble(items[3])
                            };
                            phaseVSWRList.Add(phaseVSWR);
                        }
                    }
                }
                return phaseVSWRList.ToArray();
            }
            catch (Exception ex) when (ex is IndexOutOfRangeException || ex is FormatException)
            {
                ArgumentException argEx = new ArgumentException("Unable to parse VSWRs and Phases from the command return value.", ex);
                throw argEx;
            }
        }

        private MotorPosition[] ParsePositionString(string info)
        {
            string[] returnLines = info.Split('\n');
            try
            {
                List<MotorPosition> currentPositions = new List<MotorPosition>();
                if (info != null)
                {
                    foreach (string line in returnLines)
                    {
                        if (line.StartsWith("POS:"))
                        {
                            string[] allPositionInfo = line.Substring(4).Trim().Split(new[] { 'A' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string positionInfo in allPositionInfo)
                            {
                                string[] positions = positionInfo.Trim().Split('=');
                                short axis = Convert.ToInt16(positions[0]);
                                int position = Convert.ToInt32(positions[1]);
                                MotorPosition motorPosition = new MotorPosition 
                                {
                                    Axis = axis, 
                                    Position = position 
                                };
                                currentPositions.Add(motorPosition);
                            }
                            break;
                        }
                    }
                }
                return currentPositions.ToArray();
            }
            catch (Exception ex) when (ex is IndexOutOfRangeException || ex is FormatException)
            {
                ArgumentException argEx = new ArgumentException("Unable to parse Positions from the command return value.", ex);
                throw argEx;
            }
        }

        private void DefaultInstrumentSetup()
        {
            _tuner.Send("ECHO OFF");
            _tuner.ShowError = 0;
            _tuner.GetStatus();
        }

        private void Reset()
        {
            _tuner.Send("RESET");
            WaitForOperationComplete(60);
            DefaultInstrumentSetup();
            ErrorQuery();
        }

        private ITunerXClass _tuner = null;
        #endregion PrivateMethods
    }
}
