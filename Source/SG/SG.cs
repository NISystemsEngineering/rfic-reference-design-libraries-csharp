using NationalInstruments.DataInfrastructure;
using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.ModularInstruments.NIRfsgPlayback;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;

namespace NationalInstruments.ReferenceDesignLibraries
{
    /// <summary>Defines common types and methods for generating waveforms with NI-RFSG.</summary>
    public static class SG
    {
        //Suppress warning for obselete code as LoadWaveformFromTDMS intentionally uses 
        //an outdated method in order to support older waveform files
#pragma warning disable CS0612

        #region Type Definitions
        /// <summary>Defines common instrument settings used for generation.</summary>
        public struct InstrumentConfiguration
        {
            /// <summary>Specifies the port to configure.</summary>
            public string SelectedPorts;
            /// <summary>Specifies the source of the Reference Clock signal.</summary>
            public string ReferenceClockSource;
            /// <summary>Specifies the frequency of the generated RF signal in Hz. For arbitrary waveform generation, this property specifies the center frequency of the signal.</summary>
            public double CarrierFrequency_Hz;
            /// <summary>Specifies the desired average input power in dBm that the DUT should receive from the generator.</summary>
            public double DutAverageInputPower_dBm;
            /// <summary>Specifies the external attenuation, if any, in dB between the RF signal generator and the device under test.</summary>
            public double ExternalAttenuation_dB;
            /// <summary>Defines the local oscillator sharing behavior for VST devices.</summary>
            public LocalOscillatorSharingMode LOSharingMode;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static InstrumentConfiguration GetDefault()
            {
                return new InstrumentConfiguration()
                {
                    SelectedPorts = "",
                    ReferenceClockSource = RfsgFrequencyReferenceSource.PxiClock.ToString(),
                    CarrierFrequency_Hz = 1e9,
                    DutAverageInputPower_dBm = -10.0,
                    ExternalAttenuation_dB = 0.0,
                    LOSharingMode = LocalOscillatorSharingMode.Automatic
                };
            }

            /// <summary>Returns the struct with default values set based upon the instrument model of <paramref name="rfsg"/>.</summary>
            /// <param name="rfsg">The open RFSG session to configure.</param>
            /// <returns>The struct with default values set based upon the instrument model of <paramref name="rfsg"/>.</returns>
            public static InstrumentConfiguration GetDefault(NIRfsg rfsg)
            {
                InstrumentConfiguration instrConfig = GetDefault();
                string instrumentModel = rfsg.Identity.InstrumentModel;
                if (Regex.IsMatch(instrumentModel, "NI PXIe-5830"))
                {
                    instrConfig.SelectedPorts = "if0";
                    instrConfig.CarrierFrequency_Hz = 6.5e9;
                }
                else if (Regex.IsMatch(instrumentModel, "NI PXIe-5831"))
                {
                    instrConfig.SelectedPorts = "rf0/port0";
                    instrConfig.CarrierFrequency_Hz = 28e9;
                }
                return instrConfig;
            }
        }

        /// <summary>Defines timing information for configuring bursted waveform generation.</summary>
        public struct WaveformTimingConfiguration
        {
            /// <summary>Specifes the percentage of the generation period in which the signal should be active. Idle time will be added to the generation script in order to 
            /// create the requested duty cycle.</summary>
            public double DutyCycle_Percent;
            /// <summary>Specifies the time in seconds prior to the start of the RF burst in which the DUT should be enabled.</summary>
            public double PreBurstTime_s;
            /// <summary>Specifies the time in seconds after the conclusion of the RF burst before which the DUT should be disabled.</summary>
            public double PostBurstTime_s;
            /// <summary>Specifies the trigger terminal in which to export a trigger on the first sample of the burst.</summary>
            public string BurstStartTriggerExport;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static WaveformTimingConfiguration GetDefault()
            {
                return new WaveformTimingConfiguration()
                {
                    DutyCycle_Percent = 50.0,
                    PreBurstTime_s = 1e-6,
                    PostBurstTime_s = 1e-6,
                    BurstStartTriggerExport = "PXI_Trig0"
                };
            }
        }

