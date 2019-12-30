using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfSceneSimulation
{
    public class ReachablePoint : Border
    {
        static ReachablePoint()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ReachablePoint), new FrameworkPropertyMetadata(typeof(ReachablePoint)));
        }

        private bool IsInDesignMode
        {
            get { return DesignerProperties.GetIsInDesignMode(this); }
        }

        internal int Index
        {
            get;
            set;
        }

        //public string PointName
        //{
        //    get { return (string)GetValue(PointNameProperty); }
        //    set { SetValue(PointNameProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for PointName.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty PointNameProperty =
        //    DependencyProperty.Register("PointName", typeof(string), typeof(ReachablePoint), new PropertyMetadata(null));



        public double X
        {
            get { return (double)GetValue(XProperty); }
            internal set { SetValue(XProperty, value); }
        }


        // Using a DependencyProperty as the backing store for X.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double), typeof(ReachablePoint), new PropertyMetadata(0.0));



        public double Y
        {
            get { return (double)GetValue(YProperty); }
            internal set { SetValue(YProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Y.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(double), typeof(ReachablePoint), new PropertyMetadata(0.0));




        public string To
        {
            get { return (string)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        // Using a DependencyProperty as the backing store for To.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(string), typeof(ReachablePoint), new PropertyMetadata(null));



        public override string ToString()
        {
            return string.Format("Name:{0} To:{1}", Name, To);
        }
    }
}
