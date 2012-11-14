// colored vertex lighting
Shader "Andruids/TriplanarNew" {
    // a single color property
    Properties {
//        _Color ("Main Color", Color) = (1,.5,.5,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    // define one subshader
    SubShader {
    	Tags { "Queue" = "Geometry" "RenderType" = "Opaque" }
 //       Pass {
 //       	Tags { "Name" = "MyPass" "LightMode" = "Vertex"}
 //         Material {
 //               Diffuse [_Color]
 //           }
 //           Lighting On
 //       }
//        Pass {
			CGPROGRAM
			#pragma surface surf BlinnPhong
			#include "UnityCG.cginc"
			
			
/*	half4 LightingSimpleLambert (SurfaceOutput s, half3 lightDir, half atten) {
          half NdotL = dot (s.Normal, lightDir);
          half4 c;
          c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten * 2);
          c.a = s.Alpha;
          return c;
      }*/
      
			struct Input {
			  float2 uv_MainTex;
			};
			
			sampler2D _MainTex;
			
			void surf (Input IN, inout SurfaceOutput o) {
				o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
			}
			ENDCG
//		}
    }
    Fallback "Diffuse"
} 
