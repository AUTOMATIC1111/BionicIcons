using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace BionicIcons
{
    class Graphic_SingleWithMask : Graphic_Single
    {
        public static string maskPath;

        public override void Init(GraphicRequest req)
        {
            this.data = req.graphicData;
            this.path = req.path;
            this.color = req.color;
            this.colorTwo = req.colorTwo;
            this.drawSize = req.drawSize;
            MaterialRequest req2 = default(MaterialRequest);
            req2.mainTex = ContentFinder<Texture2D>.Get(req.path, true);
            req2.shader = req.shader;
            req2.color = this.color;
            req2.colorTwo = this.colorTwo;
            req2.renderQueue = req.renderQueue;
            req2.shaderParameters = req.shaderParameters;
            if (req.shader.SupportsMaskTex())
            {
                req2.maskTex = ContentFinder<Texture2D>.Get(maskPath, false);
            }
            this.mat = MaterialPool.MatFrom(req2);
        }

    }
}
