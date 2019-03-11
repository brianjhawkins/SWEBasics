Shader "Unlit/Water"
{
    Properties
    {
		_CDTex("Column Data Texture", 2D) = "white" {}
		_FTex("Flux Texture", 2D) = "red" {}
		_VTex("Velocity Texture", 2D) = "blue" {}
		_TintColor("Tint Color", Color) = (0, 0, 0, 1)
		_Transparency("Transparency", Range(0.0, 0.5)) = 0.25
		_CutoutThresh("Cutout Threshold", Range(0.0, 1.0)) = 0.2
		_Distance("Distance", float) = 1
		_Amplitude("Amplitude", float) = 1
		_Speed("Speed", float) = 1
		_Amount("Amount", Range(0.0, 1.0)) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _CDTex;
            float4 _CDTex_ST;
			float4 _TintColor;
			float _Transparency;
			float _CutoutThresh;
			float _Distance;
			float _Amplitude;
			float _Speed;
			float _Amount;

            v2f vert (appdata v)
            {
                v2f o;
				v.vertex.x += sin(_Time.y + v.vertex.y * _Amplitude) * _Distance * _Amount;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _CDTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_CDTex, i.uv) + _TintColor;
				col.a = _Transparency;
				if (col.r < _CutoutThresh)discard;
				// ^ Equivalent to clip(col.r - _CutoutThresh);
                return col;
            }
            ENDCG
        }
    }
}
