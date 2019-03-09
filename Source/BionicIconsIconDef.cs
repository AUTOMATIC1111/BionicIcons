using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Verse
{
    public class BionicIconsIconDef : Def
    {
        public string bodyPart;
        public string texture;

        public ThingDef thingDef;
        public string thingDefNameContains;
        public bool SolidOnly = false;
        public bool SoftOnly = false;
    }
}
