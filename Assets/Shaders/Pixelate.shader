Shader "Custom/Pixelate"
{
	Properties
	{
		[HideInInspector] _MainTex("Texture", 2D) = "white" {}
		_Amount("Amount", Range(0, 1)) = 0.5 //TO DO: Support for separate axes?
		_Precission("Precission", Int) = 3 //TO DO: Support for separate axes?
		[MaterialToggle] _InterpolateColors("Interpolate Colors", Float) = 1
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

		Cull Off
		Lighting Off
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			Name "Pixelate"
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing

			#include "UnityCG.cginc"
			
			struct v2f
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			v2f vert(v2f i)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_TRANSFER_INSTANCE_ID(i, o);
				o.pos = UnityObjectToClipPos(i.pos);
				o.uv = i.uv;
				return o;
			}

			fixed4 Pixelate(sampler2D tex, float2 uv, int2 pixelate, int2 precission = int2(3, 3), bool interpolateColors = true)
			{
				fixed4 rescol = float4(0, 0, 0, 0);
				if ((pixelate.x > 1) || (pixelate.y > 1))
				{
					float2 pixelSize = 1 / float2(_ScreenParams.x, _ScreenParams.y);
					float2 BlockSize = pixelSize * pixelate;
					float2 CurrentBlock = float2(
						(floor(uv.x / BlockSize.x) * BlockSize.x),
						(floor(uv.y / BlockSize.y) * BlockSize.y)
						);

					float2 precissionSize = BlockSize / precission;
					for (int x = 0; x < precission.x; x++)
						for (int y = 0; y < precission.y; y++)
							rescol += tex2D(tex, CurrentBlock + (float2(x, y) * precissionSize));
					rescol /= precission.x * precission.y;

					if (!interpolateColors)
					{
						fixed4 finalColor = rescol;
						float minDif = 1;
						for (int x = 0; x < precission.x; x++)
							for (int y = 0; y < precission.y; y++)
							{
								fixed4 pixel = tex2D(tex, CurrentBlock + (float2(x, y) * precissionSize));
								float dif = length(rescol - pixel);
								if (dif < minDif)
								{
									finalColor = pixel;
									minDif = dif;
								}
							}
						rescol = finalColor;
					}
				}
				else
				{
					rescol = tex2D(tex, uv);
				}
				return rescol;
			}

			sampler2D _MainTex;
			half _Amount;
			int _Precission;
			float _InterpolateColors;

			fixed4 frag(v2f i) : SV_Target
			{
				int pixelateX = _ScreenParams.x * _Amount;
				int pixelateY = pixelateX; //TO DO: Support for separate axes?
				int2 pixelate = int2(pixelateX, pixelateY);
				return Pixelate(_MainTex, i.uv, pixelate, int2(_Precission, _Precission), _InterpolateColors > 0);
			}
			ENDCG
		}
	}
}