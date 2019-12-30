using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfSceneSimulation
{
    public class RotateInfo : ActionInfo
    {
        public double Angle { get; set; }

        public bool IsAnticlockwise { get; set; }
    }
}
