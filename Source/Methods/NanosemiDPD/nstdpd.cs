using System;
using System.Runtime.InteropServices;
using NationalInstruments;

namespace NanoSemiLinearizer.Interop
{
    public static class nstdpd
    {
        #region Public Structs and Enums
        /// <summary>
        ///CFR configuration structure.
        ///Dictates the behavior the NSTDPD CFR algorithm.
        ///All fields must have non-Infinity, non-NaN values.
        ///
        ///The num_bands, fc_list, and bw_list fields describe the spectral mask of the input waveform as a list of carrier bands with a certain bandwidths and center frequencies.
        ///If the offset field is nonzero, the mask described by num_bands/fc_list/bw_list will be frequency-shifted by that value.
        ///The overal spectrum mask described by the first four fields of this struct must be fully contained within the range[-f_sample / 2, f_sample / 2].
        ///
        ///struct NST_CFR_CONFIG_STRUCT
        ///</summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct CfrConfig
        {
            /// <summary>
            ///  The number of frequency bands to apply CFR to. Must be a non-negative value. 
            /// </summary>
            public int num_bands;
            /// <summary>
            /// The center frequency (MHz) of each band. 
            /// </summary>
            public float[] fc_list;
            /// <summary>
            /// The width of each frequency band (MHz). 
            /// </summary>
            public float[] bw_list;
            /// <summary>
            /// The frequency-domain baseband offset of the input waveform (MHz). 
            /// </summary>
            public float offset;
            /// <summary>
            /// The sampling rate (MHz) of the input waveform. Must be a positive value. 
            /// </summary>
            public float f_sample;
            /// <summary>
            /// The desired PAPR (dB) of the CFR output. Must be a non-negative value. 
            /// </summary>
            public float targetPAPR;
            /// <summary>
            /// For FDD waveforms set this to 1.0f. For a TDD DL waveform, set this to (number of downlink subframes/total subframes). For a TDD UL waveform, set this to (number of uplink subframes/total subframes). Valid values are [1,0).
            /// </summary>
            public float TDD_duty_cycle;
        }

        /// <summary>
        /// DPD Performance Levels.
        /// Adaptive DPD offers four tiers of DPD performance, which are enumerated here.
        /// Linearizer performance & execution time increase for higher performance levels.
        /// </summary>
        public enum DpdLevel
        {
            /// NST_DPD_LEVEL3 -> 3
            NST_DPD_LEVEL3 = 3,
            /// NST_DPD_LEVEL2 -> 2
            NST_DPD_LEVEL2 = 2,
            /// NST_DPD_LEVEL1 -> 1
            NST_DPD_LEVEL1 = 1,
            /// NST_DPD_LEVEL0 -> 0
            NST_DPD_LEVEL0 = 0,
        }

        /// <summary>
        /// DPD configuration Structure.
        /// Dictates the behavior of the DPD Training algorithms.
        /// struct NST_DPD_CONFIG_STRUCT
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct DpdConfig
        {
            /// <summary>
            /// Configures the performance level of the DPD algorithm
            /// </summary>
            public DpdLevel lvl;
            /// <summary>
            /// The robustness coefficient for DPD training. Must be a positive value. 
            /// </summary>
            public float rho;
            /// <summary>
            /// The absolute value of the endpoints of the VSG full-scale range, [-abs_vsg_max,abs_vsg_max]. Must be a positive value.
            /// </summary>
            public float abs_vsg_max;
            /// <summary>
            /// The number of samples to use when training the DPD. Must be a positive value less than the number of samples in the waveform. 
            /// </summary>
            public int training_samples;
            /// <summary>
            /// The sampling rate (MHz) of the input waveform. Must be a positive value. 
            /// </summary>
            public float f_sample;
            /// <summary>
            /// The DPD training algorithm will operate on input samples in the range [TDD_training_start_point , TDD_training_start_point + training_samples). For FDD, this can safely be set to 0. For TDD DL, choose this value so that the training algorithm is operating on a DL subframe. For TDD UL, choose this value so that the training algorithm is operating on a UL subframe. It is recommended that the subframe immediately after a UL-to-DL or DL-to-UL transition not be used for training.
            /// </summary>
            public int TDD_training_start_point;
        }

