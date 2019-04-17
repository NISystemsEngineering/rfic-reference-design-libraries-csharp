using System;
using NationalInstruments.ModularInstruments.NIScope;
using static NationalInstruments.ReferenceDesignLibraries.Scope;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class ScopeExample
    {
        static void Main()
        {
            NIScope myScope = new NIScope("5154", false, false);

            ScopeConfiguration scopeConfig = GetDefaultScopeConfiguration();
            MeasurementConfiguration measConfig = GetDefaultMeasurementConfiguration();

            ConfigureScope(myScope, scopeConfig, "0");
            ConfigureMeasurement(myScope, measConfig, "0");

            MeasurementResults myResults = MeasureChannel(myScope, "0");
            Console.WriteLine(myResults.AverageValue_V);
            Console.ReadKey();
            myScope.Close();

        }
    }
}
