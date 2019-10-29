using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.ModularInstruments.SystemServices.DeviceServices;
using NationalInstruments.ReferenceDesignLibraries;
using NationalInstruments.ReferenceDesignLibraries.SA;
using NationalInstruments.RFmx.InstrMX;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SG_SA_IntegrationTests
{
    [TestClass]
    public class SG_SA_IntegrationTests
    {
        static Dictionary<string, Tuple<NIRfsg, RFmxInstrMX>> deviceSessions = new Dictionary<string, Tuple<NIRfsg, RFmxInstrMX>>();
        static SG.Waveform lteTdd10Waveform;

        [ClassInitialize]
        public static void TestFixtureSetup(TestContext context)
        {
            var mis = new ModularInstrumentsSystem(typeof(NIRfsg));
            for (int i = 0; i < mis.DeviceCollection.Count; i++)
            {
                var device = mis.DeviceCollection[i];
                switch (device.Model)
                {
                    //case "NI PXIe-5646R":
                    //case "NI PXIe-5820":
                    case "NI PXIe-5840":
                    case "NI PXIe-5841":
                    case "NI PXIe-3621":
                    case "NI PXIe-3622":
                        if (!deviceSessions.ContainsKey(device.Model))
                        {
                            var rfsg = new NIRfsg(mis.DeviceCollection[i].Name, false, true);
                            var instr = new RFmxInstrMX(mis.DeviceCollection[i].Name, "");
                            deviceSessions.Add(device.Model, Tuple.Create(rfsg, instr));
                        }
                        break;
                }
            }

            if (deviceSessions.Count > 0)
                lteTdd10Waveform = SG.LoadWaveformFromTDMS(@"Support Files\LTE_TDD_2.0.tdms");
        }

        // returns true if instrument found
        private bool GetSessions(string model, out NIRfsg rfsg, out RFmxInstrMX instr)
        {
            rfsg = null;
            instr = null;
            if (!deviceSessions.TryGetValue(model, out Tuple<NIRfsg, RFmxInstrMX> instrSessions))
                return false;
            rfsg = instrSessions.Item1;
            instr = instrSessions.Item2;
            return true;
        }

        private string[] GetLOChannelNames(string model)
        {
            string[] loChannelNames;
            switch (model)
            {
                case "NI PXIe-3621":
                    loChannelNames = new string[] { "LO2" };
                    break;
                case "NI PXIe-3622":
                    loChannelNames = new string[] { "LO1", "LO2" };
                    break;
                default:
                    loChannelNames = new string[] { "" };
                    break;
            }
            return loChannelNames;
        }

        private void ConfigureInstrument(NIRfsg rfsg, RFmxInstrMX instr, SG.InstrumentConfiguration sgConfig, RFmxInstr.InstrumentConfiguration instrConfig)
        {
            SG.ConfigureInstrument(rfsg, sgConfig);
            SG.DownloadWaveform(rfsg, lteTdd10Waveform);
            SG.ConfigureContinuousGeneration(rfsg, lteTdd10Waveform);

            RFmxInstr.ConfigureInstrument(instr, instrConfig);
            var lteCommonConfig = RFmxLTE.CommonConfiguration.GetDefault();
            if (Regex.IsMatch(rfsg.Identity.InstrumentModel, "NI PXIe-5831"))
            {
                lteCommonConfig.SelectedPorts = "rf1/port0";
                lteCommonConfig.CenterFrequency_Hz = 22.5e9;
            }
            var lte = instr.GetLteSignalConfiguration();
            RFmxLTE.ConfigureCommon(instr, lte, lteCommonConfig);
            RFmxLTE.ConfigureSignal(lte, RFmxLTE.SignalConfiguration.GetDefault());
            RFmxLTE.ConfigureModAcc(lte, RFmxLTE.ModAccConfiguration.GetDefault());

            lte.Commit("");
            rfsg.Utility.Commit();
        }

        [TestMethod()]
        public void Test5830AutomaticLOSharing()
        {
            Test583xAutomaticLOSharing("NI PXIe-3621");
        }

        [TestMethod()]
        public void Test5831AutomaticLOSharing()
        {
            Test583xAutomaticLOSharing("NI PXIe-3622");
        }

        private void Test583xAutomaticLOSharing(string model)
        {
            if (!GetSessions(model, out NIRfsg rfsg, out RFmxInstrMX instr))
                Assert.Inconclusive();

            SG.InstrumentConfiguration sgConfig = SG.InstrumentConfiguration.GetDefault(rfsg);
            RFmxInstr.InstrumentConfiguration instrConfig = RFmxInstr.InstrumentConfiguration.GetDefault(instr);

            ConfigureInstrument(rfsg, instr, sgConfig, instrConfig);

            using (new AssertionScope())
            {
                instr.GetAutomaticSGSASharedLO("", out RFmxInstrMXAutomaticSGSASharedLO rfmxAutomaticSGSASharedLO);
                rfmxAutomaticSGSASharedLO.Should().Be(RFmxInstrMXAutomaticSGSASharedLO.Enabled);

                foreach (var channelName in GetLOChannelNames(model))
                {
                    instr.GetLOSource(channelName, out string rfmxLOSource);
                    instr.GetLOExportEnabled(channelName, out bool rfmxLOExportEnabled);
                    rfmxLOSource.Should().Be(RFmxInstrMXConstants.LOSourceSGSAShared);
                    rfmxLOExportEnabled.Should().BeFalse();

                    rfsg.RF.LocalOscillator[channelName].Source.Should().Be(RfsgLocalOscillatorSource.SGSAShared);
                    rfsg.RF.LocalOscillator[channelName].LOOutEnabled.Should().BeFalse();
                }
            }
        }

        [TestMethod()]
        public void Test5840AutomaticLOSharing()
        {
            Test584xAutomaticLOSharing("NI PXIe-5840");
        }

        [TestMethod()]
        public void Test5841AutomaticLOSharing()
        {
            Test584xAutomaticLOSharing("NI PXIe-5841");
        }

        private void Test584xAutomaticLOSharing(string model)
        {
            if (!GetSessions(model, out NIRfsg rfsg, out RFmxInstrMX instr))
                Assert.Inconclusive();

            SG.InstrumentConfiguration sgConfig = SG.InstrumentConfiguration.GetDefault(rfsg);
            RFmxInstr.InstrumentConfiguration instrConfig = RFmxInstr.InstrumentConfiguration.GetDefault(instr);

            ConfigureInstrument(rfsg, instr, sgConfig, instrConfig);

            using (new AssertionScope())
            {
                instr.GetAutomaticSGSASharedLO("", out RFmxInstrMXAutomaticSGSASharedLO rfmxAutomaticSGSASharedLO);
                instr.GetLOSource("", out string rfmxLOSource);
                instr.GetLOExportEnabled("", out bool rfmxLOExportEnabled);

                rfmxAutomaticSGSASharedLO.Should().Be(RFmxInstrMXAutomaticSGSASharedLO.Enabled);
                rfmxLOSource.Should().Be(RFmxInstrMXConstants.LOSourceLOIn);
                rfmxLOExportEnabled.Should().BeFalse();

                rfsg.RF.LocalOscillator.Source.Should().Be(RfsgLocalOscillatorSource.Onboard);
                rfsg.RF.LocalOscillator.LOOutEnabled.Should().BeFalse();
            }
        }

        [TestMethod()]
        public void Test5830NoLOSharing()
        {
            Test583xNoLOSharing("NI PXIe-3621");
        }

        [TestMethod()]
        public void Test5831NoLOSharing()
        {
            Test583xNoLOSharing("NI PXIe-3622");
        }

        public void Test583xNoLOSharing(string model)
        {
            if (!GetSessions(model, out NIRfsg rfsg, out RFmxInstrMX instr))
                Assert.Inconclusive();

            SG.InstrumentConfiguration sgConfig = SG.InstrumentConfiguration.GetDefault(rfsg);
            RFmxInstr.InstrumentConfiguration instrConfig = RFmxInstr.InstrumentConfiguration.GetDefault(instr);

            sgConfig.LOSharingMode = LocalOscillatorSharingMode.None;
            instrConfig.LOSharingMode = LocalOscillatorSharingMode.None;

            ConfigureInstrument(rfsg, instr, sgConfig, instrConfig);

            using (new AssertionScope())
            {
                instr.GetAutomaticSGSASharedLO("", out RFmxInstrMXAutomaticSGSASharedLO rfmxAutomaticSGSASharedLO);
                rfmxAutomaticSGSASharedLO.Should().Be(RFmxInstrMXAutomaticSGSASharedLO.Disabled);

                foreach (string channelName in GetLOChannelNames(model))
                {
                    instr.GetLOSource(channelName, out string rfmxLOSource);
                    instr.GetLOExportEnabled(channelName, out bool rfmxLOExportEnabled);

                    rfmxLOSource.Should().Be(RFmxInstrMXConstants.LOSourceOnboard);
                    rfmxLOExportEnabled.Should().BeFalse();

                    rfsg.RF.LocalOscillator[channelName].Source.Should().Be(RfsgLocalOscillatorSource.Onboard);
                    rfsg.RF.LocalOscillator[channelName].LOOutEnabled.Should().BeFalse();
                }
            }
        }

        [TestMethod()]
        public void Test5840NoLOSharing()
        {
            Test584xNoLOSharing("NI PXIe-5840");
        }

        [TestMethod()]
        public void Test5841NoLOSharing()
        {
            Test584xNoLOSharing("NI PXIe-5841");
        }

        private void Test584xNoLOSharing(string model)
        {
            if (!GetSessions(model, out NIRfsg rfsg, out RFmxInstrMX instr))
                Assert.Inconclusive();

            SG.InstrumentConfiguration sgConfig = SG.InstrumentConfiguration.GetDefault(rfsg);
            RFmxInstr.InstrumentConfiguration instrConfig = RFmxInstr.InstrumentConfiguration.GetDefault(instr);

            sgConfig.LOSharingMode = LocalOscillatorSharingMode.None;
            instrConfig.LOSharingMode = LocalOscillatorSharingMode.None;

            ConfigureInstrument(rfsg, instr, sgConfig, instrConfig);

            using (new AssertionScope())
            {
                instr.GetAutomaticSGSASharedLO("", out RFmxInstrMXAutomaticSGSASharedLO rfmxAutomaticSGSASharedLO);
                instr.GetLOSource("", out string rfmxLOSource);
                instr.GetLOExportEnabled("", out bool rfmxLOExportEnabled);

                rfmxAutomaticSGSASharedLO.Should().Be(RFmxInstrMXAutomaticSGSASharedLO.Disabled);
                rfmxLOSource.Should().Be(RFmxInstrMXConstants.LOSourceOnboard);
                rfmxLOExportEnabled.Should().BeFalse();

                rfsg.RF.LocalOscillator.Source.Should().Be(RfsgLocalOscillatorSource.Onboard);
                rfsg.RF.LocalOscillator.LOOutEnabled.Should().BeFalse();
            }
        }

        [TestMethod()]
        public void Test5830ManualLOSharing()
        {
            Test583xManualLOSharing("NI PXIe-3621");
        }

        [TestMethod()]
        public void Test5831ManualLOSharing()
        {
            Test583xManualLOSharing("NI PXIe-3622");
        }

        private void Test583xManualLOSharing(string model)
        {
            if (!GetSessions(model, out NIRfsg rfsg, out RFmxInstrMX instr))
                Assert.Inconclusive();

            SG.InstrumentConfiguration sgConfig = SG.InstrumentConfiguration.GetDefault(rfsg);
            RFmxInstr.InstrumentConfiguration instrConfig = RFmxInstr.InstrumentConfiguration.GetDefault(instr);

            sgConfig.LOSharingMode = LocalOscillatorSharingMode.Manual;
            for (int i = 0; i < sgConfig.LORoutingConfigurations.Length; i++)
                sgConfig.LORoutingConfigurations[i].Source = "SG_SA_Shared";
            instrConfig.LOSharingMode = LocalOscillatorSharingMode.Manual;
            for (int i = 0; i < instrConfig.LORoutingConfigurations.Length; i++)
                instrConfig.LORoutingConfigurations[i].Source = "SG_SA_Shared";

            ConfigureInstrument(rfsg, instr, sgConfig, instrConfig);

            using (new AssertionScope())
            {
                instr.GetAutomaticSGSASharedLO("", out RFmxInstrMXAutomaticSGSASharedLO rfmxAutomaticSGSASharedLO);
                rfmxAutomaticSGSASharedLO.Should().Be(RFmxInstrMXAutomaticSGSASharedLO.Disabled);

                foreach (string channelName in GetLOChannelNames(model))
                {
                    instr.GetLOSource(channelName, out string rfmxLOSource);
                    instr.GetLOExportEnabled(channelName, out bool rfmxLOExportEnabled);

                    rfmxLOSource.Should().Be(RFmxInstrMXConstants.LOSourceSGSAShared);
                    rfmxLOExportEnabled.Should().BeFalse();

                    rfsg.RF.LocalOscillator[channelName].Source.Should().Be(RfsgLocalOscillatorSource.SGSAShared);
                    rfsg.RF.LocalOscillator[channelName].LOOutEnabled.Should().BeFalse();
                }
            }
        }

        [TestMethod()]
        public void Test5840ManualLOSharing()
        {
            Test584xManualLOSharing("NI PXIe-5840");
        }

        [TestMethod()]
        public void Test5841ManualLOSharing()
        {
            Test584xManualLOSharing("NI PXIe-5841");
        }

        private void Test584xManualLOSharing(string model)
        {
            if (!GetSessions(model, out NIRfsg rfsg, out RFmxInstrMX instr))
                Assert.Inconclusive();

            SG.InstrumentConfiguration sgConfig = SG.InstrumentConfiguration.GetDefault(rfsg);
            RFmxInstr.InstrumentConfiguration instrConfig = RFmxInstr.InstrumentConfiguration.GetDefault(instr);

            sgConfig.LOSharingMode = LocalOscillatorSharingMode.Manual;
            sgConfig.LORoutingConfigurations[0].ExportEnabled = true;
            instrConfig.LOSharingMode = LocalOscillatorSharingMode.Manual;
            instrConfig.LORoutingConfigurations[0].Source = RFmxInstrMXConstants.LOSourceLOIn;

            ConfigureInstrument(rfsg, instr, sgConfig, instrConfig);

            using (new AssertionScope())
            {
                instr.GetAutomaticSGSASharedLO("", out RFmxInstrMXAutomaticSGSASharedLO rfmxAutomaticSGSASharedLO);
                instr.GetLOSource("", out string rfmxLOSource);
                instr.GetLOExportEnabled("", out bool rfmxLOExportEnabled);

                rfmxAutomaticSGSASharedLO.Should().Be(RFmxInstrMXAutomaticSGSASharedLO.Disabled);
                rfmxLOSource.Should().Be(RFmxInstrMXConstants.LOSourceLOIn);
                rfmxLOExportEnabled.Should().BeFalse();
                
                rfsg.RF.LocalOscillator.Source.Should().Be(RfsgLocalOscillatorSource.Onboard);
                rfsg.RF.LocalOscillator.LOOutEnabled.Should().BeTrue();
            }
        }

        [TestMethod()]
        public void Test5830AutomaticLOOffset()
        {
            TestAutomaticLOOffset("NI PXIe-3621");
        }

        [TestMethod()]
        public void Test5831AutomaticLOOffset()
        {
            TestAutomaticLOOffset("NI PXIe-3622");
        }

        [TestMethod()]
        public void Test5840AutomaticLOOffset()
        {
            TestAutomaticLOOffset("NI PXIe-5840");
        }

        [TestMethod()]
        public void Test5841AutomaticLOOffset()
        {
            TestAutomaticLOOffset("NI PXIe-5841");
        }

        private void TestAutomaticLOOffset(string model)
        {
            if (!GetSessions(model, out NIRfsg rfsg, out RFmxInstrMX instr))
                Assert.Inconclusive();

            SG.InstrumentConfiguration sgConfig = SG.InstrumentConfiguration.GetDefault(rfsg);
            RFmxInstr.InstrumentConfiguration instrConfig = RFmxInstr.InstrumentConfiguration.GetDefault(instr);

            ConfigureInstrument(rfsg, instr, sgConfig, instrConfig);

            using (new AssertionScope())
            {
                instr.GetLOLeakageAvoidanceEnabled("", out RFmxInstrMXLOLeakageAvoidanceEnabled rfmxLOLeakageAvoidanceEnabled);
                instr.GetDownconverterFrequencyOffset("", out double rfmxDownconverterFrequencyOffset);
                instr.GetLOFrequencyStepSize("", out double rfmxLoFrequencyStepSize);

                rfmxLOLeakageAvoidanceEnabled.Should().Be(RFmxInstrMXLOLeakageAvoidanceEnabled.True);
                rfmxDownconverterFrequencyOffset.Should().NotBeInRange(-rfmxLoFrequencyStepSize / 2, rfmxLoFrequencyStepSize / 2);

                double rfsgUpconverterFrequencyStepSize = rfsg.RF.LocalOscillator.FrequencyStepSize;
                rfsg.RF.Upconverter.FrequencyOffset.Should().NotBeInRange(-rfsgUpconverterFrequencyStepSize / 2, rfsgUpconverterFrequencyStepSize / 2);
            }
        }

        [TestMethod()]
        public void Test5830NoLOOffset()
        {
            TestNoLOOffset("NI PXIe-3621");
        }

        [TestMethod()]
        public void Test5831NoLOOffset()
        {
            TestNoLOOffset("NI PXIe-3622");
        }

        [TestMethod()]
        public void Test5840NoLOOffset()
        {
            TestNoLOOffset("NI PXIe-5840");
        }

        [TestMethod()]
        public void Test5841NoLOOffset()
        {
            TestNoLOOffset("NI PXIe-5841");
        }

        public void TestNoLOOffset(string model)
        {
            if (!GetSessions(model, out NIRfsg rfsg, out RFmxInstrMX instr))
                Assert.Inconclusive();

            SG.InstrumentConfiguration sgConfig = SG.InstrumentConfiguration.GetDefault(rfsg);
            RFmxInstr.InstrumentConfiguration instrConfig = RFmxInstr.InstrumentConfiguration.GetDefault(instr);

            lteTdd10Waveform.LOOffsetMode = LocalOscillatorFrequencyOffsetMode.NoOffset;
            instrConfig.LOOffsetConfiguration.Mode = LocalOscillatorFrequencyOffsetMode.NoOffset;

            ConfigureInstrument(rfsg, instr, sgConfig, instrConfig);

            using (new AssertionScope())
            {
                instr.GetLOLeakageAvoidanceEnabled("", out RFmxInstrMXLOLeakageAvoidanceEnabled rfmxLOLeakageAvoidanceEnabled);
                instr.GetDownconverterFrequencyOffset("", out double rfmxDownconverterFrequencyOffset);
                instr.GetLOFrequencyStepSize("", out double rfmxLoFrequencyStepSize);

                rfmxLOLeakageAvoidanceEnabled.Should().Be(RFmxInstrMXLOLeakageAvoidanceEnabled.False);
                rfmxDownconverterFrequencyOffset.Should().BeInRange(-rfmxLoFrequencyStepSize / 2, rfmxLoFrequencyStepSize / 2);

                double rfsgUpconverterFrequencyStepSize = rfsg.RF.LocalOscillator.FrequencyStepSize;
                rfsg.RF.Upconverter.FrequencyOffset.Should().BeInRange(-rfsgUpconverterFrequencyStepSize / 2, rfsgUpconverterFrequencyStepSize / 2);
            }
        }

        [TestMethod()]
        public void Test5830UserDefinedLOOffset()
        {
            TestUserDefinedLOOffset("NI PXIe-3621");
        }

        [TestMethod()]
        public void Test5831UserDefinedLOOffset()
        {
            TestUserDefinedLOOffset("NI PXIe-3622");
        }

        [TestMethod()]
        public void Test5840UserDefinedLOOffset()
        {
            TestUserDefinedLOOffset("NI PXIe-5840");
        }

        [TestMethod()]
        public void Test5841UserDefinedLOOffset()
        {
            TestUserDefinedLOOffset("NI PXIe-5841");
        }

        public void TestUserDefinedLOOffset(string model)
        {
            if (!GetSessions(model, out NIRfsg rfsg, out RFmxInstrMX instr))
                Assert.Inconclusive();

            SG.InstrumentConfiguration sgConfig = SG.InstrumentConfiguration.GetDefault(rfsg);
            RFmxInstr.InstrumentConfiguration instrConfig = RFmxInstr.InstrumentConfiguration.GetDefault(instr);

            instrConfig.LOOffsetConfiguration.Mode = LocalOscillatorFrequencyOffsetMode.UserDefined;
            instrConfig.LOOffsetConfiguration.Offset_Hz = 27.5e6;
            lteTdd10Waveform.LOOffsetMode = LocalOscillatorFrequencyOffsetMode.UserDefined;
            sgConfig.LOOffset_Hz = 27.5e6;

            ConfigureInstrument(rfsg, instr, sgConfig, instrConfig);

            using (new AssertionScope())
            {
                instr.GetLOLeakageAvoidanceEnabled("", out RFmxInstrMXLOLeakageAvoidanceEnabled rfmxLOLeakageAvoidanceEnabled);
                instr.GetDownconverterFrequencyOffset("", out double rfmxDownconverterFrequencyOffset);
                instr.GetLOFrequencyStepSize("", out double rfmxLoFrequencyStepSize);

                rfmxLOLeakageAvoidanceEnabled.Should().Be(RFmxInstrMXLOLeakageAvoidanceEnabled.False);
                rfmxDownconverterFrequencyOffset.Should().BeApproximately(instrConfig.LOOffsetConfiguration.Offset_Hz, rfmxLoFrequencyStepSize / 2);

                double rfsgUpconverterFrequencyStepSize = rfsg.RF.LocalOscillator.FrequencyStepSize;
                rfsg.RF.Upconverter.FrequencyOffset.Should().BeApproximately(sgConfig.LOOffset_Hz, rfsgUpconverterFrequencyStepSize / 2);
            }
        }

        [ClassCleanup]
        public static void TestFixtureTearDown()
        {
            foreach (var keyValue in deviceSessions)
            {
                keyValue.Value.Item1.Close();
                keyValue.Value.Item2.Close();
            }
        }
    }
}
