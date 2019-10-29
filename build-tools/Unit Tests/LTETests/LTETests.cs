using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static NationalInstruments.ReferenceDesignLibraries.SA.RFmxLTE;
using static NationalInstruments.ReferenceDesignLibraries.SG;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.LteMX;
using NationalInstruments.DataInfrastructure;
using NationalInstruments;

namespace LTETests
{
    [TestClass]
    public class LTETests
    {
        [TestMethod]
        public void TestOfflineAnalysisSingleCarrierTdd()
        {
            RFmxInstrMX instr = new RFmxInstrMX("", "AnalysisOnly=1");
            RFmxLteMX lte = instr.GetLteSignalConfiguration();

            ConfigureCommon(instr, lte, CommonConfiguration.GetDefault());
            SignalConfiguration signalConfig = SignalConfiguration.GetDefault();
            signalConfig.DuplexScheme = RFmxLteMXDuplexScheme.Tdd;
            ConfigureSignal(lte, signalConfig);
            ModAccConfiguration modAccConfig = ModAccConfiguration.GetDefault();
            modAccConfig.MeasurementOffset = 4;
            ConfigureModAcc(lte, modAccConfig);

            lte.Commit("");

            instr.GetRecommendedIQPreTriggerTime("", out double pretriggerTime);
            PrecisionTimeSpan timeOffset = new PrecisionTimeSpan(-pretriggerTime);

            Waveform wfm = LoadWaveformFromTDMS(@"Support Files\LTE_TDD_2.0.tdms");

            Buffer<ComplexSingle> readBuffer = wfm.Data.GetBuffer(true);
            WritableBuffer<ComplexSingle> writeBuffer = wfm.Data.GetWritableBuffer();
            
            int sampleOffset = (int)Math.Round(pretriggerTime * wfm.SampleRate);
            for (int i = 0; i < readBuffer.Size; i++)
                writeBuffer[i] = readBuffer[(i - sampleOffset + readBuffer.Size) % readBuffer.Size];
            wfm.Data.PrecisionTiming = PrecisionWaveformTiming.CreateWithRegularInterval(
                wfm.Data.PrecisionTiming.SampleInterval, timeOffset);

            lte.AnalyzeIQ("", "", wfm.Data, true, out _);
            ModAccResults modAccResults = FetchModAcc(lte);

            instr.Close();

            Assert.IsTrue(modAccResults.ComponentCarrierResults[0].MeanRmsCompositeEvm < 0.001);
        }
    }
}
