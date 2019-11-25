using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.ModularInstruments.SystemServices.DeviceServices;
using NationalInstruments.ReferenceDesignLibraries;
using NationalInstruments.ReferenceDesignLibraries.SA;
using NationalInstruments.RFmx.InstrMX;
using System.Text.RegularExpressions;

namespace SG_SA_IntegrationTests
{
    [TestClass]
    public class SG_SA_IntegrationTests
    {
        static Waveform lteTdd10Waveform;

        [ClassInitialize]
        public static void TestFixtureSetup(TestContext context)
        {
            lteTdd10Waveform = SG.LoadWaveformFromTDMS(@"Support Files\LTE_TDD_2.0.tdms");
        }

        private bool FindDevice(string model, out string resourceName)
        {
            resourceName = null;
            switch (model)
            {
                case "NI PXIe-5830":
                    model = "NI PXIe-3621";
                    break;
                case "NI PXIe-5831":
                    model = "NI PXIe-3622";
                    break;
            }
            using (var mis = new ModularInstrumentsSystem(typeof(NIRfsg)))
            {
                for (int i = 0; i < mis.DeviceCollection.Count; i++)
                    if (mis.DeviceCollection[i].Model.Equals(model))
                    {
                        resourceName = mis.DeviceCollection[i].Name;
                        return true;
                    }
            }
            return false;
        }

        private void OpenTransceiverSessions(string resourceName, out NIRfsg rfsg, out RFmxInstrMX instr)
        {
            rfsg = new NIRfsg(resourceName, false, true);
            instr = new RFmxInstrMX(resourceName, "");
        }

        private void FindAndOpenTransceiverSessionsWithAssertion(string model, out NIRfsg rfsg, out RFmxInstrMX instr)
        {
            if (!FindDevice(model, out string resourceName))
                Assert.Inconclusive("No device found.");
            OpenTransceiverSessions(resourceName, out rfsg, out instr);
        }

        private void CloseTransceiverSessions(NIRfsg rfsg, RFmxInstrMX instr)
        {
            rfsg?.Close();
            instr?.Close();
        }

        private void CommitTestConfiguration(NIRfsg rfsg, RFmxInstrMX instr)
        {
            SG.DownloadWaveform(rfsg, lteTdd10Waveform);
            SG.ConfigureContinuousGeneration(rfsg, lteTdd10Waveform);
            
            var lteCommonConfig = CommonConfiguration.GetDefault();

            string instrumentModel = rfsg.Identity.InstrumentModel;
            if (Regex.IsMatch(instrumentModel, "NI PXIe-5830"))
            {
                lteCommonConfig.SelectedPorts = "if1";
                lteCommonConfig.CenterFrequency_Hz = 6.5e9;
            }
            else if (Regex.IsMatch(instrumentModel, "NI PXIe-5831"))
            {
                lteCommonConfig.SelectedPorts = "rf1/port0";
                lteCommonConfig.CenterFrequency_Hz = 28e9;
            }
            var lte = instr.GetLteSignalConfiguration();
            RFmxLTE.ConfigureCommon(lte, lteCommonConfig);
            RFmxLTE.ConfigureStandard(lte, RFmxLTE.StandardConfiguration.GetDefault());
            RFmxLTE.ConfigureModAcc(lte, RFmxLTE.ModAccConfiguration.GetDefault());

            lte.Commit("");
            rfsg.Utility.Commit();
        }

        private void ApplyAutomaticLOConfiguration(NIRfsg rfsg, RFmxInstrMX instr)
        {
            SG.InstrumentConfiguration sgConfig = SG.InstrumentConfiguration.GetDefault(rfsg);
            SG.ConfigureInstrument(rfsg, sgConfig);

            RFmxInstr.InstrumentConfiguration instrConfig = RFmxInstr.InstrumentConfiguration.GetDefault();
            RFmxInstr.ConfigureInstrument(instr, instrConfig);
        }

