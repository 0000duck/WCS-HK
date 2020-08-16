using System;
using System.ComponentModel;
using SqlSugar;

namespace iFactory.DataService.Model
{
    [SugarTable("plc_tags")]
    public class DatabaseTag
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { set; get; }

        public int GroupId { set; get; }
        /// <summary>
        /// 周期读取标识位
        /// </summary>
        public bool CycleRead { set; get; } = false;
        public int Length { set; get; }
        public string TagName { set; get; }
        public int DataType { set; get; }
        public string TagAddr { set; get; }
        [SugarColumn(IsNullable = true)]
        public string description { set; get; }
       
    }
    
}
