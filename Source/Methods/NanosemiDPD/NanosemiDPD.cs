using System;
using NationalInstruments.RFmx.SpecAnMX;
using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.ModularInstruments.NIRfsgPlayback;
using static NationalInstruments.ReferenceDesignLibraries.SG;
using NanoSemiLinearizer.Interop;

namespace NationalInstruments.ReferenceDesignLibraries.Methods
{
    public static class NanosemiDPD
    {
        #region Type Definitions

        /// <summary>Defines commmon settings for the application of crest factor reduction on the reference waveform prior to applying DPD for a single carrier channel.</summary>
        public struct PreDpdCrestFactorReductionConfiguration
        {

            public nstdpd.CfrConfig CfrConfiguration;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static PreDpdCrestFactorReductionConfiguration GetDefault()
            {
                return new PreDpdCrestFactorReductionConfiguration
                {
                    CfrConfiguration = nstdpd.CfrGetDefaultConfig()
                };

            }
        }

        public struct NanosemiDPDConfiguration
        {
            public nstdpd.DpdConfig DpdConfiguration;
            /// <summary>Specifies the number of iterations over which Nanosemi DPD  is computed using indirect-learning architecture.</summary>
            public int NumberOfIterations;
            /// <summary>Specifies the duration of the reference waveform considered for the DPD measurement.
            public double IQAcquisitionLength_s;

            public static NanosemiDPDConfiguration GetDefault()
            {
                return new NanosemiDPDConfiguration
                {

                    DpdConfiguration = nstdpd.DpdGetDefaultConfig(),
                    NumberOfIterations = 3,
                    IQAcquisitionLength_s = 1e-3

                };
            }
        }


        public struct NanosemiDPDResults
        {
            /// <summary>Returns the <see cref="Waveform"/> whose data represents the complex baseband equivalent of the RF signal
            /// after applying digital pre-distortion. 
            public Waveform PredistortedWaveform;
        }
        #endregion

        #region ConfigureNanosemiDPD

        /// <param name="specAn">Specifies the SpecAn signal to configure.</param>
        /// <param name="referenceWaveform">Specifies the <see cref="Waveform"/> whose data defines the complex baseband equivalent of the RF signal on which the 
        /// pre-DPD signal conditioning is applied. 
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. 
        public static void ConfigureNanosemi(RFmxSpecAnMX specAn, NanosemiDPDConfiguration nsDPDConfig, Waveform referenceWaveform, string selectorString = "")
        {
            specAn.SelectMeasurements("", RFmxSpecAnMXMeasurementTypes.IQ, true);
            specAn.IQ.Configuration.ConfigureAcquisition(selectorString, referenceWaveform.SampleRate, 1, nsDPDConfig.IQAcquisitionLength_s, 0);
            nsDPDConfig.DpdConfiguration.f_sample = (float)(referenceWaveform.SampleRate * 1e-6);
            nstdpd.DpdResetTraining(nsDPDConfig.DpdConfiguration);

        }
        #endregion

        #region PerformDPD

        /// <summary>Acquires the incoming signal, trains the DPD model, applies the DPD model to the reference waveform, and downloads the predistorted waveform to the generator.
        ///  If generation is in progress when this function is called, generation will resume with the predistorted waveform.</summary>
        /// <param name="specAn">Specifies the SpecAn signal to configure.</param>
        /// <param name="rfsgSession">Specifies the open RFSG session to configure.</param>
        /// <param name="referenceWaveform">Specifies the <see cref="Waveform"/> whose data defines the complex baseband equivalent of the RF signal applied at the input 
        /// port of the device under test when performing the measurement. </param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. 
        /// <returns>Common results after the application of lookup table based DPD.</returns>
        public static NanosemiDPDResults PerformNanosemiDPD(RFmxSpecAnMX specAn, NIRfsg rfsgSession, Waveform referenceWaveform, NanosemiDPDConfiguration nsDPDConfig, string selectorString = "")
        {
            IntPtr rfGeneratorHandle;
            rfGeneratorHandle = rfsgSession.GetInstrumentHandle().DangerousGetHandle();
            NanosemiDPDResults nanosemiDPDResults = new NanosemiDPDResults()
            {
                PredistortedWaveform = referenceWaveform
            };
            nanosemiDPDResults.PredistortedWaveform.Data = referenceWaveform.Data.Clone(); // clone waveform so RFmx can't act on reference waveform
            nanosemiDPDResults.PredistortedWaveform.UpdateNameAndScript(referenceWaveform.Name + "postNsDpd");
            ComplexWaveform<ComplexSingle> acquiredIQData = new ComplexWaveform<ComplexSingle>(0);


            for (int dpdIter = 0; dpdIter < nsDPDConfig.NumberOfIterations; dpdIter++)
            {
                specAn.Initiate(selectorString, "");
                specAn.IQ.Results.FetchData(selectorString, 3, 0, -1, ref acquiredIQData);

                nstdpd.DpdTrain(referenceWaveform.Data, acquiredIQData, nsDPDConfig.DpdConfiguration);
                nanosemiDPDResults.PredistortedWaveform.Data = nstdpd.DpdApply(referenceWaveform.Data, nsDPDConfig.DpdConfiguration);

                //Modify RFSG waveform to DPD waveform and Initiate generation again. 
                RfsgGenerationStatus preDpdGenerationStatus = rfsgSession.CheckGenerationStatus();
                rfsgSession.Abort();
                NIRfsgPlayback.ClearAllWaveforms(rfGeneratorHandle);

                //Download DPD waveform to the RF Generator and update scripts
                NIRfsgPlayback.DownloadUserWaveform(rfGeneratorHandle, nanosemiDPDResults.PredistortedWaveform.Name, nanosemiDPDResults.PredistortedWaveform.Data, true);
                NIRfsgPlayback.RetrieveWaveformPapr(rfGeneratorHandle, nanosemiDPDResults.PredistortedWaveform.Name, out nanosemiDPDResults.PredistortedWaveform.PAPR_dB);
                NIRfsgPlayback.RetrieveWaveformSampleRate(rfGeneratorHandle, nanosemiDPDResults.PredistortedWaveform.Name, out nanosemiDPDResults.PredistortedWaveform.SampleRate);
                ApplyWaveformAttributes(rfsgSession, nanosemiDPDResults.PredistortedWaveform);

                //Initiate generation of post DPD waveform
                rfsgSession.Initiate();
            }
            return nanosemiDPDResults;
        }
        #endregion
    }
}
