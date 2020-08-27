using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.DevComServer
{
    public interface IPLCHelper:IDisposable
    {
        void ConnectToPlc(string Address);
        void ConnectToPlc(string Address, int Port);
        void ConnectToPlc(string Address, int Port, PLCType plcType);
        void ConnectToPlc(string Address, int port, byte station =0);
        void ReConnectToPlc();

        bool CheckConnect();

        bool WriteValue(string Address, bool value);
        bool WriteValue(string Address, short value);
        bool WriteValue(string Address, int value);
        bool WriteValue(string Address, float value);
        // bool WriteValue(string address, string value, bool WCharMode = true, int Length = -1);

        bool ReadValue(string Address, out bool value);
        bool ReadValue(string Address, out short value);
        bool ReadValue(string Address, out int value);
        bool ReadValue(string Address, out float value);

        bool BatchReadValue(string Address, ushort length, out bool[] value);
        bool BatchReadValue(string Address, ushort length, out short[] value);
        bool BatchReadValue(string Address, ushort length, out int[] value);
        bool BatchReadValue(string Address, ushort length, out float[] value);
        bool BatchReadValue(string Address, ushort length, out string[] value);

        bool BatchWriteValue(string Address, short[] value);
    }
}
