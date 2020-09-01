using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace RFIDLib
{
    public class Constans
    {       
        public static Color errorColor = (Color)ColorConverter.ConvertFromString("#FFEB2B2B");
        public static Color successColor= (Color)ColorConverter.ConvertFromString("#FF0DE510");
        public static Color whtieColor = (Color)ColorConverter.ConvertFromString("#00000000");
        public static byte[] getHeartBeatCode = { 0xfa, 01, 01, 0xfa };
        public static byte[] getFilterDataCode = { 0xfa, 01, 03, 0xf8 };
        public static byte[] getFilterSNCode = { 0xfa, 01, 04, 0xff };
        public static byte[] burnFilterDataCode = { 0xfa, 17, 02 };
        public static byte[] reburnFilterDataCode = { 0xfa, 17, 05 };


    }
}