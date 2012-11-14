Shader "Andruids/TriplanarBen2"
{
	Properties 
	{
		_Scale("_Scale", Float) = 0
		_TopTex("_TopTex", 2D) = "black" {}
		_WallTex("_WallTex", 2D) = "black" {}
		_BottomTex("_BottomTex", 2D) = "black" {}

	}
	
	SubShader 
	{
		Tags
		{
			"Queue"="Geometry"
			//"IgnoreProjector"="False"
			"RenderType"="Opaque"
		}
	
		Cull Back
//		ZWrite On
//		ZTest LEqual
//		ColorMask RGBA

		CGPROGRAM
		#pragma surface surf Lambert noforwardadd
//		#pragma target 2.0
		float _Scale;
		
		sampler2D _TopTex;
//		sampler2D _WallTex;
//		sampler2D _BottomTex;
		
		struct Input {
			fixed2 worldPos;
//			float3 sWorldNormal;
		};

		void surf (Input IN, inout SurfaceOutput o) {
//			o.Normal = float3(0.0,0.0,1.0);
			o.Alpha = 1.0;
			o.Albedo = 0.0;
			
/*			float4 Multiply3=float4( IN.worldPos.x, IN.worldPos.y,IN.worldPos.z,1.0 ) * _Scale.xxxx;
			float4 Swizzle0=float4(Multiply3.x, Multiply3.y, Multiply3.x, Multiply3.y);
			float4 Tex2D0=tex2D(_WallTex,Swizzle0.xy);
			float4 Abs0=abs(float4( IN.sWorldNormal.x, IN.sWorldNormal.y,IN.sWorldNormal.z,1.0 ));
			float4 Pow0=pow(Abs0,float4( 3,3,3,3 ));
			float4 Split2=Pow0;
			float4 Add0=float4( Split2.x, Split2.x, Split2.x, Split2.x) + float4( Split2.y, Split2.y, Split2.y, Split2.y);
			float4 Add1=Add0 + float4( Split2.z, Split2.z, Split2.z, Split2.z);
			float4 Divide2=float4( Split2.z, Split2.z, Split2.z, Split2.z) / Add1;
			float4 Multiply0=Tex2D0 * Divide2;
			float4 Swizzle1=float4(Multiply3.x, Multiply3.z, Multiply3.x, Multiply3.z);
			float4 Tex2D1=tex2D(_TopTex,Swizzle1.xy);
			float4 Divide1=float4( Split2.y, Split2.y, Split2.y, Split2.y) / Add1;
			float4 Multiply1=Tex2D1 * Divide1;
			float4 Add2=Multiply0 + Multiply1;
			float4 Swizzle2=float4(Multiply3.y, Multiply3.z, Multiply3.y, Multiply3.z);
			float4 Tex2D2=tex2D(_BottomTex,Swizzle2.xy);
			float4 Divide0=float4( Split2.x, Split2.x, Split2.x, Split2.x) / Add1;
			float4 Multiply2=Tex2D2 * Divide0;
			float4 Add3=Add2 + Multiply2;
			float4 Master0_1_NoInput = float4(0,0,1,1);
			float4 Master0_2_NoInput = float4(0,0,0,0);
			float4 Master0_3_NoInput = float4(0,0,0,0);
			float4 Master0_4_NoInput = float4(0,0,0,0);
			float4 Master0_5_NoInput = float4(1,1,1,1);
			float4 Master0_7_NoInput = float4(0,0,0,0);
			float4 Master0_6_NoInput = float4(1,1,1,1);
			o.Albedo = Add3;
			o.Normal = normalize(o.Normal);*/

			o.Albedo = tex2D(_TopTex, IN.worldPos).rgb;
			}
		ENDCG
	}
	Fallback "Diffuse"
}