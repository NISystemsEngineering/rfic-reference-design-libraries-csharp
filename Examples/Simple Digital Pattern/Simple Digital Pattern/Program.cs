using NationalInstruments.ModularInstruments.NIDigital;
using System;
using System.IO;
using static NationalInstruments.ReferenceDesignLibraries.Digital;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class Digital_Example
    {
        static void Main()
        {
            NIDigital digital = new NIDigital("PXIe-6570", false, false, "");

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

            // Alternatively, you could use the following command to search for all matching NI-Digital project files
            // projectFiles = Digital.Utilities.SearchForProjectFiles(Path.GetFullPath(@"Support Files\"), true)

            LoadProjectFiles(digital, projectFiles);

            SourcePinConfiguration sourcePin = SourcePinConfiguration.GetDefault();
            sourcePin.PinName = "DUTPin1";

            ConfigureAndSourcePin(digital, sourcePin);

            TriggerConfiguration triggerConfig = new TriggerConfiguration
            {
                BurstTriggerType = TriggerType.None
            };


            InitiatePatternGeneration(digital, "new_pattern", triggerConfig);

            Console.WriteLine("Pattern generation has begun. Press any key to abort, disconnect pins, and close the program.");
            Console.ReadKey();

            digital.PatternControl.Abort();

            DisconnectAndClose(digital);
        }


    }
}
