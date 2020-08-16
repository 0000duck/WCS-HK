using iFactory.DataService.IService;
using iFactory.DataService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.UserManage
{
    public class UserInitial
    {
       /// <summary>
       /// 用户管理初始化
       /// </summary>
       /// <param name="_userService"></param>
       /// <param name="_userGroupService"></param>
        public static void UserDataInitial(IUserService _userService, IUserGroupService _userGroupService)
        {
            long opId=0, managerId=1, adminId=2;
          
            UserGroup userGroup= _userGroupService.QueryableToEntity(x=>x.group_code== "Operator");
            if(userGroup==null)
            {
                userGroup = new UserGroup() { group_code = "Operator", group_name = "操作员", group_description = "" };
                opId= _userGroupService.InsertBigIdentity(userGroup);
            }

            userGroup = _userGroupService.QueryableToEntity(x => x.group_code == "Manager");
            if (userGroup == null)
            {
                userGroup = new UserGroup() { group_code = "Manager", group_name = "管理员", group_description = "" };
                managerId= _userGroupService.InsertBigIdentity(userGroup);
            }

            userGroup = _userGroupService.QueryableToEntity(x => x.group_code == "Administrator");
            if (userGroup == null)
            {
                userGroup = new UserGroup() { group_code = "Administrator", group_name = "系统管理员", group_description = "" };
                adminId= _userGroupService.InsertBigIdentity(userGroup);
            }

            SystemUser user = _userService.QueryableToEntity(x => x.user_name == "admin");
            if (user == null)
            {
                user = new SystemUser() { user_name = "admin", user_password = "admin", user_type = (int)adminId, create_time = DateTime.Now };
                _userService.Insert(user);

            }

            user = _userService.QueryableToEntity(x => x.user_name == "op");
            if (user == null)
            {
                user = new SystemUser() { user_name = "op", user_password = "1", user_type = (int)opId, create_time = DateTime.Now };
                _userService.Insert(user);
            }
        }
    }
}
