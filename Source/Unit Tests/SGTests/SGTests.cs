using Microsoft.VisualStudio.TestTools.UnitTesting;
using NationalInstruments.ReferenceDesignLibraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NationalInstruments.DataInfrastructure;
using NationalInstruments.ModularInstruments.NIRfsg;

namespace NationalInstruments.ReferenceDesignLibraries.Tests
{
    [TestClass()]
    public class SGTests
    {
        [TestMethod()]
        public void LoadWaveformWithoutBurstDataTest()
        {
            NIRfsg rfsg = new NIRfsg("test", false, false, "Simulate=1,DriverSetup=Model:5840");

            string filePath = Path.GetFullPath(@"Support Files\No Burst.tdms");
            SG.Waveform waveform = SG.LoadWaveformFromTDMS(rfsg, filePath);

            Assert.IsTrue(waveform.BurstStartLocations[0] == 0, "Burst start set to 0");
            Assert.IsTrue(waveform.BurstStopLocations[0] == waveform.WaveformData.SampleCount - 1, "Burst stop set to last sample");
            rfsg.Close();
        }
        [TestMethod()]
        public void LoadMultiBurstDataTest()
        {
            NIRfsg rfsg = new NIRfsg("test", false, false, "Simulate=1,DriverSetup=Model:5840");

            string filePath = Path.GetFullPath(@"Support Files\LTE TDD Waveform.tdms");
            SG.Waveform waveform = SG.LoadWaveformFromTDMS(rfsg, filePath);

            double actualLength = waveform.WaveformData.SampleCount / waveform.SampleRate;

            Assert.AreEqual(waveform.BurstLength_s, actualLength, 1e-9,
                "Burst length should be properly calculated for multi-burst waveforms.");
            rfsg.Close();
        }
    }
}