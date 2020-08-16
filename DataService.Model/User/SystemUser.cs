using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.DataService.Model
{
    [SugarTable("system_user")]
    public class SystemUser
    {
        /// <summary>
        /// 用户id(主键,自增列)
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        /// <summary>
        /// 账户
        /// </summary>
        [SugarColumn(Length = 20)]
        public string user_name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [SugarColumn(Length = 20)]
        public string user_password { get; set; }
        /// <summary>
        /// 原密码
        /// </summary>
        [SugarColumn(IsIgnore =true)]
        public string old_password { get; set; }
        /// <summary>
        /// 用户类别
        /// </summary>
        public int user_type { get; set; }
        /// <summary>
        /// 用户描述
        /// </summary>
        [SugarColumn(Length = 150,IsNullable =true)]
        public string comment { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime create_time { get; set; } = DateTime.Now;
    }
}
