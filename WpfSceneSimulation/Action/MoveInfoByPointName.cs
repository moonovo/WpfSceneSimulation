using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfSceneSimulation
{
    public class MoveInfoByPointName : ActionInfo
    {
        public string StartPointName { get; set; }

        public string EndPointName { get; set; }
    }
}
