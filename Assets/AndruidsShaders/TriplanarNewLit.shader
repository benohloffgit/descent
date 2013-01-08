// == TRIPLANAR TEXTURE PROJECTION SHADERS ==
// Copyright(c) Broken Toy Games, 2011. Do not redistribute.
// http://www.brokentoygames.com

Shader "Andruids/TriplanarNewLit" {

	Properties {
		_TexBase ("Texture (Base)", 2D) = "white" {}
		_TexCeil ("Texture (Ceiling)", 2D) = "white" {}
		_TexWall ("Texture (Wall)", 2D) = "white" {}
		_LightMap ("Lightmap (RGB)", 2D) = "grey" {}
	}
	
	Category {
		SubShader { 
			Tags { "Queue"="Geometry" "RenderType"="Opaque" }
			Cull Back
		CGPROGRAM
		#pragma surface surf MyLambert noforwardadd vertex:vert 
		
		// noambient novertexlights 
		#pragma target 2.0
		#pragma exclude_renderers flash
		sampler2D _TexBase;
		sampler2D _TexCeil;
		sampler2D _TexWall;
		sampler2D _LightMap;
		
		struct Input {
//			half4 pos : SV_POSITION;
//			half2  uv : TEXCOORD0;
//			fixed2  uv;
			fixed3 localPos;
			fixed3 localNormal;
//			fixed3 worldPos;
//			fixed3 worldNormal;
		};
		
		void vert (inout appdata_full v, out Input i) {
//			i.pos = mul (UNITY_MATRIX_MVP, v.vertex);
//			i.uv = TRANSFORM_TEX (v.texcoord, _LightMap);
			i.localPos = v.vertex;
			i.localNormal = v.normal;
//			i.uv = v.texcoord;
//			i.worldNormal = mul(_Object2World, fixed4(v.normal, 0.0f)).xyz;
		}
		
		void surf (Input i, inout SurfaceOutput o) {			
//			fixed3 projnormal = saturate(pow(IN.localNormal*1.5, 2));
			fixed3 projnormal = saturate(pow(i.localNormal, 4));
//			fixed3 projnormal = abs(IN.localNormal);
//			fixed3 projnormal = abs(IN.worldNormal);
						
			fixed total = projnormal.x + projnormal.y + projnormal.z;
			
			projnormal.x /= total;
			projnormal.y /= total;
			projnormal.z /= total;
			
			fixed3 color0_ = tex2D(_TexBase, i.localPos.yx); 
			fixed3 color1_ = tex2D(_TexWall, i.localPos.zx); 
			fixed3 color2_ = tex2D(_TexCeil, i.localPos.yz);

			o.Albedo = (color0_ * projnormal.z + color1_ * projnormal.y + color2_ * projnormal.x);// * (i.localNormal);
			//o.Albedo *= tex2D(_LightMap, i.localPos.xy ).rgb * 2;
			o.Albedo *= tex2D(_LightMap, (i.localPos.xy/10.0)*_SinTime ).rgb;

		}
		
		half4 LightingMyLambert (SurfaceOutput o, fixed3 lightDir, fixed atten) {
          fixed NdotL = dot (o.Normal, lightDir);
          fixed4 c;
          //c.rgb = o.Albedo;
          c.rgb = o.Albedo * _LightColor0.rgb * (NdotL * atten * 2);
          //c.rgb = o.Albedo;
          c.a = o.Alpha;
          return c;
      	}
		ENDCG
		}
	}
	//FallBack "Mobile/Diffuse"
}
