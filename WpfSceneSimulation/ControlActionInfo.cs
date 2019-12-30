using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfSceneSimulation
{
    internal class ControlActionInfo
    {
        public ControlActionInfo(FrameworkElement control)
        {
            Control = control;
            ActionQueue = new Queue<ActionInfo>();
        }

        public FrameworkElement Control { get; private set; }

        public Queue<ActionInfo> ActionQueue { get; private set; }

        public bool InAction { get; set; }

        public override string ToString()
        {
            if (Control == null) return "null";
            return string.Format("{0}", Control.Name);
        }
    }
}
