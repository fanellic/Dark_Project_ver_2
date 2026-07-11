Shader "Custom/SnowFootPrint"
{
	Properties
    {
        _SnowMask ("Snow Mask", 2D) = "black" {}

        _SnowColor ("Snow Color", Color) =
        (1,1,1,1)

        _WorldSize ("World Size", Float) = 20

        _Depth ("Footprint Depth", Float) = 0.05

        _SnowCenter ("Snow Center", Vector) =
        (0,0,0,0)
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _SnowMask;

            float4 _SnowColor;

            float _WorldSize;

            float _Depth;

            float4 _SnowCenter;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;

                float footprint : TEXCOORD0;

                float2 uv : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;

                // WORLD POSITION
                float3 worldPos =
                    mul(
                        unity_ObjectToWorld,
                        v.vertex
                    ).xyz;

                // WORLD-SPACE UVs
                float halfSize = _WorldSize * 0.5;

                float2 uv;

                uv.x =
                    (worldPos.x -
                    (_SnowCenter.x - halfSize))
                    / _WorldSize;

                uv.y =
                    (worldPos.z -
                    (_SnowCenter.z - halfSize))
                    / _WorldSize;

                o.uv = uv;

                // SAMPLE FOOTPRINT MASK
                float footprint =
                    tex2Dlod(
                        _SnowMask,
                        float4(uv,0,0)
                    ).r;

                o.footprint = footprint;

                // DISPLACE SNOW DOWNWARD
                worldPos.y -=
                    footprint * _Depth;

                // FINAL POSITION
                o.pos =
                    UnityWorldToClipPos(
                        worldPos
                    );

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // DEBUG:
                // return float4(i.uv,0,1);

                // DEBUG:
                // return i.footprint;

                // DARKEN COMPRESSED SNOW
                float darken =
                    lerp(
                        1.0,
                        0.05,
                        i.footprint
                    );

                return
                    _SnowColor * darken;
            }

            ENDCG
        }
    }
}