        /// <summary>Defines different modes for controlling the DUT state when using bursted generation.</summary>
        public enum PAENMode
        {
            /// <summary>Disables exporting a signal to control a DUT during generation.</summary>
            Disabled,
            /// <summary>Exports a signal to turn on a DUT at the start of generation, and exports a signal to turn off the DUT after <see cref="AbortGeneration(NIRfsg, int)"/> is called.</summary>
            Static,
            /// <summary>Exports a signal to dynamically turn on and off a DUT just before and just after each RF burst from the signal generator.</summary>
            Dynamic
        };

        /// <summary>Defines different parameters for controlling a DUT when using bursted generation.</summary>
        public struct PAENConfiguration
        {
            /// <summary>Specifies the mode to use for controlling the DUT state during generation.</summary>
            public PAENMode PAEnableMode;
            /// <summary>Specifies the behavior of the signal specified in <see cref="PAEnableTriggerExportTerminal"/>. If controlling the DUT directly with the PFI line, set
            /// the value to <see cref="RfsgMarkerEventOutputBehaviour.Toggle"/> to toggle the digital line high and low with each burst. If controlling the DUT with another instrument, set the value
            /// to <see cref="RfsgMarkerEventOutputBehaviour.Pulse"/> to trigger the instrument before and after each burst.</summary>
            public RfsgMarkerEventOutputBehaviour PAEnableTriggerMode;
            /// <summary>Specifies the length of time in seconds required to send the enable command to the DUT. This is only used when the DUT state is controlled via digital commands, and should be set based
            /// on the length of the command pattern being sent. The default value of 0 is sufficient for directly controlling the DUT state via the PFI line.</summary>
            public double CommandEnableTime_s;
            /// <summary>Specifies the length of time in seconds required to send the disable command to the DUT. This is only used when the DUT state is controlled via digital commands, and should be set based
            /// on the length of the command pattern being sent. The default value of 0 is sufficient for directly controlling the DUT state via the PFI line.</summary>
            public double CommandDisableTime_s;
            /// <summary>Specifies the terminal in which to export a signal used to enable and disable the DUT.</summary>
            public string PAEnableTriggerExportTerminal;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static PAENConfiguration GetDefault()
            {
                return new PAENConfiguration
                {
                    //Default configuration is set for a DUT with a simple digital toggle high/low
                    //for PA Enable
                    PAEnableMode = PAENMode.Dynamic,
                    PAEnableTriggerExportTerminal = RfsgMarkerEventExportedOutputTerminal.Pfi0.ToString(),
                    PAEnableTriggerMode = RfsgMarkerEventOutputBehaviour.Toggle,
                    CommandEnableTime_s = 0,
                    CommandDisableTime_s = 0
                };
            }
        }
        #endregion

        /// <summary>Configures common instrument settings for generation.</summary>
        /// <param name="rfsgHandle">The open RFSG session to configure.</param>
        /// <param name="instrConfig">The common instrument settings to configure.</param>
        public static void ConfigureInstrument(NIRfsg rfsgHandle, InstrumentConfiguration instrConfig)
        {
            rfsgHandle.SignalPath.SelectedPorts = instrConfig.SelectedPorts;
            rfsgHandle.RF.ExternalGain = -instrConfig.ExternalAttenuation_dB;
            rfsgHandle.RF.Configure(instrConfig.CarrierFrequency_Hz, instrConfig.DutAverageInputPower_dBm);

            rfsgHandle.FrequencyReference.Source = RfsgFrequencyReferenceSource.FromString(instrConfig.ReferenceClockSource);

            // Only configure LO settings on supported VSTs
            if (Regex.IsMatch(rfsgHandle.Identity.InstrumentModel, "NI PXIe-58[34].")) // Matches 583x and 584x VST families
            {
                IntPtr rfsgPtr = rfsgHandle.GetInstrumentHandle().DangerousGetHandle();
                NIRfsgPlayback.RetrieveAutomaticSGSASharedLO(rfsgPtr, "", out RfsgPlaybackAutomaticSGSASharedLO currentMode);
                if (instrConfig.LOSharingMode == LocalOscillatorSharingMode.None && currentMode != RfsgPlaybackAutomaticSGSASharedLO.Disabled)
                {
                    //Setting this property resets other settings, which can create issues. Hence, it is only set if the value
                    //is different than the current mode.
                    NIRfsgPlayback.StoreAutomaticSGSASharedLO(rfsgPtr, "", RfsgPlaybackAutomaticSGSASharedLO.Disabled);
                }
                else if (instrConfig.LOSharingMode == LocalOscillatorSharingMode.Automatic && currentMode != RfsgPlaybackAutomaticSGSASharedLO.Enabled)
                {
                    //Setting this property resets other settings, which can create issues. Hence, it is only set if the value
                    //is different than the current mode.
                    NIRfsgPlayback.StoreAutomaticSGSASharedLO(rfsgPtr, "", RfsgPlaybackAutomaticSGSASharedLO.Enabled);
                }
            }
            //Do nothing; any configuration for LOs with standalone VSGs should be configured manually. 
            //Baseband instruments don't have LOs. Unsupported VSTs must be configured manually.
        }

