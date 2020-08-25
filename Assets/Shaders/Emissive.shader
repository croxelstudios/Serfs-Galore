Shader "Custom/Emissive"
{
    Properties
    {
        _EmissiveColor ("EmissiveColor", Color) = (1,1,1,1)
		_EmissiveTex("EmissiveTexture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent"}
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
			Name "PASS"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
				fixed4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            sampler2D _EmissiveTex;
            float4 _EmissiveTex_ST;

			UNITY_INSTANCING_BUFFER_START(Emissive)
				UNITY_DEFINE_INSTANCED_PROP(float4, _EmissiveColor)
				#define _EmissiveColor_arr _EmissiveColor
			UNITY_INSTANCING_BUFFER_END(Emissive)

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _EmissiveTex);
				o.color = v.color * UNITY_ACCESS_INSTANCED_PROP(_EmissiveColor_arr, _EmissiveColor);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
				fixed4 col = i.color
					* tex2D(_EmissiveTex, i.uv);
				//col.a *= tex2D(_MainTex, i.uv).a;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
