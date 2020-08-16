using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.DevComServer
{
    public interface IPLCHelper:IDisposable
    {
        void ConnectToPlc(string Addr);
        void ConnectToPlc(string Addr, int Port);
        void ConnectToPlc(string Addr, int Port, PLCType plcType);
        void ReConnectToPlc();

        bool CheckConnect();

        bool WriteValue(string address, bool value);
        bool WriteValue(string address, short value);
        bool WriteValue(string address, int value);
        bool WriteValue(string address, float value);
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
    }
}