        #endregion
       
        #region Private Data Structures
        /// <summary>
        /// Separate version of CFR Configuration Struct to allow for easier DLL calling (where Fc and BW arrays are separate inputs to the CFR methods
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct CfrConfigLV
        {
            public int num_bands;
            public float offset;
            public float f_sample;
            public float targetPAPR;
            public float TDD_duty_cycle;
        }

        /// <summary>
        /// DPD trained or untrained.
        /// </summary>
        private enum dpdTrainedStatus
        {
            /// The DPD state has been modified by the training algorithm.
            NST_DPD_TRAINED = 1,
            /// The DPD state has been reset to its default value. In this state the DPD output matches its input.
            NST_DPD_UNTRAINED = 0, 
        }
        #endregion

        #region NanoSemi CFR
        /// <summary>
        /// Populates the CFR configuration structure with default settings.
        /// </summary>
        /// <returns>Default settings structure</returns>
        public static CfrConfig CfrGetDefaultConfig()
        {
            PInvoke.nst_dpd_cfr_get_default_config(out CfrConfig config);
            config.num_bands = 1;
            config.bw_list = new float[1] { 0.0f };
            config.fc_list = new float[1] { 0.0f };
            return config;
        }

        /// <summary>
        /// The CFR algorithm clip-and-filters the input waveform to satisfy config.targetPAPR.
        /// The input waveform must not contain Infinity or NaN samples.
        /// All fields of the CFR configuration struct must have valid settings.
        /// </summary>
        /// <param name="wfm_in_i">The I channel of the input waveform to apply CFR to.</param>
        /// <param name="wfm_in_q">The Q channel of the input waveform to apply CFR to.</param>
        /// <param name="wfm_out_i">Returns the I channel of the waveform with CFR applied, with the same number of samples as wfm_in.</param>
        /// <param name="wfm_out_q">Returns the Q channel of the waveform with CFR applied, with the same number of samples as wfm_in.</param>
        /// <param name="CfrConfiguration">Configures the behavior of the CFR algorithm.</param>
        public static void CfrApply(float[] wfm_in_i, float[] wfm_in_q, out float[] wfm_out_i, out float[] wfm_out_q, CfrConfig CfrConfiguration)
        {
            CfrConfigLV lvconf = new CfrConfigLV()
            {
                num_bands = CfrConfiguration.num_bands,
                offset = CfrConfiguration.offset,
                f_sample = CfrConfiguration.f_sample,
                targetPAPR = CfrConfiguration.targetPAPR,
                TDD_duty_cycle = CfrConfiguration.TDD_duty_cycle
            };

            int waveformLength = wfm_in_i.Length;
            float[] wfm_out_i_pinv = new float[waveformLength];
            float[] wfm_out_q_pinv = new float[waveformLength];

            int pInvokeResult = PInvoke.nst_dpd_cfr_apply_labview(wfm_in_i, wfm_in_q, waveformLength,  wfm_out_i_pinv,  wfm_out_q_pinv, lvconf, CfrConfiguration.fc_list, CfrConfiguration.bw_list);
            TestForError(pInvokeResult);
   
            wfm_out_i = wfm_out_i_pinv;
            wfm_out_q = wfm_out_q_pinv;
        }

        /// <summary>
        /// The CFR algorithm clip-and-filters the input waveform to satisfy config.targetPAPR.
        /// The input waveform must not contain Infinity or NaN samples.
        /// All fields of the CFR configuration struct must have valid settings.
        /// </summary>
        /// <param name="WaveformIn">The Input waveform to apply CFR to.</param>
        /// <param name="Cfrconfiguration">Configures the behavior of the CFR algorithm.</param>
        /// <returns>Waveform with CFR applied</returns>
        public static ComplexWaveform<ComplexSingle> CfrApply(ComplexWaveform<ComplexSingle> WaveformIn, CfrConfig CfrConfiguration)
        {
            ComplexSingle.DecomposeArray(WaveformIn.GetRawData(), out float[] IArray, out float[] QArray);
            CfrApply(IArray, QArray, out float[] WfmOutI, out float[] WfmOutQ, CfrConfiguration);

            //Handle the necessary Conversion from I/Q arrays to NI ComplexWaveform Datatype
            ComplexSingle[] PostCFRWaveformData = ComplexSingle.ComposeArray(WfmOutI, WfmOutQ);
            ComplexWaveform<ComplexSingle> CfrWaveformOut = new ComplexWaveform<ComplexSingle>(0);
            CfrWaveformOut.Append(PostCFRWaveformData);
            CfrWaveformOut.PrecisionTiming = WaveformIn.PrecisionTiming;

            return CfrWaveformOut;
        }
        #endregion