        /// <summary>Loads a waveform and relevant properties from a TDMS file.</summary>
        /// <param name="filePath">Specifies the absolute path to the .TDMS waveform file on disk.</param>
        /// <param name="waveformName">(Optional) Specifies the name to use to represent the waveform. The file name will be used by default.</param>
        /// <returns>The waveform data and associated properties represented in the Waveform type.</returns>
        public static Waveform LoadWaveformFromTDMS(string filePath, string waveformName = "")
        {
            Waveform waveform = new Waveform();

            if (string.IsNullOrEmpty(waveformName))
            {
                waveformName = Path.GetFileNameWithoutExtension(filePath);
                waveformName = FormatWaveformName(waveformName);
            }

            waveform.Name = waveformName;
            NIRfsgPlayback.ReadWaveformFromFileComplex(filePath, ref waveform.Data);
            NIRfsgPlayback.ReadWaveformFileVersionFromFile(filePath, out string waveformVersion);

            if (waveformVersion == "1.0.0")
            {
                // 1.0.0 waveforms use peak power adjustment = papr + runtime scaling
                // we will scale the waveform and calculate papr and runtime scaling manually
                float peak = ComplexSingle.GetMagnitudes(waveform.Data.GetRawData()).Max();
                waveform.RuntimeScaling = 20.0 * Math.Log10(peak);
                NIRfsgPlayback.ReadPeakPowerAdjustmentFromFile(filePath, 0, out double peakPowerAdjustment);
                waveform.PAPR_dB = peakPowerAdjustment + waveform.RuntimeScaling;

                // scale the waveform to full scale
                WritableBuffer<ComplexSingle> waveformBuffer = waveform.Data.GetWritableBuffer();
                ComplexSingle scale = ComplexSingle.FromPolar(1.0f / peak, 0.0f);
                for (int i = 0; i < waveform.Data.SampleCount; i++)
                    waveformBuffer[i] = waveformBuffer[i] * scale; // multiplication is faster than division
            }
            else
            {
                NIRfsgPlayback.ReadPaprFromFile(filePath, 0, out waveform.PAPR_dB); //Version 2.0.0 and later
                NIRfsgPlayback.ReadRuntimeScalingFromFile(filePath, 0, out waveform.RuntimeScaling);
            }

            NIRfsgPlayback.ReadSampleRateFromFile(filePath, 0, out waveform.SampleRate);
            NIRfsgPlayback.ReadSignalBandwidthFromFile(filePath, 0, out waveform.SignalBandwidth_Hz);
            if (waveform.SignalBandwidth_Hz == 0.0)
                waveform.SignalBandwidth_Hz = 0.8 * waveform.SampleRate;

            NIRfsgPlayback.ReadBurstStartLocationsFromFile(filePath, 0, ref waveform.BurstStartLocations);
            NIRfsgPlayback.ReadBurstStopLocationsFromFile(filePath, 0, ref waveform.BurstStopLocations);
            //Statement reads: if NOT BurstStartLocations > 0 AND expression is not null (? operand)
            //In other words, manually set BurstStartLocations when the length is 0 or less or array is null
            if (!(waveform.BurstStartLocations?.Length > 0))
                waveform.BurstStartLocations = new int[1] { 0 }; //Set burst start to the first sample(0)
            if (!(waveform.BurstStopLocations?.Length > 0))
                waveform.BurstStopLocations = new int[1] { waveform.Data.SampleCount - 1 }; //Set burst stop to the last sample (number of samples minus one)

            // calculating IdleDurationPresent like this also accounts for tools like wlan sfp that put in burst start and stop locations even if there is no idle time in the waveform
            waveform.IdleDurationPresent = waveform.BurstStopLocations.First() - waveform.BurstStartLocations.First() < waveform.Data.SampleCount - 1;

            waveform.BurstLength_s = CalculateWaveformDuration(waveform.BurstStartLocations, waveform.BurstStopLocations, waveform.SampleRate);

            return waveform;
        }

