using Microsoft.VisualStudio.TestTools.UnitTesting;
using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.ModularInstruments.NIRfsgPlayback;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;

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
        static SG.Waveform[] waveforms;
        static IniData[] iniData;
        [ClassInitialize]
        public static void TestFixtureSetup(TestContext context)
        {
            sim = new NIRfsg("sim", false, false, "Simulate=1,RangeCheck=0,DriverSetup=Model:5646R");
            //string path = Path.Combine(context.TestRunDirectory, @"Support Files");
            files = Directory.GetFiles("Support Files", "*.tdms");
            waveforms = new SG.Waveform[files.Length];
            iniData = new IniData[files.Length];

            FileIniDataParser parser = new FileIniDataParser();

            string fileName, directory, iniPath;
            for (int i =0; i < files.Length; i++)
            {
                waveforms[i] = SG.LoadWaveformFromTDMS(files[i]);
                fileName = Path.GetFileNameWithoutExtension(files[i]);
                directory = Path.GetDirectoryName(files[i]);
                iniPath = Path.Combine(directory, fileName + ".ini");
                iniData[i] = parser.ReadFile(iniPath);
            }
        }
        public delegate void TestAction(string fileName, SG.Waveform waveform, string filePath, IniData fileConfig);
        public void LoopFiles(TestAction action)
        {
            string fileName, filePath;
            SG.Waveform waveform;
            IniData fileConfig;
            for (int i = 0; i < files.Length; i++)
            {
                filePath = files[i];
                fileName = Path.GetFileNameWithoutExtension(filePath);
                waveform = waveforms[i];
                fileConfig = iniData[i];
                action(fileName, waveform, filePath, fileConfig);
            }
        }
        [TestMethod()]
        public void RuntimeScalingLeqTo0()
        {
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                Assert.IsTrue(waveform.RuntimeScaling <= 0,
                    $"File \"{fileName}\" runtime scaling should be leq 0.");
            });
        }
        [TestMethod()]
        public void WaveformDxEqOneOverFs()
        {
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                double fs = 1/ waveform.WaveformData.PrecisionTiming.SampleInterval.FractionalSeconds;
                Assert.AreEqual(fs, waveform.SampleRate, .001, $"File \"{fileName}\" sample rate" +
                    $"should be equal to 1/f_s.");
            });
        }
        [TestMethod()]
        public void WaveformYNotEmpty()
        {
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                Assert.IsTrue(waveform.WaveformData.Samples.Count > 0, $"File \"{fileName}\" waveform samples" +
                    $" should be greater than 0");
            });
        }
        [TestMethod()]
        public void PaprGeqTo0()
        {
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                Assert.IsTrue(waveform.PAPR_dB >= 0, $"File \"{fileName}\" PAPR should be geq than 0");
            });
        }
        [TestMethod()]
        public void BurstLengthGeq0()
        {
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                Assert.IsTrue(waveform.BurstLength_s >= 0, $"File \"{fileName}\" burst length should be geq than 0");
            });
        }
        [TestMethod()]
        public void SampleRateGreaterThan0()
        {
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                Assert.IsTrue(waveform.SampleRate > 0, $"File \"{fileName}\" sample rate should be greater than 0");
            });
        }
        [TestMethod()]
        public void WaveformDxGreaterThan0()
        {
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                Assert.IsTrue(waveform.WaveformData.PrecisionTiming.SampleInterval.FractionalSeconds > 0, 
                    $"File \"{fileName}\" waveform.dx should be greater than 0");
            });
        }
        [TestMethod()]
        public void BurstStartLocsNotEmpty()
        {
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                Assert.IsTrue(waveform.BurstStartLocations.Length > 0,
                    $"File \"{fileName}\" burst start locations not empty.");
            });
        }
        [TestMethod()]
        public void BurstStopLocsNotEmpty()
        {
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                Assert.IsTrue(waveform.BurstStopLocations.Length > 0,
                    $"File \"{fileName}\" burst stop locations not empty.");
            });
        }
        [TestMethod()]
        public void PaprCalcAgreesWithCalculated()
        {
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                System.IntPtr rfsgHandle = sim.DangerousGetInstrumentHandle();
                NIRfsgPlayback.DownloadUserWaveform(rfsgHandle, waveform.WaveformName, 
                    waveform.WaveformData, waveform.IdleDurationPresent);
                NIRfsgPlayback.RetrieveWaveformPapr(rfsgHandle,
                    waveform.WaveformName, out double calcPapr);
                Assert.AreEqual(calcPapr, waveform.PAPR_dB, .1,
                    $"File \"{fileName}\" PAPR is correctly calculated.");
            });
        }
        [TestMethod()]
        public void EntireWaveformRead()
        {
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                NIRfsgPlayback.ReadWaveformSizeFromFile(filePath, 0, out int size);
                Assert.AreEqual(size, waveform.WaveformData.SampleCount,
                    $"File \"{fileName}\" all samples read.");
            });
        }
        [TestMethod()]
        public void DictionaryWaveformMatchesLoaded()
        {
            
            LoopFiles((fileName, waveform, filePath, fileConfig) =>
            {
                SG.DownloadWaveform(sim, waveform);
                SG.Waveform newWaveform = SG.GetWaveformParametersByName(sim, waveform.WaveformName);
                //Actual waveform data is not returned. Hence, explicitly set it so that this does not trigger a failure. 
                newWaveform.WaveformData = waveform.WaveformData;
                Assert.AreEqual(waveform, newWaveform,
                    $"File \"{fileName}\" waveform read from library matches loaded type.");
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
                Assert.AreEqual(idlePresent, waveform.IdleDurationPresent,
                    $"File \"{fileName}\" idle duration calculated correctly.");
            });
        }
        [ClassCleanup]
        public static void TestFixtureTearDown()
        {
            sim.Close();
        }
    }
}