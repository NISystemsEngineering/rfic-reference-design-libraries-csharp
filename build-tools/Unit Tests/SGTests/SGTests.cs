using Microsoft.VisualStudio.TestTools.UnitTesting;
using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.ModularInstruments.NIRfsgPlayback;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using FluentAssertions;
using FluentAssertions.Execution;

//[assembly: Parallelize(Workers = 2, Scope = ExecutionScope.MethodLevel)]
namespace NationalInstruments.ReferenceDesignLibraries.Tests
{
    //Suppress warning for obselete code as LoadWaveformFromTDMS intentionally uses 
    //an outdated method in order to support older waveform files
#pragma warning disable CS0612
    [TestClass]
    public class WaveformTests
    {
        static NIRfsg sim;
        static string[] files;
        static Waveform[] waveforms;
        static IniData[] iniData;
        [ClassInitialize]
        public static void TestFixtureSetup(TestContext context)
        {
            sim = new NIRfsg("sim", false, false, "Simulate=1,RangeCheck=0,DriverSetup=Model:5646R");
            //string path = Path.Combine(context.TestRunDirectory, @"Support Files");
            files = Directory.GetFiles("Support Files", "*.tdms");
            waveforms = new Waveform[files.Length];
            iniData = new IniData[files.Length];

            FileIniDataParser parser = new FileIniDataParser();

            string fileName, directory, iniPath;

            for (int i = 0; i < files.Length; i++)
            {
                waveforms[i] = SG.LoadWaveformFromTDMS(files[i]);
                fileName = Path.GetFileNameWithoutExtension(files[i]);
                directory = Path.GetDirectoryName(files[i]);
                iniPath = Path.Combine(directory, fileName + ".ini");

                {
                    iniData[i] = parser.ReadFile(iniPath);
                }
            }
        }
        public delegate void TestAction(string fileName, Waveform waveform, string filePath, IniData fileConfig);
        public void LoopFiles(TestAction action)
        {
            string fileName, filePath;
            Waveform waveform;
            IniData fileConfig;
            using (new AssertionScope())
            {
                for (int i = 0; i < files.Length; i++)
                {
                    filePath = files[i];
                    fileName = Path.GetFileNameWithoutExtension(filePath);
                    waveform = waveforms[i];
                    fileConfig = iniData[i];
                    action(fileName, waveform, filePath, fileConfig);
                }
            }
        }
        [TestMethod()]
        public void RuntimeScalingLeqTo0()
        {
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                waveform.RuntimeScaling.Should().BeLessOrEqualTo(0, $"of loading file \"{fileName}\"");
            });
        }
        [TestMethod()]
        public void WaveformDxEqOneOverFs()
        {
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                double fs = 1/ waveform.Data.PrecisionTiming.SampleInterval.FractionalSeconds;
                waveform.SampleRate.Should().BeApproximately(fs, .001, $"of loading file \"{fileName}\"");
            });
        }
        [TestMethod()]
        public void WaveformYNotEmpty()
        {
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                waveform.Data.Samples.Count.Should().NotBe(0, $"of loading file \"{fileName}\"");
            });
        }
        [TestMethod()]
        public void PaprGeqTo0()
        {
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                waveform.PAPR_dB.Should().BeGreaterOrEqualTo(0, $"of loading file \"{fileName}\"");
            });
        }
        [TestMethod()]
        public void BurstLengthGeq0()
        {
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                waveform.BurstLength_s.Should().BeGreaterOrEqualTo(0, $"of loading file \"{fileName}\"");
            });
        }
        [TestMethod()]
        public void SampleRateGreaterThan0()
        {
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                waveform.SampleRate.Should().BeGreaterThan(0, $"of loading file \"{fileName}\"");
            });
        }
        [TestMethod()]
        public void WaveformDxGreaterThan0()
        {
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                waveform.Data.PrecisionTiming.SampleInterval.FractionalSeconds
                .Should().BeGreaterThan(0, $"of loading file \"{fileName}\"");
            });
        }
        [TestMethod()]
        public void BurstStartLocsNotEmpty()
        {
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                waveform.BurstStartLocations.Should().NotBeEmpty($"of loading file \"{fileName}\"");
            });
        }
        [TestMethod()]
        public void BurstStopLocsNotEmpty()
        {
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                waveform.BurstStopLocations.Should().NotBeEmpty($"of loading file \"{fileName}\"");
            });
        }
        [TestMethod()]
        public void PaprCalcAgreesWithCalculated()
        {
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                System.IntPtr rfsgHandle = sim.GetInstrumentHandle().DangerousGetHandle();
                NIRfsgPlayback.DownloadUserWaveform(rfsgHandle, waveform.Name, 
                    waveform.Data, true);
                NIRfsgPlayback.RetrieveWaveformPapr(rfsgHandle,
                    waveform.Name, out double calcPapr);
                waveform.PAPR_dB.Should().BeApproximately(calcPapr, 0.1,
                    $"of loading file \"{fileName}\"");
            });
        }
        [TestMethod()]
        public void EntireWaveformRead()
        {
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                NIRfsgPlayback.ReadWaveformSizeFromFile(filePath, 0, out int size);
                waveform.Data.SampleCount.Should().Be(size, $"of loading file \"{fileName}\"");
            });
        }
        [TestMethod()]
        public void DictionaryWaveformMatchesLoaded()
        {
            
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                SG.DownloadWaveform(sim, waveform);
                Waveform newWaveform = SG.GetWaveformParametersByName(sim, waveform.Name);

                newWaveform.Should().BeEquivalentTo(waveform, options =>
                {
                    //Actual waveform data is not returned. Hence, exclude it from comparison
                    options.Excluding(w => w.Data);
                    //Ensure each member is compared; otherwise, it will just compare the two structs as whole values
                    options.ComparingByMembers<Waveform>();
                    return options;
                }, $"of loading file \"{fileName}\"");
            });
        }
        [TestMethod()]
        public void IdleDurationCalculatedCorrectly()
        {

            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                string idle = fileConfig["TestData"]["IdleDuration"];
                bool idlePresent = bool.Parse(idle);
                SG.DownloadWaveform(sim, waveform);
                waveform.IdleDurationPresent.Should().Be(idlePresent, $"of knowledge of file \"{fileName}\"");
            });
        }
        [ClassCleanup]
        public static void TestFixtureTearDown()
        {
            sim.Close();
        }
    }
}