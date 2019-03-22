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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _CDTex;
			float4 _CDTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
				float x = v.uv.x;
				float y = v.uv.y;
				float4 tex = tex2Dlod(_CDTex, float4(x, y, 0, 0));
				float4 left = tex2Dlod(_CDTex, float4(x - 1, y, 0, 0));
				float4 right = tex2Dlod(_CDTex, float4(x + 1, y, 0, 0));
				float4 top = tex2Dlod(_CDTex, float4(x, y - 1, 0, 0));
				float4 bottom = tex2Dlod(_CDTex, float4(x, y + 1, 0, 0));
				float texHeight = tex.r + tex.g;
				float leftHeight = left.r + left.g;
				float rightHeight = right.r + right.g;
				float topHeight = top.r + top.g;
				float bottomHeight = bottom.r + bottom.g;

				texHeight = lerp(texHeight, leftHeight, 0);
				texHeight = lerp(texHeight, rightHeight, 0);
				texHeight = lerp(texHeight, topHeight, 0);
				texHeight = lerp(texHeight, bottomHeight, 0);


				v.vertex.y += (texHeight);
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _CDTex);
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
