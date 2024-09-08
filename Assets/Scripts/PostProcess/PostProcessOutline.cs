using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[System.Serializable]
[PostProcess(typeof(PostProcessOutlineRenderer), PostProcessEvent.AfterStack, "Outline")]
public sealed class PostProcessOutline : PostProcessEffectSettings
{
    public FloatParameter Thickness = new() { value = 1.0f };
    public FloatParameter DepthMin = new() { value = 0.0f };
    public FloatParameter DepthMax = new() { value = 1.0f };
}
