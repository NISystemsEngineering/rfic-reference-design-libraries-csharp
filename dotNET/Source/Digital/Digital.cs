using System;
using System.Text;
using NationalInstruments.ModularInstruments.NIDigital;


namespace NationalInstruments.ReferenceDesignLibraries
{
    public class Digital
    {
        #region Type Definitions
        public struct ProjectFiles
        {
            public string PinMapFile;
            public string[] SpecificationsFiles;
            public string[] PinLevelsFiles;
            public string[] TimingFiles;
            public string[] DigitalPatternFiles;
        }
        public struct SourcePinConfiguration
        {
            public string PinName;
            public PpmuOutputFunction PinOutputFunction;
            public double Voltage_V;
            public double Current_A;
        }
        public static SourcePinConfiguration GetDefaultSourcePinConfiguration()
        {
            return new SourcePinConfiguration
            {
                PinOutputFunction = PpmuOutputFunction.DCVoltage,
                Voltage_V = 1.8,
            };
        }
        public struct TriggerConfiguration
        {
            public TriggerType BurstTriggerType;
            public DigitalEdge DigitalEdgeType;
            public string DigitalEdgeSource;
        }
        public static TriggerConfiguration GetDefaultTriggerConfiguration()
        {
            return new TriggerConfiguration
            {
                BurstTriggerType = TriggerType.DigitalEdge,
                DigitalEdgeType = DigitalEdge.Rising,
                DigitalEdgeSource = "PXI_Trig0",
            };
        }
        #endregion
        #region Configuration
        public static void LoadProjectFiles(NIDigital nIDigital, ProjectFiles projectFiles)
        {
            nIDigital.LoadPinMap(projectFiles.PinMapFile);
            nIDigital.LoadSpecifications(projectFiles.SpecificationsFiles);
            nIDigital.LoadLevels(projectFiles.SpecificationsFiles);
            nIDigital.LoadTiming(projectFiles.TimingFiles);
            foreach (string path in projectFiles.DigitalPatternFiles) nIDigital.LoadPattern(path);

            if (projectFiles.PinLevelsFiles.Length == 1 && projectFiles.TimingFiles.Length == 1)
            {
                nIDigital.ApplyLevelsAndTiming("", projectFiles.PinLevelsFiles[0], projectFiles.TimingFiles[0]);
            }
        }
        public static void ConfigureAndSourcePin(NIDigital nIDigital, SourcePinConfiguration sourceConfig)
        {
            DigitalPinSet pinSet = nIDigital.PinAndChannelMap.GetPinSet(sourceConfig.PinName);
            pinSet.Ppmu.OutputFunction = sourceConfig.PinOutputFunction;
            switch (sourceConfig.PinOutputFunction)
            {
                case PpmuOutputFunction.DCCurrent:
                    pinSet.Ppmu.DCCurrent.CurrentLevel = sourceConfig.Current_A;
                    break;
                case PpmuOutputFunction.DCVoltage:
                    pinSet.Ppmu.DCVoltage.VoltageLevel = sourceConfig.Voltage_V;
                    break;
            }
            pinSet.Ppmu.Source();
        }
        public static void ConfigureAndBurstPattern(NIDigital nIDigital, string patternStartLabel, TriggerConfiguration triggerConfig)
        {
            switch (triggerConfig.BurstTriggerType)
            {
                case TriggerType.Software:
                    nIDigital.Trigger.StartTrigger.Software.Configure();

                    // This call to BurstPattern returns immediately because waitUntilDone is 'false.'
                    nIDigital.PatternControl.BurstPattern(string.Empty, patternStartLabel, true, false, TimeSpan.FromSeconds(10));
                    break;
                case TriggerType.DigitalEdge:
                    nIDigital.Trigger.StartTrigger.DigitalEdge.Configure(triggerConfig.DigitalEdgeSource, triggerConfig.DigitalEdgeType);
                    nIDigital.PatternControl.BurstPattern("", patternStartLabel, true, TimeSpan.FromSeconds(10));
                    break;
                case TriggerType.None:
                    nIDigital.PatternControl.BurstPattern(string.Empty, patternStartLabel, true, TimeSpan.FromSeconds(10));
                    break;
            }
        }
        #endregion
        #region Results
        #endregion
    }
}
