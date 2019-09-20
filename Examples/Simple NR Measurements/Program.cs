using NationalInstruments.RFmx.InstrMX;
using System;
using System.Linq;
using static NationalInstruments.ReferenceDesignLibraries.SA.RFmxNR;

namespace Simple_NR_Measurements
{
    class Program
    {
        static void Main(string[] args)
        {
            // open device session
            Console.Write("Initializing instrument..");
            var instr = new RFmxInstrMX("VST2_01", "");
            var nr = instr.GetNRSignalConfiguration();
            Console.WriteLine("done");

            // Get default configurations
            Console.Write("Getting default configurations..");
            var commonConfig = CommonConfiguration.GetDefault();
            var sigConfig = SignalConfiguration.GetDefault();

            var modAccConfig = ModAccConfiguration.GetDefault();
            var acpConfig = AcpConfiguration.GetDefault();
            var chpConfig = ChpConfiguration.GetDefault();
            Console.WriteLine("done");

            // make changes that deviate from default
            sigConfig.ComponentCarrierConfigurations = Enumerable.Repeat(ComponentCarrierConfiguration.GetDefault(), 2).ToArray(); // set to number of carriers
            for (int i = 0; i < sigConfig.ComponentCarrierConfigurations.Length; i++)
                sigConfig.ComponentCarrierConfigurations[i].CellId = i;

            // configure instrument and measurement
            Console.Write("Configuring measurement..");
            ConfigureCommon(instr, nr, commonConfig);
            ConfigureSignal(nr, sigConfig);

            ConfigureModacc(nr, modAccConfig);
            ConfigureAcp(nr, acpConfig);
            ConfigureChp(nr, chpConfig);
            Console.WriteLine("done");

            // start measurement
            Console.Write("Initiating and fetching results..");
            nr.Initiate("", "");

            // get results and print to console
            var modAccResults = FetchModAcc(nr);
            var acpResults = FetchAcp(nr);
            var chpResults = FetchChp(nr);
            Console.WriteLine("done");

            // print results to console
            for (int i = 0; i < modAccResults.ComponentCarrierResults.Length; i++)
            {
                Console.WriteLine();
                var componentCarrier = modAccResults.ComponentCarrierResults[i];
                Console.WriteLine(string.Format("---ModAcc Results Component Carrier {0}\n" +
                    "Peak Composite EVM Subcarrier Index: {1}\n" +
                    "Peak Composite EVM Symbol Index: {2}\n" +
                    "Mean RMS Composite EVM: {3:0.###}\n" +
                    "Max Peak Composite EVM: {4:0.###}\n" +
                    "Mean Frequency Error (Hz): {5:0.###}\n" +
                    "Peak Composite EVM Slot Index: {6}", 
                    i,
                    componentCarrier.PeakCompositeEvmSubcarrierIndex,
                    componentCarrier.PeakCompositeEvmSymbolIndex,
                    componentCarrier.MeanRmsCompositeEvm,
                    componentCarrier.MaxPeakCompositeEvm,
                    componentCarrier.MeanFrequencyError_Hz,
                    componentCarrier.PeakCompositeEvmSlotIndex));
            }
            for (int i = 0; i < acpResults.ComponentCarrierResults.Length; i++)
            {
                Console.WriteLine();
                var componentCarrier = acpResults.ComponentCarrierResults[i];
                Console.WriteLine(string.Format("---ACP Results Component Carrier {0}\n" +
                    "Absolute Power (dBm): {1:0.###}\n" +
                    "Relative Power (dB): {2:0.###}", 
                    i, componentCarrier.AbsolutePower_dBm, componentCarrier.RelativePower_dB));
            }
            for (int i = 0; i < acpResults.OffsetResults.Length; i++)
            {
                Console.WriteLine();
                var offsetResult = acpResults.OffsetResults[i];
                Console.WriteLine(string.Format("---ACP Results Offset {0}---\n" +
                    "Lower Absolute Power (dBm): {1:0.###}\n" +
                    "Lower RelativePower (dB): {2:0.###}\n" +
                    "Upper Absolute Power (dBm): {3:0.###}\n" +
                    "Upper Relative Power (dB): {4:0.###}\n" +
                    "Offset Frequency (MHz): {5:0.###}\n" +
                    "Offset Integration Bandwidth (MHz): {6:0.###}",
                    i, offsetResult.LowerAbsolutePower_dBm, offsetResult.LowerRelativePower_dB, offsetResult.UpperAbsolutePower_dBm,
                    offsetResult.UpperRelativePower_dB, offsetResult.OffsetFrequency_Hz / 1E6, offsetResult.OffsetIntegrationBandwidth_Hz / 1E6));
            }
            Console.WriteLine();
            Console.WriteLine(string.Format("---CHP Results---\n" +
                "Total Aggregated Power (dBm): {0:0.###}", chpResults.TotalAggregatedPower_dBm));
            for (int i = 0; i < chpResults.ComponentCarrierResults.Length; i++)
            {
                Console.WriteLine();
                var componentCarrier = chpResults.ComponentCarrierResults[i];
                Console.WriteLine(string.Format("---CHP Results Component Carrier {0}\n" +
                    "Absolute Power (dBm): {1:0.###}\n" +
                    "Relative Power (dB): {2:0.###}", 
                    i, componentCarrier.AbsolutePower_dBm, componentCarrier.RelativePower_dB));
            }

            // close instrument and wait on user
            instr.Close();
            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
