// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Transparent"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	_Color("Color (RGBA)", Color) = (1, 1, 1, 1) // add _Color property
	}

		SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Cull front
		LOD 100

		Pass
	{
		CGPROGRAM

#pragma vertex vert alpha
#pragma fragment frag alpha

#include "UnityCG.cginc"

		struct appdata_t
	{
		float4 vertex   : POSITION;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex  : SV_POSITION;
		half2 texcoord : TEXCOORD0;
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;
	float4 _Color;

	v2f vert(appdata_t v)
	{
		v2f o;

		o.vertex = UnityObjectToClipPos(v.vertex);
		v.texcoord.x = 1 - v.texcoord.x;
		o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 col = tex2D(_MainTex, i.texcoord) * _Color; // multiply by _Color
	return col;
	}

		ENDCG
	}
	}
}