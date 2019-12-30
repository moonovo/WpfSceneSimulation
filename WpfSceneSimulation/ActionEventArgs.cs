using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace WpfSceneSimulation
{
    public class ActionEventArgs : RoutedEventArgs
    {
        public ActionEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
        }

        public Storyboard Storyboard { get; set; }

        public ActionInfo ActionInfo { get; set; }
    }
}