        private void ApplyNoLOSharingConfiguration(NIRfsg rfsg, RFmxInstrMX instr)
        {
            SG.InstrumentConfiguration sgConfig = SG.InstrumentConfiguration.GetDefault(rfsg);
            sgConfig.LOSharingMode = LocalOscillatorSharingMode.None;
            SG.ConfigureInstrument(rfsg, sgConfig);

            RFmxInstr.InstrumentConfiguration instrConfig = RFmxInstr.InstrumentConfiguration.GetDefault();
            instrConfig.LOSharingMode = LocalOscillatorSharingMode.None;
            RFmxInstr.ConfigureInstrument(instr, instrConfig);
        }

        [TestMethod()]
        public void Test5830AutomaticLOSharing()
        {
            FindAndOpenTransceiverSessionsWithAssertion("NI PXIe-5830", out NIRfsg rfsg, out RFmxInstrMX instr);
            ApplyAutomaticLOConfiguration(rfsg, instr);
            CommitTestConfiguration(rfsg, instr);

            using (new AssertionScope())
            {
                instr.GetAutomaticSGSASharedLO("", out RFmxInstrMXAutomaticSGSASharedLO rfmxAutomaticSGSASharedLO);
                rfmxAutomaticSGSASharedLO.Should().Be(RFmxInstrMXAutomaticSGSASharedLO.Enabled);

                instr.GetLOSource("LO2", out string rfmxLOSource);
                rfmxLOSource.Should().Be(RFmxInstrMXConstants.LOSourceSGSAShared);

                rfsg.RF.LocalOscillator["LO2"].Source.Should().Be(RfsgLocalOscillatorSource.SGSAShared);
            }

            CloseTransceiverSessions(rfsg, instr);
        }

        [TestMethod()]
        public void Test5831AutomaticLOSharing()
        {
            FindAndOpenTransceiverSessionsWithAssertion("NI PXIe-5831", out NIRfsg rfsg, out RFmxInstrMX instr);
            ApplyAutomaticLOConfiguration(rfsg, instr);
            CommitTestConfiguration(rfsg, instr);

            using (new AssertionScope())
            {
                instr.GetAutomaticSGSASharedLO("", out RFmxInstrMXAutomaticSGSASharedLO rfmxAutomaticSGSASharedLO);
                rfmxAutomaticSGSASharedLO.Should().Be(RFmxInstrMXAutomaticSGSASharedLO.Enabled);

                foreach (var channelName in new string[] { "LO1", "LO2" })
                {
                    instr.GetLOSource(channelName, out string rfmxLOSource);
                    rfmxLOSource.Should().Be(RFmxInstrMXConstants.LOSourceSGSAShared);

                    rfsg.RF.LocalOscillator[channelName].Source.Should().Be(RfsgLocalOscillatorSource.SGSAShared);
                }
            }

            CloseTransceiverSessions(rfsg, instr);
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
            FindAndOpenTransceiverSessionsWithAssertion(model, out NIRfsg rfsg, out RFmxInstrMX instr);
            ApplyAutomaticLOConfiguration(rfsg, instr);
            CommitTestConfiguration(rfsg, instr);

            using (new AssertionScope())
            {
                instr.GetAutomaticSGSASharedLO("", out RFmxInstrMXAutomaticSGSASharedLO rfmxAutomaticSGSASharedLO);
                rfmxAutomaticSGSASharedLO.Should().Be(RFmxInstrMXAutomaticSGSASharedLO.Enabled);
                
                instr.GetLOSource("", out string rfmxLOSource);
                rfmxLOSource.Should().Be(RFmxInstrMXConstants.LOSourceLOIn);

                rfsg.RF.LOOutExportConfigureFromRfsa.Should().Be(RfsgLOOutExportConfigureFromRfsa.Enabled);
            }

            CloseTransceiverSessions(rfsg, instr);
        }

        [TestMethod()]
        public void Test5830NoLOSharing()
        {
            FindAndOpenTransceiverSessionsWithAssertion("NI PXIe-5830", out NIRfsg rfsg, out RFmxInstrMX instr);
            ApplyNoLOSharingConfiguration(rfsg, instr);
            CommitTestConfiguration(rfsg, instr);

            using (new AssertionScope())
            {
                instr.GetAutomaticSGSASharedLO("", out RFmxInstrMXAutomaticSGSASharedLO rfmxAutomaticSGSASharedLO);
                rfmxAutomaticSGSASharedLO.Should().Be(RFmxInstrMXAutomaticSGSASharedLO.Disabled);

                instr.GetLOSource("LO2", out string rfmxLOSource);
                rfmxLOSource.Should().Be(RFmxInstrMXConstants.LOSourceOnboard);

                rfsg.RF.LocalOscillator["LO2"].Source.Should().Be(RfsgLocalOscillatorSource.Onboard);
            }

            CloseTransceiverSessions(rfsg, instr);
        }

