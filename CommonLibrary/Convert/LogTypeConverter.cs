using System;
using System.Globalization;
using System.Windows.Data;

namespace iFactory.CommonLibrary
{
    /// <summary>
    /// 根据日志类别，显示对应的信息。=0为消息，=1为错误
    /// </summary>
    public class LogTypeConverter : IValueConverter
    {
        /// <summary>
        /// 将数据转换成需要显示的格式  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int keyValue = System.Convert.ToInt32(value);

            if (keyValue == (int)LogTypeEnum.Info)
            {
                return "消息";
            }
            else
            {
                return "错误";
            }
        }
        /// <summary>
        /// 将显示的数据格式转回  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns> 
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string des = value.ToString();

            if (des.Equals("消息"))
            {
                return (int)LogTypeEnum.Info;
            }
            return (int)LogTypeEnum.Error;
        }
    }
}
