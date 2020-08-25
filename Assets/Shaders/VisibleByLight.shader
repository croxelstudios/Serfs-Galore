Shader "Custom/LightReceiver" 
{
    Properties 
    {
		_Color("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Main Tex", 2D) = "white" {}

		_CellValue("Cell Value", Float) = 50
		_CellMultiplier("CellMult", Float) = 5
		_CellBias("Cell Bias", Float) = 0.01
		//TO DO: The cell shading system needs to be improved, probably using the code in TENANT

		_Rotation("Rotation", Range(-180, 180)) = -45

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
        Tags {"Queue" = "Geometry" "RenderType" = "Opaque"}
 
        Pass
		{
			Name "LightFilter"
            Tags {"LightMode" = "ForwardAdd"}                       // Again, this pass tag is important otherwise Unity may not give the correct light information.
            Blend One One                                          // Additively blend this pass with the previous one(s). This pass gets run once per pixel light.
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd_fullshadows                        // This line tells Unity to compile this pass for forward add, giving attenuation information for the light.
                
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
			#include "PabloShadingCG.cginc"
                
            struct v2f
            {
                float4  pos         : SV_POSITION;
                float2  uv          : TEXCOORD0;
                float3  lightDir    : TEXCOORD2;
                float3	normal		: TEXCOORD1;
                LIGHTING_COORDS(3,4)                            // Macro to send shadow & attenuation to the vertex shader.
            };
 
            v2f vert (appdata_tan v)
            {
                v2f o;
                    
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                   	
				o.lightDir = ObjSpaceLightDir(v.vertex);
					
				o.normal = v.normal;
                TRANSFER_VERTEX_TO_FRAGMENT(o);                 // Macro to send shadow & attenuation to the fragment shader.
                return o;
            }
 
            sampler2D _MainTex;
			UNITY_INSTANCING_BUFFER_START(LightsEffect)
				UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
				#define _Color_arr LightsEffect
				UNITY_DEFINE_INSTANCED_PROP(fixed4, _MainTex_ST)
				#define _MainTex_ST_arr LightsEffect
			UNITY_INSTANCING_BUFFER_END(LightsEffect)

			float _CellValue;
			float _CellMultiplier;
			float _CellBias;

			//Rotation-related data
			float _Rotation;
			//
 
            fixed4 _LightColor0; // Colour of the light used in this pass.

            fixed4 frag(v2f i) : COLOR
            {
                i.lightDir = normalize(i.lightDir);
                    
                fixed atten = LIGHT_ATTENUATION(i); // Macro to get you the combined shadow & attenuation value.
 
				fixed4 st = UNITY_ACCESS_INSTANCED_PROP(_MainTex_ST_arr, _MainTex_ST);
				float2 uv = RotateUV(i.uv, _Rotation, 1.0);

                fixed4 tex = tex2D(_MainTex, uv * st.xy + st.zw) * UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color);
                   
				fixed3 normal = i.normal;                    
                fixed diff = saturate(dot(normal, i.lightDir));
                   
				fixed attenCellShaded = ceil((atten + _CellBias) * _CellValue * _CellMultiplier) / _CellValue;

                fixed4 c;
				c.rgb = (tex.rgb * tex.a * _LightColor0.rgb * diff) * attenCellShaded; // Diffuse and specular.
                c.a = tex.a;
                return c;
            }
            ENDCG
        }
    }
    FallBack "VertexLit"    // Use VertexLit's shadow caster/receiver passes.
}