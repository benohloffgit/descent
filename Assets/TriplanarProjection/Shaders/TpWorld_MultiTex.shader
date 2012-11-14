// == TRIPLANAR TEXTURE PROJECTION SHADERS ==
// Copyright(c) Broken Toy Games, 2011. Do not redistribute.
// http://www.brokentoygames.com

Shader "Triplanar Textures/Triplanar Terrain" {

	Properties {
	_Color ("Diffuse Color", Color) = (1.0, 1.0, 1.0, 1.0)
	_Tiling ("Tiling", Float) = 1
	_TexBase ("Top (RGB)", 2D) = "white" {}
	_TexSides ("Sides (RGB)", 2D) = "white" {}
	}
	
Category {
	SubShader { 
	//#Blend
	ZWrite On
	//#CatTags
	Tags { "RenderType"="Opaque" }
		//Tags { "RenderType"="Opaque" }
		//#LOD
		LOD 400
		//#GrabPass
		CGPROGRAM
		//#LightingModelTag
		#pragma surface surf BlinnPhong vertex:vert 
		//alphatest:_Cutoff
		 
		//#TargetSM
		#pragma target 2.0
		#pragma multi_compile_builtin
		//#UnlitCGDefs
		float _Tiling;
		sampler2D _TexBase, _TexSides, _TexBottom;
		float4 _Color;
		float _Power;
		
		struct Input {
			//float4 vertex;
			//#UVDefs
			//float3 normal;
			//float2 uvCoords;
			float3 worldPos;
			float3 myworldNormal;
			INTERNAL_DATA
			float4 blendColor;
		};
		
		void vert (inout appdata_full v, out Input o) {
			o.myworldNormal = mul(_Object2World, float4(v.normal, 0.0f)).xyz;
			o.worldPos = mul(_Object2World, v.vertex);
			o.blendColor = v.vertex;
		}
		
		void surf (Input IN, inout SurfaceOutput o) {
			float4 texBase = 1.0;
			
		//Assign UVs			
			half2 s = float2(_Tiling/10, _Tiling/10);
			half2 tex0 = s * IN.worldPos.zy;
			half2 tex1 = s * IN.worldPos.zx;
			half2 tex2 = s * IN.worldPos.xy;
			
		// Calculate top-only projection
			half3 blendWeights = IN.myworldNormal;
			blendWeights.y = max(IN.myworldNormal.y, 0);
			half blendBottom = min(IN.myworldNormal.y, 0);
			blendWeights = saturate(pow(blendWeights*0.25, 10));
			blendBottom = saturate(pow(blendBottom*0.25, 10));
			half3 blendMult = 1/(blendWeights.x + blendWeights.y + blendWeights.z + blendBottom);			
			blendWeights *= blendMult;
			blendBottom *= blendMult;

		// Calculate projections for Base channel.
			half4 color0_ = tex2D(_TexSides, tex0).xyzw; 
			half4 color1_ = tex2D(_TexBase, tex1).xyzw; 
			half4 color2_ = tex2D(_TexSides, tex2).xyzw;
			half4 color3_ = tex2D(_TexSides, tex1).xyzw;		
		
		// Combine all that stuff...
			texBase = 
			(color0_ * blendWeights.x) + 
			(color1_ * blendWeights.y) + (color3_ * blendBottom) + 
			(color2_ * blendWeights.z);
		
		// Assign results to shader.
			o.Albedo = texBase.rgba * _Color;
		}
		ENDCG
		}
	}
	FallBack "Triplanar Textures/Triplanar World"
}