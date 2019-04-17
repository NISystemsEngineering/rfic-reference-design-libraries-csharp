using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NationalInstruments.ModularInstruments.NIDCPower;
using static NationalInstruments.ReferenceDesignLibraries.Supply;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class SupplyExample
    {
        static void Main()
        {
            string channelNames = "0";
            NIDCPower dcPower = new NIDCPower("4139", channelNames, false);

            SupplyConfiguration supplyConfig = new SupplyConfiguration();
            supplyConfig.SetDefaults();

            supplyConfig.OutputFunction = DCPowerSourceOutputFunction.DCVoltage;
            supplyConfig.VoltageLevel_V = 3;
            supplyConfig.CurrentLevel_A = 1;
            supplyConfig.TransientResponseMode = DCPowerSourceTransientResponse.Fast;

            ConfigureSupply(dcPower, supplyConfig, channelNames);

            MeasurementConfiguration measConfig = new MeasurementConfiguration
            {
                MeasureWhenMode = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete,
                SenseMode = DCPowerMeasurementSense.Remote,
                MeasurementMode = MeasurementConfiguration.MeasurementModeConfiguration.SinglePoint,
                MeasurementTime_s = 2e-3,
                MeasurementTriggerTerminal = "PXI_Trig0"
            };

            ConfigureMeasurement(dcPower, measConfig, channelNames);

            TurnSupplyOnOrOff(dcPower, SupplyPowerMode.PowerOn, channelNames);

            MeasurementResults results = MeasureSupplyIV(dcPower, channelNames);
            results = Utilities.CalculateAverageIV(results, 3);

            TurnSupplyOnOrOff(dcPower, SupplyPowerMode.PowerOff, channelNames);

            CloseSupply(dcPower, channelNames);
        }
    }
}
