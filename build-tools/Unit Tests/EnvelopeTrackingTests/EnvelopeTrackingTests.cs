using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static NationalInstruments.ReferenceDesignLibraries.Methods.EnvelopeTracking;
using NationalInstruments.ReferenceDesignLibraries;
using FluentAssertions;
using FluentAssertions.Execution;
using NationalInstruments;

namespace EnvelopeTrackingTests
{
    [TestClass]
    public class EnvelopeTrackingTests
    {
        [TestMethod()]
        public void TestConfigureLookUpTable()
        {
            LookUpTableConfiguration lutConfig = new LookUpTableConfiguration()
            {
                DutAverageInputPower_dBm = 10.0, // results in no scaling of iq data
                DutInputPower_dBm = new float[] { 1.0f, 3.0f, 2.0f, 4.0f },
                SupplyVoltage_V = new float[] { 5.0f, 7.0f, 6.0f, 8.0f }
            };

            Waveform referenceWaveform = new Waveform()
            {
                Data = ComplexWaveform<ComplexSingle>.FromArray1D(new ComplexSingle[] {
                    ComplexSingle.FromSingle(0.5f),
                    ComplexSingle.FromSingle(1.5f),
                    ComplexSingle.FromSingle(2.5f),
                    ComplexSingle.FromSingle(3.5f),
                    ComplexSingle.FromSingle(4.5f)
                })
            };

            // p = 10log(i^2) + 10
            // i = sqrt(10**(p - 10) / 10)
            var writableBuffer = referenceWaveform.Data.GetWritableBuffer();
            for (int i = 0; i < referenceWaveform.Data.SampleCount; i++)
                writableBuffer[i] = ComplexSingle.FromSingle((float)Math.Sqrt(Math.Pow(10.0, (writableBuffer[i].Real - 10.0) / 10.0)));

            Waveform envelopeWaveform = ConfigureEnvelopeWaveform(referenceWaveform, lutConfig);
            ComplexSingle.DecomposeArrayPolar(envelopeWaveform.Data.GetRawData(), out float[] yi, out _);

            float[] solution = new float[] { 4.5f, 5.5f, 6.5f, 7.5f, 8.5f };
            using (new AssertionScope())
            {
                for (int i = 0; i < yi.Length; i++)
                    yi[i].Should().BeApproximately(solution[i], 0.1f);
            }
        }
    }
}
