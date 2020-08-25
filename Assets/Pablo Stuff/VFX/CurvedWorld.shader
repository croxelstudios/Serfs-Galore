Shader "Custom/Curvedd"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Base (RGB)", 2D) = "white" {}
        [MaterialToggle] DoNotCurve("No curve", Float) = 0
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
        //[Enum(Off,0,Front,1,Back,2)] _Cull("Cull", Int) = 0
        _CutOff("CutOff", Range(0, 1)) = 0.5
        //[MaterialToggle] _zWrite("Z Write", Float) = 1
        //[Enum(Less,0,Greater,1,LEqual,2,GEqual,3,Equal,4,NotEqual,5,Always,6)]
        //_zTest("Z Test", Int) = 2
        [MaterialToggle] _ColorAlphaMultiply("ColorAlphaMultiply", Float) = 0
    }

    SubShader
    {
        Tags {"Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
        LOD 200

        Cull Off
        ZWrite true
        ZTest LEqual

        CGPROGRAM
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma multi_compile_local _ PIXELSNAP_ON
        #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
        #pragma surface surf Lambert alphatest:_CutOff vertex:vert addshadow
        #include "UnityCG.cginc"

        // Global Shader values
        uniform float2 _BendAmount;
        uniform float3 _BendOrigin;
        uniform float _BendFalloff;

        sampler2D _MainTex;
        fixed4 _Color;

        struct c_appdata_full
        {
            float4 vertex    : POSITION;  // The vertex position in model space.
            float3 normal    : NORMAL;    // The vertex normal in model space.
            float4 texcoord  : TEXCOORD0; // The first UV coordinate.
            float4 texcoord1 : TEXCOORD1; // The second UV coordinate.
            float4 texcoord2 : TEXCOORD2; // The second UV coordinate.
            float4 tangent   : TANGENT;   // The tangent vector in Model Space (used for normal mapping).
            float4 color     : COLOR;     // Per-vertex color
            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
        };

        struct Input
        {
            float2 uv_MainTex;
            float4 color : COLOR;
        };

        inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip)
        {
            return float4(pos.xy * flip, pos.z, 1.0);
        }

        float4 Curve(float4 v)
        {
            //HACK: Considerably reduce amount of Bend
            _BendAmount *= .0001;

            float4 world = mul(unity_ObjectToWorld, v);

            float dist = length(world.xz - _BendOrigin.xz);

            dist = max(0, dist - _BendFalloff);

            // Distance squared
            dist = dist * dist;

            world.xy += dist * _BendAmount;
            return mul(unity_WorldToObject, world);
        }

        #ifdef UNITY_INSTANCING_ENABLED
            UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
                // SpriteRenderer.Color while Non-Batched/Instanced.
                UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteRendererColorArray)
                // this could be smaller but that's how bit each entry is regardless of type
                UNITY_DEFINE_INSTANCED_PROP(fixed2, unity_SpriteFlipArray)
            UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

            #define _RendererColor  UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)
            #define _Flip           UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteFlipArray)
        #endif // instancing

        CBUFFER_START(UnityPerDrawSprite)
            #ifndef UNITY_INSTANCING_ENABLED
            fixed4 _RendererColor;
            fixed2 _Flip;
            #endif
            float _EnableExternalAlpha;
        CBUFFER_END

        bool DoNotCurve;

        void vert(inout c_appdata_full v)
        {
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(v);

            v.vertex = UnityFlipSprite(v.vertex, _Flip);
            if (!DoNotCurve) v.vertex = Curve(v.vertex);
            v.texcoord = v.texcoord;
            v.color = v.color * _Color *_RendererColor;

            #ifdef PIXELSNAP_ON
            v.vertex = UnityPixelSnap(v.vertex);
            #endif
        }

        sampler2D _AlphaTex;

        fixed4 SampleSpriteTexture(float2 uv)
        {
            fixed4 color = tex2D(_MainTex, uv);

            #if ETC1_EXTERNAL_ALPHA
            fixed4 alpha = tex2D(_AlphaTex, uv);
            color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
            #endif

            return color;
        }

        bool _ColorAlphaMultiply;

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 c = SampleSpriteTexture(IN.uv_MainTex) * IN.color;
            if (_ColorAlphaMultiply) c.rgb *= c.a;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }

        ENDCG
    }

    Fallback "Mobile/Diffuse"
    //Generalize, Improve
}
