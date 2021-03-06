﻿Shader "Hidden/TerrainEngine/BillboardTree" {
    Properties {
            _MainTex ("Base (RGB)", 2D) = "black" {}
    }
   
    SubShader {
            Tags { "Queue" = "Transparent-100" "IgnoreProjector"="True" "RenderType"="TreeBillboard" }
           
            Pass {
                    ColorMask rgb
                    Blend SrcAlpha OneMinusSrcAlpha
                    AlphaTest Greater 0.9
                    ZWrite Off Cull Off
                   
                    CGPROGRAM
                    #pragma vertex vert
                    #include "UnityCG.cginc"
                    #include "TerrainEngine.cginc"
                    #pragma fragment frag

                    struct v2f {
                            float4 pos : POSITION;
                            fixed4 color : COLOR0;
                            float2 uv : TEXCOORD0; 
                    };

                    v2f vert (appdata_tree_billboard v) {
                            v2f o;
                            TerrainBillboardTree(v.vertex, v.texcoord1.xy, v.texcoord.y);  
                            o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
                            o.uv.x = v.texcoord.x;
                            o.uv.y = v.texcoord.y > 0;
                            o.color =  v.color;
                            return o;
                    }

                    sampler2D _MainTex;
                    float3 tree_color;
                    float4 unity_FogColor; 
                    
                    fixed4 frag(v2f input) : COLOR
                    {
                            fixed4 col = tex2D( _MainTex, input.uv);
                            col.rgb *= unity_FogColor.rgb;
                      		col.rgb *= tree_color.rgb;
                            clip(col.a);
                            return col;
                    }
                    ENDCG                  
            }
    }

    SubShader {
            Tags { "Queue" = "Transparent-100" "IgnoreProjector"="True" "RenderType"="TreeBillboard" }
           
            Pass {

                    CGPROGRAM
                    #pragma vertex vert
                    #pragma exclude_renderers shaderonly
                    #include "UnityCG.cginc"
                    #include "TerrainEngine.cginc"

                    struct v2f {
                            float4 pos : POSITION;
                            fixed4 color : COLOR0;
                            float2 uv : TEXCOORD0;
                    };

                    v2f vert (appdata_tree_billboard v) {
                            v2f o;
                            TerrainBillboardTree(v.vertex, v.texcoord1.xy, v.texcoord.y);  
                            o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
                            o.uv.x = v.texcoord.x;
                            o.uv.y = v.texcoord.y > 0;
                            o.color = v.color;
                            return o;
                    }
                    ENDCG                  

                    ColorMask rgb
                    Blend SrcAlpha OneMinusSrcAlpha
                   
                    ZWrite Off Cull Off
                   
                    AlphaTest Greater 0.9
                    SetTexture [_MainTex] { combine texture * primary, texture }
            }
    }
   
    Fallback Off
}

