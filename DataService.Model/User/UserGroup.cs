using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.DataService.Model
{
    [SugarTable("user_group")]
    public class UserGroup
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        /// <summary>
        /// 群组编码
        /// </summary>
        [SugarColumn(Length = 20)]
        public string group_code { get; set; }
        /// <summary>
        /// 群组名称
        /// </summary>
        [SugarColumn(Length = 200,IsNullable =true)]
        public string group_name { get; set; }
        /// <summary>
        /// 群组描述
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string group_description { get; set; }
    }
}
