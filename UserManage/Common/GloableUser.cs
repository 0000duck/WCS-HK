using iFactory.CommonLibrary;
using iFactory.DataService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFactory.UserManage
{
    public class GloableUserInfo
    {
        /// <summary>
        /// 当前登录的用户
        /// </summary>
        public static SystemUser LoginUser;
        /// <summary>
        /// 用户组的列表
        /// </summary>
        public static List<EnumStruct<int>> UserTypeList = EnumHelper.ConvertEnumToList<int>(typeof(UserType));

        static GloableUserInfo()
        {

        }


    }
}
