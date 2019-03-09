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
            HashSet<ThingDef> processed = new HashSet<ThingDef>();
            List<BionicIconsIconDef> directReplacements = new List<BionicIconsIconDef>();

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
                if (def.bodyPart != null)
                {
                    BodyPartDef bodyPart = DefDatabase<BodyPartDef>.GetNamedSilentFail(def.bodyPart);
                    if (bodyPart == null) continue;

                    List<BionicIconsIconDef> list;
                    if (!replacementsIcon.TryGetValue(bodyPart, out list))
                    {
                        list = new List<BionicIconsIconDef>();
                        replacementsIcon.Add(bodyPart, list);
                    }

                    list.Add(def);
                }
                else if (def.thingDef != null)
                {
                    ThingDef thingDef = DefDatabase<ThingDef>.GetNamedSilentFail(def.thingDef);
                    if (thingDef == null) continue;

                    if (processed.Contains(thingDef)) continue;
                    replace(thingDef, def.texture, Color.white);
                    processed.Add(thingDef);
                }
            }

            foreach (RecipeDef recipe in DefDatabase<RecipeDef>.AllDefs)
            {
                if (recipe.appliedOnFixedBodyParts.NullOrEmpty()) continue;

                foreach (BodyPartDef bodyPart in recipe.appliedOnFixedBodyParts)
                {
                    foreach (IngredientCount ing in recipe.ingredients)
                    {
                        foreach (ThingDef def in ing.filter.AllowedThingDefs)
                        {
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
            if (!def.isTechHediff) return false;
            if (def.graphicData == null) return false;

            List<BionicIconsTextureDef> colors;
            if (!replacements.TryGetValue(def.graphicData.texPath, out colors)) return false;

            List<BionicIconsIconDef> icons;
            if (!replacementsIcon.TryGetValue(bodyPart, out icons)) return false;

            string tex = null;
            foreach (BionicIconsIconDef option in icons)
            {
                if (option.nameContains != null && !def.defName.Contains(option.nameContains)) continue;
                if (recipe.addsHediff != null)
                {
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
                if (option.nameContains != null && !def.defName.Contains(option.nameContains)) continue;

                color = option.color;
                break;
            }

            replace(def, tex, color);
            return true;
        }

        private static void replace(ThingDef def, string tex, Color color)
        {
            def.graphicData.graphicClass = typeof(Graphic_Single);
            def.graphicData.drawSize = new Vector2(1.0f, 1.0f);
            def.graphicData.color = color;
            def.graphicData.texPath = tex;

            graphicDataInit.Invoke(def.graphicData, new object[] { });
            def.uiIcon = def.graphicData.Graphic.MatSingle.mainTexture as Texture2D;
            def.uiIconColor = color;
        }
    }
}
