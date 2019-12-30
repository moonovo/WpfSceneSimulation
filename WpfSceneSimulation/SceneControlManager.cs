using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfSceneSimulation
{
    public class SceneControlManager : DependencyObject
    {
        /// <summary>
        /// 是否为静态控件
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetIsStatic(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsStaticProperty);
        }

        public static void SetIsStatic(DependencyObject obj, bool value)
        {
            obj.SetValue(IsStaticProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsStatic.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsStaticProperty =
            DependencyProperty.RegisterAttached("IsStatic", typeof(bool), typeof(SceneControlManager), new PropertyMetadata(true));


        /// <summary>
        /// 动态控件需指定当前位置点 未指定则控件不显示
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ReachablePoint GetCurPosition(DependencyObject obj)
        {
            return (ReachablePoint)obj.GetValue(CurPositionProperty);
        }

        public static void SetCurPosition(DependencyObject obj, ReachablePoint value)
        {
            obj.SetValue(CurPositionProperty, value);
        }

        // Using a DependencyProperty as the backing store for CurPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurPositionProperty =
            DependencyProperty.RegisterAttached("CurPosition", typeof(ReachablePoint), typeof(SceneControlManager), new PropertyMetadata(null));


    }
}
