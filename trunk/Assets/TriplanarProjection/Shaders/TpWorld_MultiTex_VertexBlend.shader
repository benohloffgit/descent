// == TRIPLANAR TEXTURE PROJECTION SHADERS ==
// Copyright(c) Broken Toy Games, 2011. Do not redistribute.
// http://www.brokentoygames.com

Shader "Triplanar Textures/Triplanar Terrain Vertex Blend" {
	Properties {
		_Color ("Diffuse Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Tiling ("Tiling", Float) = 1
		_Power ("Projection Power", Float) = 5
		_MainTex ("Top (RGB)", 2D) = "white" {}
		_MainTex2 ("Sides (RGB)", 2D) = "white" {}
		_BlendTex1 ("Top Blend Red (RGBA)", 2D) = "white" {}
		_BlendTex2 ("Top Blend Green (RGBA)", 2D) = "white" {}
		_BlendTex3 ("Top Blend Blue (RGBA)", 2D) = "white" {}
		_BlendTex4 ("Top Blend Alpha (RGBA)", 2D) = "white" {}		
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		LOD 400
		CGPROGRAM
		#pragma surface surf BlinnPhong vertex:vert
		#pragma target 3.0

		sampler2D _MainTex, _MainTex2, _BlendTex1, _BlendTex2, _BlendTex3, _BlendTex4;
		uniform float _Tiling, _Power;
		float4 _Color;
		
		struct Input {
			float3 worldPos; // Handled automatically
			float3 myWorldNormal; // Handled in Vert;
			float4 color;
		};
		
		void vert (inout appdata_full v, out Input o) {
			o.myWorldNormal = mul((float3x3)_Object2World, SCALED_NORMAL);
			o.color = v.color;
		}
		
		void surf (Input IN, inout SurfaceOutput o) {
			
			half2 uv1,uv2,uv3;
			
			float2 s = float2(_Tiling/10, _Tiling/10);
			uv1 = s * IN.worldPos.zy; // X-projection
			uv2 = s * IN.worldPos.xz; // Y-projeciton
			uv3 = s * IN.worldPos.xy; // Z-projection
			
			half3 blendWeights = IN.myWorldNormal;
			blendWeights.y = max(IN.myWorldNormal.y, 0);
			half blendBottom = min(IN.myWorldNormal.y, 0);
			blendWeights = saturate(pow(blendWeights*0.25, _Power));
			blendBottom = saturate(pow(blendBottom*0.25, _Power));
			half3 blendMult = 1/(blendWeights.x + blendWeights.y + blendWeights.z + blendBottom);			
			blendWeights *= blendMult;
			blendBottom *= blendMult;
			
			
			// Perform the color lookups
			half4 c1,c2,c3,c4,cb;
			c1 = tex2D(_MainTex2,uv1).xyzw;
			c2 = tex2D(_MainTex,uv2).xyzw;
			c3 = tex2D(_MainTex2,uv3).xyzw;
			c4 = tex2D(_MainTex2,uv2).xyzw;
			
			// Blend parameters
			half vertBlend, mapBlend, blendVal;
			
			// Calculate the first texture blend using Red channel.
			cb = tex2D(_BlendTex1, uv2).xyzw;
			vertBlend = IN.color.r;
			mapBlend = cb.a;
			blendVal = pow((1.5*vertBlend),1.5);
			blendVal = clamp(blendVal, 0.0, 1.0);
			
			c2 = lerp(c2, cb, blendVal);
			
			// Calculate the second texture blend using Green channel.
			cb = tex2D(_BlendTex2, uv2).xyzw;
			vertBlend = IN.color.g;
			mapBlend = cb.a;
			blendVal = pow((1.5*vertBlend),1.5);
			blendVal = clamp(blendVal, 0.0, 1.0);
			
			c2 = lerp(c2, cb, blendVal);			
			
			// Calculate the third texture blend using Blue channel.
			cb = tex2D(_BlendTex3, uv2).xyzw;
			vertBlend = IN.color.b;
			mapBlend = cb.a;
			blendVal = pow((1.5*vertBlend),1.5);
			blendVal = clamp(blendVal, 0.0, 1.0);
			
			c2 = lerp(c2, cb, blendVal);
			
			// Calculate the fourth texture blend using Alpha channel.
			cb = tex2D(_BlendTex4, uv2).xyzw;
			vertBlend = IN.color.a;
			mapBlend = cb.a;
			blendVal = pow((1.5*vertBlend),1.5);
			blendVal = clamp(blendVal, 0.0, 1.0);
			
			c2 = lerp(c2, cb, blendVal);
			
			// Combine projections
			half3 bc =
				(c1 * blendWeights.x) +
				(c2 * blendWeights.y) + (c4 * blendBottom) +
				(c3 * blendWeights.z);
			
			o.Albedo = bc * _Color;
		}
		ENDCG
	} 
	FallBack "Triplanar Textures/Triplanar Terrain"
}