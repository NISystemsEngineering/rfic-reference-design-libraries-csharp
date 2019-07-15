using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Ivi.Driver;
using NationalInstruments.DataInfrastructure;
using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.ModularInstruments.NIRfsgPlayback;


namespace NationalInstruments.ReferenceDesignLibraries
{
    public static class SG
    {
        #region Type Definitions
        public struct InstrumentConfiguration
        {
            public string ReferenceClockSource;
            public double CarrierFrequency_Hz;
            public double AverageInputPower_dBm;
            public double ExternalAttenuation_dBm;
            public string BurstStartTriggerExportTerminal;
            public bool ShareLOSGToSA;
            public void SetDefaults()
            {
                ReferenceClockSource = RfsgFrequencyReferenceSource.PxiClock.ToString();
                CarrierFrequency_Hz = 1e9;
                AverageInputPower_dBm = 0;
                ExternalAttenuation_dBm = 0;
                BurstStartTriggerExportTerminal = RfsgMarkerEventExportedOutputTerminal.PxiTriggerLine0.ToString();
                ShareLOSGToSA = true;
            }
        }
        public struct Waveform
        {
            public string WaveformName;
            public ComplexWaveform<ComplexSingle> WaveformData;
            public double SignalBandwidth_Hz;
            public double PAPR_dB;
            public double BurstLength_s;
            public double SampleRate;
            public int[] BurstStartLocations;
            public int[] BurstStopLocations;
        }
        public struct WaveformTimingConfiguration
        {
            public double DutyCycle_Percent;
            public double PreBurstTime_s;
            public double PostBurstTime_s;

            public void SetDefaults()
            {
                DutyCycle_Percent = 50;
                PreBurstTime_s = 1e-6;
                PostBurstTime_s = 1e-6;
            }
        }
        public enum PAENMode { Disabled, Static, Dynamic };
        public struct PAENConfiguration
        {
            public PAENMode PAEnableMode;
            public RfsgMarkerEventOutputBehaviour PAEnableTriggerMode;
            public double CommandEnableTime_s;
            public double CommandDisableTime_s;
            public string PAEnableTriggerExportTerminal;
            
