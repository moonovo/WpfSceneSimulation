using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace WpfSceneSimulation
{
    public class SceneCanvas : Canvas
    {
        static SceneCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SceneCanvas), new FrameworkPropertyMetadata(typeof(SceneCanvas)));
        }

        public SceneCanvas()
        {
            //this.Loaded += SceneCanvas_Loaded;
            this.Initialized += SceneCanvas_Initialized;
        }

        #region 初始化

        private void SceneCanvas_Initialized(object sender, EventArgs e)
        {
            InitChildren();
            InitPointList();
            InitEdgeList();
        }

        /// <summary>
        /// 初始化Canvas中的一级子控件
        /// </summary>
        private void InitChildren()
        {
            foreach (var child in this.Children)
            {
                var reachablePoint = child as ReachablePoint;
                if (reachablePoint != null)
                {
                    reachablePoint.Index = m_pointList.Count;
                    m_pointList.Add(reachablePoint);
                }
                else
                {
                    var control = child as FrameworkElement;
                    if (control != null && !SceneControlManager.GetIsStatic(control))
                    {
                        m_dynamicControlList.Add(control);
                        m_controlActionInfoList.Add(new ControlActionInfo(control));
                    }
                }
            }
        }

        /// <summary>
        /// 设置可到达点的X和Y值
        /// X和Y值为可到达点控件的中点在Canvas中的位置
        /// </summary>
        private void InitPointList()
        {
            foreach (var point in m_pointList)
            {
                var left = Canvas.GetLeft(point);
                var top = Canvas.GetTop(point);
                var width = point.Width;
                var height = point.Height;
                if (Double.IsNaN(width)) width = 0;
                if (Double.IsNaN(height)) height = 0;
                point.X = left + width / 2;
                point.Y = top + height / 2;
            }
        }

        /// <summary>
        /// 初始化所有边
        /// </summary>
        private void InitEdgeList()
        {
            foreach (var point in m_pointList)
            {
                if (string.IsNullOrWhiteSpace(point.To)) continue;
                List<string> toPointNameList = point.To.Split(',').ToList();
                foreach (var toPointName in toPointNameList)
                {
                    var toPoint = m_pointList.SingleOrDefault(o => o.Name == toPointName);
                    if (toPoint == null) continue;
                    m_edgeList.Add(new DirectedEdge() { StartPoint = point, EndPoint = toPoint });
                }
            }
        }

        #endregion

        #region 路由事件

        /// <summary>
        /// 移动开始路由事件
        /// </summary>
        public static readonly RoutedEvent MovingStartEvent =
            EventManager.RegisterRoutedEvent("MovingStart", RoutingStrategy.Bubble, typeof(EventHandler<ActionEventArgs>), typeof(SceneCanvas));

        /// <summary>
        /// 移动开始
        /// </summary>
        public event RoutedEventHandler MovingStart
        {
            add { this.AddHandler(MovingStartEvent, value); }
            remove { this.RemoveHandler(MovingStartEvent, value); }
        }


        /// <summary>
        /// 移动结束路由事件
        /// </summary>
        public static readonly RoutedEvent MovingEndEvent =
            EventManager.RegisterRoutedEvent("MovingEnd", RoutingStrategy.Bubble, typeof(EventHandler<ActionEventArgs>), typeof(SceneCanvas));

        /// <summary>
        /// 移动结束
        /// </summary>
        public event RoutedEventHandler MovingEnd
        {
            add { this.AddHandler(MovingEndEvent, value); }
            remove { this.RemoveHandler(MovingEndEvent, value); }
        }

        #endregion

        private List<FrameworkElement> m_dynamicControlList = new List<FrameworkElement>();
        private List<ReachablePoint> m_pointList = new List<ReachablePoint>();
        private List<DirectedEdge> m_edgeList = new List<DirectedEdge>();
        private List<ControlActionInfo> m_controlActionInfoList = new List<ControlActionInfo>();



        public ActionInfo ActionInfo
        {
            get { return (ActionInfo)GetValue(ActionInfoProperty); }
            set { SetValue(ActionInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActionInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActionInfoProperty =
            DependencyProperty.Register("ActionInfo", typeof(ActionInfo), typeof(SceneCanvas), new PropertyMetadata(ActionInfoChanged));

        private static void ActionInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var canvas = d as SceneCanvas;
            if (e.NewValue is MoveInfoByPointName)
            {
                canvas.AddMoveInfoByPointName(e.NewValue as MoveInfoByPointName);
            }
            else if (e.NewValue is MoveInfoByPosition)
            {
                canvas.AddMoveInfoByPosition(e.NewValue as MoveInfoByPosition);
            }
        }


        private void HandleActionInfo(ControlActionInfo controlActionInfo)
        {
            if (controlActionInfo == null)
                throw new ArgumentNullException("controlActionInfo");

            if (controlActionInfo.ActionQueue.Count == 0) return;
            if (controlActionInfo.InAction) return;
            var actionInfo = controlActionInfo.ActionQueue.Dequeue();
            if (actionInfo is MoveInfoByPosition)
            {
                HandleMoveInfoByPosition(controlActionInfo, actionInfo as MoveInfoByPosition);
            }
        }

        private void HandleMoveInfoByPosition(ControlActionInfo controlActionInfo, MoveInfoByPosition moveInfo)
        {
            var control = controlActionInfo.Control;
            var timeSpan = moveInfo.TimeSpan;
            double width = Double.IsNaN(control.Width) ? 0 : control.Width;
            double height = Double.IsNaN(control.Height) ? 0 : control.Height;

            if (timeSpan.TotalMilliseconds == 0) // 如果时间间隔为0 那么直接对位置进行赋值
            {
                this.RaiseEvent(new ActionEventArgs(MovingStartEvent, this) { Storyboard = null, ActionInfo = moveInfo });
                controlActionInfo.InAction = true;
                control.SetValue(Canvas.LeftProperty, moveInfo.DestX - width / 2);
                control.SetValue(Canvas.TopProperty, moveInfo.DestY - height / 2);
                controlActionInfo.InAction = false;
                this.RaiseEvent(new ActionEventArgs(MovingEndEvent, this) { Storyboard = null, ActionInfo = moveInfo });
                HandleActionInfo(controlActionInfo);
            }
            else
            {
                var storyboard = new Storyboard();

                var xda = new DoubleAnimation();
                Storyboard.SetTarget(xda, control);
                Storyboard.SetTargetProperty(xda, new PropertyPath(Canvas.LeftProperty));
                xda.From = (double)control.GetValue(Canvas.LeftProperty);
                xda.To = (moveInfo.DestX - width / 2);
                xda.Duration = new Duration(timeSpan);

                var yda = new DoubleAnimation();
                Storyboard.SetTarget(yda, control);
                Storyboard.SetTargetProperty(yda, new PropertyPath(Canvas.TopProperty));
                yda.From = (double)control.GetValue(Canvas.TopProperty);
                yda.To = (moveInfo.DestY - height / 2);
                yda.Duration = new Duration(timeSpan);
                storyboard.Children.Add(xda);
                storyboard.Children.Add(yda);

                storyboard.Completed += (sender, e) =>
                {
                    controlActionInfo.InAction = false;
                    this.RaiseEvent(new ActionEventArgs(MovingEndEvent, this) { Storyboard = storyboard, ActionInfo = moveInfo });
                    HandleActionInfo(controlActionInfo);
                };
                this.RaiseEvent(new ActionEventArgs(MovingStartEvent, this) { Storyboard = storyboard, ActionInfo = moveInfo });
                storyboard.Begin(this);
                controlActionInfo.InAction = true;
            }
        }


        /// <summary>
        /// 添加包含起始点名和结束点点名的移动信息
        /// 将此信息解析为一个或多个MoveInfoByPosition
        /// </summary>
        /// <param name="moveInfo"></param>
        private void AddMoveInfoByPointName(MoveInfoByPointName moveInfo)
        {
            if (moveInfo == null) return;
            var control = GetDynamicControlByName(moveInfo.ControlName);
            var startPoint = GetReachablePointByName(moveInfo.StartPointName);
            var endPoint = GetReachablePointByName(moveInfo.EndPointName);
            var timeSpan = moveInfo.TimeSpan;

            if (m_pointList.SingleOrDefault(o => o == startPoint) == null)
                throw new Exception("起点不在可到达点列表中");
            if (m_pointList.SingleOrDefault(o => o == endPoint) == null)
                throw new Exception("终点不在可到达点列表中");

            // 每个点的前一个点
            List<ReachablePoint> prePointList = m_pointList.Select(o => (ReachablePoint)null).ToList();
            // 每个点是否被访问过
            List<bool> isVisitedList = m_pointList.Select(o => false).ToList();
            Queue<ReachablePoint> queue = new Queue<ReachablePoint>();
            queue.Enqueue(startPoint);
            isVisitedList[startPoint.Index] = true;
            while (queue.Count > 0)
            {
                var curPoint = queue.Dequeue();
                var nearbyPointList = m_edgeList.Where(o => o.StartPoint == curPoint && !isVisitedList[o.EndPoint.Index]).Select(o => o.EndPoint).ToList();
                nearbyPointList.ForEach(o => isVisitedList[o.Index] = true);
                nearbyPointList.ForEach(o => prePointList[o.Index] = curPoint);
                // 找到了
                if (nearbyPointList.SingleOrDefault(o => o == endPoint) != null)
                {
                    break;
                }
                nearbyPointList.ForEach(o => queue.Enqueue(o));
            }

            var controlActionInfo = m_controlActionInfoList.SingleOrDefault(o => o.Control == control);
            if (controlActionInfo != null)
            {
                var edgeList = FindPath(prePointList, endPoint);
                if (edgeList == null || edgeList.Count == 0)
                {
                    if (startPoint == endPoint) edgeList = new List<DirectedEdge>() { new DirectedEdge() { StartPoint = startPoint, EndPoint = endPoint } };
                    else return;
                }

                int eachms = (int)(timeSpan.TotalMilliseconds / edgeList.Count);
                TimeSpan eachTimeSpan = new TimeSpan(0, 0, 0, 0, eachms);
                //edgeList.ForEach(o => controlActionInfo.ActionQueue.Enqueue(new MoveInfoByEdge() { DirectedEdge = o, TimeSpan = eachTimeSpan }));
                edgeList.ForEach(o => controlActionInfo.ActionQueue.Enqueue(new MoveInfoByPosition() { DestX = o.EndPoint.X, DestY = o.EndPoint.Y, TimeSpan = eachTimeSpan }));
            }
            HandleActionInfo(controlActionInfo);
        }

        private List<DirectedEdge> FindPath(List<ReachablePoint> prePointList, ReachablePoint endPoint)
        {
            List<DirectedEdge> edgeList = new List<DirectedEdge>();
            var curPoint = endPoint;
            while (prePointList[curPoint.Index] != null)
            {
                var prePoint = prePointList[curPoint.Index];
                var directedEdge = m_edgeList.SingleOrDefault(o => o.StartPoint == prePoint && o.EndPoint == curPoint);
                edgeList.Add(directedEdge);
                curPoint = prePoint;
            }
            edgeList.Reverse();
            return edgeList;
        }


        /// <summary>
        /// 添加包含目标位置的移动信息
        /// </summary>
        /// <param name="moveInfo"></param>
        private void AddMoveInfoByPosition(MoveInfoByPosition moveInfo)
        {
            if (moveInfo == null) throw new ArgumentNullException("moveInfo");
            var control = GetDynamicControlByName(moveInfo.ControlName);
            var controlActionInfo = m_controlActionInfoList.SingleOrDefault(o => o.Control == control);
            controlActionInfo.ActionQueue.Enqueue(moveInfo);
            HandleActionInfo(controlActionInfo);
        }

        /// <summary>
        /// 通过名字获取可到达点
        /// </summary>
        /// <param name="name">可到达点名字</param>
        /// <returns></returns>
        private ReachablePoint GetReachablePointByName(string name)
        {
            var reachPoint = m_pointList.SingleOrDefault(o => o.Name == name);
            if (reachPoint == null)
            {
                throw new Exception(string.Format("不存在名字为{0}的可到达点", name));
            }
            return reachPoint;
        }


        /// <summary>
        /// 通过名字获取动态控件
        /// </summary>
        /// <param name="name">动态控件名字</param>
        /// <returns></returns>
        private FrameworkElement GetDynamicControlByName(string name)
        {
            var control = m_dynamicControlList.SingleOrDefault(o => o.Name == name);
            if (control == null)
            {
                throw new Exception(string.Format("不存在名字为{0}的动态控件", name));
            }
            return control;
        }
    }
}