        [TestMethod()]
        public void Test5831NoLOSharing()
        {
            FindAndOpenTransceiverSessionsWithAssertion("NI PXIe-5831", out NIRfsg rfsg, out RFmxInstrMX instr);
            ApplyNoLOSharingConfiguration(rfsg, instr);
            CommitTestConfiguration(rfsg, instr);

            using (new AssertionScope())
            {
                instr.GetAutomaticSGSASharedLO("", out RFmxInstrMXAutomaticSGSASharedLO rfmxAutomaticSGSASharedLO);
                rfmxAutomaticSGSASharedLO.Should().Be(RFmxInstrMXAutomaticSGSASharedLO.Disabled);

                foreach ( string channelName in new string[] {"LO1", "LO2"})
                {
                    instr.GetLOSource(channelName, out string rfmxLOSource);
                    rfmxLOSource.Should().Be(RFmxInstrMXConstants.LOSourceOnboard);

                    rfsg.RF.LocalOscillator[channelName].Source.Should().Be(RfsgLocalOscillatorSource.Onboard);
                }
            }

            CloseTransceiverSessions(rfsg, instr);
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
            FindAndOpenTransceiverSessionsWithAssertion(model, out NIRfsg rfsg, out RFmxInstrMX instr);
            ApplyNoLOSharingConfiguration(rfsg, instr);
            CommitTestConfiguration(rfsg, instr);

            using (new AssertionScope())
            {
                instr.GetAutomaticSGSASharedLO("", out RFmxInstrMXAutomaticSGSASharedLO rfmxAutomaticSGSASharedLO);
                rfmxAutomaticSGSASharedLO.Should().Be(RFmxInstrMXAutomaticSGSASharedLO.Disabled);

                instr.GetLOSource("", out string rfmxLOSource);
                rfmxLOSource.Should().Be(RFmxInstrMXConstants.LOSourceOnboard);

                rfsg.RF.LOOutExportConfigureFromRfsa.Should().Be(RfsgLOOutExportConfigureFromRfsa.Disabled);
            }

            CloseTransceiverSessions(rfsg, instr);
        }

        [TestMethod()]
        public void Test5830AutomaticLOOffset()
        {
            TestAutomaticLOOffset("NI PXIe-5830");
        }

        [TestMethod()]
        public void Test5831AutomaticLOOffset()
        {
            TestAutomaticLOOffset("NI PXIe-5831");
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
            FindAndOpenTransceiverSessionsWithAssertion(model, out NIRfsg rfsg, out RFmxInstrMX instr);
            ApplyAutomaticLOConfiguration(rfsg, instr);
            CommitTestConfiguration(rfsg, instr);

            using (new AssertionScope())
            {
                instr.GetLOLeakageAvoidanceEnabled("", out RFmxInstrMXLOLeakageAvoidanceEnabled rfmxLOLeakageAvoidanceEnabled);
                rfmxLOLeakageAvoidanceEnabled.Should().Be(RFmxInstrMXLOLeakageAvoidanceEnabled.True);

                instr.GetDownconverterFrequencyOffset("", out double rfmxDownconverterFrequencyOffset);
                instr.GetLOFrequencyStepSize("", out double rfmxLoFrequencyStepSize);
                rfmxDownconverterFrequencyOffset.Should().NotBeInRange(-rfmxLoFrequencyStepSize / 2, rfmxLoFrequencyStepSize / 2);

                double rfsgUpconverterFrequencyStepSize = rfsg.RF.LocalOscillator.FrequencyStepSize;
                rfsg.RF.Upconverter.FrequencyOffset.Should().NotBeInRange(-rfsgUpconverterFrequencyStepSize / 2, rfsgUpconverterFrequencyStepSize / 2);
            }

            CloseTransceiverSessions(rfsg, instr);
        }
    }
}
