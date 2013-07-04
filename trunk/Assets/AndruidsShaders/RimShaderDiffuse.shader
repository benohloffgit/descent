Shader "Andruids/Rim Shader Diffuse" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
      _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
      _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf MyLambert noforwardadd
	  #pragma target 2.0
	  #pragma exclude_renderers flash
	  
      struct Input {
          float2 uv_MainTex;
//          float2 uv_BumpMap;
          float3 viewDir;
      };
      
      sampler2D _MainTex;
      float4 _RimColor;
      float _RimPower;
      
      void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
//          o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
          half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
          o.Emission = _RimColor.rgb * pow (rim, _RimPower);
      }
      
      half4 LightingMyLambert (SurfaceOutput o, fixed3 lightDir, fixed atten) {
          fixed NdotL = dot(o.Normal, lightDir);
          fixed4 c;
          c.rgb = o.Albedo; //no light
          //c.rgb = o.Albedo * _LightColor0.rgb; no light direction
//          c.rgb = o.Albedo * _LightColor0.rgb * (NdotL * atten * 2);
          
          //c.rgb = o.Albedo;
          c.a = o.Alpha;
          return c;
      }
      	
      ENDCG
    } 
    Fallback "Diffuse"
}