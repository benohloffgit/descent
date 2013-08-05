
//    // after CGPROGRAM;
//    #include "AutoLight.cginc"
     
    // in v2f struct;
//    LIGHTING_COORDS(0,1) // replace 0 and 1 with the next available TEXCOORDs in your shader, don't put a semicolon at the end of this line.
     
    // in vert shader;
//    TRANSFER_VERTEX_TO_FRAGMENT(o); // Calculates shadow and light attenuation and passes it to the frag shader.
     
    //in frag shader;
//    float atten = LIGHT_ATTENUATION(i); // This is a float for your shadow/attenuation value, multiply your lighting value by this to get shadows. Replace i with whatever you've defined your input struct to be called (e.g. frag(v2f [b]i[/b]) : COLOR { ... ).
    
Shader "Andruids/Room/Unlit Vertex Movement" {

	Properties {
		_MainTex ("Texture (Base)", 2D) = "white" {}
		_Amount ("Amount", Range(0,1) ) = 0.5
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
		sampler2D _MainTex;
		fixed _Amount;
		
		struct Input {
//			fixed3 pos : SV_POSITION;
			float2 uv_MainTex;
			float3 localPos;
//			fixed3 localNormal;
//			fixed3 worldNormal;
//			fixed4 screenPos;
		};
		
		void vert (inout appdata_full v, out Input i) {
			float3 worldPos = mul(_Object2World, v.vertex).xyz;
//			v.vertex.xz += _SinTime.w * v.vertex.y;
//			v.vertex.yz += _SinTime.w * v.vertex.x;
			v.vertex.xy += sin(_Time.w + worldPos.x/12.0) * v.vertex.z * _Amount;
			//v.vertex.z += _SinTime.w * v.vertex.y;
			//i.localPos = v.vertex;
		}
		
		void surf (Input i, inout SurfaceOutput o) {			
			o.Albedo = tex2D(_MainTex, i.uv_MainTex).rgb;
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
	Fallback "VertexLit"
}
