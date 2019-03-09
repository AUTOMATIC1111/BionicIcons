using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;
using System;

namespace BionicIcons
{
    [StaticConstructorOnStartup]
    public class BionicIcons
    {
        static MethodInfo graphicDataInit = typeof(GraphicData).GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Instance);
        static Dictionary<string, List<BionicIconsTextureDef>> replacements = new Dictionary<string, List<BionicIconsTextureDef>>();
        static Dictionary<BodyPartDef, List<BionicIconsIconDef>> replacementsIcon = new Dictionary<BodyPartDef, List<BionicIconsIconDef>>();

        static BionicIcons()
        {
            foreach (BionicIconsTextureDef def in DefDatabase<BionicIconsTextureDef>.AllDefs)
            {
                List<BionicIconsTextureDef> list;
                if (!replacements.TryGetValue(def.texture, out list))
                {
                    list = new List<BionicIconsTextureDef>();
                    replacements.Add(def.texture, list);
                }

                list.Add(def);
            }

            foreach (BionicIconsIconDef def in DefDatabase<BionicIconsIconDef>.AllDefs)
            {
                if (def.bodyPart == null) continue;
                BodyPartDef bodyPart = DefDatabase<BodyPartDef>.GetNamedSilentFail(def.bodyPart);
                if (bodyPart == null) continue;


                List<BionicIconsIconDef> list;
                if (! replacementsIcon.TryGetValue(bodyPart, out list)) {
                    list = new List<BionicIconsIconDef>();
                    replacementsIcon.Add(bodyPart, list);
                }

                list.Add(def);
            }

            HashSet<ThingDef> processed = new HashSet<ThingDef>();
            foreach (RecipeDef recipe in DefDatabase<RecipeDef>.AllDefs)
            {
                if (recipe.appliedOnFixedBodyParts.NullOrEmpty()) continue;

                foreach (BodyPartDef bodyPart in recipe.appliedOnFixedBodyParts)
                {
                    foreach (IngredientCount ing in recipe.ingredients)
                    {
                        foreach (ThingDef def in ing.filter.AllowedThingDefs)
                        {
                            if (!def.isTechHediff) continue;
                            if (def.graphicData == null) continue;
                            if (processed.Contains(def)) continue;

                            if (processDef(recipe, bodyPart, def))
                            {
                                processed.Add(def);
                            }
                        }
                    }
                }
            }
        }

        private static bool processDef(RecipeDef recipe, BodyPartDef bodyPart, ThingDef def)
        {
            List<BionicIconsTextureDef> colors;
            if (!replacements.TryGetValue(def.graphicData.texPath, out colors)) return false;

            List<BionicIconsIconDef> icons;
            if (!replacementsIcon.TryGetValue(bodyPart, out icons)) return false;

            string tex = null;
            foreach (BionicIconsIconDef option in icons)
            {
                if (option.thingDef != null && def != option.thingDef) continue;
                if (option.thingDefNameContains != null && !def.defName.Contains(option.thingDefNameContains)) continue;
                if (recipe.addsHediff!=null) {
                    bool isSolid = recipe.addsHediff.addedPartProps != null && recipe.addsHediff.addedPartProps.solid;

                    if (option.SolidOnly && !isSolid) continue;
                    if (option.SoftOnly && isSolid) continue;
                }

                tex = option.texture;
                break;
            }
            if (tex == null) return false;

            Color color = Color.white;
            foreach (BionicIconsTextureDef option in colors)
            {
                if (option.thingDefNameContains != null && !def.defName.Contains(option.thingDefNameContains)) continue;

                color = option.color;
                break;
            }

            def.graphicData.graphicClass = typeof(Graphic_Single);
            def.graphicData.drawSize = new Vector2(1.0f, 1.0f);
            def.graphicData.color = color;
            def.graphicData.texPath = tex;
            
            graphicDataInit.Invoke(def.graphicData, new object[] { });
            def.uiIcon = def.graphicData.Graphic.MatSingle.mainTexture as Texture2D;
            def.uiIconColor = color;

            return true;
        }
    }
}
