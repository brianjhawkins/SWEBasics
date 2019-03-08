Shader "Example/Diffuse Simple"
{
	Properties{
		_MainTex("Texture", 2D) = "white" {}
	}

		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		#pragma surface surf Lambert

        struct Input
        {
            float2 uv_MainTex;
        };

		sampler2D _MainTex;

        void surf (Input IN, inout SurfaceOutput o)
        {
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
			o.Alpha = tex2D(_MainTex, IN.uv_MainTex).a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
