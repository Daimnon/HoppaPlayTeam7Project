using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessOutlineRenderer : PostProcessEffectRenderer<PostProcessOutline>
{
    public override void Render(PostProcessRenderContext context)
    {
        PropertySheet sheet = context.propertySheets.Get(Shader.Find("Hidden/Outline"));
        sheet.properties.SetFloat("_Thickness", settings.Thickness);
        sheet.properties.SetFloat("_MinDepth", settings.DepthMin);
        sheet.properties.SetFloat("_MaxDepth", settings.DepthMax);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
