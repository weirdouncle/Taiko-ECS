Shader "Universal Render Pipeline/3D/OutlineTransparent"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_AlphaClipBorder("Alpha Clip Border", Float) = 0
		_ClipJustBorder("Clip Just Border", Float) = 0
		_BlackLineParam("BlackLineParam", Vector) = (960,0.0001, 2.5, 0.8)
		_Alpha("Alpha", Range(0,1)) = 1
	}
	SubShader
	{
		Tags {"Queue" = "AlphaTest" "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

			Name "OutlinePass"
			Cull Front
			ZWrite On
			Blend One Zero
			ZTest LEqual
		

        Pass
        {
            Tags { "LightMode" = "UniversalForward" "Queue"="Transparent" "RenderType"="Transparent"}

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"
            #if defined(DEBUG_DISPLAY)
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/InputData2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"
            #endif

            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile_fragment _ DEBUG_DISPLAY

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
				float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS      : SV_POSITION;
                float2  uv              : TEXCOORD0;
				float3 normalOS : NORMAL;
                UNITY_VERTEX_OUTPUT_STEREO
            };

			CBUFFER_START(UnityPerMaterial)
				float4 _MainTex_ST;
				float _AlphaClipBorder;
				float _ClipJustBorder;
				float4 _BlackLineParam;
				float _Alpha;
			CBUFFER_END

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			Varyings UnlitVertex(Attributes v)
			{
				Varyings o;
				o.uv = v.uv;
				float4x4 pose_p = transpose(unity_CameraInvProjection);
				float4 r0;
				r0.xy = pose_p[0].xy + pose_p[1].xy;
				r0.xy = r0.xy + pose_p[2].xy;
				r0.xy = pose_p[3].xy * _ProjectionParams.yy + r0.xy;
				r0.x = r0.x / r0.y;

				float4x4 pose_iv = transpose(unity_MatrixInvV);
				r0.yzw = mul(unity_WorldToObject, pose_iv[0]).xyz;

				float4 r1;
				r1.x = dot(r0.yzw, v.normalOS.xyz);				//r1.x = dot(r0.yzwy, in.NORMAL.xyzx);

				r0.yzw = mul(unity_WorldToObject, pose_iv[1]).xyz;
				r1.y = dot(r0.yzw, v.normalOS.xyz);

				r0.yzw = mul(unity_WorldToObject, pose_iv[2]).xyz;

				r1.z = dot(r0.yzw, v.normalOS.xyz);
				r0.y = dot(r1.xyz, r1.xyz);
				r0.y = rsqrt(r0.y);
				
				r1.xyz *= r0.yyy;			//r1.xyz = r0.yyy * r1.xyz;
				r1.w = -r1.y;
				r0.y = dot(r1.xw, r1.xw);
				r0.y = rsqrt(r0.y);
				r0.yz = r0.yy * r1.xw;
				r0.w = r1.y != 0;

				float4 r2;
				r2.xy = r1.xz != float2(0.0, 0.0);					//r2.xy = float2(0.0, 0.0) != r1.xz;
				r0.w = max(r0.w, r2.x);
				r1.y = max(r2.y, r0.w);
				
				r0.yz *= _BlackLineParam.zz;					//r0.yz = r0.yz * cb0[0].zz;
				r2.xy = r0.yx * v.color.yy;						//r2.xy = r0.yx * in.COLOR.yy;
				r2.xy = r0.yz / _BlackLineParam.x;
				r2.z = r0.x * r2.y;

				r0 = mul(unity_ObjectToWorld, float4(v.positionOS, 1));

				float4x4 pose_vp = transpose(UNITY_MATRIX_VP);			//ÊÇVPµÄ×ªÖÃ
				float4 r3 = pose_vp[1] * r0.yyyy;					//r3.xyzw = r0.yyyy * cb4[18].xyzw;
				r3.xyzw += pose_vp[0].xyzw * r0.xxxx;				//r3.xyzw = cb4[17].xyzw*r0.xxxx + r3.xyzw;						
				r3.xyzw += pose_vp[2].xyzw * r0.zzzz;				//r3.xyzw = cb4[19].xyzw*r0.zzzz + r3.xyzw;	
				r0.xyzw = pose_vp[3].xyzw * r0.wwww + r3.xyzw;		//r0.xyzw = cb4[20].xyzw*r0.wwww + r3.xyzw;

				o.positionCS.xy = r2.xz * v.color.y + r0.xy;			//out.SV_POSITION.xy = r2.xz*in.COLOR.yy + r0.xy;
				o.positionCS.z = r0.z - _BlackLineParam.y;				//out.SV_POSITION.z = r0.z - cb0[0].y;
				o.positionCS.w = r0.w;									//out.SV_POSITION.w = r0.w;

				r0.x = dot(r1.xzw, r1.xzw);		//r0.x = dot(r1.xzwx, r1.xzwx);
				r0.x = rsqrt(r0.y);													//r0.x = rsqrt(r0.x);
				r0.xyz = r0.xxx * r1.xwz;

				o.normalOS = r0.xyz;
				return o;
			}

			half4 UnlitFragment(Varyings i) : SV_Target
			{
				float4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

				// Combine checks for normal.z and texture alpha
				float discardValue = max(step(_BlackLineParam.w, i.normalOS.z), step(mainTex.a, 0.0));
				clip(-discardValue); // Discard if either condition is true

				// Output black color with texture alpha
				return float4(0, 0, 0, 1);
			}
			ENDHLSL
		}
	}
    Fallback "Sprites/Default"
}