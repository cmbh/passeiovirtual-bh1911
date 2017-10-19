Shader "Sokay/Simple Shader" {
Properties {
    _Color ("Tint (A = Opacity)", Color) = (1,1,1,1) 
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100
    
    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha 
     
    // BACKFACING PASS
    Pass {
        Lighting Off
        Cull Front
        ColorMaterial AmbientAndDiffuse
        SetTexture [_MainTex] {
            ConstantColor [_Color]
            combine texture * constant
        }
        
        SetTexture [_MainTex] {
            combine previous  * primary
        }
    }
    
    // FRONT PASS
    Pass {
        Lighting Off
        Cull Back
        ColorMaterial AmbientAndDiffuse
        SetTexture [_MainTex] {
            ConstantColor [_Color]
            combine texture * constant
        }
        
        SetTexture [_MainTex] {
            combine previous  * primary
        }
        
    }
}

}