        /// <summary>Downloads a previously loaded waveform to the instrument and sets associated properties.</summary>
        /// <param name="rfsgHandle">The open RFSG session to configure.</param>
        /// <param name="waveform">The waveform data and associated properties to download to the instrument. Use <see cref="LoadWaveformFromTDMS(string, string)"/> to load a waveform from a TDMS file.</param>
        public static void DownloadWaveform(NIRfsg rfsgHandle, Waveform waveform)
        {
            IntPtr rfsgPtr = rfsgHandle.GetInstrumentHandle().DangerousGetHandle();
            rfsgHandle.Abort();

            try
            {
                rfsgHandle.Arb.ClearWaveform(waveform.Name); //Clear existing waveform to avoid erros
            }
            catch (Exception ex)
            {
                if (ex is Ivi.Driver.OutOfRangeException || ex is Ivi.Driver.IviCDriverException) { } //Intentionally ignore this exception; clearing the waveform failed because it doesn't exist
                else
                    throw;
            }

            rfsgHandle.RF.PowerLevelType = RfsgRFPowerLevelType.PeakPower; // set power level to peak before writing so RFSG doesn't scale waveform
            rfsgHandle.Arb.WriteWaveform(waveform.Name, waveform.Data);

            //Store loaded parameters
            NIRfsgPlayback.StoreWaveformSignalBandwidth(rfsgPtr, waveform.Name, waveform.SignalBandwidth_Hz);
            NIRfsgPlayback.StoreWaveformPapr(rfsgPtr, waveform.Name, waveform.PAPR_dB);
            NIRfsgPlayback.StoreWaveformBurstStartLocations(rfsgPtr, waveform.Name, waveform.BurstStartLocations);
            NIRfsgPlayback.StoreWaveformBurstStopLocations(rfsgPtr, waveform.Name, waveform.BurstStopLocations);
            NIRfsgPlayback.StoreWaveformSampleRate(rfsgPtr, waveform.Name, waveform.SampleRate);
            NIRfsgPlayback.StoreWaveformRuntimeScaling(rfsgPtr, waveform.Name, waveform.RuntimeScaling);
            NIRfsgPlayback.StoreWaveformSize(rfsgPtr, waveform.Name, waveform.Data.SampleCount);

            //Manually configure additional settings
            NIRfsgPlayback.StoreWaveformLOOffsetMode(rfsgPtr, waveform.Name, NIRfsgPlaybackLOOffsetMode.Auto);
            NIRfsgPlayback.StoreWaveformRFBlankingEnabled(rfsgPtr, waveform.Name, false);
        }

        /// <summary>Configures the generator to generate the waveform continuously with a trigger on the first sample of the waveform. Use <see cref="AbortGeneration(NIRfsg, int)"/>
        /// to abort generation to always ensure complete packet generation.</summary>
        /// <param name="rfsgHandle">The open RFSG session to configure.</param>
        /// <param name="waveform">Specifies the waveform to generate continuously. Call <see cref="DownloadWaveform(NIRfsg, Waveform)"/> prior to calling this function.</param>
        /// <param name="markerEventExportTerminal">(Optional) Specifies the terminal name where the trigger at the start of the waveform should be exported to. 
        /// The default value is to generate a trigger on the PXI_Trig0 line.</param>
        /// <returns>The Waveform with the "Script" parameter set to the newly created generation script.</returns>
        public static Waveform ConfigureContinuousGeneration(NIRfsg rfsgHandle, Waveform waveform, string markerEventExportTerminal = "PXI_Trig0")
        {
            //Configure the trigger to be generated on the first sample of each waveform generation,
            //denoted in the script below as "marker0"
            rfsgHandle.DeviceEvents.MarkerEvents[0].ExportedOutputTerminal = RfsgMarkerEventExportedOutputTerminal.FromString(markerEventExportTerminal);

            //A software trigger is configured that is used in the script below to control generation of
            //the script. This ensures that a complete packet is always generated before aborting, and
            //allows all generation functions to share a single abort function.
            rfsgHandle.Triggers.ScriptTriggers[0].ConfigureSoftwareTrigger();

            // Create continuous generation script that is unique to the waveform
            waveform.Script = $@"script REPEAT{waveform.Name}
                                    repeat until ScriptTrigger0
                                        generate {waveform.Name} marker0(0)
                                    end repeat
                                end script";

            // Configure the instrument to generate this waveform
            ApplyWaveformAttributes(rfsgHandle, waveform);

            // Return updated waveform struct to caller
            return waveform;
        }

