Shader "Unlit/Water"
{
    Properties
    {
		_CDTex("Column Data Texture", 2D) = "white" {}
		_FTex("Flux Texture", 2D) = "red" {}
		_VTex("Velocity Texture", 2D) = "blue" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
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
				float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float4 color : COLOR;
            };

            sampler2D _CDTex;
			float4 _CDTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _CDTex);
				o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_CDTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
