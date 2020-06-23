using NationalInstruments.ModularInstruments.NIDCPower;
using System;
using System.Linq;

namespace NationalInstruments.ReferenceDesignLibraries
{
    public static class Supply
    {
        #region Type Definitions
        public struct CustomTransientResponse
        {
            public double GainBandwidth;
            public double CompensationFrequency;
            public double PoleZeroRatio;
            public static CustomTransientResponse GetDefault()
            {
                return new CustomTransientResponse
                {
                    GainBandwidth = 5000,
                    CompensationFrequency = 50000,
                    PoleZeroRatio = 1,
                };
            }
        }
        
        public struct SupplyConfiguration
        {
            public DCPowerSourceOutputFunction OutputFunction;

            public double VoltageLevel_V;
            public double CurrentLimit_A;
            public double VoltageLimit_V;
            public double CurrentLevel_A;

            public DCPowerSourceTransientResponse TransientResponseMode;
            public CustomTransientResponse CustomTransientConfig;
            public static SupplyConfiguration GetDefault()
            {
                return new SupplyConfiguration
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    VoltageLevel_V = 3,
                    CurrentLevel_A = 0.001,
                    VoltageLimit_V = 1,
                    CurrentLimit_A = 0.001,
                    TransientResponseMode = DCPowerSourceTransientResponse.Normal,
                    CustomTransientConfig = CustomTransientResponse.GetDefault()
                };
            }
        }
        
        public enum MeasurementModeConfiguration { SinglePoint, Record };
        public struct MeasurementConfiguration
        {
            public DCPowerMeasurementWhen MeasureWhenMode;
            public DCPowerMeasurementSense SenseMode;
            public string MeasurementTriggerTerminal;
            public MeasurementModeConfiguration MeasurementMode;
            public double MeasurementTime_s;
            public static MeasurementConfiguration GetDefault()
            {
                return new MeasurementConfiguration
                {
                    MeasureWhenMode = DCPowerMeasurementWhen.OnMeasureTrigger,
                    SenseMode = DCPowerMeasurementSense.Local,
                    MeasurementTriggerTerminal = DCPowerDigitalEdgeMeasureTriggerInputTerminal.PxiTriggerLine0.ToString(),
                    MeasurementMode = MeasurementModeConfiguration.SinglePoint,
                    MeasurementTime_s = 1e-3
                };
            }
        }
        
