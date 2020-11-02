using NationalInstruments.ModularInstruments.NIDCPower;
using System;
using static NationalInstruments.ReferenceDesignLibraries.Supply;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class SupplyExample
    {
        /// <summary>
        /// This example illustrates how to use the NI-DCPower APIs to configure the SMU and measure the results.
        /// </summary>
        static void Main()
        {
            string channelNames = "0";
            NIDCPower dcPower = new NIDCPower("4139", channelNames, false);

            // Configure instrument settings
            SupplyConfiguration supplyConfig = SupplyConfiguration.GetDefault();

            supplyConfig.OutputFunction = DCPowerSourceOutputFunction.DCVoltage;
            supplyConfig.VoltageLevel_V = 3;
            supplyConfig.CurrentLevel_A = 0.001;
            supplyConfig.VoltageLimit_V = 3;
            supplyConfig.CurrentLimit_A = 0.001;

            ConfigureSupply(dcPower, supplyConfig, channelNames);

            // Configure measurement related parameters
            MeasurementConfiguration measConfig = MeasurementConfiguration.GetDefault();
            measConfig.MeasureWhenMode = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete;
            measConfig.MeasurementTime_s = 2e-3;

            ConfigureMeasurement(dcPower, measConfig, channelNames);

            TurnSupplyOnOrOff(dcPower, SupplyPowerMode.PowerOn, channelNames);
            
            MeasurementResults results = MeasureSupplyIV(dcPower, channelNames);
            
            // Calculate the average of the acquired results
            results = Utilities.CalculateAverageIV(results);

            Console.WriteLine($"The average voltage measured was {results.Voltage_V[0]} with a current of {results.Current_A[0]}");
            Console.WriteLine("Press any key to turn off the supply and exit the program.");

            Console.ReadKey();

            TurnSupplyOnOrOff(dcPower, SupplyPowerMode.PowerOff, channelNames);

            CloseSupply(dcPower);
        }
    }
}
