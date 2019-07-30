using NationalInstruments.ModularInstruments.NIDCPower;
using System;
using static NationalInstruments.ReferenceDesignLibraries.Supply;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class SupplyExample
    {
        static void Main()
        {
            string channelNames = "0";
            NIDCPower dcPower = new NIDCPower("4139", channelNames, false);

            // Configure instrument settings
            SupplyConfiguration supplyConfig = SupplyConfiguration.GetDefault();

            supplyConfig.OutputFunction = DCPowerSourceOutputFunction.DCVoltage;
            supplyConfig.VoltageLevel_V = 3;
            supplyConfig.CurrentLevel_A = 1;

            ConfigureSupply(dcPower, supplyConfig, channelNames);

            // Configure measurement related parameters
            MeasurementConfiguration measConfig = new MeasurementConfiguration
            {
                MeasureWhenMode = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete,
                SenseMode = DCPowerMeasurementSense.Remote,
                // A MeasurementMode of "Record" acquires multiple smaples over the requested measurement 
                // time at the supply's maximum sampling rate. "Single Point" will take a single measurement
                // over that duration and average the power and current results.
                MeasurementMode = MeasurementModeConfiguration.Record,
                MeasurementTime_s = 2e-3,
                MeasurementTriggerTerminal = "PXI_Trig0"
            };

            ConfigureMeasurement(dcPower, measConfig, channelNames);

            TurnSupplyOnOrOff(dcPower, SupplyPowerMode.PowerOn, channelNames);

            MeasurementResults results = MeasureSupplyIV(dcPower, channelNames);

            // Calculate the average of the acquired results
            results = Utilities.CalculateAverageIV(results);

            Console.WriteLine($"The average voltage measured was {results.Voltage_V[0]} with a current of {results.Current_A[0]}");
            Console.WriteLine("Press any key to turn off the supply and exit the program.");

            Console.ReadKey();

            TurnSupplyOnOrOff(dcPower, SupplyPowerMode.PowerOff, channelNames);

            CloseSupply(dcPower, channelNames);
        }
    }
}