        public enum SupplyPowerMode { PowerOn, PowerOff };
        public struct MeasurementResults
        {
            public double[] Voltage_V;
            public double[] Current_A;
            public double[] Power_W;
        }
        #endregion
        public static void ConfigureSupply(NIDCPower supplyHandle, SupplyConfiguration supplyConfig, string channelNames = "")
        {
            supplyHandle.Source.Mode = DCPowerSourceMode.SinglePoint;

            supplyHandle.Outputs[channelNames].Source.Output.Function = supplyConfig.OutputFunction;
            supplyHandle.Outputs[channelNames].Source.TransientResponse = supplyConfig.TransientResponseMode;

            switch (supplyConfig.OutputFunction)
            {
                case DCPowerSourceOutputFunction.DCVoltage:
                    supplyHandle.Outputs[channelNames].Source.Voltage.CurrentLimitAutorange = DCPowerSourceCurrentLimitAutorange.On;
                    supplyHandle.Outputs[channelNames].Source.Voltage.VoltageLevelAutorange = DCPowerSourceVoltageLevelAutorange.On;

                    supplyHandle.Outputs[channelNames].Source.Voltage.VoltageLevel = supplyConfig.VoltageLevel_V;
                    supplyHandle.Outputs[channelNames].Source.Voltage.CurrentLimit = supplyConfig.CurrentLimit_A;
                    break;
                case DCPowerSourceOutputFunction.DCCurrent:
                    supplyHandle.Outputs[channelNames].Source.Current.CurrentLevelAutorange = DCPowerSourceCurrentLevelAutorange.On;
                    supplyHandle.Outputs[channelNames].Source.Current.VoltageLimitAutorange = DCPowerSourceVoltageLimitAutorange.On;

                    supplyHandle.Outputs[channelNames].Source.Current.CurrentLevel = supplyConfig.CurrentLevel_A;
                    supplyHandle.Outputs[channelNames].Source.Current.VoltageLimit = supplyConfig.VoltageLimit_V;
                    break;
                default:
                    throw new NotImplementedException("Pulse voltage/current is not implemented.");
            }

            if (supplyConfig.TransientResponseMode == DCPowerSourceTransientResponse.Custom)
            {
                switch (supplyConfig.OutputFunction)
                {
                    case DCPowerSourceOutputFunction.DCVoltage:
                        supplyHandle.Outputs[channelNames].Source.CustomTransientResponse.Voltage.CompensationFrequency =
                            supplyConfig.CustomTransientConfig.CompensationFrequency;
                        supplyHandle.Outputs[channelNames].Source.CustomTransientResponse.Voltage.GainBandwidth =
                            supplyConfig.CustomTransientConfig.GainBandwidth;
                        supplyHandle.Outputs[channelNames].Source.CustomTransientResponse.Voltage.PoleZeroRatio =
                           supplyConfig.CustomTransientConfig.PoleZeroRatio;
                        break;
                    case DCPowerSourceOutputFunction.DCCurrent:
                        supplyHandle.Outputs[channelNames].Source.CustomTransientResponse.Current.CompensationFrequency =
                            supplyConfig.CustomTransientConfig.CompensationFrequency;
                        supplyHandle.Outputs[channelNames].Source.CustomTransientResponse.Current.GainBandwidth =
                            supplyConfig.CustomTransientConfig.GainBandwidth;
                        supplyHandle.Outputs[channelNames].Source.CustomTransientResponse.Current.PoleZeroRatio =
                           supplyConfig.CustomTransientConfig.PoleZeroRatio;
                        break;
                }
            }

        }
        public static void ConfigureMeasurement(NIDCPower supplyHandle, MeasurementConfiguration measConfig, string channelNames = "")
        {
            //On demand mode does not allow for multiple records to be acquired, so we need to validate the configuration given to this function. 

            if (measConfig.MeasureWhenMode == DCPowerMeasurementWhen.OnDemand &&
                measConfig.MeasurementMode == MeasurementModeConfiguration.Record)
            {
                throw new ArgumentException("On Demand measurements can only be configured for a single measurement mode",
                    "MeasurementMode, MeasureWhenMode");
            }

            supplyHandle.Measurement.Configuration.MeasureWhen = measConfig.MeasureWhenMode;
            supplyHandle.Measurement.Configuration.IsRecordLengthFinite = true;

            if (measConfig.MeasureWhenMode == DCPowerMeasurementWhen.OnMeasureTrigger)
            {
                supplyHandle.Triggers.MeasureTrigger.DigitalEdge.Configure(
                    DCPowerDigitalEdgeMeasureTriggerInputTerminal.FromString(measConfig.MeasurementTriggerTerminal), DCPowerTriggerEdge.Rising);
            }

            supplyHandle.Outputs[channelNames].Measurement.Sense = measConfig.SenseMode;

            int recordLength;
            double apertureTime;

            /*Single Point: Acquire a single measurement averaged over the duration of the Measurement Time
              Record: Acquire samples at the maximum sampling rate of the supply for the total duration of Measurement Time. */
            switch (measConfig.MeasurementMode)
            {
                case MeasurementModeConfiguration.Record:
                    //Set the aperture time to the minimum value and read it back. This sets the "sample rate". 
                    //Then, we calculate how many records we need to acquire at that sample rate to get the requested measurement time.
                    supplyHandle.Outputs[channelNames].Measurement.ApertureTime = 0;
                    double minApertureTime = supplyHandle.Outputs[channelNames].Measurement.ApertureTime; //dt (Seconds per Sample)
                    recordLength = (int)Math.Ceiling(measConfig.MeasurementTime_s / minApertureTime) + 1; // (Time_s)/(dt S/s) = #of samples
                    apertureTime = minApertureTime;
                    break;
                case MeasurementModeConfiguration.SinglePoint:
                default:
                    //Acquire a single record that is the average measurement over Measurement Time 
                    apertureTime = measConfig.MeasurementTime_s;
                    recordLength = 1;
                    break;
            }

            supplyHandle.Outputs[channelNames].Measurement.ApertureTimeUnits = DCPowerMeasureApertureTimeUnits.Seconds;
            supplyHandle.Outputs[channelNames].Measurement.ApertureTime = apertureTime;
            supplyHandle.Outputs[channelNames].Measurement.RecordLength = recordLength;
        }
        public static void TurnSupplyOnOrOff(NIDCPower supplyHandle, SupplyPowerMode powerMode, string channelNames = "")
        {
            switch (powerMode)
            {
                case SupplyPowerMode.PowerOn:
                    try
                    {
                        supplyHandle.Outputs[channelNames].Source.Output.Enabled = true;
                        //Initiate sourcing and wait until the output settles
                        supplyHandle.Control.Initiate();
                        supplyHandle.Events.SourceCompleteEvent.WaitForEvent(new PrecisionTimeSpan(1));
                        supplyHandle.Control.Abort();
                        //Despite the fact that we have aborted, sourcing continues until output is disabled.
                        //We abort because we do not wish to make measurements until requested.
                    }
                    catch (Exception e)
                    {
                        //Safety; disable output on any error
                        supplyHandle.Outputs[channelNames].Source.Output.Enabled = false;
                        supplyHandle.Control.Abort();
                        throw e;
                    }
                    break;
                case SupplyPowerMode.PowerOff:
                    //To disable output, hardware needs to be in the running state.
                    //Initiate, disable, then abort.
                    supplyHandle.Control.Initiate();
                    supplyHandle.Outputs[channelNames].Source.Output.Enabled = false;
                    supplyHandle.Control.Abort();
                    break;
            }
        }
        public static MeasurementResults MeasureSupplyIV(NIDCPower supplyHandle, string channelNames = "")
        {
            MeasurementResults results;

            DCPowerMeasurementWhen measureMode = supplyHandle.Measurement.Configuration.MeasureWhen;
            int samplesToFetch = supplyHandle.Outputs[channelNames].Measurement.RecordLength;

            supplyHandle.Control.Initiate();
            switch (measureMode)
            {
                case DCPowerMeasurementWhen.OnMeasureTrigger:
                case DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete:
                    DCPowerFetchResult fetchResult = supplyHandle.Measurement.Fetch(channelNames, new PrecisionTimeSpan(1), samplesToFetch);
                    results.Voltage_V = fetchResult.VoltageMeasurements;
                    results.Current_A = fetchResult.CurrentMeasurements;
                    break;
                case DCPowerMeasurementWhen.OnDemand:
                default:
                    DCPowerMeasureResult measureResult = supplyHandle.Measurement.Measure(channelNames);
                    results.Voltage_V = measureResult.VoltageMeasurements;
                    results.Current_A = measureResult.CurrentMeasurements;
                    break;
            }
            supplyHandle.Control.Abort();
            //Calculate power
            results.Power_W = new double[results.Voltage_V.Length];

            for (int i = 0; i < results.Voltage_V.Length; i++)
            {
                results.Power_W[i] = results.Voltage_V[i] * results.Current_A[i];
            }
            return results;
        }
        public static void CloseSupply(NIDCPower supplyHandle)
        {
            //Quickly ensures all outputs are disabled and resets the device
            supplyHandle.Utility.Disable();
            supplyHandle.Close();
        }
        public static class Utilities
        {
            public static MeasurementResults CalculateAverageIV(MeasurementResults measuredResults, int offsetMeasurement = 0)
            {
                MeasurementResults calculatedResults = new MeasurementResults
                {
                    Voltage_V = new double[1],
                    Current_A = new double[1],
                    Power_W = new double[1]
                };

                if (offsetMeasurement > 0 && measuredResults.Voltage_V.Length > 1)
                {
                    int length = measuredResults.Voltage_V.Length - offsetMeasurement;
                    double[] offsetVoltage = new double[length], offsetCurrent = new double[length];

                    Array.Copy(measuredResults.Voltage_V, offsetMeasurement, offsetVoltage, 0, length);
                    Array.Copy(measuredResults.Current_A, offsetMeasurement, offsetCurrent, 0, length);

                    calculatedResults.Voltage_V[0] = offsetVoltage.Average();
                    calculatedResults.Current_A[0] = offsetCurrent.Average();
                }
                else
                {
                    calculatedResults.Voltage_V[0] = measuredResults.Voltage_V.Average();
                    calculatedResults.Current_A[0] = measuredResults.Current_A.Average();
                }
                calculatedResults.Power_W[0] = measuredResults.Current_A[0] * measuredResults.Voltage_V[0];
                return calculatedResults;
            }
        }
    }
    
}