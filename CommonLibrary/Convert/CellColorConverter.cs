using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace iFactory.CommonLibrary
{
    /// <summary>
    /// 单元格颜色
    /// </summary>
    public class CellColorConverter : IValueConverter
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
                return new SolidColorBrush(Colors.Black);
            }
            else
            {
                return new SolidColorBrush(Colors.Red);
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
            throw new NotImplementedException();
        }
    }
}
