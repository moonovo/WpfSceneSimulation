using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfSceneSimulation
{
    public class Scene : Grid
    {
        static Scene()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Scene), new FrameworkPropertyMetadata(typeof(Scene)));
        }

        public Scene()
        {
            this.SizeChanged += Scene_SizeChanged;
        }

        /// <summary>
        /// 当前是否为设计模式
        /// </summary>
        private bool IsInDesignMode
        {
            get { return DesignerProperties.GetIsInDesignMode(this); }
        }

        /// <summary>
        /// 现场实际宽度
        /// </summary>
        public double SceneWidth
        {
            get { return (double)GetValue(SceneWidthProperty); }
            set { SetValue(SceneWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SceneWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SceneWidthProperty =
            DependencyProperty.Register("SceneWidth", typeof(double), typeof(Scene), new PropertyMetadata(100.0, SceneWidthChanged));

        private static void SceneWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Scene scene = d as Scene;
            if (scene.Children.Count != 1) return;
            var content = scene.Children[0] as SceneCanvas;
            if (scene.IsInDesignMode && content != null)
            {
                content.Width = (double)e.NewValue;
            }
        }

        /// <summary>
        /// 现场实际高度
        /// </summary>
        public double SceneHeight
        {
            get { return (double)GetValue(SceneHeightProperty); }
            set { SetValue(SceneHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SceneHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SceneHeightProperty =
            DependencyProperty.Register("SceneHeight", typeof(double), typeof(Scene), new PropertyMetadata(100.0, SceneHeightChanged));

        private static void SceneHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Scene scene = d as Scene;
            if (scene.Children.Count != 1) return;
            var content = scene.Children[0] as SceneCanvas;
            if (scene.IsInDesignMode && content != null)
            {
                content.Height = (double)e.NewValue;
            }
        }


        /// <summary>
        /// 是否固定宽高比例
        /// </summary>
        public bool FixedRatio
        {
            get { return (bool)GetValue(FixedRatioProperty); }
            set { SetValue(FixedRatioProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FixedRatio.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FixedRatioProperty =
            DependencyProperty.Register("FixedRatio", typeof(bool), typeof(Scene), new PropertyMetadata(false));


        private void Scene_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsInDesignMode) return;
            if (this.Children.Count != 1) return;
            var content = this.Children[0] as SceneCanvas;
            content.RenderTransformOrigin = new Point(0.5, 0.5);
            TransformGroup tgnew = new TransformGroup();
            ScaleTransform st = new ScaleTransform();

            if (this.FixedRatio)
            {
                double min = Math.Min(this.ActualWidth / SceneWidth, this.ActualHeight / SceneHeight);
                st.ScaleX = min;
                st.ScaleY = min;
            }
            else
            {
                st.ScaleX = this.ActualWidth / SceneWidth;
                st.ScaleY = this.ActualHeight / SceneHeight;
            }

            //Console.WriteLine(string.Format("Parent.ActualWidth:{0} Parent.ActualHeight:{1}", this.ActualWidth, this.ActualHeight));
            //Console.WriteLine(string.Format("ActualWidth:{0} ActualHeight:{1} ScaleX:{2} ScaleY:{3}", content.ActualWidth, content.ActualHeight, st.ScaleX, st.ScaleY));

            tgnew.Children.Add(st);

            // 给图像赋值Transform变换属性
            content.RenderTransform = tgnew;
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            var canvas = visualAdded as SceneCanvas;
            if (canvas == null) throw new Exception("内容只能有一个并且必须为SceneCanvas");
            if (this.Children.Count != 1) throw new Exception("内容只能有一个");
            canvas.Height = SceneHeight;
            canvas.Width = SceneWidth;
            canvas.HorizontalAlignment = HorizontalAlignment.Center;
            canvas.VerticalAlignment = VerticalAlignment.Center;
            //canvas.VisualChildrenChanged += Canvas_VisualChildrenChanged;
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

    }
}
