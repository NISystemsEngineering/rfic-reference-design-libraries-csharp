using System;
using System.IO;
using System.Linq;
using System.Text;
using Ivi.Driver;
using NationalInstruments.DataInfrastructure;
using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.ModularInstruments.NIRfsgPlayback;


namespace NationalInstruments.ReferenceDesignLibraries
{
    public class SG
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
        public enum PAENMode { Disabled, Static, Dynamic };
        public struct DynamicGenerationAndPAENConfiguration
        {
            public double DutyCycle_Percent;
            public double PreBurstTime_s;
            public double PostBurstTime_s;
            public PAENMode PAEnableMode;
            public RfsgMarkerEventOutputBehaviour PAEnableTriggerMode;
            public double CommandLengthTime_s;
            public string PAEnableTriggerExportTerminal;

            public void SetDefaults()
            {
                //Default configuration is set for a DUT with a simple digital toggle high/low
                //for PA Enable
                DutyCycle_Percent = 50;
                PreBurstTime_s = 500e-9;
                PostBurstTime_s = 500e-9;
                PAEnableMode = PAENMode.Dynamic;
                PAEnableTriggerExportTerminal = RfsgMarkerEventExportedOutputTerminal.Pfi0.ToString();
                PAEnableTriggerMode = RfsgMarkerEventOutputBehaviour.Pulse;
                CommandLengthTime_s = 0;
            }
        }
        #endregion
        public static void ConfigureInstrument(ref NIRfsg rfsgHandle, InstrumentConfiguration instrConfig)
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
        public static Waveform LoadWaveformFromTDMS(ref NIRfsg rfsgHandle, string filePath, string waveformName = "", bool normalizeWaveform = true)
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

            waveform.SampleRate = 1 / waveform.WaveformData.PrecisionTiming.SampleInterval.FractionalSeconds; //Seconds per sample
            waveform.BurstLength_s = (waveform.BurstStopLocations[0] - waveform.BurstStartLocations[0]) / waveform.SampleRate; //  no. samples / (samples/s) = len_s

            if (normalizeWaveform) NormalizeWaveform(ref waveform);

            return waveform;
        }
        public static void DownloadWaveform(ref NIRfsg rfsgHandle, ref Waveform waveform)
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
        public static void ConfigureDynamicGenerationAndPAEnableControl(ref NIRfsg rfsgHandle, ref Waveform waveform, DynamicGenerationAndPAENConfiguration dynamicConfig, 
            out double period, out double idleTime)
        {
            IntPtr rfsgPtr = rfsgHandle.GetInstrumentHandle().DangerousGetHandle();
            string scriptName = String.Format("{0}{1}", waveform.WaveformName, dynamicConfig.DutyCycle_Percent);

            if (dynamicConfig.DutyCycle_Percent <= 0)
            {
                throw new System.ArgumentOutOfRangeException("DutyCycle_Percent", dynamicConfig.DutyCycle_Percent, "Duty cycle must be greater than 0 %");
            }

            //Calculate various timining information
            double dutyCycle = dynamicConfig.DutyCycle_Percent / 100;
            double totalBurstTime = dynamicConfig.PreBurstTime_s + waveform.BurstLength_s + dynamicConfig.PostBurstTime_s;
            idleTime = (totalBurstTime / dutyCycle) - totalBurstTime;
            period = totalBurstTime + idleTime;

            //Convert all time based values to sample based values
            long preBurstSamp, postBurstSamp, idleSamp, commandSamples;
            preBurstSamp = (long)Math.Round(dynamicConfig.PreBurstTime_s * waveform.SampleRate);
            postBurstSamp = (long)Math.Round(dynamicConfig.PostBurstTime_s * waveform.SampleRate);
            idleSamp = (long)Math.Round(idleTime * waveform.SampleRate);
            commandSamples = (long)Math.Round(dynamicConfig.CommandLengthTime_s * waveform.SampleRate);

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
            //You must explicitly disable the PA after aborting generation.
            if (dynamicConfig.PAEnableMode == PAENMode.Static)
                sb.AppendLine("wait 8 marker1(7)");

            //Configure for endless repeating
            sb.AppendLine("Repeat forever");

            //Configure the idle time prior to each packet generation
            sb.Append($"wait {idleSamp}");

            //If PAEN Mode is dynamic we need to trigger the PA to enable
            if (dynamicConfig.PAEnableMode == PAENMode.Dynamic)
            {
                //PA Enable is triggered at or before the last sample of the wait period
                long PAEnableTriggerLoc = idleSamp - commandSamples - 1;
                sb.Append($" marker1({PAEnableTriggerLoc})");
            }

            sb.AppendLine();

            //Configure waiting for the pre-burst time
            sb.AppendLine($"wait {preBurstSamp}");

            //Configure generation of the selected waveform but only for the burst length
            sb.Append($"generate {waveform.WaveformName} subset({waveform.BurstStartLocations[0]},{waveform.BurstStopLocations[0]})");

            //Check to see if the command time is longer than the post-burst time, which determines when the PA disable command needs sent
            bool LongCommand = dynamicConfig.PostBurstTime_s < dynamicConfig.CommandLengthTime_s;

            if (dynamicConfig.PAEnableMode == PAENMode.Dynamic && LongCommand)
            {
                //Trigger is placed a number of samples from the end of the burst corresponding with
                //how much longer than the post burst time it is
                long PADisableTriggerLoc = waveform.BurstStopLocations[0] - (commandSamples - postBurstSamp) - 1;
                sb.Append($" marker1({PADisableTriggerLoc})");
            }
            sb.AppendLine();

            //Configure waiting for the post-burst time
            sb.Append($"wait {postBurstSamp}");

            //If the ommand time is shorter than the post-burst time, the disable trigger must be sent
            //during the post-burst time 
            if (dynamicConfig.PAEnableMode == PAENMode.Dynamic && !LongCommand)
            {
                long PADisableTriggerLoc = postBurstSamp - commandSamples - 1;
                sb.Append($" marker1({PADisableTriggerLoc})");
            }
            sb.AppendLine();
            //Close out the script
            sb.AppendLine("end repeat");
            sb.AppendLine("end script");
            #endregion

            //Download the generation script to the generator for later initiation
            NIRfsgPlayback.SetScriptToGenerateSingleRfsg(rfsgPtr, sb.ToString());

            //Configure the triggering for PA enable if selected
            if (dynamicConfig.PAEnableMode != PAENMode.Disabled)
            {
                rfsgHandle.DeviceEvents.MarkerEvents[1].ExportedOutputTerminal = RfsgMarkerEventExportedOutputTerminal.FromString(
                    dynamicConfig.PAEnableTriggerExportTerminal);
                rfsgHandle.DeviceEvents.MarkerEvents[1].OutputBehaviour = dynamicConfig.PAEnableTriggerMode;
            }
        }
        public static void ConfigureRF(ref NIRfsg rfsgHandle, InstrumentConfiguration instrConfig)
        {
            rfsgHandle.RF.ExternalGain = -instrConfig.ExternalAttenuation_dBm;
            rfsgHandle.RF.Configure(instrConfig.CarrierFrequency_Hz, instrConfig.AverageInputPower_dBm);
        }
        public static Waveform GetWaveformParametersByName(ref NIRfsg rfsgHandle, string waveformName)
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
        public static void TogglePFILine(ref NIRfsg rfsgHandle, RfsgMarkerEventToggleInitialState toggleDirection = RfsgMarkerEventToggleInitialState.DigitalLow)
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
        public static void CloseInstrument(ref NIRfsg rfsgHandle)
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

        public class Utilities
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
