// == TRIPLANAR TEXTURE PROJECTION SHADERS ==
// Copyright(c) Broken Toy Games, 2011. Do not redistribute.
// http://www.brokentoygames.com

Shader "Triplanar Textures/(Mobile) Single Axis Unlit" {
	
	Properties {
    _Tiling ("Tiling", Float) = 1
    _MainTex ("Texture", 2D) = "white" { }
	}
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			half _Tiling;
			sampler2D _MainTex;

			struct v2f {
				half4  pos : SV_POSITION;
				half2  uv : TEXCOORD0;
				half3 localPos;
				half3 localNormal;
			};

			half4 _MainTex_ST;

			v2f vert (appdata_base v) {
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				o.localNormal = mul(_Object2World, float4(v.normal, 0.0f));
				o.localPos = mul(_Object2World, v.vertex);
				return o;
			}

			half4 frag (v2f i) : COLOR {
				half2 s = half2(_Tiling/10, _Tiling/10);
				
				// =========================================================
				// === Setup the projection axis and direction. Change to suit your needs. ===
				// x+: .yz
				// x-: .zy
				// y+: .xz
				// y-: .zx
				// z+: .xy (default)
				// z-: .yx
				// =========================================================
				half2 tex0 = s * i.localPos.xy;
				
				half4 color0_ = tex2D(_MainTex, tex0);		
				half4 texcol = tex2D (_MainTex, i.uv);
				
				return color0_;
			}
			ENDCG
		}
	}
	Fallback "VertexLit"
}