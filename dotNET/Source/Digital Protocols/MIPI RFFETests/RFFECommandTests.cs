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
            RFFECommand test = RFFECommand.Reg0Write;

            //Create Private object for testing private class methods
            PrivateObject priv = new PrivateObject(test);

            //Should be odd parity - 0
            string bitString = "111110000000";
            var result = priv.Invoke("CalculateParity", bitString);
            Assert.AreEqual(result, "0");

            //Should be odd parity - 1
            bitString = "111110000001";
            result = priv.Invoke("CalculateParity", bitString);
            Assert.AreEqual(result, "1");
        }

        [TestMethod()]
        public void BitStringToArrayTest()
        {
            RFFECommand test = RFFECommand.Reg0Write;
            //Create Private object for testing private class methods
            PrivateObject priv = new PrivateObject(test);

            string bitString = "11110000";
            uint[] bitArray = new uint[8] {1,1,1,1,0,0,0,0};

            //REsult should be the same bit array represented above
            uint[] result = (uint[])priv.Invoke("BitStringToArray", bitString);
            CollectionAssert.AreEqual(result, bitArray);
        }
    }
}