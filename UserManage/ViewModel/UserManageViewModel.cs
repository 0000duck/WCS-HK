using GalaSoft.MvvmLight;
using iFactory.CommonLibrary;
using iFactory.DataService.IService;
using iFactory.DataService.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace iFactory.UserManage
{
    public class UserManageViewModel : ViewModelBase
    {
        public IUserService userService;
        public IUserGroupService userGroupService;

        public SystemUser EditModel { set; get; }
        public SystemUser SelectModel { set; get; }
        public ObservableCollection<SystemUser> ModelList { set; get; } = new ObservableCollection<SystemUser>();
        public List<UserGroup> UserGroupList { set; get; } = new List<UserGroup>();
        public List<EnumStruct<int>> TypeList = EnumHelper.ConvertEnumToList<int>(typeof(UserType));

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public UserManageViewModel(IUserService userService,
                                   IUserGroupService userGroupService)
        {
            this.userService = userService;
            this.userGroupService = userGroupService;
        }
        /// <summary>
        /// 加载所有
        /// </summary>
        /// <param name="user_tyepe"></param>
        public void LoadAllModels(int user_tyepe=-1)
        {
            ModelList.Clear();
            List<SystemUser> users = new List<SystemUser>();
            if(user_tyepe>0)
            {
                users = userService.QueryableToList(x => x.id > 0 && x.user_type== user_tyepe);
            }
            else
            {
                users = userService.QueryableToList(x => x.id > 0);
            }
            foreach (var item in users)
            {
                ModelList.Add(item);
            }
        }

        #region 对象操作
        public bool Insert(SystemUser model)
        {
            long id = userService.InsertBigIdentity(model);
            if (id>0)
            {
                return true;
            }
            return false;
        }
        public bool Update(SystemUser model)
        {
            if(userService.UpdateEntity(model))
            {
                return true;
            }
            return false;
        }
        public bool Remove(SystemUser model)
        {
            if (userService.IsAny(x => x.id == model.id))
            {
                if (userService.Delete(x => x.id == model.id))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}