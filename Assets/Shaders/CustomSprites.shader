Shader "Custom/Sprites"
{
	Properties
	{
		_MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[Enum(Off,0,Front,1,Back,2)] _Cull("Cull", Int) = 2
		_CutOff("CutOff", Range(0, 1)) = 0
		[MaterialToggle] _zWrite("Z Write", Float) = 0
		[Enum(Less,0,Greater,1,LEqual,2,GEqual,3,Equal,4,NotEqual,5,Always,6)]
		_zTest("Z Test", Int) = 2
		[MaterialToggle] _ColorAlphaMultiply("ColorAlphaMultiply", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull [_Cull]
		ZWrite [_zWrite]
		ZTest [_zTest]
		Lighting Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			Name "CustomSprites"
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord  : TEXCOORD0;
			};

			UNITY_INSTANCING_BUFFER_START(CustomSprites)
				UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
				#define _Color_arr CustomSprites
			UNITY_INSTANCING_BUFFER_END(CustomSprites)

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color);
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			half _AlphaSplitEnabled;
			float _CutOff;
			bool _ColorAlphaMultiply;

			fixed4 SampleSpriteTexture(float2 uv)
			{
				fixed4 color = tex2D(_MainTex, uv);

				#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D(_AlphaTex, uv).r;
				#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
				if (c.a < _CutOff) discard;
				if (_ColorAlphaMultiply) c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}
