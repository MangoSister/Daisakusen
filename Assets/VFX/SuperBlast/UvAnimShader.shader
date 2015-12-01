Shader "Custom/UvAnimShader" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_AnimSpeedX("Animation Speed X", Range(0,10)) = 0
		_AnimSpeedY("Animation Speed Y", Range(0,10)) = 0
	}
	SubShader 
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 200
		ZWrite Off
		Lighting Off
		Blend One One
		//Blend SrcAlpha OneMinusSrcAlpha
		//Blend DstColor SrcColor // 2x Multiplicative
		CGPROGRAM
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alpha



		uniform sampler2D _MainTex;
		uniform half _AnimSpeedX;
		uniform half _AnimSpeedY;

		struct Input 
		{
			float2 uv_MainTex;
		};

		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex + half2(_AnimSpeedX, _AnimSpeedY) * _Time) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Emission = c.rgb * c.a;
		}



		ENDCG
	} 

	FallBack Off
}
