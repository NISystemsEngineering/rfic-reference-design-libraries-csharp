using System;
using System.Collections.Generic;
using System.Text;
using static NationalInstruments.ReferenceDesignLibraries.Digital;
using NationalInstruments.ModularInstruments.NIDigital;

namespace NationalInstruments.ReferenceDesignLibraries.DigitalProtocols
{
    public class MIPI_RFFE
    {
        #region Type Definitions
        public struct RegisterData
        {
            public byte SlaveAddress;
            public ushort RegisterAddress;
            public byte[] WriteRegisterData;
            public byte ByteCount;
        }
        public struct ReadData
        {
            public string[] Register;
            public byte[] Data;
        }
        #endregion
        #region Command Classes
        public abstract class RFFECommand
        {
            internal abstract int ByteCountLimit
            {
                get;
            }
            internal abstract int UpperAddressLimit
            {
                get;
            }
            public abstract string SourceName { get;  }
            static public RFFECommand Reg0Write
            {
                get { return new Reg0Write(); }
            }

            public void ValidateLogic(RegisterData regData)
            {
                if (regData.SlaveAddress < 0x0 || regData.SlaveAddress >= 0x10)
                {
                    throw new System.ArgumentOutOfRangeException("SlaveAddress", regData.SlaveAddress.ToString("X"), "Slave Address out of range");
                }
                if (regData.RegisterAddress < 0x0 || regData.RegisterAddress > UpperAddressLimit )
                {
                    throw new System.ArgumentOutOfRangeException("RegisterAddress", regData.SlaveAddress.ToString("X"), 
                        "Register Address out of range. Check that the address is valid based on the selected command.");
                }
                if (regData.ByteCount <= 0 || regData.ByteCount > ByteCountLimit)
                {
                    throw new System.ArgumentOutOfRangeException("ByteCount", regData.ByteCount, "Byte Count out of range");
                }
            }
            public abstract void CreateSourceData(RegisterData regData, out uint[] sourceData, out int byteCount);
            public string CalculateParity(string bitString)
            {
                uint[] bitArray = BitStringToArray(bitString);
                uint sum = 0;
                for (int i = 0; i < bitArray.Length; i++) sum += bitArray[i];

                //Even 1's is a 1
                //Odd 1's is a 0 to achieve odd parity
                if (sum % 2 == 0) return "1";
                else return "0";
            }
            public uint[] BitStringToArray(string bitString)
            {
                char[] charArray = bitString.ToCharArray();
                uint[] bitArray = new uint[charArray.Length];

                for (int i = 0; i < charArray.Length; i++)
                {
                    bitArray[i] = (uint)char.GetNumericValue(charArray[i]);
                }
                return bitArray;
            }
        }
        class Reg0Write : RFFECommand
        {
            internal override int ByteCountLimit => 1;
            internal override int UpperAddressLimit => 0xFFFF;
            public override string SourceName => "Reg0Write";
            public override void CreateSourceData(RegisterData regData, out uint[] sourceData, out int byteCount)
            {
                //For Reg0Write, build 4 bit SA and 7 bit Data with parity.
                //COMMAND/ADDRESS/DATA FRAME
                /*
                string slaveAddress = Convert.ToString(regData.SlaveAddress, 2);
                //Only take the first element of register data
                string data = Convert.ToString(regData.WriteRegisterData[0], 2);

                byteCount = 1;
                sourceData = new uint[0];*/
                throw new System.NotImplementedException("Still in implementation");
            }
        }
        #endregion
        public static ReadData BurstRFFE(NIDigital niDigital, RegisterData regData, string pinName,
            RFFECommand rffeCommand, TriggerConfiguration triggerConfig)
        {
            //Check data is valid
            rffeCommand.ValidateLogic(regData);

            //Create source and capture waveforms in driver
            CreateRFFEWaveforms(niDigital, pinName, rffeCommand);

            //Create dynamic source waveform data for selected command
            rffeCommand.CreateSourceData(regData, out uint[] sourceData, out int byteCount);
            niDigital.SourceWaveforms.WriteBroadcast(rffeCommand.SourceName, sourceData);

            //reg0 set based on amount of bytes used
            niDigital.PatternControl.WriteSequencerRegister("reg0", byteCount);

            //Burst pattern based on the input trigger settings
            Digital.InitiatePatternGeneration(niDigital, rffeCommand.SourceName, triggerConfig);

            //On read calls only, return capture data
            return new ReadData();
        }
        static void CreateRFFEWaveforms(NIDigital niDigital, string pinName, RFFECommand rffeCommand)
        {
            //Create 1 bit sample width source and 8 bit sample width capture waveforms,
            //using appropriate name for the command.
            niDigital.SourceWaveforms.CreateSerial(pinName, rffeCommand.SourceName,
                SourceDataMapping.Broadcast, 1, BitOrder.MostSignificantBitFirst);
            niDigital.CaptureWaveforms.CreateSerial(pinName, rffeCommand.SourceName,
                8, BitOrder.MostSignificantBitFirst);
        }
    }
}
