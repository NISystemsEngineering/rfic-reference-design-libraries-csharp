using Microsoft.VisualStudio.TestTools.UnitTesting;
using NationalInstruments.ModularInstruments.NIScope;
using static NationalInstruments.ReferenceDesignLibraries.Scope;

namespace NationalInstruments.ReferenceDesignLibraries.Tests
{
    [TestClass()]
    public class ScopeTests
    {
        [TestMethod()]
        public void ConfigureMeasurementTest()
        {
            NIScope testScope = new NIScope("SIM", false, false, "Simulate = 1");

            MeasurementConfiguration measConfig = MeasurementConfiguration.GetDefault();

            ConfigureMeasurement(testScope, measConfig, "0");

            //Validate basic property sets
            Assert.AreEqual(measConfig.SampleRate_Hz, testScope.Acquisition.SampleRateMin);
            Assert.AreEqual(measConfig.ScopeTriggerSource, testScope.Trigger.Source.ToString());
            Assert.AreEqual(measConfig.TriggerEdge, testScope.Trigger.EdgeTrigger.Slope);
            Assert.AreEqual(measConfig.ScopeTriggerType, testScope.Trigger.Type);

            //Validate that the measurement time is properly calculated by teh code
            Assert.AreEqual(measConfig.MeasurementTime_s, testScope.Acquisition.TimePerRecord.TotalSeconds, 1e-6);

            //Validate that various trigges are setup correctly
            measConfig.ScopeTriggerType = ScopeTriggerType.Immediate;
            ConfigureMeasurement(testScope, measConfig, "0");
            Assert.AreEqual(measConfig.ScopeTriggerType, testScope.Trigger.Type);

            measConfig.ScopeTriggerType = ScopeTriggerType.Software;
            ConfigureMeasurement(testScope, measConfig, "0");
            Assert.AreEqual(measConfig.ScopeTriggerType, testScope.Trigger.Type);

            testScope.Close();
        }

        [TestMethod()]
        public void ConfigureScopeTest()
        {
            NIScope testScope = new NIScope("SIM", false, false, "Simulate = 1");

            ScopeConfiguration scopeConfig = ScopeConfiguration.GetDefault();

            ConfigureScope(testScope, scopeConfig, "0");

            //Check that all properties are properly applied to the scope session
            Assert.AreEqual((double)scopeConfig.InputImpedance, testScope.Channels["0"].InputImpedance,"Impedance");
            Assert.AreEqual(scopeConfig.ScopeCouplingMode, testScope.Channels["0"].Coupling, "Coupling");
            Assert.AreEqual(scopeConfig.VerticalOffset_V, testScope.Channels["0"].Offset, "Offset");
            Assert.AreEqual(scopeConfig.VerticalRange_V, testScope.Channels["0"].Range, "Range");
            Assert.AreEqual(scopeConfig.ProbeAttenuation, testScope.Channels["0"].ProbeAttenuation, "Probe Attenuation");
            Assert.AreEqual(scopeConfig.ScopeClockSource, testScope.Timing.ReferenceClockSource.ToString(),"Clock Source");


            testScope.Close();
        }
    }
}