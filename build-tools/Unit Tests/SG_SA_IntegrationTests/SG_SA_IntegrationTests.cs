using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.LteMX;
using NationalInstruments.ModularInstruments.SystemServices.DeviceServices;
using NationalInstruments.ReferenceDesignLibraries;
using NationalInstruments.ReferenceDesignLibraries.SA;
using FluentAssertions;
using FluentAssertions.Execution;

namespace SG_SA_IntegrationTests
{
    [TestClass]
    public class SG_SA_IntegrationTests
    {
        static Dictionary<string, Tuple<NIRfsg, RFmxInstrMX>> deviceSessions = new Dictionary<string, Tuple<NIRfsg, RFmxInstrMX>>();
        static SG.Waveform lteTddWaveform;

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
                lteTddWaveform = SG.LoadWaveformFromTDMS(@"Support Files\LTE_TDD_2.0.tdms");

            // configure LTE sessions
            foreach (var keyVal in deviceSessions)
            {
                var rfsg = keyVal.Value.Item1;
                SG.DownloadWaveform(rfsg, lteTddWaveform);

                var lteCommonConfig = RFmxLTE.CommonConfiguration.GetDefault();
                switch (keyVal.Key)
                {
                    case "NI PXIe-3621":
                    case "NI PXIe-3622":
                        lteCommonConfig.SelectedPorts = "rf1/port0";
                        lteCommonConfig.CenterFrequency_Hz = 22.5e9;
                        break;
                }
                var ltemx = keyVal.Value.Item2.GetLteSignalConfiguration();
                RFmxLTE.ConfigureCommon(keyVal.Value.Item2, ltemx, lteCommonConfig);
                RFmxLTE.ConfigureSignal(ltemx, RFmxLTE.SignalConfiguration.GetDefault());
                RFmxLTE.ConfigureModAcc(ltemx, RFmxLTE.ModAccConfiguration.GetDefault());
            }
        }

        [TestMethod()]
        public void Test5831AutomaticLOSharing()
        {
            if (!deviceSessions.TryGetValue("NI PXIe-3622", out Tuple<NIRfsg, RFmxInstrMX> instrSessions))
                Assert.Inconclusive("No instrument present.");

            var rfsg = instrSessions.Item1;
            var sgConfig = SG.InstrumentConfiguration.GetDefault(rfsg);
            SG.ConfigureInstrument(rfsg, sgConfig);
            SG.ConfigureContinuousGeneration(rfsg, lteTddWaveform);

            var instrmx = instrSessions.Item2;
            var instrmxConfig = RFmxInstr.InstrumentConfiguration.GetDefault(instrmx);
            RFmxInstr.ConfigureInstrument(instrmx, instrmxConfig);

            instrmx.GetLteSignalConfiguration().Commit("");
            rfsg.Utility.Commit();

            using (new AssertionScope())
            {
                instrmx.GetAutomaticSGSASharedLO("", out RFmxInstrMXAutomaticSGSASharedLO rfmxAutomaticSGSASharedLO);
                rfmxAutomaticSGSASharedLO.Should().Be(RFmxInstrMXAutomaticSGSASharedLO.Enabled);

                foreach (string channelName in new string[] { "LO1", "LO2" })
                {
                    instrmx.GetLOSource(channelName, out string rfmxLOSource);
                    instrmx.GetLOExportEnabled(channelName, out bool rfmxLOExportEnabled);

                    rfmxLOSource.Should().Be(RFmxInstrMXConstants.LOSourceSGSAShared);
                    rfmxLOExportEnabled.Should().BeFalse();

                    rfsg.RF.LocalOscillator[channelName].Source.Should().Be(RfsgLocalOscillatorSource.SGSAShared);
                    rfsg.RF.LocalOscillator[channelName].LOOutEnabled.Should().BeFalse();
                }
            }
        }

