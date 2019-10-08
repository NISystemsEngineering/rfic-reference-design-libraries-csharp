using Microsoft.VisualStudio.TestTools.UnitTesting;
using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.ModularInstruments.NIRfsgPlayback;
using System.IO;

namespace NationalInstruments.ReferenceDesignLibraries.Tests
{
    [TestClass()]
    public class SGTests
    {
        //Suppress warning for obselete code as LoadWaveformFromTDMS intentionally uses 
        //an outdated method in order to support older waveform files
#pragma warning disable CS0612
        [TestMethod()]
        public void LoadNoBurstDataTest()
        {
            string filePath = Path.GetFullPath(@"Support Files\BT No Burst.tdms");
            SG.Waveform waveform = SG.LoadWaveformFromTDMS(filePath, "");

            Assert.IsTrue(waveform.BurstStartLocations[0] == 0, "Burst start set to 0");
            Assert.IsTrue(waveform.BurstStopLocations[0] == waveform.WaveformData.SampleCount - 1, "Burst stop set to last sample");

            double papr = 0.0; // papr != runtime scaling for file version 1.0.0 so use empirical constant
            //NIRfsgPlayback.ReadWaveformFileVersionFromFile(filePath, out string waveformVersion);
            //if (waveformVersion == "1.0.0") NIRfsgPlayback.ReadPeakPowerAdjustmentFromFile(filePath, 0, out papr);
            //else NIRfsgPlayback.ReadPaprFromFile(filePath, 0, out papr); //Version 2.0.0 and later

            Assert.AreEqual(waveform.PAPR_dB, papr, .001,
                "PAPR for a no-burst waveform should match what is reported by RFmx.");
        }
        [TestMethod()]
        public void LoadMultiBurstDataTest()
        {
            string filePath = Path.GetFullPath(@"Support Files\LTE TDD Waveform.tdms");
            SG.Waveform waveform = SG.LoadWaveformFromTDMS(filePath);

            double actualLength = waveform.WaveformData.SampleCount / waveform.SampleRate;

            Assert.AreEqual(waveform.BurstLength_s, actualLength, 1e-9,
                "Burst length should be properly calculated for multi-burst waveforms.");

            double papr;
            NIRfsgPlayback.ReadWaveformFileVersionFromFile(filePath, out string waveformVersion);
            if (waveformVersion == "1.0.0") NIRfsgPlayback.ReadPeakPowerAdjustmentFromFile(filePath, 0, out papr);
            else NIRfsgPlayback.ReadPaprFromFile(filePath, 0, out papr); //Version 2.0.0 and later

            Assert.AreEqual(waveform.PAPR_dB, papr, .001,
                "PAPR for a multi-burst waveform should match what is reported by RFmx.");
        }
        [TestMethod()]
        public void LoadSingleBurstDataTest()
        {
            string filePath = Path.GetFullPath(@"Support Files\WLAN Single Burst.tdms");
            SG.Waveform waveform = SG.LoadWaveformFromTDMS(filePath);

            //This value is known empirically from the waveform configuration
            double actualLength = 5.132112e-3;

            Assert.AreEqual(waveform.BurstLength_s, actualLength, 1e-6,
                "Burst length should be properly calculated for a single-burst waveforms.");

            double papr;
            NIRfsgPlayback.ReadWaveformFileVersionFromFile(filePath, out string waveformVersion);
            if (waveformVersion == "1.0.0") NIRfsgPlayback.ReadPeakPowerAdjustmentFromFile(filePath, 0, out papr);
            else NIRfsgPlayback.ReadPaprFromFile(filePath, 0, out papr); //Version 2.0.0 and later

            Assert.AreEqual(waveform.PAPR_dB, papr, .001,
                "PAPR for a single-burst waveform should match what is reported by RFmx.");
        }
    }
}