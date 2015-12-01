Shader "Custom/RGBSplit" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SplitOffset("RGB Split Offset", Vector) = (0,0,0,0)
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
			uniform half2 _SplitOffset;

			fixed4 frag(v2f_img i) : COLOR
			{
				//_SplitOffset = clamp(
				//fixed4 src = tex2D(_MainTex, i.uv);
				return fixed4
				(
					tex2D(_MainTex, i.uv - _SplitOffset).r,
					tex2D(_MainTex, i.uv).g,
					tex2D(_MainTex, i.uv + _SplitOffset).b,
					0
				);
			}

			ENDCG
		}
	} 
	FallBack "Diffuse"
}
