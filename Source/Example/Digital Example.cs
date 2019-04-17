using System;
using System.Text;
using System.IO;
using NationalInstruments.ModularInstruments.NIDigital;
using static NationalInstruments.ReferenceDesignLibraries.Digital;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class Digital_Example
    {
        static void Main()
        {
            NIDigital digital = new NIDigital("PXI1Slot2", false, false, "");

            string pinMapPath = Path.GetFullPath(@"Support Files\PinMap.pinmap");
            string specPath = Path.GetFullPath(@"Support Files\Specifications.specs");
            string levelsPath = Path.GetFullPath(@"Support Files\PinLevels.digilevels");
            string timingPath = Path.GetFullPath(@"Support Files\Timing.digitiming");
            string patternPath = Path.GetFullPath(@"Support Files\Pattern.digipat");

            ProjectFiles projectFiles = new ProjectFiles
            {
                PinMapFile = pinMapPath,
                SpecificationsFiles = new string[1] { specPath },
                PinLevelsFiles = new string[1] { levelsPath },
                TimingFiles = new string[1] { timingPath },
                DigitalPatternFiles = new string[1] {patternPath}
            };

            LoadProjectFiles(digital, projectFiles);

            SourcePinConfiguration sourcePin = GetDefaultSourcePinConfiguration();
            sourcePin.PinName = "DUTPin1";

            ConfigureAndSourcePin(digital, sourcePin);

            TriggerConfiguration triggerConfig = new TriggerConfiguration
            {
                BurstTriggerType = TriggerType.None
            };

            InitiatePatternGeneration(digital, "new_pattern", triggerConfig);

            digital.PatternControl.Abort();

            DisconnectAndClose(digital);
        }


    }
}
