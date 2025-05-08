Shader "Snowy/Particles/VertexLit Blended" {
Properties {
    _EmisColor ("Emissive Color", Color) = (.2,.2,.2,0)
    _MainTex ("Particle Texture", 2D) = "white" {}
}

SubShader {
    Tags { "Queue"="Transparent" "RenderType"="Transparent" "PreviewType"="Plane" "RenderPipeline" = "UniversalRenderPipeline" "IgnoreProjector" = "True"}
    Tags { "LightMode" = "UniversalForward" }
    Cull Off
    Lighting On
    Material { Emission [_EmisColor] }
    ColorMaterial AmbientAndDiffuse
    ZWrite Off
    ColorMask RGB
    Blend SrcAlpha OneMinusSrcAlpha
    Pass {
        SetTexture [_MainTex] { combine primary * texture }
    }
}
}