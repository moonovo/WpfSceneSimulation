using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfSceneSimulation
{
    public class DirectedEdge
    {
        public ReachablePoint StartPoint { get; set; }

        public ReachablePoint EndPoint { get; set; }

        public override string ToString()
        {
            return string.Format("{0} To {1}", StartPoint.Name, EndPoint.Name);
        }
    }
}
