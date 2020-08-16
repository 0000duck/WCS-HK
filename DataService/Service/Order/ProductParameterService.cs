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
    public class ProductParameterService : BaseService<ProductParameter>, IProductParameterService
    {
        public ProductParameterService(ISqlSugarClient sqlSugarClient) : base(sqlSugarClient)
        {

        }
    }
}
