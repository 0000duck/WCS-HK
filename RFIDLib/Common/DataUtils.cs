using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RFIDLib
{
    public class DataUtils
    {
        public byte[] f_NumToHex(string data)
        {
            char[] dataChar = new char[4];
            dataChar = data.ToCharArray();
            Array.Reverse(dataChar);
            byte[] dataByte = new byte[dataChar.Length];
            for (int i = 0; i < dataChar.Length; i++)
            {
                dataByte[i] = (byte)dataChar[i];
            }
            Array.Reverse(dataByte);
            return dataByte;
        }

        public byte[] s_NumToHex(string data)
        {
            char[] dataChar = new char[8];
            int[] temp = new int[8];
            dataChar = data.ToCharArray();
            Array.Reverse(dataChar);
            for (int i = 0; i < dataChar.Length; i++)
            {
                temp[i] = dataChar[i];
            }
            Array.Reverse(temp);
            string dataStr = "";
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] == 0)
                {
                    dataStr += "0";
                }
                else
                {
                    dataStr += Convert.ToChar(temp[i]);
                }
            }
            byte[] byteData = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                byteData[i] = Convert.ToByte(dataStr.Substring(i * 2, 2));

                byteData[i] = (byte)((byteData[i] % 10) + (byteData[i] / 10) * 16);
            }

            return byteData;

        }

        public ArrayList bytesToArray(byte[] bytes)
        {
            ArrayList al = new ArrayList();
            foreach (byte b in bytes)
            {
                al.Add(b);
            }

            return al;
        }

        public byte[] arrayToBytes(ArrayList al)
        {
            byte[] bytes = new byte[al.Count];
            for (int i = 0; i < al.Count; i++)
            {
                bytes[i] = (byte)al[i];
            }
            return bytes;
        }

        public string resultAnalyse(string result)
        {
            if (result != "")
            {
                int code = getResultCode(result);
                return codeAnalyse(code);
            }
            else
            {
                return "工装连接异常。";
            }
        }

        public string[] getRecieveSN(string data)
        {
            string[] result = { "", "" };
            string sData = "";

            if (data != "")
            {
                if (data.Length == 30)
                {
                    result[0] = data.Substring(6, 14);

                    sData = data.Substring(20, 8);
                    ArrayList al = new ArrayList();
                    for (int j = 0; j < 4; j++)
                    {
                        string strTemp = sData.Substring(j * 2, 2);
                        if (!strTemp.Equals("00"))
                        {
                            al.Add(strTemp);
                        }
                        else
                        {
                            if (al.Count > 0)
                            {
                                al.Add(strTemp);
                            }
                        }
                    }

                    string temp = "0";
                    if (al.Count != 0)
                    {
                        byte[] bTemp = new byte[al.Count];
                        uint ilife = 0;

                        for (int k = 0; k < al.Count; k++)
                        {
                            bTemp[k] = Convert.ToByte(al[k].ToString(), 16);
                            ilife += (uint)bTemp[k] << (k*8); 
                        }
                        uint ihour = ilife / 3600;
                        uint imin = (ilife % 3600) / 60;
                        uint isec = ilife % 60;
                        temp = Convert.ToString(ihour);
                        temp += "时";
                        temp += Convert.ToString(imin);
                        temp += "分";
                        temp += Convert.ToString(isec);
                        temp += "秒";
                    }
                    else
                    {
                        //foreach (var b in al)
                        //{
                        //    temp += b.ToString();
                        //}
                        //temp = "0";
                    }
                    result[1] = temp;
                }
                else
                {
                    result[0] = "数据读取异常。";
                }
            }
            else
            {
                result[0] = "工装连接异常。";
            }

            return result;

        }

        public int getResultCode(string result)
        {
            if (result != ""&&result.Length>7)
            {
                return int.Parse(result.Substring(4, 2));
            }
            else
            {
                return 0;
            }
            
        }

        public string codeAnalyse(int code)
        {
            if (code == 80)
            {
                return "烧录成功。";
            }
            else if (code == 81)
            {
                return "工装正常。";
            }
            else if(code == 82)
            {
                return "读卡超时。请检查标签是否放置正确。";
            }
            else if(code == 83)
            {
                return "数据校验失败";
            }
            else if (code == 84)
            {
                return "滤芯数据读取成功";
            }
            else if(code == 85)
            {
                return "SN读取成功";
            }
            else
            {
                return "未知错误。请检查";
            }
        }

        public string[] getRecieveFilterData(string data)
        {
            
            string[] result = { "", "", "", "" };
            if (data != "")
            {
                
                int code = getResultCode(data);
                if (code == 84)
                {
                    if (data.Length == 40)
                    {
                        string[] str = new string[4];
                        if (data != "")
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                str[i] = data.Substring(6 + 8 * i, 8);

                            }
                        }

                        byte[,] bytes = new byte[4, 4];
                        for (int i = 0; i < 4; i++)
                        {
                            ArrayList al = new ArrayList();
                            for (int j = 0; j < 4; j++)
                            {
                                string strTemp = str[i].Substring(j * 2, 2);
                                if (!strTemp.Equals("00"))
                                {
                                    al.Add(strTemp);
                                }
                                else
                                {
                                    if (al.Count > 0)
                                    {
                                        al.Add(strTemp);
                                    }
                                }
                            }

                            string temp = "";
                            if (i < 2 && al.Count != 0)
                            {
                                byte[] bTemp = new byte[al.Count];

                                for (int k = 0; k < al.Count; k++)
                                {
                                    bTemp[k] = Convert.ToByte(al[k].ToString(), 16);
                                }
                                temp = Encoding.ASCII.GetString(bTemp);
                            }
                            else
                            {
                                foreach (var b in al)
                                {
                                    temp += b.ToString();
                                }
                            }
                            result[i] = temp;
                        }
                    }
                   
                }             
            }

            return result;
        }       

        public string showFilterData(string[] str)
        {
            if (str[0] != "")
            {
                string result = "";
                result = "工厂编号：" + str[0] + "\n" + "产品编号：" + str[1] + "\n" + "生产日期：" + str[2] + "\n"
                    + "流水号：" + str[3];
                return result;
            }
            else
            {
                return "读取异常。";
            }
        }



    }
}