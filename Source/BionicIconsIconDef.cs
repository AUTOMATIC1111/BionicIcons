using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Verse
{
    public class BionicIconsIconDef : Def
    {
        public string bodyPart;
        public List<string> bodyParts;
        public string thingDef;
        public string texture;

        public string nameContains;
        public bool SolidOnly = false;
        public bool SoftOnly = false;

        public IEnumerable<string> BodyParts() {
            if (bodyPart != null)
                yield return bodyPart;

            if (bodyParts != null)
            {
                foreach(string s in bodyParts)
                    yield return s;
            }

            yield break;
        }
    }
}
