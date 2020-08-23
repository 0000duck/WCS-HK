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
        public ObservableCollection<TreeNodeData> ModelTrees { set; get; } = new ObservableCollection<TreeNodeData>();
        public ObservableCollection<ProductParameter> ModelList { set; get; } = new ObservableCollection<ProductParameter>();
        public ProductParameter EditModel { set; get; }

        private readonly IProductParameterService _productParameterService;

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
            ModelTrees.Clear();
            ModelList.Clear();
            var list = _productParameterService.QueryableToList(x => x.id > 0).OrderBy(x=>x.product_name).ToList();
            if(list !=null && list.Count>0)
            {
                foreach (var item in list)
                {
                    ModelList.Add(item);
                    AddTreeNode(item);
                }
                EditModel = list[0];
            }
            else
            {
                EditModel = new ProductParameter();
            }
        }
        #region 对象操作
        public bool Insert(ProductParameter model)
        {
            long id = _productParameterService.InsertBigIdentity(model);
            if (id > 0)
            {
                model.id = (int)id;
                ModelList.Add(model);
                AddTreeNode(model);
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
            if(model.id>0)
            {
                if (_productParameterService.UpdateEntity(model))
                {
                    UpdateTreeNode(model);//更新树形菜单
                    return true;
                }
            }
            else
            {
                return Insert(model);
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
        public bool Remove(int ModelId)
        {
            var model = _productParameterService.QueryableToEntity(x=>x.id== ModelId);
            if(model !=null)
            {
                if (_productParameterService.Delete(x => x.id == ModelId))
                {
                    model = ModelList.FirstOrDefault(x => x.id == ModelId);
                    if(model !=null)
                    {
                        ModelList.Remove(model);//本地队列删除
                        RemoveTreeNode(model);
                    }
                    return true;
                }
            }

            return false;
        }
        #endregion

        private void AddTreeNode(ProductParameter model)
        {
            TreeNodeData n = new TreeNodeData() { id = model.id, Name = model.product_name,DisplayName= model.product_name };
            ModelTrees.Add(n);
        }
        private void UpdateTreeNode(ProductParameter model)
        {
            if (ModelTrees.Any(x => x.id == model.id))
            {
                var node = ModelTrees.FirstOrDefault(x => x.id == model.id);
                node.Name = model.product_name;
                node.DisplayName = model.product_name;
            }
        }
        private void RemoveTreeNode(ProductParameter model)
        {
            if (ModelTrees.Any(x => x.id == model.id))
            {
                var node = ModelTrees.FirstOrDefault(x => x.id == model.id);
                ModelTrees.Remove(node);//删除左侧树形
            }
        }
    }
}
