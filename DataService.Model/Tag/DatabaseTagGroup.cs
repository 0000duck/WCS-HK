using System;
using System.ComponentModel;
using SqlSugar;

namespace iFactory.DataService.Model
{
    [SugarTable("plc_group")]
    public class DatabaseTagGroup
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { set; get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// ip
        /// </summary>
        public string Ip { set; get; }
        /// <summary>
        /// 端口号 
        /// </summary>
        public int Port { set; get; }
        private bool _IsConnected = false;
       
        public bool Active { set; get; } = true;
        /// <summary>
        /// 扫描周期
        /// </summary>
        public int CycleTime { set; get; } = 500;
        /// <summary>
        /// 心跳位
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string HeartBit { set; get; }
        /// <summary>
        /// plc类型。0西门子，1欧姆龙
        /// </summary>
        public int DeviceType { set; get; } = 0;
        [SugarColumn(IsNullable = true)]
        public string Description { set; get; }

    }
    
}
