using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Objectives
{
    None,

    Collect,        // "use" an object that is of collectible type (e.g.; a backpack)
    Use,            // use an interactive object (e.g. toothbrush)
    AreaTrigger,    // collide with an areatrigger (e.g.; outside door trigger)
    Misc,           // completed by an external script
}
