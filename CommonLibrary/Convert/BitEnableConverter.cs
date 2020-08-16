using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace iFactory.CommonLibrary
{
    /// <summary>
    /// bit位转换器
    /// </summary>
    public class BitEnableConverter : IValueConverter
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
            bool keyValue = System.Convert.ToBoolean(value);

            if (keyValue == true)
            {
                return "是";
            }
            return "否";
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

            if (des.Equals("是"))
            {
                return true;
            }
            return false;
        }
    }
}
