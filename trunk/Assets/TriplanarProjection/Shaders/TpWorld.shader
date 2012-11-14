// == TRIPLANAR TEXTURE PROJECTION SHADERS ==
// Copyright(c) Broken Toy Games, 2011. Do not redistribute.
// http://www.brokentoygames.com

Shader "Triplanar Textures/Triplanar World" {

	Properties {
	_Color ("Diffuse Color", Color) = (1.0, 1.0, 1.0, 1.0)
	_Tiling ("Tiling", Float) = 1
	_TexBase ("Top (RGB)", 2D) = "white" {}
	}
	
	Category {
		SubShader { 
		//#Blend
			ZWrite On
		//#CatTags
			Tags { "RenderType"="Opaque" }
		//#LOD
			LOD 400
		//#GrabPass
		CGPROGRAM
		//#LightingModelTag
		#pragma surface surf Lambert vertex:vert 
		//alphatest:_Cutoff
 
		//#TargetSM
		#pragma target 2.0
		#pragma multi_compile_builtin
		//#UnlitCGDefs
		float _Tiling;
		sampler2D _TexBase;
		sampler2D _BumpBase;
		float4 _Color;
		
		struct Input {
			float4 vertex;
			//#UVDefs
			float3 normal;
			float3 localPos;
			float2 uvCoords;
			float3 localNormal;
			INTERNAL_DATA
		};
		
		void vert (inout appdata_full v, out Input o) {
			o.localNormal = mul(_Object2World, float4(v.normal, 0.0f));
			o.localPos = mul(_Object2World, v.vertex);
		}
		
		void surf (Input IN, inout SurfaceOutput o) {
			float4 diffuse = 1.0;
			
			//#PreFragBody			
			float2 s = float2(_Tiling/10, _Tiling/10);
			float2 tex0 = s * IN.localPos.xy;
			float2 tex1 = s * IN.localPos.zx;
			float2 tex2 = s * IN.localPos.zy;

			float4 color0_ = tex2D(_TexBase, tex0); 
			float4 color1_ = tex2D(_TexBase, tex1); 
			float4 color2_ = tex2D(_TexBase, tex2);			
			
			float3 projnormal = saturate(pow(normalize(IN.localNormal)*1.5, 10)); 
			diffuse = lerp(lerp(color1_, color0_, projnormal.z), color2_, projnormal.x);

			o.Albedo = diffuse.rgb * _Color;
		}
		ENDCG
		}
	}
	FallBack "Diffuse"
}