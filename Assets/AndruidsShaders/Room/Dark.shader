
//    // after CGPROGRAM;
//    #include "AutoLight.cginc"
     
    // in v2f struct;
//    LIGHTING_COORDS(0,1) // replace 0 and 1 with the next available TEXCOORDs in your shader, don't put a semicolon at the end of this line.
     
    // in vert shader;
//    TRANSFER_VERTEX_TO_FRAGMENT(o); // Calculates shadow and light attenuation and passes it to the frag shader.
     
    //in frag shader;
//    float atten = LIGHT_ATTENUATION(i); // This is a float for your shadow/attenuation value, multiply your lighting value by this to get shadows. Replace i with whatever you've defined your input struct to be called (e.g. frag(v2f [b]i[/b]) : COLOR { ... ).
    
Shader "Andruids/Room/Dark" {

	Properties {
		_TexBase ("Texture (Base)", 2D) = "white" {}
		_TexCeil ("Texture (Ceiling)", 2D) = "white" {}
		_TexWall ("Texture (Wall)", 2D) = "white" {}
		_LightMap ("Lightmap (RGB)", 2D) = "grey" {}
		_Darkness ("_Darkness", Range(0,1) ) = 0.5
		_LightMapScale ("_LightMapScale", Range(0,50) ) = 20.0
	}
	
	Category {
		SubShader { 
			Tags { "Queue"="Geometry" "RenderType"="Opaque" }
			Cull Back
		CGPROGRAM
		#pragma surface surf MyLambert noforwardadd vertex:vert 
		// noambient novertexlights 
		#pragma target 3.0
		#pragma exclude_renderers flash
		sampler2D _TexBase;
		sampler2D _TexCeil;
		sampler2D _TexWall;
		sampler2D _LightMap;
		fixed _Darkness;
		fixed _LightMapScale;
		
		struct Input {
//			half4 pos : SV_POSITION;
//			half2  uv : TEXCOORD0;
//			fixed2  uv;
			fixed3 localPos;
			fixed3 localNormal;
//			fixed3 worldPos;
//			fixed3 worldNormal;
//			fixed4 screenPos;
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
			
			fixed3 color0_ = tex2D(_TexBase, i.localPos.xy); 
			fixed3 color1_ = tex2D(_TexWall, i.localPos.xz); 
			fixed3 color2_ = tex2D(_TexCeil, i.localPos.yz);

			o.Albedo = (color0_ * projnormal.z + color1_ * projnormal.y + color2_ * projnormal.x) * _Darkness;// * (i.localNormal);
//			o.Normal = projnormal; // leads to "Shader wants tangents" complaint

			color0_ = tex2D(_LightMap, i.localPos.xy/_LightMapScale); 
			color1_ = tex2D(_LightMap, i.localPos.xz/_LightMapScale);
			color2_ = tex2D(_LightMap, i.localPos.yz/_LightMapScale);

			o.Albedo *= (color0_ * projnormal.z + color1_ * projnormal.y + color2_ * projnormal.x).rgb * 8;
			//o.Albedo *= tex2D(_LightMap, i.screenPos.xy/5.0).rgb * 8;
		}
		
		half4 LightingMyLambert (SurfaceOutput o, fixed3 lightDir, fixed atten) {
          //fixed NdotL = dot(o.Normal, lightDir);
          fixed4 c;
          c.rgb = o.Albedo; //no light
          //c.rgb = o.Albedo * _LightColor0.rgb; no light direction
          
          //c.rgb = o.Albedo * _LightColor0.rgb * (NdotL * atten * 2);
          
          //c.rgb = o.Albedo;
          c.a = o.Alpha;
          return c;
      	}
		ENDCG
		}
	}
	//FallBack "Mobile/Diffuse"
	Fallback "VertexLit"
}
