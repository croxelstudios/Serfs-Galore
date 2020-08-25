Shader "Custom/CustomStandard"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}

		_Smoothness("Smoothness", Range(0, 1)) = 0
		_Metallic("Metalness", Range(0, 1)) = 0
		[HDR] _Emission("Emission", color) = (0,0,0)
/*		//TO DO
		_BumpScale("Scale", Float) = 1.0
		[Normal] _BumpMap("Normal Map", 2D) = "bump" {}
		_Parallax("Height Scale", Range(0.005, 0.08)) = 0.02
		_ParallaxMap("Height Map", 2D) = "black" {}
*/
		_Rotation("Rotation", Range(-180, 180)) = 0.0
		_Aspect("Aspect", Float) = 1

		_CutOff("CutOff", Range(0, 1)) = 0
		[Enum(Off,0,Front,1,Back,2)] _Cull("Cull", Int) = 2
		[MaterialToggle] _zWrite("Z Write", Float) = 0
		[Enum(Less,0,Greater,1,LEqual,2,GEqual,3,Equal,4,NotEqual,5,Always,6)]
		_zTest("Z Test", Int) = 2
	}
	SubShader
	{
		Cull[_Cull]
		ZWrite[_zWrite]
		ZTest[_zTest]
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha
		#pragma target 3.0
		#include "PabloShadingCG.cginc"

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
		};

		half _Smoothness;
		half _Metallic;
		//NormalMapData
		sampler2D _BumpMap;
		//
		//HeightMapData
		sampler2D _ParallaxMap;
		//
		fixed4 _Color;
		fixed _CutOff;
		//Rotation-related data
		float _Rotation;
		float _Aspect;
		//

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			float2 uv = RotateUV(IN.uv_MainTex, _Rotation, _Aspect);
			//TO DO: Aspect is not necessary if I can get access to _MainTex_ST, but it seems I can't

			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, uv) * _Color;
			if (c.a < _CutOff) discard;
			o.Albedo = c.rgb;
			//o.Normal = UnpackNormal(tex2D(_BumpMap, uv));
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
