Shader "Custom/StencilMaskWrite"
{
	Properties
	{
		[Enum(Off,0,Front,1,Back,2)] _Cull("Cull", Int) = 2
		[MaterialToggle] _zWrite("Z Write", Float) = 0
		[IntRange] _StencilRef("Stencil Reference Value", Range(0, 255)) = 1
	}

	SubShader
	{
		Stencil
		{
			Ref[_StencilRef]
			Comp Always
			Pass Replace
		}

		Lighting Off
		Cull[_Cull]
		ZWrite[_zWrite]
		Blend Zero One

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			int frag(v2f i) : SV_Target
			{
				return 0;
			}
			ENDCG
		}
	}
}