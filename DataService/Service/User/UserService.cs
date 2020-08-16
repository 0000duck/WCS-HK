using iFactory.DataService.IService;
using iFactory.DataService.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.DataService.Service
{
    public class UserService : BaseService<SystemUser>, IUserService
    {
        public UserService(ISqlSugarClient sqlSugarClient) : base(sqlSugarClient)
        {

        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UserLogin(string Name, string Password)
        {
            try
            {
                SystemUser user = this.QueryableToEntity(x => x.user_name.Equals(Name));
                if (user != null)
                {
                    if (user.user_password.Equals(Password))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return false;
        }
    }
}
