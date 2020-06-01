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
                    // CfrConfiguration.bw_list[0] = 20.0f
                };

            }
        }

        public struct NanosemiDPDConfiguration
        {
            public nstdpd.DpdConfig DpdConfiguration;
            public int MaxIterations;
            public RFmxSpecAnMXAmpmMeasurementSampleRateMode ampmSampleRateMode;
            public double ampmSampleRate;
            public double ampmMeasurementInterval;
            public double dutAverageInputPower;
            public RFmxSpecAnMXAmpmSignalType ampmSignalType;
            public RFmxSpecAnMXAmpmThresholdEnabled ampmThresholdEnabled;
            public double ampmThresholdLevel;
            public RFmxSpecAnMXAmpmReferencePowerType ampmRefPowerType;
            public RFmxSpecAnMXAmpmSynchronizationMethod ampmSyncMethod;
            public double ampmTimeout;
            public double ampmMeanLinearGain, ampmOnedBCompressionPoint, ampmMeanRmsEvm;
            public double ampmGainErrorRange, ampmPhaseErrorRange, ampmMeanPhaseError;
            public double ampmAMAMResidual, ampmAMPMResidual;
            public RFmxSpecAnMXAmpmReferenceWaveformIdleDurationPresent ampmIdleDurationPresent;

            public static NanosemiDPDConfiguration GetDefault()
            {
                return new NanosemiDPDConfiguration
                {

                    DpdConfiguration = nstdpd.DpdGetDefaultConfig(),
                    MaxIterations = 3,
                    ampmSampleRateMode = RFmxSpecAnMXAmpmMeasurementSampleRateMode.ReferenceWaveform,
                    ampmSampleRate = 122.88e6, //Hz
                    ampmMeasurementInterval = 100e-6, //s
                    ampmSignalType = RFmxSpecAnMXAmpmSignalType.Modulated,
                    ampmThresholdEnabled = RFmxSpecAnMXAmpmThresholdEnabled.True,
                    ampmThresholdLevel = -20.0,
                    ampmRefPowerType = RFmxSpecAnMXAmpmReferencePowerType.Input,
                    ampmSyncMethod = RFmxSpecAnMXAmpmSynchronizationMethod.Direct,
                    ampmTimeout = 1.0, //s
                    ampmIdleDurationPresent = RFmxSpecAnMXAmpmReferenceWaveformIdleDurationPresent.True,
                    // DpdConfiguration.lvl = nstdpd.DpdLevel.NST_DPD_LEVEL0,
                    //DpdConfiguration.rho = 0.1f;
                    //DpdConfiguration.training_samples = 25000;
                };
            }
        }


        public struct PowerResults
        {
            /// <summary>Returns the change in the average power in the waveform due to applying digital predistion. This value is expressed in dB.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double WaveformPowerOffset_dB;
            /// <summary>Returns the actual PAPR for the pre-distorted waveform. The PAPR of the pre-distorted waveform is set to this value plus
            ///  <see cref="WaveformPowerOffset_dB"/> so that the user-specified power level of the generator is not modified. This behavior
            ///  ensures that the user does not have to change the power level to switch between the original and pre-distorted waveforms,
            ///  while ensuring the pre-distorted waveform is generated correctly.</summary>
            public double WaveformTruePapr_dB;
            /// <summary>Returns the power level of the signal generator during DPD model training.</summary>
            public double TrainingPower_dBm;
        }

        public struct NanosemiDPDResults
        {
            public Waveform PredistortedWaveform;
            public PowerResults PowerResults;
        }
        #endregion

        #region ConfigureNanosemiDPD
        public static void ConfigureNanosemi(RFmxSpecAnMX specAn, NanosemiDPDConfiguration nsDPDConfig, Waveform referenceWaveform, string selectorString = "")
        {
            specAn.SelectMeasurements("", RFmxSpecAnMXMeasurementTypes.Ampm, true);

            specAn.Ampm.Configuration.ConfigureMeasurementSampleRate("", nsDPDConfig.ampmSampleRateMode, nsDPDConfig.ampmSampleRate);
            specAn.Ampm.Configuration.ConfigureMeasurementInterval("", nsDPDConfig.ampmMeasurementInterval);

            specAn.Ampm.Configuration.ConfigureReferenceWaveform("", referenceWaveform.Data, nsDPDConfig.ampmIdleDurationPresent, nsDPDConfig.ampmSignalType);
            specAn.Ampm.Configuration.ConfigureDutAverageInputPower("", nsDPDConfig.dutAverageInputPower);
            //Power thresholding to only consider samples with a certain power down from the peak and above
            specAn.Ampm.Configuration.ConfigureDutAverageInputPower("", nsDPDConfig.dutAverageInputPower);
            specAn.Ampm.Configuration.ConfigureThreshold("", nsDPDConfig.ampmThresholdEnabled, nsDPDConfig.ampmThresholdLevel, RFmxSpecAnMXAmpmThresholdType.Relative);
            specAn.Ampm.Configuration.ConfigureReferencePowerType("", nsDPDConfig.ampmRefPowerType);
            //Exposing sync method setting because it needs to be set to "Alias Protected" for signals with separated carriers.
            specAn.Ampm.Configuration.ConfigureSynchronizationMethod("", nsDPDConfig.ampmSyncMethod);
            specAn.Commit("");


        }

        #endregion

        #region PerformDPD
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
            ComplexWaveform<ComplexSingle> processedRefWaveform = new ComplexWaveform<ComplexSingle>(0);
            ComplexWaveform<ComplexSingle> processedMeanAcquiredWaveform = new ComplexWaveform<ComplexSingle>(0);
            double meanPower_dB = 0.0;
            nstdpd.DpdResetTraining(nsDPDConfig.DpdConfiguration);
            meanPower_dB = CalculateAveragePower(ref referenceWaveform.Data, referenceWaveform.BurstLength_s * referenceWaveform.SampleRate);
            // RfsgGenerationStatus preDpdGenerationStatus = rfsgSession.CheckGenerationStatus();x

            for (int dpdIter = 0; dpdIter < nsDPDConfig.MaxIterations; dpdIter++)
            {

                specAn.Initiate(selectorString, "");

                specAn.Ampm.Results.FetchProcessedReferenceWaveform(selectorString, nsDPDConfig.ampmTimeout, ref processedRefWaveform);
                specAn.Ampm.Results.FetchProcessedMeanAcquiredWaveform(selectorString, nsDPDConfig.ampmTimeout, ref processedMeanAcquiredWaveform);

                processedRefWaveform = ScaleWaveform(ref processedRefWaveform, meanPower_dB);
                processedMeanAcquiredWaveform = ScaleWaveform(ref processedMeanAcquiredWaveform, meanPower_dB);

                //Fetch aligned IQ from AMPM measurement
                nstdpd.DpdTrain(processedRefWaveform, processedMeanAcquiredWaveform, nsDPDConfig.DpdConfiguration);
                nanosemiDPDResults.PredistortedWaveform.Data = nstdpd.DpdApply(referenceWaveform.Data, nsDPDConfig.DpdConfiguration);

                specAn.Ampm.Configuration.ConfigureReferenceWaveform("", nanosemiDPDResults.PredistortedWaveform.Data, nsDPDConfig.ampmIdleDurationPresent, nsDPDConfig.ampmSignalType);

                // (2)
                //Modify RFSG waveform to DPD waveform and Initiate generation again. 
                RfsgGenerationStatus preDpdGenerationStatus = rfsgSession.CheckGenerationStatus();
                rfsgSession.Abort();
                NIRfsgPlayback.ClearAllWaveforms(rfGeneratorHandle);

                //Download DPD waveform to the RF Generator and update scripts
                NIRfsgPlayback.DownloadUserWaveform(rfGeneratorHandle, nanosemiDPDResults.PredistortedWaveform.Name, nanosemiDPDResults.PredistortedWaveform.Data, true);
                NIRfsgPlayback.RetrieveWaveformPapr(rfGeneratorHandle, nanosemiDPDResults.PredistortedWaveform.Name, out nanosemiDPDResults.PredistortedWaveform.PAPR_dB);
                NIRfsgPlayback.RetrieveWaveformSampleRate(rfGeneratorHandle, nanosemiDPDResults.PredistortedWaveform.Name, out nanosemiDPDResults.PredistortedWaveform.SampleRate);
                // NIRfsgPlayback.SetScriptToGenerateSingleRfsg(rfGeneratorHandle, nanosemiDPDResults.PredistortedWaveform.Script);
                // DownloadWaveform(rfsgSession, nanosemiDPDResults.PredistortedWaveform); // implicit abort
                ApplyWaveformAttributes(rfsgSession, nanosemiDPDResults.PredistortedWaveform);

                // (3) Initiate generation of post DPD waveform
                rfsgSession.Initiate();

                meanPower_dB = CalculateAveragePower(ref referenceWaveform.Data, referenceWaveform.BurstLength_s * referenceWaveform.SampleRate);
            }

            //nanosemiDPDResults.PowerResults.TrainingPower_dBm = rfsgSession.RF.PowerLevel;

            return nanosemiDPDResults;
        }

        public static double CalculateAveragePower(ref ComplexWaveform<ComplexSingle> waveform, double refLength)
        {
            //calculate average power of the reference waveform
            double sum = 0.0, meanPower = 0.0, power_dB = 0.0;
            double[] wfmMagnitudes = waveform.GetMagnitudeDataArray(true);

            for (int i = 0; i < wfmMagnitudes.Length; i++)
            {
                sum += wfmMagnitudes[i];
            }
            meanPower = sum / refLength;
            power_dB = 20 * Math.Log10(meanPower) + 10;
            return power_dB;
        }

        public static ComplexWaveform<ComplexSingle> ScaleWaveform(ref ComplexWaveform<ComplexSingle> waveform, double meanPowerIn_dB)
        {
            double meanPower_dB = 0.0, meanPower = 0.0, wfmAverPwr_dB = 0.0;
            ComplexWaveform<ComplexSingle> scaledWaveform;
            scaledWaveform = waveform;
            wfmAverPwr_dB = CalculateAveragePower(ref scaledWaveform, scaledWaveform.SampleCount);
            meanPower_dB = meanPowerIn_dB - wfmAverPwr_dB;
            meanPower = Math.Pow(10, meanPower_dB / 20);

            DataInfrastructure.WritableBuffer<ComplexSingle> waveformBuffer = scaledWaveform.GetWritableBuffer();
            ComplexSingle scale = ComplexSingle.FromPolar(Convert.ToSingle(meanPower), 0.0f);
            for (int i = 0; i < scaledWaveform.SampleCount; i++)
                waveformBuffer[i] = waveformBuffer[i] * scale; // multiplication is faster than division

            return scaledWaveform;
        }

        #endregion
    }
}
