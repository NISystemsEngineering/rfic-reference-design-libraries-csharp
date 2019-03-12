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
            /*
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
                DigitalPatternFiles = new string[1] { patternPath }
            };

            BurstRFFE(digital, new RegisterData(), "", RFFECommand.Reg0Write, GetDefaultTriggerConfiguration());*/
            uint x = 0xC;
            string byteString = Convert.ToString(x, 2);
            Console.WriteLine(byteString);

            char[] cArray = byteString.ToCharArray();
            uint[] rArray = new uint[cArray.Length];

            //byte[] result = Convert.FromBase64CharArray(cArray, 0, cArray.Length);
            for (int i = 0; i < cArray.Length; i++) {
                rArray[i] = (uint)char.GetNumericValue(cArray[i]);
            }

            Console.ReadKey();
    }

    }
}
