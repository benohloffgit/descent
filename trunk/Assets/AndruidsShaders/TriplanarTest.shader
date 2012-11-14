Shader "Andruids/TriplanarBase"
{
	Properties 
	{
_Color("_Color", Color) = (1,1,1,1)
_TriplanarTex("_TriplanarTex", 2D) = "black" {}

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Geometry"
"IgnoreProjector"="False"
"RenderType"="Opaque"

		}

		
Cull Back
ZWrite On
ZTest LEqual
ColorMask RGBA
Fog{
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  vertex:vert
#pragma target 2.0


float4 _Color;
sampler2D _TriplanarTex;

			struct EditorSurfaceOutput {
				half3 Albedo;
				half3 Normal;
				half3 Emission;
				half3 Gloss;
				half Specular;
				half Alpha;
				half4 Custom;
			};
			
			inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
			{
half3 spec = light.a * s.Gloss;
half4 c;
c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
c.a = s.Alpha;
return c;

			}

			inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
			{
				half3 h = normalize (lightDir + viewDir);
				
				half diff = max (0, dot ( lightDir, s.Normal ));
				
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular*128.0);
				
				half4 res;
				res.rgb = _LightColor0.rgb * diff;
				res.w = spec * Luminance (_LightColor0.rgb);
				res *= atten * 2.0;

				return LightingBlinnPhongEditor_PrePass( s, res );
			}
			
			struct Input {
				float3 worldPos;
float3 sWorldNormal;

			};

			void vert (inout appdata_full v, out Input o) {
float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);

o.sWorldNormal = mul((float3x3)_Object2World, SCALED_NORMAL);

			}
			

			void surf (Input IN, inout EditorSurfaceOutput o) {
				o.Normal = float3(0.0,0.0,1.0);
				o.Alpha = 1.0;
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Gloss = 0.0;
				o.Specular = 0.0;
				o.Custom = 0.0;
				
float4 Swizzle0=float4(float4( IN.worldPos.x, IN.worldPos.y,IN.worldPos.z,1.0 ).x, float4( IN.worldPos.x, IN.worldPos.y,IN.worldPos.z,1.0 ).y, float4( IN.worldPos.x, IN.worldPos.y,IN.worldPos.z,1.0 ).x, float4( IN.worldPos.x, IN.worldPos.y,IN.worldPos.z,1.0 ).y);
float4 Abs0=abs(float4( IN.sWorldNormal.x, IN.sWorldNormal.y,IN.sWorldNormal.z,1.0 ));
float4 Mask2=float4(0.0,0.0,Abs0.z,0.0);
float4 Mask0=float4(Abs0.x,0.0,0.0,0.0);
float4 Mask1=float4(0.0,Abs0.y,0.0,0.0);
float4 Add0=Mask0 + Mask1;
float4 Add1=Add0 + Mask2;
float4 Divide2=Mask2 / Add1;
float4 Multiply0=Swizzle0 * Divide2;
float4 Swizzle1=float4(float4( IN.worldPos.x, IN.worldPos.y,IN.worldPos.z,1.0 ).x, float4( IN.worldPos.x, IN.worldPos.y,IN.worldPos.z,1.0 ).z, float4( IN.worldPos.x, IN.worldPos.y,IN.worldPos.z,1.0 ).x, float4( IN.worldPos.x, IN.worldPos.y,IN.worldPos.z,1.0 ).z);
float4 Divide1=Mask1 / Add1;
float4 Multiply1=Swizzle1 * Divide1;
float4 Add2=Multiply0 + Multiply1;
float4 Swizzle2=float4(float4( IN.worldPos.x, IN.worldPos.y,IN.worldPos.z,1.0 ).y, float4( IN.worldPos.x, IN.worldPos.y,IN.worldPos.z,1.0 ).z, float4( IN.worldPos.x, IN.worldPos.y,IN.worldPos.z,1.0 ).y, float4( IN.worldPos.x, IN.worldPos.y,IN.worldPos.z,1.0 ).z);
float4 Divide0=Mask0 / Add1;
float4 Multiply2=Swizzle2 * Divide0;
float4 Add3=Add2 + Multiply2;
float4 Tex2D0=tex2D(_TriplanarTex,Add3.xy);
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_2_NoInput = float4(0,0,0,0);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Tex2D0;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}