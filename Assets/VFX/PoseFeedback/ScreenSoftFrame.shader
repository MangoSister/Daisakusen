Shader "Custom/ScreenSoftFrame" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MaskTex ("Mask (A)", 2D) = "transparent" {}
		_Color("Color Tint",Color) = (1.0, 1.0, 1.0, 1.0)
		_Opaqueness("Opaqueness", Range(0,1)) = 0
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			uniform sampler2D _MainTex;
			uniform sampler2D _MaskTex;
			uniform fixed _Opaqueness;
			uniform half4 _Color;

			fixed4 frag(v2f_img i) : COLOR
			{
				//_SplitOffset = clamp(
				//fixed4 src = tex2D(_MainTex, i.uv);
				fixed4 src =  tex2D(_MainTex, i.uv);
				fixed4 mask = tex2D(_MaskTex, i.uv);
				return lerp(src, lerp(src, _Color, 1- mask.a), saturate(_Opaqueness));
				//return tex2D(_MainTex, i.uv) * 
				//fixed4(_Color.r,_Color.g,_Color.b, _Opaqueness * tex2D(_MaskTex, i.uv).a);
				//return fixed4(1,1,1,0);
			}

			ENDCG
		}
	} 
	FallBack "Diffuse"
}
