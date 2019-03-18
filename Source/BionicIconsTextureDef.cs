using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Verse
{
    public class BionicIconsTextureDef : Def
    {
        public string texture;
        public string replacement = "BionicIcons/Boxes/Default";
        public Color color = new Color(1f,1f,1f);

        public string nameContains;
    }
}