        #region NanoSemi DPD
        /// <summary>
        /// Populates the DPD configuration structure with default settings.
        /// </summary>
        /// <returns>Return the default settings structure.</returns>
        public static DpdConfig DpdGetDefaultConfig()
        {
            PInvoke.nst_dpd_get_default_config(out DpdConfig config);
            config.abs_vsg_max = 1.01f;
            return config;
        }

        /// <summary>
        /// Reset DPD to untrained state.
        /// This call(1) resets the DPD state to its default (untrained) value, and (2) sets the DPD performance level to the value of config.lvl.
        /// The DPD performance level must be valid.
        /// </summary>
        /// <param name="DpdConfiguration"></param>
        public static void DpdResetTraining(DpdConfig DpdConfiguration)
        {
            int pInvokeResult = PInvoke.nst_dpd_reset_training(DpdConfiguration);
            TestForError(pInvokeResult);
        }

        /// <summary>
        /// Query DPD training state.
        /// Indicates whether the DPD algorithm has been trained, or is in an untrained state.
        /// </summary>
        /// <returns>Returns the training state of the DPD algorithm (True = Trained)</returns>
        public static bool DpdQueryTrained()
        {
            int pInvokeResult = PInvoke.nst_dpd_query_trained(out dpdTrainedStatus trainedSt);
            TestForError(pInvokeResult);
            return (trainedSt == dpdTrainedStatus.NST_DPD_TRAINED);
        }
        /// <summary>
        /// Train the Adaptive DPD.
        /// Improve DPD linearization by comparing a predistorted waveform to the baseband DUT output waveform that results from applying that predistorted waveform to the DUT.
        /// 
        /// This function updates the DPD state in memory; it has no return values.
        /// The input waveforms must not contain Infinity or NaN samples.
        /// For the wfm_dpd argument, the magnitudes of the complex waveform samples must not exceed config.abs_vsg_max.
        /// For the wfm_Rx argument, the magnitudes of the complex waveform samples must not exceed config.abs_vsa_max.
        /// All fields of the DPD configuration struct must have valid settings.
        /// config.training_samples must be at least 5000.
        /// 
        /// </summary>
        /// <param name="wfm_dpd_i">The I channel of the predistorted waveform.</param>
        /// <param name="wfm_dpd_q">The Q channel of the predistorted waveform.</param>
        /// <param name="wfm_Rx_i">The I channel of the baseband DUT output waveform, with the same number of samples as wfm_in.</param>
        /// <param name="wfm_Rx_q">The Q channel of the baseband DUT output waveform, with the same number of samples as wfm_in.</param>
        /// <param name="DpdConfiguration">Configures the behavior of the training algorithm.</param>
        public static void DpdTrain(float[] wfm_dpd_i, float[] wfm_dpd_q, float[] wfm_Rx_i, float[] wfm_Rx_q, DpdConfig DpdConfiguration)
        {
            int waveformLength = wfm_Rx_i.Length;

            int pInvokeResult = PInvoke.nst_dpd_train(wfm_dpd_i, wfm_dpd_q, waveformLength, wfm_Rx_i, wfm_Rx_q, DpdConfiguration);
            TestForError(pInvokeResult);
        }

