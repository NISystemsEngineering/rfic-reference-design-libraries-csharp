using System;
using System.Collections.Generic;
using System.Text;
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
            public abstract int ByteCountLimit
            {
                get;
            }
            public abstract int UpperAddressLimit
            {
                get;
            }
            static public RFFECommand Reg0Write
            {
                get { return new Reg0Write(); }
            }

            public void ValidateLogic(RegisterData regData)
            {
                if (regData.SlaveAddress < 0x0 || regData.SlaveAddress > 0x10)
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

        }
        public class Reg0Write : RFFECommand
        {
            public override int ByteCountLimit => 1;
            public override int UpperAddressLimit => 0xFFFF;
        }
        #endregion
        public static ReadData BurstRFFE(NIDigital niDigital, RegisterData regData, string pinName, RFFECommand rffeCommand)
        {
            rffeCommand.ValidateLogic(regData);
            return new ReadData();
        }
    }
}
