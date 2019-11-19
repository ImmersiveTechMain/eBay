Shader "Unlit/Snow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	    _Color ("Color", color) = (1,1,1,1)
		PixelScreenWidth("Pixel Width", float) = 1920
		PixelScreenHeight("Pixel Height", float) = 1080
		FlakeSize ("Flake Size (Pixel)", float) = 65
		FlakeCount("Flake Count", int) = 2000
		MaxApperture("Max apperture (flake movement)", float ) = 200
		FallingSpeed("Falling Speed (flake movement)", float) = 50
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
			float PixelScreenWidth;
			float PixelScreenHeight;
			float FlakeSize;
			float MaxApperture;
			float FallingSpeed = 50 ;
			int FlakeCount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			float4 DrawSnowflake(float2 uv, float2 position)
			{
				float2 currentPosition = uv;
				currentPosition.x = currentPosition.x * PixelScreenWidth;
				currentPosition.y = currentPosition.y * PixelScreenHeight;

				if (currentPosition.x >= position.x - FlakeSize * 0.5 && currentPosition.x < position.x + FlakeSize * 0.5 && currentPosition.y >= position.y - FlakeSize * 0.5 && currentPosition.y < position.y + FlakeSize * 0.5)
				{
					float left = (position.x - FlakeSize * 0.5f);
					float right = (position.x + FlakeSize * 0.5f);
					float up = (position.y + FlakeSize * 0.5f);
					float down = (position.y - FlakeSize * 0.5f);

					float x = (currentPosition.x - left) / FlakeSize;
					float y = (currentPosition.y - down) / FlakeSize;

					return tex2D(_MainTex, float2(x,y));
				}
				return float4(0, 0, 0, 0);
			}

			// original random function extracted from: https://stackoverflow.com/questions/5149544/can-i-generate-a-random-number-inside-a-pixel-shader
			float random(float seed)
			{
				float2 p = float2(seed, sqrt(seed+1)*1.1587952);
				float2 K1 = float2(
					23.14069263277926, // e^pi (Gelfond's constant)
					2.665144142690225 // 2^sqrt(2) (Gelfondâ€“Schneider constant)
				);
				return (cos(dot(p, K1)) + 1) / 2;
			}

			float4 frag (v2f input) : SV_Target
            {
				float4 col = float4(0,0,0,0);
				float PI = 3.1416;
				if (FlakeCount > 0) 
				{
					for (int i = 0; i < FlakeCount; i++) 
					{
						float offset = random(i) * PI * 2;
						float alpha = offset + _Time.y;
						float localX = 0;
						if (i % 2 == 0) 
						{
							localX = sin(alpha) * MaxApperture * random(FlakeCount + i);
						}
						else { localX = cos(alpha) * MaxApperture * random(FlakeCount + i); }
						float2 origin = float2(random(FlakeCount * 2 + i) * PixelScreenWidth, random(FlakeCount * 3 + i) * PixelScreenHeight);
						float2 position = origin;
						position.x = position.x + localX;
						position.y = position.y - _Time.y * FallingSpeed;
						if (position.y < 0) { position.y = PixelScreenHeight - (abs(position.y) % PixelScreenHeight); }

						float4 _col = DrawSnowflake(input.uv, position);
						col.r = max(col.r, _col.r);
						col.g = max(col.g, _col.g);
						col.b = max(col.b, _col.b);
						col.a = max(col.a, _col.a);

					}
				}

				if (col.a > 0) { col = _Color; }
                return col;
            }
            ENDCG
        }
    }
}
