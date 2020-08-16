using iFactory.CommonLibrary;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace iFactory.UserManage
{
    /// <summary>
    /// 用户类型
    /// </summary>
    public enum UserType : int
    {
        /// <summary>
        /// 操作员
        /// </summary>
        [Description("操作员")]
        Operator = 1,
        /// <summary>
        /// 管理者
        /// </summary>
        [Description("管理员")]
        Manager = 2,
        /// <summary>
        /// 系统管理员
        /// </summary>
        [Description("系统管理员")]
        Admin = 3
    }

    /// <summary>
    /// 单元格颜色
    /// </summary>
    public class UserTypeConverter : IValueConverter
    {
        private static List<EnumStruct<int>> UserTypeList = EnumHelper.ConvertEnumToList<int>(typeof(UserType));
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
            var item = UserTypeList.FirstOrDefault(x => x.Value == keyValue);
            if (item != null)
            {
                return item.Description;
            }
            return null;
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
            var item = UserTypeList.FirstOrDefault(x => x.Description == des);
            if (item != null)
            {
                return item.Value;
            }
            return 0;
        }
    }
}