        /// <summary>Configures the generator to generate the waveform plus idle time continuously for Dynamic EVM test cases. Also configures triggering or pulsing settings to 
        /// control the DUT. Use <see cref="AbortGeneration(NIRfsg, int)"/> to abort generation to always ensure that the DUT state is returned to the initial state.</summary>
        /// <param name="rfsgHandle">The open RFSG session to configure.</param>
        /// <param name="waveform">Specifies the waveform to generate; its burst length will be used in conjuction with the duty cycle to calculate the idle time.
        /// Call <see cref="DownloadWaveform(NIRfsg, Waveform)"/> prior to calling this function.</param>
        /// <param name="waveTiming">Specifies the timing parameters used to configure the bursted generation.</param>
        /// <param name="paenConfig">Specifies parameters pertaining to how the DUT is controlled during generation.</param>
        /// <param name="period">Returns the total generation period, consisting of the waveform burst length, idle time, and any additional pre/post burst time configured.</param>
        /// <param name="idleTime">Returns the computed idle time based upon the requested duty cycle.</param>
        /// <returns>The Waveform with the "Script" parameter set to the newly created generation script.</returns>
        public static Waveform ConfigureBurstedGeneration(NIRfsg rfsgHandle, Waveform waveform, WaveformTimingConfiguration waveTiming,
            PAENConfiguration paenConfig, out double period, out double idleTime)
        {
            // Validate input parameters
            if (waveTiming.DutyCycle_Percent <= 0 || waveTiming.DutyCycle_Percent >= 100)
            {
                throw new ArgumentOutOfRangeException("DutyCycle_Percent", waveTiming.DutyCycle_Percent, "Duty cycle must be greater than 0% and less than 100%. " +
                    "For a duty cycle of 100%, use SG.ConfigureContinuous generation instead.");
            }
            if (waveTiming.PreBurstTime_s <= 0 || waveTiming.PostBurstTime_s <= 0)
            {
                throw new ArgumentOutOfRangeException("PreBurstTime | PostBurstTime", "PreBurstTime and PostBurstTime must be greater than 0 seconds");
            }

            //Download the generation script to the generator for later initiation
            waveform.Script = GenerateBurstedScript(paenConfig, waveTiming, waveform, out period, out idleTime);
            ApplyWaveformAttributes(rfsgHandle, waveform);

            //Configure the triggering for PA enable if selected
            if (paenConfig.PAEnableMode != PAENMode.Disabled)
            {
                rfsgHandle.DeviceEvents.MarkerEvents[1].ExportedOutputTerminal = RfsgMarkerEventExportedOutputTerminal.FromString(
                    paenConfig.PAEnableTriggerExportTerminal);
                rfsgHandle.DeviceEvents.MarkerEvents[1].OutputBehaviour = paenConfig.PAEnableTriggerMode;

                // Ensure that the initial state for the digital line is low when using toggle mode to ensure the DUT state is correct
                rfsgHandle.DeviceEvents.MarkerEvents[1].ToggleInitialState = RfsgMarkerEventToggleInitialState.DigitalLow;
            }
            //Configure scriptTrigger0 for software triggering. This way, when it is time to abort we can stop
            //the loop and trigger the appropriate off command if PAEN mode is Static
            rfsgHandle.Triggers.ScriptTriggers[0].ConfigureSoftwareTrigger();

            //Configure the trigger to be generated on the first sample of each waveform generation,
            //denoted in the script below as "marker0"
            rfsgHandle.DeviceEvents.MarkerEvents[0].ExportedOutputTerminal =
                RfsgMarkerEventExportedOutputTerminal.FromString(waveTiming.BurstStartTriggerExport);

            // Return updated waveform struct to caller
            return waveform;
        }

