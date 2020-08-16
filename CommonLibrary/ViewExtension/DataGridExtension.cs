using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace iFactory.CommonLibrary
{
    public delegate Point GetDragDropPosition(IInputElement theElement);

    public class DataGridExtension
    {
        /// <summary>
        /// Method checks whether the mouse is on the required Target
        /// Input Parameter (1) "Visual" -> Used to provide Rendering support to WPF
        /// Input Paraneter (2) "User Defined Delegate" positioning for Operation
        /// </summary>
        /// <param name="theTarget"></param>
        /// <param name="pos"></param>
        /// <returns>The "Rect" Information for specific Position</returns>
        private static bool IsTheMouseOnTargetRow(Visual theTarget, GetDragDropPosition pos)
        {
            Rect posBounds = VisualTreeHelper.GetDescendantBounds(theTarget);
            Point theMousePos = pos((IInputElement)theTarget);
            return posBounds.Contains(theMousePos);
        }
        /// <summary>
        /// Returns the selected DataGridRow
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private static DataGridRow GetDataGridRowItem(int index, DataGrid dgEmployee)
        {
            if (dgEmployee.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return null;

            return dgEmployee.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
        }


        /// <summary>
        /// Returns the Index of the Current Row.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static int GetDataGridItemCurrentRowIndex(GetDragDropPosition pos, DataGrid dgEmployee)
        {
            int curIndex = -1;
            for (int i = 0; i < dgEmployee.Items.Count; i++)
            {
                DataGridRow itm = GetDataGridRowItem(i, dgEmployee);
                if (IsTheMouseOnTargetRow(itm, pos))
                {
                    curIndex = i;
                    break;
                }
            }
            return curIndex;
        }
    }
}
