Shader "Universal Render Pipeline/3D/Outline"
{
    Properties
    {
		_MainTex("MainTex", 2D) = "white" {}
		_OutlineWidth("OutlineFactor", Range(0, 1)) = 0.003
		_Z("ZValue", Range(-0.01, 0.01)) = -0.0001
        // Legacy properties. They're here so that materials using this shader can gracefully fallback to the legacy sprite shader.
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
        [HideInInspector] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _AlphaTex ("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
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
            Tags { "LightMode" = "UniversalForward" "Queue"="AlphaTest" "RenderType"="Transparent"}

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
			float _OutlineWidth;
			float _Z;
			CBUFFER_END

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

            Varyings UnlitVertex(Attributes v)
            {
                float4 position0 = mul(unity_ObjectToWorld, float4(v.positionOS, 1));			//转换到世界坐标
				float4 position1;
				float4x4 pose_vp = transpose(UNITY_MATRIX_VP);			//是VP的转置
				float4 cb0 = pose_vp[1];
				position1.xyzw = position0.yyyy * cb0.xyzw;													//position1.xyzw = position0.yyyy * cb3[18].xyzw;	//cb3[17]~[20]是Matrix_VP
				position1.xyzw += pose_vp[0].xyzw * position0.xxxx;										//position1.xyzw = cb3[17].xyzw*position0.xxxx + position1.xyzw;
				position1.xyzw += pose_vp[2].xyzw * position0.zzzz;										//position1.xyzw = cb3[19].xyzw*position0.zzzz + position1.xyzw;
				position0.xyzw = pose_vp[3].xyzw * position0.wwww + position1.xyzw;						//position0.xyzw = cb3[20].xyzw*position0.wwww + position1.xyzw;

				float4x4 pose_p = transpose(unity_CameraInvProjection);					//是projection的转置矩阵
				position1.xy = pose_p[0].xy + pose_p[1].xy;								//position1.xy = cb1[10].xy + cb1[11].xy;
				position1.xy = position1.xy + pose_p[2].xy;								//position1.xy = position1.xy + cb1[12].xy;
				position1.xy = pose_p[3].xy * _ProjectionParams.yy + position1.xy;		//position1.xy = cb1[13].xy * cb0[5].yy + position1.xy;
				position1.x = position1.x / position1.y;

				//position1.yzw = cb2[5].xyz * cb3[13].yyy;					//cb3[5]~[8]是剪裁矩阵  cb3[13]~[16]是unity_MatrixInvV的装置矩阵
				//position1.yzw = cb2[4].xyz*cb3[13].xxx + position1.yzw;				//cb2[4]~[7]是unity_WorldToObject
				//position1.yzw = cb2[6].xyz*cb3[13].zzz + position1.yzw;
				//position1.yzw = cb2[7].xyz*cb3[13].www + position1.yzw;

				float4x4 pose_iv = transpose(UNITY_MATRIX_I_V);
				float4 cb3 = pose_iv[0];
				position1.yzw = mul(UNITY_MATRIX_I_M, cb3).xyz;

				float4 position2;
				position2.x = dot(position1.yzw, v.normalOS.xyz);

				cb3 = pose_iv[1];
				position1.yzw = mul(UNITY_MATRIX_I_M, cb3).xyz;
				position2.y = dot(position1.yzw, v.normalOS.xyz);

				cb3 = pose_iv[2];
				position1.yzw = mul(UNITY_MATRIX_I_M, cb3).xyz;

				position2.z = dot(position1.yzw, v.normalOS.xyz);					//dot(position1.yzay, v.normal.xyzx);
				position1.y = dot(position2.xyz, position2.xyz);		//position1.y = dot(position2.xyzx, position2.xyzx);
				position1.y = rsqrt(position1.y);

				position2.xyz *= position1.yyy;

				position2.w = -position2.y;
				position1.y = dot(position2.xw, position2.xw);		//position1.y = dot(position2.xwxx, position2.xwxx);
				position1.y = rsqrt(position1.y);
				position1.yz = position1.yy * position2.xw;	//position1.yz = position1.yy * position2.xw;
				//position1.w = position2.y != 0;

				float4 position3;
				//position3.xy = position2.xz != float2(0.0, 0.0);
				//position1.w = max(position1.w, position3.x); //(int)position1.w | (int)position3.x;
				//position2.y = max(position3.y, position1.w); //(int)position3.y | (int)position1.w;

				//position1.y = (int)position1.y & (int)position1.w ? (int)position1.y & (int)position1.w : position1.y;			//position1.yz = position1.yz & position1.ww;
				//position1.z = (int)position1.z & (int)position1.w ? (int)position1.z & (int)position1.w : position1.z;

				position1.x *= position1.z;
				position3.xy = position1.yx * v.color.yy;						//position3.xy = position1.yx * in.COLOR.yy;

				Varyings out_put;
				out_put.positionCS.xy = position3.xy * float2(_OutlineWidth, _OutlineWidth) + position0.xy;						//out.SV_POSITION.xy = position3.xy*float2(0.008333, 0.008333) + position0.xy;
				out_put.positionCS.zw = position0.zw + float2(_Z, 0);							//out.SV_POSITION.zw = position0.zw + float2(-0.0001, 0.0);

				position0.x = dot(position2.xzw, position2.xzw);					//position0.x = dot(position2.xzwx, position2.xzwx);
				position0.x = rsqrt(position0.x);
				position0.xyz = position0.xxx * position2.xwz;						//position0.xyz = position0.xxx * position2.xwz;
				
				out_put.uv = v.uv;
				//re = (int)position0.x & (int)position2.y;
				//out_put.normal = float3(v.normal.x, v.normal.y, (int)position0.z & (int)position2.y ? (int)position0.z & (int)position2.y : position0.z);
				out_put.normalOS = position0.xyz;
				//out_put.normal.xyz = (int)position0.xyz & (int)position2.yyy;		//out.NORMAL.xyz = position0.xyz & position2.yyy;
				return out_put;
            }

            float4 UnlitFragment(Varyings i) : SV_Target
            {
				half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

				// Combine checks for normal.z and texture alpha
				float discardValue = max(step(0.8, i.normalOS.z), step(texColor.a, 0.0));
				clip(-discardValue); // Discard if either condition is true
				// Output black color with texture alpha
				return half4(0, 0, 0, texColor.a);
            }
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}
