using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace BionicIcons
{
    class ThingSetMaker_AllBodyParts : ThingSetMaker
    {
        protected override IEnumerable<ThingDef> AllGeneratableThingsDebugSub(ThingSetMakerParams parms)
        {
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs.Where(x => x.isTechHediff)) {
                yield return def;
            }

            yield break;
        }

        protected override void Generate(ThingSetMakerParams parms, List<Thing> outThings)
        {
            foreach (ThingDef thingDef in AllGeneratableThingsDebugSub(parms))
            {
                ThingDef stuff = null;
                if (thingDef.MadeFromStuff)
                {
                    if (!(from x in GenStuff.AllowedStuffsFor(thingDef, TechLevel.Undefined)
                          where !PawnWeaponGenerator.IsDerpWeapon(thingDef, x)
                          select x).TryRandomElementByWeight((ThingDef x) => x.stuffProps.commonality, out stuff))
                    {
                        stuff = GenStuff.RandomStuffByCommonalityFor(thingDef, TechLevel.Undefined);
                    }
                }
                Thing thing = ThingMaker.MakeThing(thingDef, stuff);
                outThings.Add(thing);
            }
        }
    }
}
