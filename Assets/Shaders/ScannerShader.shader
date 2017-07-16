Shader "Custom/ScannerShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_ScanDistance("Scan Distance", float) = 0
		_ColorFalloff("Color Falloff", float) = 1
		_ScanlineColor("Scanline Color", Color) = (0, 0, 0, 0)
		_ScanlineWidth("Scanline Width", float) = 0.1
		_ScanlineBoldness("Scanline Boldness", float) = 1
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct VertIn
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 ray : TEXCOORD1;
			};

			struct VertOut
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uv_depth : TEXCOORD1;
				float4 interpolatedRay : TEXCOORD2;
			};

			float4 _MainTex_TexelSize;
			float4 _CameraWS;

			VertOut vert(VertIn v)
			{
				VertOut o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv.xy;
				o.uv_depth = v.uv.xy;

				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv.y = 1 - o.uv.y;
				#endif				

				o.interpolatedRay = v.ray;

				return o;
			}

			sampler2D _MainTex;
			sampler2D_float _CameraDepthTexture;
			float4 _WorldSpaceScannerPos;
			float _ScanDistance;
			float _ColorFalloff;
			float4 _ScanlineColor;
			float _ScanlineWidth;
			float _ScanlineBoldness;

			half4 frag(VertOut i) : SV_Target
			{
				half4 col = tex2D(_MainTex, i.uv);

				float rawDepth = DecodeFloatRG(tex2D(_CameraDepthTexture, i.uv_depth));
				float linearDepth = Linear01Depth(rawDepth);
				float4 dir = linearDepth * i.interpolatedRay;
				float3 pos = _WorldSpaceCameraPos + dir;

				float dist = distance(pos, _WorldSpaceScannerPos);

				if (abs(dist - _ScanDistance) <= _ScanlineWidth)
					return lerp(col, _ScanlineColor, _ScanlineBoldness);
				else if (dist < _ScanDistance && linearDepth < 1)
					return col;

				float falloffFactor = 1 - (dist - _ScanDistance) * _ColorFalloff;

				return col * falloffFactor;
			}

			ENDCG
		}
	}
}