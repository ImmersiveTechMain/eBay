﻿Shader "Unlit/FlashRed"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	    _Color ("Color", Color) = (1,0,0,1)
	    _Speed ("Speed", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
				float4 col = tex2D(_MainTex, i.uv);
				float alpha = col.a;

				float normalized = _Time.y / _Speed;
				normalized = normalized - floor(normalized);

				float4 maxColor = _Color;
				maxColor.a = alpha;
				col = lerp(float4(1, 1, 1, 1), maxColor, normalized);
				col.a = alpha;
                return col;
            }
            ENDCG
        }
    }
}