        /// <summary>Calls <see cref="NIRfsgPlayback.SetScriptToGenerateSingleRfsg(IntPtr, string)"/>, which will download the script contained in <paramref name="waveform"/> and apply
        /// all associated parameters.</summary>
        /// <param name="rfsgHandle">The open RFSG session to configure.</param>
        /// <param name="waveform">Specifies the waveform and its associated script that is to be used for generation.</param>
        public static void ApplyWaveformAttributes(NIRfsg rfsgHandle, Waveform waveform)
        {
            if (string.IsNullOrEmpty(waveform.Script)) // default to continuous if no script in waveform
                ConfigureContinuousGeneration(rfsgHandle, waveform);
            else
            {
                IntPtr rfsgPtr = rfsgHandle.GetInstrumentHandle().DangerousGetHandle();
                NIRfsgPlayback.SetScriptToGenerateSingleRfsg(rfsgPtr, waveform.Script);
            }
        }

        /// <summary>Retrieves waveform parameters from the <see cref="NIRfsgPlayback"/> waveform database based on the waveform name. 
        /// NOTE - this does not return any value for <see cref="Waveform.Data"/>. This data cannot be retrieved from the database.</summary>
        /// <param name="rfsgHandle">The open RFSG session.</param>
        /// <param name="waveformName">Specifies the name of the waveform to use to lookup its properties.</param>
        /// <returns>The waveform parameters associated with the specified name.</returns>
        public static Waveform GetWaveformParametersByName(NIRfsg rfsgHandle, string waveformName)
        {
            IntPtr rfsgPtr = rfsgHandle.GetInstrumentHandle().DangerousGetHandle();

            Waveform waveform = new Waveform
            {
                Name = waveformName
            };

            NIRfsgPlayback.RetrieveWaveformSignalBandwidth(rfsgPtr, waveformName, out waveform.SignalBandwidth_Hz);
            NIRfsgPlayback.RetrieveWaveformPapr(rfsgPtr, waveformName, out waveform.PAPR_dB);
            NIRfsgPlayback.RetrieveWaveformSampleRate(rfsgPtr, waveformName, out waveform.SampleRate);
            NIRfsgPlayback.RetrieveWaveformBurstStartLocations(rfsgPtr, waveformName, ref waveform.BurstStartLocations);
            NIRfsgPlayback.RetrieveWaveformBurstStopLocations(rfsgPtr, waveformName, ref waveform.BurstStopLocations);
            NIRfsgPlayback.RetrieveWaveformSize(rfsgPtr, waveformName, out int waveformSize);
            waveform.IdleDurationPresent = waveform.BurstStopLocations.First() - waveform.BurstStartLocations.First() < waveformSize - 1;
            NIRfsgPlayback.RetrieveWaveformRuntimeScaling(rfsgPtr, waveformName, out waveform.RuntimeScaling);

            waveform.BurstLength_s = CalculateWaveformDuration(waveform.BurstStartLocations, waveform.BurstStopLocations, waveform.SampleRate);

            return waveform;
        }

        /// <summary>Notifies running scripts configured with <see cref="ConfigureContinuousGeneration(NIRfsg, Waveform, string)"/> or 
        /// <see cref="ConfigureBurstedGeneration(NIRfsg, Waveform, WaveformTimingConfiguration, PAENConfiguration, out double, out double)"/>
        /// to enter the cleanup state, and then aborts generation. This function ensures that these scripts always reach a finished state before
        /// aborting, so as to ensure the DUT is in the desired state at the end of generation.</summary>
        /// <param name="rfsgHandle">The open RFSG session to configure.</param>
        /// <param name="timeOut_ms">(Optional) The timeout to wait for generation to complete before manually aborting.</param>
        public static void AbortGeneration(NIRfsg rfsgHandle, int timeOut_ms = 1000)
        {
            //This should trigger the generator to stop infinite generation and trigger any post
            //generation commands. For the static PA enable case, this should trigger the requisite
            //off command to disable the PA.
            rfsgHandle.Triggers.ScriptTriggers[0].SendSoftwareEdgeTrigger();

            int sleepTime_ms = 20;
            int maxIterations = (int)Math.Ceiling((double)timeOut_ms / sleepTime_ms);
            RfsgGenerationStatus genStatus = rfsgHandle.CheckGenerationStatus();

            //Poll the generation status until it is complete or the timeout period is reached
            if (genStatus == RfsgGenerationStatus.InProgress)
            {
                for (int i = 0; i < maxIterations; i++)
                {
                    genStatus = rfsgHandle.CheckGenerationStatus();
                    if (genStatus == RfsgGenerationStatus.Complete)
                        break;
                    else
                        Thread.Sleep(sleepTime_ms);
                }

                //This will only be true if we time out
                if (genStatus == RfsgGenerationStatus.InProgress)
                {
                    //If we timeed out then we need to call an explicit abort
                    rfsgHandle.Abort();
                    throw new System.ComponentModel.WarningException("Generation did not complete in the specified timeout period, so post-script actions did not complete." +
                        " If using bursted generation, you may need to manually disable the PA control line." +
                        " Increase the timeout period or ensure that scripTrigger0 is properly configured to stop generation");
                }
            }
        }

