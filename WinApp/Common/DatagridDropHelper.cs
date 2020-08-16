using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace iFactoryApp.View
{
    // Declare a Delegate which will return the position of the 
    // DragDropEventArgs and the MouseButtonEventArgs event object
    public delegate Point GetDragDropPosition(IInputElement theElement);

    public class DatagridDropHelper<T> where T :class
    {
        private int prevRowIndex = -1;
        private DataGrid datagrid;
        private ObservableCollection<T> sourceCollection;

        /// <summary>
        /// datagrid表格拖动
        /// </summary>
        /// <param name="dataGrid">表格对象</param>
        /// <param name="collection">模型队列</param>
        public DatagridDropHelper(DataGrid dataGrid, ObservableCollection<T> collection)
        {
            datagrid = dataGrid;
            sourceCollection = collection;
            //设置为true可能会导致checkbox无法选择
            datagrid.AddHandler(InkCanvas.MouseLeftButtonDownEvent, new MouseButtonEventHandler(dg_PreviewMouseLeftButtonDown), false);
            //The Event on DataGrid for selecting the Row
           //this.datagrid.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(dg_PreviewMouseLeftButtonDown);
            //this.datagrid.MouseLeftButtonDown += new MouseButtonEventHandler(dg_PreviewMouseLeftButtonDown);
            //The Drop Event
            this.datagrid.Drop += new DragEventHandler(dg_Drop);
        }

        /// <summary>
        /// Defines the Drop Position based upon the index.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dg_Drop(object sender, DragEventArgs e)
        {
            if (prevRowIndex < 0)
                return;

            int index = this.GetDataGridItemCurrentRowIndex(e.GetPosition);

            //The current Rowindex is -1 (No selected)
            if (index < 0)
                return;
            //If Drag-Drop Location are same
            if (index == prevRowIndex)
                return;
            //If the Drop Index is the last Row of DataGrid(
            // Note: This Row is typically used for performing Insert operation)
            if (index == datagrid.Items.Count - 1)
            {
                MessageBox.Show("拖动范围已经超出行范围，请拖动到最后一行之前");
                return;
            }

            T previousItem = sourceCollection[prevRowIndex];
            sourceCollection.RemoveAt(prevRowIndex);//移除
            sourceCollection.Insert(index, previousItem);//重新插入

            if (MoveFinishedEvent != null)//派送事件外部处理
            {
                MoveFinishedEvent(prevRowIndex, index);
            }
        }
        //左键按下
        void dg_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            prevRowIndex = GetDataGridItemCurrentRowIndex(e.GetPosition);

            if (prevRowIndex < 0)
                return;
            datagrid.SelectedIndex = prevRowIndex;

            T selectedItem = datagrid.Items[prevRowIndex] as T;

            if (selectedItem == null)
                return;

            //Now Create a Drag Rectangle with Mouse Drag-Effect
            //Here you can select the Effect as per your choice

            DragDropEffects dragdropeffects = DragDropEffects.Move;

            if (DragDrop.DoDragDrop(datagrid, selectedItem, dragdropeffects)
                                != DragDropEffects.None)
            {
                //Now This Item will be dropped at new location and so the new Selected Item
                datagrid.SelectedItem = selectedItem;
            }
        }

        /// <summary>
        /// Method checks whether the mouse is on the required Target
        /// Input Parameter (1) "Visual" -> Used to provide Rendering support to WPF
        /// Input Paraneter (2) "User Defined Delegate" positioning for Operation
        /// </summary>
        /// <param name="theTarget"></param>
        /// <param name="pos"></param>
        /// <returns>The "Rect" Information for specific Position</returns>
        private bool IsTheMouseOnTargetRow(Visual theTarget, GetDragDropPosition pos)
        {
            if (theTarget == null) return false;
            Rect posBounds = VisualTreeHelper.GetDescendantBounds(theTarget);
            Point theMousePos = pos((IInputElement)theTarget);
            return posBounds.Contains(theMousePos);
        }
        /// <summary>
        /// Returns the selected DataGridRow
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private DataGridRow GetDataGridRowItem(int index)
        {
            if (datagrid.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return null;

            return datagrid.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
        }

        /// <summary>
        /// Returns the Index of the Current Row.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private int GetDataGridItemCurrentRowIndex(GetDragDropPosition pos)
        {
            int curIndex = -1;
            for (int i = 0; i < datagrid.Items.Count; i++)
            {
                DataGridRow itm = GetDataGridRowItem(i);
                if (IsTheMouseOnTargetRow(itm, pos))
                {
                    curIndex = i;
                    break;
                }
            }
            return curIndex;
        }

        /// <summary>
        /// 拖动完成
        /// </summary>
        /// <param name="previousIndex">拖动前的序号</param>
        /// <param name="currentIndex">当前拖动后的序号</param>
        public delegate void MoveFinishedDelegate(int previousIndex, int currentIndex);

        /// <summary>
        /// 拖动完成
        /// </summary>
        public event MoveFinishedDelegate MoveFinishedEvent = delegate { };
    }
}