        /// <summary>
        /// Train the Adaptive DPD.
        /// Improve DPD linearization by comparing a predistorted waveform to the baseband DUT output waveform that results from applying that predistorted waveform to the DUT.
        /// 
        /// This function updates the DPD state in memory; it has no return values.
        /// The input waveforms must not contain Infinity or NaN samples.
        /// For the wfm_dpd argument, the magnitudes of the complex waveform samples must not exceed config.abs_vsg_max.
        /// All fields of the DPD configuration struct must have valid settings.
        /// config.training_samples must be at least 5000.
        /// 
        /// </summary>
        /// <param name="GeneratedWaveform">Waveform being generated to the DUT input</param>
        /// <param name="AcquiredWaveform">Acquired waveform from the DUT output</param>
        /// <param name="DpdConfiguration">Configures the behavior of the training algorithm.</param>
        public static void DpdTrain(ComplexWaveform<ComplexSingle> GeneratedWaveform, ComplexWaveform<ComplexSingle> AcquiredWaveform, DpdConfig DpdConfiguration)
        {
            ComplexSingle.DecomposeArray(GeneratedWaveform.GetRawData(), out float[] wfm_dpd_i, out float[] wfm_dpd_q);
            ComplexSingle.DecomposeArray(AcquiredWaveform.GetRawData(), out float[] wfm_Rx_i, out float[] wfm_Rx_q);
            DpdTrain(wfm_dpd_i, wfm_dpd_q, wfm_Rx_i, wfm_Rx_q, DpdConfiguration);
        }

        /// <summary>
        /// Predistort a waveform.
        /// 
        /// The input waveform must not contain Infinity or NaN samples.
        /// The magnitudes of the complex waveform samples must not exceed config.abs_vsg_max.
        /// All fields of the DPD configuration struct must have valid settings.
        /// 
        /// </summary>
        /// <param name="wfm_in_i">The I channel of the input waveform to apply DPD to.</param>
        /// <param name="wfm_in_q">The Q channel of the input waveform to apply DPD to.</param>
        /// <param name="wfm_out_i">Returns the I channel of the waveform with DPD applied, with the same number of samples as wfm_in.</param>
        /// <param name="wfm_out_q">Returns the Q channel of the waveform with DPD applied, with the same number of samples as wfm_in.</param>
        /// <param name="DpdConfiguration">Configures the behavior of the training algorithm.</param>
        public static void DpdApply(float[] wfm_in_i, float[] wfm_in_q, out float[] wfm_out_i, out float[] wfm_out_q, DpdConfig DpdConfiguration)
        {
            int waveformLength = wfm_in_i.Length;
            float[] wfm_out_i_pinv = new float[waveformLength];
            float[] wfm_out_q_pinv = new float[waveformLength];

            int pInvokeResult = PInvoke.nst_dpd_apply(wfm_in_i, wfm_in_q, waveformLength, wfm_out_i_pinv, wfm_out_q_pinv, DpdConfiguration);
            TestForError(pInvokeResult);

            wfm_out_i = wfm_out_i_pinv;
            wfm_out_q = wfm_out_q_pinv;
        }

        /// <summary>
        /// Predistort a waveform.
        /// 
        /// The input waveform must not contain Infinity or NaN samples.
        /// The magnitudes of the complex waveform samples must not exceed config.abs_vsg_max.
        /// All fields of the DPD configuration struct must have valid settings.</summary>
        /// <param name="WaveformIn">The Input waveform to apply DPD to.</param>
        /// <param name="Cfrconfiguration">Configures the behavior of the training algorithm.</param>
        /// <returns>Waveform with DPD applied</returns>
        public static ComplexWaveform<ComplexSingle> DpdApply(ComplexWaveform<ComplexSingle> WaveformIn, DpdConfig DpdConfiguration)
        {
            ComplexSingle.DecomposeArray(WaveformIn.GetRawData(), out float[] IArray, out float[] QArray);
            DpdApply(IArray, QArray, out float[] WfmOutI, out float[] WfmOutQ, DpdConfiguration);

            //Handle the necessary Conversion from I/Q arrays to NI ComplexWaveform Datatype
            ComplexSingle[] PostDPDWaveformData = ComplexSingle.ComposeArray(WfmOutI, WfmOutQ);
            ComplexWaveform<ComplexSingle> DpdWaveformOut = new ComplexWaveform<ComplexSingle>(0);
            DpdWaveformOut.Append(PostDPDWaveformData);
            DpdWaveformOut.PrecisionTiming = WaveformIn.PrecisionTiming;

            return DpdWaveformOut;
        }
        #endregion
        