        /// <summary>Aborts generation, disables the output, and closes the instrument session.</summary>
        /// <param name="rfsgHandle">The open RFSG session to close.</param>
        public static void CloseInstrument(NIRfsg rfsgHandle)
        {
            rfsgHandle.Abort();
            rfsgHandle.RF.OutputEnabled = false;
            rfsgHandle.Close();
        }

        /// <summary>Formats the waveform name in order to avoid any errors when used in a script or downloaded to the generator.</summary>
        /// <param name="waveformName">The waveform name to format.</param>
        /// <returns>The formatted waveform name.</returns>
        public static string FormatWaveformName(string waveformName)
        {
            //The RFSG playback library and script compiler won't accept names with non-text/numeric characters
            waveformName = System.Text.RegularExpressions.Regex.Replace(waveformName, "[^a-zA-Z0-9]", ""); //Remove all non-text/numeric characters
            waveformName = string.Concat("Wfm", waveformName);
            return waveformName.ToUpper();
        }

        /// <summary>Creates the generation script used for bursted generation.</summary>
        /// <param name="waveTiming">Specifies the timing parameters used to configure the bursted generation.</param>
        /// <param name="paenConfig">Specifies parameters pertaining to how the DUT is controlled during generation.</param>
        /// <param name="waveform">Specifies the waveform to generate; its burst length will be used in conjuction with the duty cycle to calculate the idle time.</param>
        /// <param name="generationPeriod_s">Returns the total generation period, consisting of the waveform burst length, idle time, and any additional pre/post burst time configured.</param>
        /// <param name="idleTime_s">Returns the computed idle time based upon the requested duty cycle.</param>
        /// <returns>The script to implement the requested bursted generation timing.</returns>
        private static string GenerateBurstedScript(PAENConfiguration paenConfig, WaveformTimingConfiguration waveTiming, Waveform waveform,
            out double generationPeriod_s, out double idleTime_s)
        {
            string scriptName = string.Format("{0}{1}", waveform.Name, waveTiming.DutyCycle_Percent);

            //Calculate various timining information
            double dutyCycle = waveTiming.DutyCycle_Percent / 100;
            double totalBurstTime = waveTiming.PreBurstTime_s + waveform.BurstLength_s + waveTiming.PostBurstTime_s;
            idleTime_s = (totalBurstTime / dutyCycle) - totalBurstTime;
            generationPeriod_s = totalBurstTime + idleTime_s;

            //Convert all time based values to sample based values
            long preBurstSamp, postBurstSamp, idleSamp, enableSamples, disableSamples;
            preBurstSamp = TimeToSamples(waveTiming.PreBurstTime_s, waveform.SampleRate);
            postBurstSamp = TimeToSamples(waveTiming.PostBurstTime_s, waveform.SampleRate);
            idleSamp = TimeToSamples(idleTime_s, waveform.SampleRate);
            enableSamples = TimeToSamples(paenConfig.CommandEnableTime_s, waveform.SampleRate);
            disableSamples = TimeToSamples(paenConfig.CommandDisableTime_s, waveform.SampleRate);

            //RFSG enforces a minimum wait time of 8 samples, so ensure that the minimum pre/post burst time
            // and idle time are at least 8 samples long
            if (preBurstSamp < 8) preBurstSamp = 8;
            if (postBurstSamp < 8) postBurstSamp = 8;
            if (idleSamp < 8) idleSamp = 8;

            //Initialize the script StringBuilder with the first line of the script (name)
            StringBuilder sb = new StringBuilder($"script {scriptName}");
            sb.AppendLine();

            #region Script Building
            //If we have a static PA Enable mode, ensure that we trigger at the beginning of the script prior to looping.
            if (paenConfig.PAEnableMode == PAENMode.Static)
                sb.AppendLine("wait 8 marker1(7)");

            //Configure for endless repeating
            sb.AppendLine("Repeat until scriptTrigger0");

            //Configure the idle time prior to each packet generation
            sb.Append($"wait {idleSamp}");

            //If PAEN Mode is dynamic we need to trigger the PA to enable
            if (paenConfig.PAEnableMode == PAENMode.Dynamic)
            {
                //PA Enable is triggered at or before the last sample of the wait period
                long PAEnableTriggerLoc = idleSamp - enableSamples - 1;
                sb.Append($" marker1({PAEnableTriggerLoc})");
            }

            sb.AppendLine();

            //Configure waiting for the pre-burst time
            sb.AppendLine($"wait {preBurstSamp}");

            //Configure generation of the selected waveform but only for the burst length; send a trigger at the beginning of each burst
            sb.Append($"generate {waveform.Name} subset({waveform.BurstStartLocations[0]},{waveform.BurstStopLocations[0]}) marker0(0)");

            //Check to see if the command time is longer than the post-burst time, which determines when the PA disable command needs sent
            bool LongCommand = waveTiming.PostBurstTime_s <= paenConfig.CommandDisableTime_s;

            if (paenConfig.PAEnableMode == PAENMode.Dynamic && LongCommand)
            {
                //Trigger is placed a number of samples from the end of the burst corresponding with
                //how much longer than the post burst time it is
                long PADisableTriggerLoc = waveform.BurstStopLocations[0] - (disableSamples - postBurstSamp) - 1;
                sb.Append($" marker1({PADisableTriggerLoc})");
            }
            sb.AppendLine();

            //Configure waiting for the post-burst time
            sb.Append($"wait {postBurstSamp}");

            //If the ommand time is shorter than the post-burst time, the disable trigger must be sent
            //during the post-burst time 
            if (paenConfig.PAEnableMode == PAENMode.Dynamic && !LongCommand)
            {
                long PADisableTriggerLoc = postBurstSamp - disableSamples - 1;
                sb.Append($" marker1({PADisableTriggerLoc})");
            }
            sb.AppendLine();
            //Close out the script
            sb.AppendLine("end repeat");

            //If we have a static PA Enable mode, ensure that we trigger at the end of the script prior to concluding.
            if (paenConfig.PAEnableMode == PAENMode.Static)
                sb.AppendLine("wait 10 marker1(0)");

            sb.AppendLine("end script");
            #endregion

            return sb.ToString();
        }