            public void SetDefaults()
            {
                //Default configuration is set for a DUT with a simple digital toggle high/low
                //for PA Enable
                PAEnableMode = PAENMode.Dynamic;
                PAEnableTriggerExportTerminal = RfsgMarkerEventExportedOutputTerminal.Pfi0.ToString();
                PAEnableTriggerMode = RfsgMarkerEventOutputBehaviour.Toggle;
                CommandEnableTime_s = 0;
                CommandDisableTime_s = 0;
            }
        }
        #endregion
        public static void ConfigureInstrument(NIRfsg rfsgHandle, InstrumentConfiguration instrConfig)
        {

            rfsgHandle.Arb.GenerationMode = RfsgWaveformGenerationMode.Script;
            rfsgHandle.RF.PowerLevelType = RfsgRFPowerLevelType.PeakPower;

            rfsgHandle.RF.ExternalGain = -instrConfig.ExternalAttenuation_dBm;
            rfsgHandle.RF.Configure(instrConfig.CarrierFrequency_Hz, instrConfig.AverageInputPower_dBm);

            rfsgHandle.FrequencyReference.Source = RfsgFrequencyReferenceSource.FromString(instrConfig.ReferenceClockSource);
            rfsgHandle.DeviceEvents.MarkerEvents[0].ExportedOutputTerminal =
                RfsgMarkerEventExportedOutputTerminal.FromString(instrConfig.BurstStartTriggerExportTerminal);

            if (instrConfig.ShareLOSGToSA)
            {
                rfsgHandle.RF.LocalOscillator.LOOutEnabled = true;
                rfsgHandle.RF.LocalOscillator.Source = RfsgLocalOscillatorSource.Onboard;
            }
        }
        public static Waveform LoadWaveformFromTDMS(NIRfsg rfsgHandle, string filePath, string waveformName = "", bool normalizeWaveform = true)
        {
            Waveform waveform = new Waveform
            {
                WaveformData = new ComplexWaveform<ComplexSingle>(0)
            };

            if (string.IsNullOrEmpty(waveformName))
            {
                waveformName = Path.GetFileNameWithoutExtension(filePath);
                Utilities.FormatWaveformName(ref waveformName);
            }
            waveform.WaveformName = waveformName;
            NIRfsgPlayback.ReadWaveformFromFileComplex(filePath, ref waveform.WaveformData);
            NIRfsgPlayback.ReadSignalBandwidthFromFile(filePath, 0, out waveform.SignalBandwidth_Hz);

            NIRfsgPlayback.ReadWaveformFileVersionFromFile(filePath, out string waveformVersion);
            if (waveformVersion == "1.0.0") NIRfsgPlayback.ReadPeakPowerAdjustmentFromFile(filePath, 0, out waveform.PAPR_dB);
            else NIRfsgPlayback.ReadPaprFromFile(filePath, 0, out waveform.PAPR_dB); //Version 2.0.0 and later

            NIRfsgPlayback.ReadBurstStartLocationsFromFile(filePath, 0, ref waveform.BurstStartLocations);
            NIRfsgPlayback.ReadBurstStopLocationsFromFile(filePath, 0, ref waveform.BurstStopLocations);
            
            // If the waveform does not have burst start or stop locations stored, then we will set the burst start to 
            // the first sample (0) and the stop to the last sample (number of samples minus one
            if (waveform.BurstStartLocations == null)
                waveform.BurstStartLocations = new int[1] { 0 };
            //Separate checks because null array throws exception when checking length
            else if (waveform.BurstStopLocations.Length <= 0)
                waveform.BurstStartLocations = new int[1] { 0 };

            if (waveform.BurstStopLocations == null)
                waveform.BurstStopLocations = new int[1] { waveform.WaveformData.SampleCount - 1 };
            //Separate checks because null array throws exception when checking length
            else if (waveform.BurstStopLocations.Length <= 0)
                waveform.BurstStopLocations = new int[1] { waveform.WaveformData.SampleCount - 1 };

            waveform.SampleRate = 1 / waveform.WaveformData.PrecisionTiming.SampleInterval.FractionalSeconds; //Seconds per sample
            waveform.BurstLength_s = (waveform.BurstStopLocations[0] - waveform.BurstStartLocations[0]) / waveform.SampleRate; //  no. samples / (samples/s) = len_s

            if (normalizeWaveform) NormalizeWaveform(ref waveform);

            return waveform;
        }
        public static void DownloadWaveform(NIRfsg rfsgHandle, ref Waveform waveform)
        {
            IntPtr rfsgPtr = rfsgHandle.GetInstrumentHandle().DangerousGetHandle();

            rfsgHandle.Abort();

            try
            {
                rfsgHandle.Arb.ClearWaveform(waveform.WaveformName); //Clear existing waveform to avoid erros
            }
            catch (Ivi.Driver.OutOfRangeException e)
            { //Intentionally ignore this exception; clearing the waveform failed because it doesn't exist
            }

            rfsgHandle.Arb.WriteWaveform(waveform.WaveformName, waveform.WaveformData);

            //Store loaded parameters
            NIRfsgPlayback.StoreWaveformSignalBandwidth(rfsgPtr, waveform.WaveformName, waveform.SignalBandwidth_Hz);
            NIRfsgPlayback.StoreWaveformPapr(rfsgPtr, waveform.WaveformName, waveform.PAPR_dB);
            NIRfsgPlayback.StoreWaveformBurstStartLocations(rfsgPtr, waveform.WaveformName, waveform.BurstStartLocations);
            NIRfsgPlayback.StoreWaveformBurstStopLocations(rfsgPtr, waveform.WaveformName, waveform.BurstStopLocations);
            NIRfsgPlayback.StoreWaveformSampleRate(rfsgPtr, waveform.WaveformName, waveform.SampleRate);

            //Manually configure additional settings
            NIRfsgPlayback.StoreWaveformLOOffsetMode(rfsgPtr, waveform.WaveformName, NIRfsgPlaybackLOOffsetMode.Disabled);
            NIRfsgPlayback.StoreWaveformRuntimeScaling(rfsgPtr, waveform.WaveformName, -1.5);
            NIRfsgPlayback.StoreWaveformRFBlankingEnabled(rfsgPtr, waveform.WaveformName, false);
        }
        public static void ConfigureWaveformTimingAndPAControl(NIRfsg rfsgHandle, ref Waveform waveform, WaveformTimingConfiguration waveTiming,
            PAENConfiguration paenConfig, out double period, out double idleTime)
        {
            IntPtr rfsgPtr = rfsgHandle.GetInstrumentHandle().DangerousGetHandle();
            string scriptName = String.Format("{0}{1}", waveform.WaveformName, waveTiming.DutyCycle_Percent);

            if (waveTiming.DutyCycle_Percent <= 0)
            {
                throw new System.ArgumentOutOfRangeException("DutyCycle_Percent", waveTiming.DutyCycle_Percent, "Duty cycle must be greater than 0 %");
            }

            //Calculate various timining information
            double dutyCycle = waveTiming.DutyCycle_Percent / 100;
            double totalBurstTime = waveTiming.PreBurstTime_s + waveform.BurstLength_s + waveTiming.PostBurstTime_s;
            idleTime = (totalBurstTime / dutyCycle) - totalBurstTime;
            period = totalBurstTime + idleTime;

            //Convert all time based values to sample based values
            long preBurstSamp, postBurstSamp, idleSamp, enableSamples, disableSamples;
            preBurstSamp = TimeToSamples(waveTiming.PreBurstTime_s, waveform.SampleRate);
            postBurstSamp = TimeToSamples(waveTiming.PostBurstTime_s, waveform.SampleRate);
            idleSamp = TimeToSamples(idleTime, waveform.SampleRate);
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
            sb.Append($"generate {waveform.WaveformName} subset({waveform.BurstStartLocations[0]},{waveform.BurstStopLocations[0]}) marker0(0)");

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

            //Download the generation script to the generator for later initiation
            NIRfsgPlayback.SetScriptToGenerateSingleRfsg(rfsgPtr, sb.ToString());

            //Configure the triggering for PA enable if selected
            if (paenConfig.PAEnableMode != PAENMode.Disabled)
            {
                rfsgHandle.DeviceEvents.MarkerEvents[1].ExportedOutputTerminal = RfsgMarkerEventExportedOutputTerminal.FromString(
                    paenConfig.PAEnableTriggerExportTerminal);
                rfsgHandle.DeviceEvents.MarkerEvents[1].OutputBehaviour = paenConfig.PAEnableTriggerMode;

                //Configure scriptTrigger0 for software triggering. This way, when it is time to abort we can stop
                //the loop and trigger the appropriate off command if PAEN mode is Static
                rfsgHandle.Triggers.ScriptTriggers[0].ConfigureSoftwareTrigger();
            }
        }
        public static Waveform GetWaveformParametersByName(NIRfsg rfsgHandle, string waveformName)
        {
            IntPtr rfsgPtr = rfsgHandle.GetInstrumentHandle().DangerousGetHandle();

            Waveform waveform = new Waveform
            {
                WaveformName = waveformName
            };
            NIRfsgPlayback.RetrieveWaveformSignalBandwidth(rfsgPtr, waveformName, out waveform.SignalBandwidth_Hz);
            NIRfsgPlayback.RetrieveWaveformPapr(rfsgPtr, waveformName, out waveform.PAPR_dB);
            NIRfsgPlayback.RetrieveWaveformSampleRate(rfsgPtr, waveformName, out waveform.SampleRate);
            NIRfsgPlayback.RetrieveWaveformBurstStartLocations(rfsgPtr, waveformName, ref waveform.BurstStartLocations);
            NIRfsgPlayback.RetrieveWaveformBurstStopLocations(rfsgPtr, waveformName, ref waveform.BurstStopLocations);

            waveform.BurstLength_s = (waveform.BurstStopLocations[0] - waveform.BurstStartLocations[0]) / waveform.SampleRate;

            return waveform;
        }
        public static void TogglePFILine(NIRfsg rfsgHandle, RfsgMarkerEventToggleInitialState toggleDirection = RfsgMarkerEventToggleInitialState.DigitalLow)
        {
            rfsgHandle.Abort();

            //Ensure that the terminal is configured to the proper toggle state
            rfsgHandle.DeviceEvents.MarkerEvents[1].ExportedOutputTerminal = RfsgMarkerEventExportedOutputTerminal.Pfi0;
            rfsgHandle.DeviceEvents.MarkerEvents[1].OutputBehaviour = RfsgMarkerEventOutputBehaviour.Toggle;
            rfsgHandle.DeviceEvents.MarkerEvents[1].ToggleInitialState = toggleDirection;

            //Create a script that doesn't do anything, but ensures that the requested intitial toggle behavior
            //is applied to the hardware to toggle the PFI line correctly
            string cachedScriptName = rfsgHandle.Arb.Scripting.SelectedScriptName;
            string toggleScript =
                @"script toggleScript
                    wait 10
                end script";
            rfsgHandle.Arb.Scripting.WriteScript(toggleScript);
            rfsgHandle.Arb.Scripting.SelectedScriptName = "ToggleScript";

            rfsgHandle.Initiate();
            rfsgHandle.Abort();

            //Return the active script to the previous
            rfsgHandle.Arb.Scripting.SelectedScriptName = cachedScriptName;
            rfsgHandle.Utility.Commit();
        }
        public static void AbortDynamicGeneration(NIRfsg rfsgHandle, int timeOut_ms = 1000)
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
                    throw new System.TimeoutException("Dynamic generation did not complete in the specified timeout period. Increase the timeout period" +
                        "or ensure that scripTrigger0 is properly configured to stop generation");
                }
            }
        }
        public static void CloseInstrument(NIRfsg rfsgHandle)
        {
            rfsgHandle.Abort();
            rfsgHandle.RF.OutputEnabled = false;
            rfsgHandle.Close();
        }
        private static void NormalizeWaveform(ref Waveform waveform)
        {
            //Normalize the waveform data

            double[] magnitudeArray = waveform.WaveformData.GetMagnitudeDataArray(false);
            ComplexSingle waveformMax = new ComplexSingle((float)magnitudeArray.Max(), 0);
            WritableBuffer<ComplexSingle> waveformBuffer = waveform.WaveformData.GetWritableBuffer();
            for (int i = 0; i < waveformBuffer.Count(); i++)
            {
                waveformBuffer[i] = waveformBuffer[i] / waveformMax; //Scale each point by the max value
            }

            //Calculate PAPR over only the burst length

            int offset = waveform.BurstStartLocations[0];
            int count = waveform.BurstStopLocations[0] - offset;
            double[] waveformBurst = new double[count];
            //Retrieve the subset of the magnitude waveform covering the waveform burst
            Array.Copy(waveform.WaveformData.GetMagnitudeDataArray(false), offset, waveformBurst, 0, count);

            for (int i = 0; i < count; i++)
            {
                waveformBurst[i] = Math.Pow(waveformBurst[i], 2);
            }
            double rms = Math.Sqrt(waveformBurst.Sum() / count); //RMS = sqrt(1/n*(|x_0|^2+|x_1|^2...|x_n|^2))

            //PAPR (Peak to Average Power Ratio) = Peak Power/Avg Power
            //PAPR (dB) = 20*log(max/avg)
            //Since we already scaled the data, the max value is simply 1
            waveform.PAPR_dB = 20 * Math.Log10(1 / rms);

        }
        private static long TimeToSamples(double time, double sampleRate)
        {
            return (long)Math.Round(time * sampleRate);
        }

        public static class Utilities
        {
            public static void FormatWaveformName(ref string waveformName)
            {
                //The RFSG playback library and script compiler won't accept names with non-text/numeric characters
                waveformName = System.Text.RegularExpressions.Regex.Replace(waveformName, "[^a-zA-Z0-9]", ""); //Remove all non-text/numeric characters
                waveformName = string.Concat("Wfm", waveformName);
                waveformName = waveformName.ToUpper();
            }
        }
    }

}
