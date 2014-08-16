Shader "Alpha/VertexLit Colored Tex" {
        Properties {
                _Color ("Main Color", Color) = (1,1,1,1)
//                _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
                _BumpMap ("Bumpmap", 2D) = "bump" {}
        }
 
        SubShader {
                Tags {
//                        "Queue" = "Transparent"
//                        "RenderType" = "Transparent"
					"RenderType" = "Opaque"
                }
                
		        Pass {
					Blend SrcAlpha OneMinusSrcAlpha
					
//		            SetTexture [_MainTex] {
//		                Combine texture * primary
//		            }
		        }

                CGPROGRAM
                #pragma surface surf Lambert alpha
                struct Input {
                        float4 color : color;
//                        float2 uv_mainTex;
                        float2 uv_BumpMap;
                };
//                sampler2D _MainTex;
                sampler2D _BumpMap;
                fixed4 _Color;
               
                void surf(Input IN, inout SurfaceOutput o) {
//                        o.Albedo = tex2D(_MainTex, IN.uv_mainTex).rgb * IN.color.rgb * _Color.rgb;
//                        o.Alpha = tex2D(_MainTex, IN.uv_mainTex).a * IN.color.a * _Color.a;
//						o.Albedo = tex2D(_MainTex, IN.uv_mainTex).rgb - IN.color.rgb;
//						o.Albedo = tex2D(_MainTex, IN.uv_mainTex).rgb * IN.color.rgb;
						o.Albedo = IN.color.rgb;
//						o.Alpha = IN.color.a * _Color.a;
						o.Alpha = 1.0;
						o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
                        o.Specular = 0.2;
                        o.Gloss = 1.0;
                }
                ENDCG
        }
        Fallback "Alpha/VertexLit", 1
}