        /// <summary>Converts samples to time based on the sample rate.</summary>
        /// <param name="time">Specifies the time in seconds.</param>
        /// <param name="sampleRate">Specifies the sample rate in samples/second.</param>
        /// <returns>The rounded sample count pertaining to the time value specified.</returns>
        private static long TimeToSamples(double time, double sampleRate)
        {
            return (long)Math.Round(time * sampleRate);
        }

        /// <summary>Calculates the waveform duration between the first and final burst.</summary>
        /// <param name="BurstStartLocations">The burst start locations loaded from the waveform file.</param>
        /// <param name="BurstStopLocations">The burst stop locations loaded from the waveform file.</param>
        /// <param name="SampleRate">The waveform sample rate loaded from the waveform file.</param>
        /// <returns>The waveform duration in seconds.</returns>
        private static double CalculateWaveformDuration(int[] BurstStartLocations, int[] BurstStopLocations, double SampleRate)
        {
            int finalStopIndex = BurstStopLocations.Length - 1;
            //Calculate the number of samples from the first burst to end of final burst
            //Add one to arrive at the total number of samples
            //Divide by the sample rate to get the time in seconds
            return (BurstStopLocations[finalStopIndex] - BurstStartLocations[0] + 1) / SampleRate;
        }
    }

}
