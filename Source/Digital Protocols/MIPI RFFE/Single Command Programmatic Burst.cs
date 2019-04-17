using System;
using System.Text;
using System.Collections;
using System.IO;
using NationalInstruments.ModularInstruments.NIDigital;
using static NationalInstruments.ReferenceDesignLibraries.Digital;
using static NationalInstruments.ReferenceDesignLibraries.DigitalProtocols.MIPI_RFFE;

namespace MIPI_RFFE
{
    class Multiple_Command_Programmatic_Burst
    {
        public static void Main()
        {
            string DigitalPath = Path.GetFullPath(@"Digital Project");

            string pinMapPath = Path.Combine(DigitalPath, "PinMap.pinmap");
            string specPath = Path.Combine(DigitalPath, "Specifications.specs");
            string levelsPath = Path.Combine(DigitalPath, "PinLevels.digilevels");
            string timingPath = Path.Combine(DigitalPath, "Timing.digitiming");
            string[] patternPaths = Directory.GetFiles(Path.Combine(DigitalPath, "RFFE Command Patterns"), "*.digipat");

            ProjectFiles projectFiles = new ProjectFiles
            {
                PinMapFile = pinMapPath,
                SpecificationsFiles = new string[1] { specPath },
                PinLevelsFiles = new string[1] { levelsPath },
                TimingFiles = new string[1] { timingPath },
                DigitalPatternFiles = patternPaths
            };

            //Initialize hardware and load pinmap plus sheets into
            //digital pattern instrument. Most of the fucntions below are
            //a lightweight wrapper around NI - Digital functions.

            //If you change the insturment name below, you must also change the instrument name
            //in the PinMap file
            NIDigital digital = new NIDigital("PXIe-6570", false, false, "");

            LoadProjectFiles(digital, projectFiles);

            //Turn RFFE bus power on
            digital.PinAndChannelMap.GetPinSet("RFFEVIO").WriteStatic(PinState._1);

            //Setup new register data to send to register 0
            RegisterData regData = new RegisterData {
                SlaveAddress = 0xF, //15
                WriteRegisterData = new byte[1] { 0x8 },
                ByteCount = 1
            };

            //Trgger type is set to none so burst will start immediately
            TriggerConfiguration triggerConfig = new TriggerConfiguration {
                BurstTriggerType = TriggerType.None,
            };

            //Burst a Reg0Write command using the data specified in regData
            BurstRFFE(digital, regData, "RFFEDATA", RFFECommand.Reg0Write, triggerConfig);

            Console.ReadKey();

            DisconnectAndClose(digital);
        }

    }
}
