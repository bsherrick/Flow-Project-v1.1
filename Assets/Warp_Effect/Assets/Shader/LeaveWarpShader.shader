Shader "Warp Effect/LeaveWarpShader" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		_DisplaceTex("Displacement Texture", 2D) = "white" {}
		_Magnitude("Magnitude", Range(-0.1, 0.1)) = 0.05
		_SpeedX("Speed X", Range(-5, 5)) = 1
		_SpeedY("Speed Y", Range(-5, 5)) = 1
	}

	SubShader {
		// No culling or depth
		Cull Off
		ZWrite Off
		ZTest Always
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
	
			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _DisplaceTex;
			float _Magnitude;
			float _SpeedX;
			float _SpeedY;

			fixed4 frag (v2f i) : SV_Target {
				float2 distuv = float2( (sin(i.uv.x - i.uv.y) / tan(i.uv.x + i.uv.y)) + _Time.x * _SpeedX, (cos(i.uv.x + i.uv.y) + log2(i.uv.x + i.uv.y)) + _Time.x * _SpeedY);

				float2 disp = tex2D(_DisplaceTex, distuv).xy;
				disp = ((disp * 2) - 1) * _Magnitude;

				float4 col = tex2D(_MainTex, i.uv + disp);

				return col;
			}
			ENDCG
		}
	}
}
