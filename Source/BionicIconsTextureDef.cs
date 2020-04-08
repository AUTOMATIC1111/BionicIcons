using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Verse
{
    public class BionicIconsTextureDef : Def
    {
        public static Color colorMissing = new Color(0f, 0f, 0f);

        public string texture;
        public string replacement = "BionicIcons/Boxes/Default";
        public Color color = new Color(1f, 1f, 1f);
        public Color colorIcon = new Color(0.25f, 0.25f, 0.25f);
        public Color onlyForColor = colorMissing;

        public string nameContains;
        public string onlyForMod;
    }
}
