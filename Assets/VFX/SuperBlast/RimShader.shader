Shader "Custom/RimShader" 
{
	Properties 
	{
		_Color("Color", Color) = (0,0,0,1)
		_RimColor("Rim Color", Color) = (0,0,0,1)
		_RimPower("Rim Power", Range(0.1,8.0)) = 3.0
	}
	
	SubShader 
	{
		Pass
		{
			Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
			LOD 200
			ZWrite Off
			Lighting Off
			//Cull Front
			Blend One One
			//Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			uniform fixed4 _Color;
			uniform fixed4 _RimColor;
			uniform half _RimPower;

			struct v2f
			{
				float4 pos : POSITION;
				float3 normal : TEXCOORD;
				float3 viewDir :TEXCOORD1;
			};

			v2f vert(appdata_base i)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
				o.normal = mul(_Object2World, half4(i.normal, 0.0));
				o.viewDir = normalize(_WorldSpaceCameraPos - mul(_Object2World, i.vertex).xyz);
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				half rim = 1.0 - saturate(dot (normalize(i.viewDir), i.normal));
				fixed3 rimCol = (_RimColor * pow (rim, _RimPower)).rgb;
				return _Color * fixed4(rimCol,1) * _Color.a;
			}

			ENDCG
		}

	} 

	FallBack "Diffuse"
}
