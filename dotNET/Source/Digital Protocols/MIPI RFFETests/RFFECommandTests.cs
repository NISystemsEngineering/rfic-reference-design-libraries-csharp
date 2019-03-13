using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NationalInstruments.ReferenceDesignLibraries.DigitalProtocols.MIPI_RFFE;


namespace NationalInstruments.ReferenceDesignLibraries.DigitalProtocols.Tests
{
    [TestClass()]
    public class RFFECommandTests
    {
        [TestMethod()]
        public void CalculateParityTest()
        {
            //Should be odd parity - 0
            string bitString = "111110000000";
            string result = CalculateParity(bitString);
            Assert.AreEqual(result, "0");

            //Should be odd parity - 1
            bitString = "111110000001";
            result = CalculateParity(bitString);
            Assert.AreEqual(result, "1");
        }

        [TestMethod()]
        public void BitStringToArrayTest()
        {

            string bitString = "11110000";
            uint[] bitArray = new uint[8] {1,1,1,1,0,0,0,0};

            //REsult should be the same bit array represented above
            uint[] result = BitStringToArray(bitString);
            CollectionAssert.AreEqual(result, bitArray);
        }

        [TestMethod()]
        public void Reg0WriteTest()
        {
            RegisterData testData = new RegisterData
            {
                SlaveAddress = 0xF, //15
                WriteRegisterData = new byte[1] {8}
            };

            RFFECommand Reg0 = RFFECommand.Reg0Write;

            //Result should be 111100010001
            Reg0.CreateSourceData(testData, out uint[] sourceData, out int numByte);
            uint[] knownData = BitStringToArray("111100010001");

            CollectionAssert.AreEqual(sourceData, knownData, "Reg0Write data should match known data");

        }
    }
}