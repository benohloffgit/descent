// == TRIPLANAR TEXTURE PROJECTION SHADERS ==
// Copyright(c) Broken Toy Games, 2011. Do not redistribute.
// http://www.brokentoygames.com

Shader "Andruids/TriplanarNewLit" {

	Properties {
		_TexBase ("Texture (Base)", 2D) = "white" {}
		_TexCeil ("Texture (Ceiling)", 2D) = "white" {}
		_TexWall ("Texture (Wall)", 2D) = "white" {}
	}
	
	Category {
		SubShader { 
			Tags { "Queue"="Geometry" "RenderType"="Opaque" }
			Cull Back
		CGPROGRAM
		#pragma surface surf Lambert noforwardadd vertex:vert
		#pragma target 2.0
		#pragma exclude_renderers flash
		sampler2D _TexBase;
		sampler2D _TexCeil;
		sampler2D _TexWall;
		
		struct Input {
//			half4 pos : SV_POSITION;
//			half2  uv : TEXCOORD0;
			fixed3 localPos;
			fixed3 localNormal;
//			fixed3 worldNormal;
		};
		
		void vert (inout appdata_full v, out Input o) {
//			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
//			o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
			o.localPos = v.vertex;
			o.localNormal = v.normal;
//			o.worldNormal = mul(_Object2World, fixed4(v.normal, 0.0f)).xyz;
		}
		
		void surf (Input IN, inout SurfaceOutput o) {			
//			fixed3 projnormal = saturate(pow(IN.localNormal*1.5, 2));
			fixed3 projnormal = saturate(pow(IN.localNormal, 4));
//			fixed3 projnormal = abs(IN.localNormal);
//			fixed3 projnormal = abs(IN.worldNormal);

/*						
			fixed3 color1_;
			fixed3 color2_;
			if (projnormal.y > projnormal.x) {
				if (projnormal.y > projnormal.z) {
					if (projnormal.z > projnormal.x) {
						//yzx
						color1_ = tex2D(_TexWall, IN.localPos.xz); // green grass
						color2_ = tex2D(_TexCeil, IN.localPos.xz); // red lava
					} else {
						//yxz
						color1_ = tex2D(_TexWall, IN.localPos.yz); // green grass
						color2_ = tex2D(_TexCeil, IN.localPos.yz); // red lava
					}
				} else {
					if (projnormal.y > projnormal.x) {
						// zyx
						color1_ = tex2D(_TexWall, IN.localPos.xz); // green grass
						color2_ = tex2D(_TexCeil, IN.localPos.xz); // red lava
					} else {
						// zxy
						color1_ = tex2D(_TexWall, IN.localPos.yz); // green grass
						color2_ = tex2D(_TexCeil, IN.localPos.yz); // red lava
					}
				}
			} else {
				if (projnormal.x > projnormal.z) {
					//x
					color1_ = tex2D(_TexWall, IN.localPos.yz); // green grass
					color2_ = tex2D(_TexCeil, IN.localPos.yz); // red lava
				} else {
					//z
					color1_ = tex2D(_TexWall, IN.localPos.xz); // green grass
					color2_ = tex2D(_TexCeil, IN.localPos.xz); // red lava
				}
			}*/
			
/*			if (projnormal.x >= projnormal.y) {
				if (projnormal.x >= projnormal.z) {
					if (projnormal.z >= projnormal.y) {
						projnormal.x = 1.0;
					}
				}
			}
			if (projnormal.z >= projnormal.x) {
				if (projnormal.z >= projnormal.y) {
					if (projnormal.y >= projnormal.x) {
						projnormal.z = 1.0;
					}
				}
			}*/
			/*if (projnormal.y == 0.0) {
				projnormal.x = 1.0;
			}
			if (projnormal.x == 0.0) {
				projnormal.z = 1.0;
			}*/

/*			fixed3 color0_ = tex2D(_TexBase, IN.localPos.yx); // grey stone
			fixed3 color1_ = tex2D(_TexWall, IN.localPos.zx); // green grass
			fixed3 color2_ = tex2D(_TexCeil, IN.localPos.yz); // red lava
				
			//o.Albedo = lerp(lerp(color1_, color0_, projnormal.z), color2_, projnormal.x);	
//			o.Albedo = color0_ * projnormal.z + color1_ * projnormal.y + color2_ * projnormal.x;	
//			o.Albedo = lerp(lerp(color1_, color0_, projnormal.z), lerp(color2_, color0_, projnormal.z), projnormal.x);	
			o.Albedo = lerp(lerp(color1_, color2_, projnormal.x), color0_, projnormal.z);
//			o.Albedo = tex2D(_TexBase, IN.localPos).rgb;
*/
						
			fixed total = projnormal.x + projnormal.y + projnormal.z;
			
			projnormal.x /= total;
			projnormal.y /= total;
			projnormal.z /= total;
			
			fixed3 color0_ = tex2D(_TexBase, IN.localPos.yx); 
			fixed3 color1_ = tex2D(_TexWall, IN.localPos.zx); 
			fixed3 color2_ = tex2D(_TexCeil, IN.localPos.yz);

			o.Albedo = color0_ * projnormal.z + color1_ * projnormal.y + color2_ * projnormal.x;

		}
		ENDCG
		}
	}
	//FallBack "Mobile/Diffuse"
}
