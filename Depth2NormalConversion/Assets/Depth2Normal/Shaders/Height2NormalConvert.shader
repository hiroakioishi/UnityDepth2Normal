Shader "Hidden/Height2Normal" {
	Properties {
		_MainTex         ("Height Map",        2D   ) = "white" {}
		_BumpHeightScale ("Bump Height Scale", Float) = 0.01
		_TexelSizeScale  ("Texel Size Scale",  Float) = 1
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	struct v2f {
		float4 position : POSITION;
		float2 texcoord : TEXCOORD0;
	};
	
	uniform sampler2D _MainTex;
	float4 _MainTex_TexelSize;
	uniform float     _BumpHeightScale;
	uniform float     _TexelSizeScale;
	
	v2f vert (appdata_img v) {
		v2f o;
		o.position = mul (UNITY_MATRIX_MVP, v.vertex);
		o.texcoord = v.texcoord.xy;
		return o;
	}
	
	float4 _MainTex_ST;
	
	half4 frag (v2f i) : COLOR {
		
		float2 uv  = i.texcoord.xy;
		float2 uvE = uv + fixed2(_MainTex_TexelSize.x * _TexelSizeScale, 0.0);
		float2 uvN = uv + fixed2(0.0, _MainTex_TexelSize.y * _TexelSizeScale);
		
		fixed  height  = tex2D(_MainTex, uv ).x * _BumpHeightScale;
		fixed  heightE = tex2D(_MainTex, uvE).x * _BumpHeightScale;
		fixed  heightN = tex2D(_MainTex, uvN).x * _BumpHeightScale;
		
		// BiNormal Vector
		fixed3 bv = fixed3(uvN.x, uvN.y, heightN) - fixed3(uv.x, uv.y, height);
		fixed3 tv = fixed3(uvE.x, uvE.y, heightE) - fixed3(uv.x, uv.y, height);
		
		bv = normalize(bv);
		tv = normalize(tv);
		
		fixed3 norm = 0.5 + 0.5 * normalize(cross(tv, bv));
		//fixed3 norm = fixed3 (height, height, height);
		return half4(norm.xyz, 1.0);
	}
	ENDCG
	
	SubShader {
	
		//Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		//Blend SrcAlpha OneMinusSrcAlpha
		//AlphaTest Greater .01
		Cull Off Lighting Off ZWrite On
		Pass {
			CGPROGRAM
			#pragma vertex   vert
			#pragma fragment frag
			ENDCG
		}
	}
	FallBack off
	
}
