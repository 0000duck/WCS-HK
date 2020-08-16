using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace iFactory.CommonLibrary
{
    public class ObservableCollectionHelper
    {
        /// <summary>
        /// 插入对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Collection"></param>
        /// <param name="item"></param>
        public static void InsertItem<T>(ObservableCollection<T> Collection, T item,int index=0)
        {
            try
            {
                if (System.Windows.Application.Current == null) return;
                ThreadPool.QueueUserWorkItem(delegate
                {
                    System.Threading.SynchronizationContext.SetSynchronizationContext(new
                        System.Windows.Threading.DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
                    System.Threading.SynchronizationContext.Current.Post(pl =>
                    {
                        Collection.Insert(index, item);
                    }, null);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        /// <summary>
        /// 增加对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Collection"></param>
        /// <param name="item"></param>
        public static void AddItem<T>(ObservableCollection<T> Collection, T item)
        {
            try
            {
                if (System.Windows.Application.Current == null) return;
                ThreadPool.QueueUserWorkItem(delegate
                {
                    System.Threading.SynchronizationContext.SetSynchronizationContext(new
                        System.Windows.Threading.DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
                    System.Threading.SynchronizationContext.Current.Post(pl =>
                    {
                        Collection.Add(item);
                    }, null);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// 移除对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Collection"></param>
        /// <param name="item"></param>
        public static void RemoveItem<T>(ObservableCollection<T> Collection, T item)
        {
            try
            {
                if (System.Windows.Application.Current == null) return;
                ThreadPool.QueueUserWorkItem(delegate
                {
                    System.Threading.SynchronizationContext.SetSynchronizationContext(new
                        System.Windows.Threading.DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
                    System.Threading.SynchronizationContext.Current.Post(pl =>
                    {
                        Collection.Remove(item);
                    }, null);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// 清除对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Collection"></param>
        /// <param name="item"></param>
        public static void ClearItem<T>(ObservableCollection<T> Collection)
        {
            try
            {
                if (System.Windows.Application.Current == null) return;
                ThreadPool.QueueUserWorkItem(delegate
                {
                    System.Threading.SynchronizationContext.SetSynchronizationContext(new
                        System.Windows.Threading.DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
                    System.Threading.SynchronizationContext.Current.Post(pl =>
                    {
                        Collection.Clear();
                    }, null);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
