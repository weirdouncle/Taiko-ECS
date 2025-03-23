Shader "Don/Color" {
    Properties {
        _MainTex ("Texture", 2D) = "white" { }
		_Green("FaceColor", Color) = (0.972549,0.282353,0.1568628,1)
		_Blue("BodyColor", Color) = (0.4078431,0.7529412,0.7529412,1)
		_Red("SkinColor", Color) = (0.972549,0.9411765,0.8784314,1)
        [Enum(UnityEngine.Rendering.CullMode)] _Culling ("Culling", Float) = 2

    }
    SubShader {
		Lighting Off
		LOD 100
        Tags { "IGNOREPROJECTOR" = "true" "RenderType" = "Opaque" "RenderPipeline"="UniversalRenderPipeline" }

		Pass {
			Tags { "RenderType" = "Opaque" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
			Cull [_Culling]
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			struct Attributes {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct Varyings {
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
			};
			
		CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_ST;
			float _Cutoff;
			float4 _Red;
			float4 _Blue;
			float4 _Green;
			CBUFFER_END

		TEXTURE2D(_MainTex);
		SAMPLER(sampler_MainTex);

			Varyings vert(Attributes v)
			{
				Varyings o;
				o.vertex = TransformObjectToHClip(v.vertex.xyz);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			half4 frag(Varyings i) : SV_Target
			{
				float4 col= SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
				float4 position1;
				position1.xyz = col.yyy * _Green.xyz;
				position1.xyz = _Red.xyz * col.xxx + position1.xyz;
				float4 o;
				o.xyz = _Blue.xyz * col.zzz + position1.xyz;
				o.a = col.a;
				return o;
			}
			ENDHLSL
		}
    }
}