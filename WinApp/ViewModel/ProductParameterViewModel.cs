using iFactory.CommonLibrary;
using iFactory.DataService.IService;
using iFactory.DataService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace iFactoryApp.ViewModel
{
    public class ProductParameterViewModel
    {
        private readonly IProductParameterService _productParameterService;
        public ObservableCollection<NodeData> trees = new ObservableCollection<NodeData>();
        public ObservableCollection<ProductParameter> ModelList { set; get; } = new ObservableCollection<ProductParameter>();
        public ProductParameter EditModel { set; get; }
        public ProductParameter SelectedModel { set; get; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public ProductParameterViewModel(IProductParameterService productParameterService)
        {
            _productParameterService = productParameterService;
            LoadAllInfos();
        }
        public void LoadAllInfos()
        {
            var list = _productParameterService.QueryableToList(x => x.id > 0).OrderBy(x=>x.product_name).ToList();
            foreach (var item in list)
            {
                ModelList.Add(item);

                NodeData n = new NodeData();
                n.Name = item.product_name;//device_id
                trees.Add(n);
            }
        }
        #region 对象操作
        public bool Insert(ProductParameter model)
        {
            long id = _productParameterService.InsertBigIdentity(model);
            if (id > 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="IsUpdateDetails">更新明细</param>
        /// <returns></returns>
        public bool Update(ProductParameter model, bool IsUpdateDetails = true)
        {
            if (_productParameterService.UpdateEntity(model))
            {
                return true;
            }
            return false;
        }
        public bool Remove(ProductParameter model, bool RemoveListOnly = false)
        {
            if (RemoveListOnly)
            {
                if (ModelList.Remove(model))
                {
                    return true;
                }
            }
            else
            {
                if (_productParameterService.IsAny(x => x.id == model.id))
                {
                    if (_productParameterService.Delete(x => x.id == model.id))
                    {
                         return true;
                    }
                }
            }

            return false;
        }
        #endregion
    }

    public class NodeData
    {
        public string Name { get; set; }
        public string Root { get; set; }
        public string DisplayName { get; set; }

        public List<NodeData> ChildNodes { get; set; }

        public NodeData()
        {
            ChildNodes = new List<NodeData>();
        }
    }
}