        [TestMethod()]
        public void Test5831NoLOSharing()
        {
            if (!deviceSessions.TryGetValue("NI PXIe-3622", out Tuple<NIRfsg, RFmxInstrMX> instrSessions))
                Assert.Inconclusive("No instrument present.");

            var rfsg = instrSessions.Item1;
            var sgConfig = SG.InstrumentConfiguration.GetDefault(rfsg);
            sgConfig.LOSharingMode = LocalOscillatorSharingMode.None;
            SG.ConfigureInstrument(rfsg, sgConfig);
            SG.ConfigureContinuousGeneration(rfsg, lteTddWaveform);

            var instrmx = instrSessions.Item2;
            var instrmxConfig = RFmxInstr.InstrumentConfiguration.GetDefault(instrmx);
            instrmxConfig.LOSharingMode = LocalOscillatorSharingMode.None;
            RFmxInstr.ConfigureInstrument(instrmx, instrmxConfig);

            instrmx.GetLteSignalConfiguration().Commit("");
            rfsg.Utility.Commit();

            using (new AssertionScope())
            {
                instrmx.GetAutomaticSGSASharedLO("", out RFmxInstrMXAutomaticSGSASharedLO rfmxAutomaticSGSASharedLO);
                rfmxAutomaticSGSASharedLO.Should().Be(RFmxInstrMXAutomaticSGSASharedLO.Disabled);

                foreach (string channelName in new string[] { "LO1", "LO2" })
                {
                    instrmx.GetLOSource(channelName, out string rfmxLOSource);
                    instrmx.GetLOExportEnabled(channelName, out bool rfmxLOExportEnabled);

                    rfmxLOSource.Should().Be(RFmxInstrMXConstants.LOSourceOnboard);
                    rfmxLOExportEnabled.Should().BeFalse();

                    rfsg.RF.LocalOscillator[channelName].Source.Should().Be(RfsgLocalOscillatorSource.Onboard);
                    rfsg.RF.LocalOscillator[channelName].LOOutEnabled.Should().BeFalse();
                }
            }
        }

        [TestMethod()]
        public void Test5831ManualLOSharing()
        {
            if (!deviceSessions.TryGetValue("NI PXIe-3622", out Tuple<NIRfsg, RFmxInstrMX> instrSessions))
                Assert.Inconclusive("No instrument present.");

            var rfsg = instrSessions.Item1;
            var sgConfig = SG.InstrumentConfiguration.GetDefault(rfsg);
            sgConfig.LOSharingMode = LocalOscillatorSharingMode.Manual;
            for (int i = 0; i < sgConfig.LORoutingConfigurations.Length; i++)
                sgConfig.LORoutingConfigurations[i].Source = "SG_SA_Shared";

            SG.ConfigureInstrument(rfsg, sgConfig);
            SG.ConfigureContinuousGeneration(rfsg, lteTddWaveform);

            var instrmx = instrSessions.Item2;
            var instrmxConfig = RFmxInstr.InstrumentConfiguration.GetDefault(instrmx);
            instrmxConfig.LOSharingMode = LocalOscillatorSharingMode.Manual;
            for (int i = 0; i < instrmxConfig.LORoutingConfigurations.Length; i++)
                instrmxConfig.LORoutingConfigurations[i].Source = "SG_SA_Shared";
            RFmxInstr.ConfigureInstrument(instrmx, instrmxConfig);

            instrmx.GetLteSignalConfiguration().Commit("");
            rfsg.Utility.Commit();

            using (new AssertionScope())
            {
                instrmx.GetAutomaticSGSASharedLO("", out RFmxInstrMXAutomaticSGSASharedLO rfmxAutomaticSGSASharedLO);
                rfmxAutomaticSGSASharedLO.Should().Be(RFmxInstrMXAutomaticSGSASharedLO.Disabled);

                foreach (string channelName in new string[] { "LO1", "LO2" })
                {
                    instrmx.GetLOSource(channelName, out string rfmxLOSource);
                    instrmx.GetLOExportEnabled(channelName, out bool rfmxLOExportEnabled);

                    rfmxLOSource.Should().Be(RFmxInstrMXConstants.LOSourceSGSAShared);
                    rfmxLOExportEnabled.Should().BeFalse();

                    rfsg.RF.LocalOscillator[channelName].Source.Should().Be(RfsgLocalOscillatorSource.SGSAShared);
                    rfsg.RF.LocalOscillator[channelName].LOOutEnabled.Should().BeFalse();
                }
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