        #region Utilities
                /// <summary>
                /// NSTDPD software module version. Get the version number of this release of NSTDPD.
                /// </summary>
                /// <returns>NSTDPD Version Number</returns>
                public static float GetVersion()
                {
                    int pInvokeResult = PInvoke.nst_dpd_version(out float version);
                    TestForError(pInvokeResult);
                    return version;
                }
        #endregion
        
        #region PInvoke
        private class PInvoke
        {
            private const string nstdpddll = @"C:\NanoSemi\bin\nstdpd.dll";

            [DllImport(nstdpddll, EntryPoint = "nst_dpd_error_description", CallingConvention = CallingConvention.StdCall)]
            public static extern int nst_dpd_error_description(int code, out IntPtr description);

            [DllImport(nstdpddll, EntryPoint = "nst_dpd_version", CallingConvention = CallingConvention.StdCall)]
            public static extern int nst_dpd_version(out float version);

            [DllImport(nstdpddll, EntryPoint = "nst_dpd_cfr_get_default_config", CallingConvention = CallingConvention.StdCall)]
            public static extern int nst_dpd_cfr_get_default_config(out CfrConfig config);
            
            [DllImport(nstdpddll, EntryPoint = "nst_dpd_cfr_apply_labview", CallingConvention = CallingConvention.StdCall)]          
            public static extern int nst_dpd_cfr_apply_labview([In] float[] wfm_in_i, [In] float[] wfm_in_q, int count, [In, Out] float[] wfm_out_i, [In, Out] float[] wfm_out_q, CfrConfigLV config, [In] float[] fc_list, [In] float[] bw_list);

            [DllImport(nstdpddll, EntryPoint = "nst_dpd_reset_training", CallingConvention = CallingConvention.StdCall)]
            public static extern int nst_dpd_reset_training(DpdConfig DpdConfiguration);

            [DllImport(nstdpddll, EntryPoint = "nst_dpd_train", CallingConvention = CallingConvention.StdCall)]
            public static extern int nst_dpd_train(float[] wfm_dpd_i, float[] wfm_dpd_q, int count, float[] wfm_Rx_i, float[] wfm_Rx_q, DpdConfig DpdConfiguration);

            [DllImport(nstdpddll, EntryPoint = "nst_dpd_train_without_align", CallingConvention = CallingConvention.Cdecl)]
            public static extern int nst_dpd_train_without_align(float[] wfm_dpd_i, float[] wfm_dpd_q, int count, float[] wfm_Rx_i, float[] wfm_Rx_q, DpdConfig DpdConfiguration);

            [DllImport(nstdpddll, EntryPoint = "nst_dpd_apply", CallingConvention = CallingConvention.StdCall)]
            public static extern int nst_dpd_apply([In] float[] wfm_in_i, [In] float[] wfm_in_q, int count, [In, Out] float[] wfm_out_i, [In, Out] float[] wfm_out_q, DpdConfig DpdConfiguration);

            [DllImport(nstdpddll, EntryPoint = "nst_dpd_query_trained", CallingConvention = CallingConvention.StdCall)]
            public static extern int nst_dpd_query_trained(out dpdTrainedStatus status);

            [DllImport(nstdpddll, EntryPoint = "nst_dpd_get_default_config", CallingConvention = CallingConvention.StdCall)]
            public static extern int nst_dpd_get_default_config(out DpdConfig config);
        }
        private static int TestForError(int status)
        {
            if (status < 0)
            {
                string errorString = GetError(status);
                throw new ExternalException(errorString, status);
            }
            return status;
        }
        private static string GetError(int code)
        {
            PInvoke.nst_dpd_error_description(code, out IntPtr errorPtr);
            string errorString = Marshal.PtrToStringAnsi(errorPtr);
            return errorString;
        }
        #endregion
    }
}
