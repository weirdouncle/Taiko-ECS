/****************************************************************************
 *
 * Copyright (c) 2020 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

using UnityEngine;
using UnityEngine.Scripting;
using System;
using System.Runtime.InteropServices;

namespace CriWare {

namespace CriMana.Detail
{

	// Fallback to this renderer if none are usable (Prime)

	public static partial class AutoResisterRendererResourceFactories
	{
		[Preserve]
		[RendererResourceFactoryPriority(10000)]
		public class RendererResourceFactorySofdecPrimeYuvRawData : RendererResourceFactory
		{
			public override RendererResource CreateRendererResource(int playerId, MovieInfo movieInfo, bool additive, Shader userShader)
			{
				bool isCodecSuitable = movieInfo.codecType == CodecType.SofdecPrime;
			#if (UNITY_EDITOR || !UNITY_SWITCH)
				isCodecSuitable |= movieInfo.codecType == CodecType.VP9;
			#endif
			#if (!UNITY_STANDALONE && UNITY_GAMECORE)
				isCodecSuitable |= movieInfo.codecType == CodecType.H264;
			#endif
				bool isSuitable = isCodecSuitable;
				return isSuitable
					? new RendererResourceSofdecPrimeYuvRawData(playerId, movieInfo, additive, userShader)
					: null;
			}

			protected override void OnDisposeManaged()
			{
			}

			protected override void OnDisposeUnmanaged()
			{
			}
		}
	}


	public class RendererResourceSofdecPrimeYuvRawData : RendererResource
	{
		private int width, height;
		private int chromaWidth, chromaHeight;
		private int alphaWidth, alphaHeight;
		private bool useUserShader;
		private CodecType codecType;

		private Vector4 movieTextureST = Vector4.zero;
		private Vector4 movieChromaTextureST = Vector4.zero;
		private Vector4 movieAlphaTextureST = Vector4.zero;

		private static Int32 NumTextureSets { get { return 1; } }

		private Texture2D[][] textures = new Texture2D[NumTextureSets][];
		private Int32 currentTextureSet = 0;
		private Int32 drawTextureSet = 0;
		System.IntPtr[] nativePixels;

		private Int32 playerID;
		private bool hasTextureUpdated = false;
		private bool hasRenderedNewFrame = false;
		private bool isStoppingForSeek = false;

		public RendererResourceSofdecPrimeYuvRawData(int playerId, MovieInfo movieInfo, bool additive, Shader userShader)
		{
			CalculateTextureSize(ref width, ref height, (int)movieInfo.width, (int)movieInfo.height, movieInfo.codecType, false);
			CalculateTextureSize(ref chromaWidth, ref chromaHeight, (int)movieInfo.width, (int)movieInfo.height, movieInfo.codecType, true);
			CalculateTextureSize(ref alphaWidth, ref alphaHeight, (int)movieInfo.width, (int)movieInfo.height, CriMana.CodecType.SofdecPrime, false);

			this.additive = additive;
			hasAlpha = movieInfo.hasAlpha;
			useUserShader = userShader != null;
			codecType = movieInfo.codecType;

			if (useUserShader) {
				shader = userShader;
			} else {
				string shaderName = "CriMana/SofdecPrimeYuv";
				shader = Shader.Find(shaderName);
			}

			UpdateMovieTextureST(movieInfo.dispWidth, movieInfo.dispHeight);

			for (int i = 0; i < NumTextureSets; i++) {
				textures[i] = new Texture2D[hasAlpha ? 4 : 3];
				textures[i][0] = new Texture2D(width, height, TextureFormat.Alpha8, false);
				textures[i][0].wrapMode = TextureWrapMode.Clamp;
				textures[i][1] = new Texture2D(chromaWidth, chromaHeight, TextureFormat.Alpha8, false);
				textures[i][1].wrapMode = TextureWrapMode.Clamp;
				textures[i][2] = new Texture2D(chromaWidth, chromaHeight, TextureFormat.Alpha8, false);
				textures[i][2].wrapMode = TextureWrapMode.Clamp;
				if (hasAlpha) {
					textures[i][3] = new Texture2D(alphaWidth, alphaHeight, TextureFormat.Alpha8, false);
					textures[i][3].wrapMode = TextureWrapMode.Clamp;
				}
			}
			nativePixels = new IntPtr[hasAlpha ? 4 : 3];

			playerID = playerId;
		}


		protected override void OnDisposeManaged()
		{
		}


		protected override void OnDisposeUnmanaged()
		{
			CRIWARE3BB32612(playerID, 0 ,null);

			for (int i = 0; i < NumTextureSets; i++) {
				DisposeTextures(textures[i]);
			}
			textures = null;
		}


		public override bool IsPrepared()
		{ return hasTextureUpdated; }


		public override bool ContinuePreparing()
		{ return true; }


		public override bool IsSuitable(int playerId, MovieInfo movieInfo, bool additive, Shader userShader)
		{
			int targetWidth = 0, targetHeight = 0;
			CalculateTextureSize(ref targetWidth, ref targetHeight, (int)movieInfo.width, (int)movieInfo.height, movieInfo.codecType, false);

			bool isCodecSuitable = movieInfo.codecType == codecType;
			bool isSizeSuitable = (width == targetWidth) && (height >= targetHeight);
			bool isAlphaSuitable = hasAlpha == movieInfo.hasAlpha;
			bool isAdditiveSuitable = this.additive == additive;
			bool isShaderSuitable = this.useUserShader ? (userShader == shader) : true;
			return isCodecSuitable && isSizeSuitable && isAlphaSuitable && isAdditiveSuitable && isShaderSuitable;
		}

		public override bool OnPlayerStopForSeek()
		{
			isStoppingForSeek = hasTextureUpdated;
			hasTextureUpdated = false;
			hasRenderedNewFrame = false;
			return true;
		}


		public override bool HasRenderedNewFrame()
		{
			return hasRenderedNewFrame;
		}

		public override void AttachToPlayer(int playerId)
		{
			CRIWAREEF8C5366(playerID, 0, new IntPtr[1]);
			hasTextureUpdated = false;
			hasRenderedNewFrame = false;
		}


		public override bool UpdateFrame(int playerId, FrameInfo frameInfo, ref bool frameDrop)
		{
			bool isFrameUpdated = CRIWAREBF11A5D7(playerId, 0, null, frameInfo, ref frameDrop);
			if (isFrameUpdated && !frameDrop) {
				UpdateMovieTextureST(frameInfo.dispWidth, frameInfo.dispHeight);
				drawTextureSet = currentTextureSet;
				currentTextureSet = (currentTextureSet + 1) % NumTextureSets;
			}
			return isFrameUpdated;
		}


		public override bool UpdateMaterial(Material material)
		{
			if (!hasTextureUpdated && NumTextureSets > 1)
				return false;

			if (currentMaterial != material) {
				currentMaterial = material;
				SetupStaticMaterialProperties();
			}

			if (material == null) {
				return false;
			}

			material.SetTexture("_TextureY", textures[drawTextureSet][0]);
			material.SetTexture("_TextureU", textures[drawTextureSet][1]);
			material.SetTexture("_TextureV", textures[drawTextureSet][2]);
			material.SetVector("_MovieTexture_ST", movieTextureST);
			material.SetVector("_MovieChromaTexture_ST", movieChromaTextureST);
			if (hasAlpha) {
				material.SetTexture("_TextureA", textures[drawTextureSet][3]);
				material.SetVector("_MovieAlphaTexture_ST", movieAlphaTextureST);
			}
			hasRenderedNewFrame = hasTextureUpdated;

			return (hasTextureUpdated || isStoppingForSeek);
		}


		private void UpdateMovieTextureST(System.UInt32 dispWidth, System.UInt32 dispHeight)
		{
			float uScale = (dispWidth != width) ? (float)(dispWidth - 1) / width : 1.0f;
			float vScale = (dispHeight != height) ? (float)(dispHeight - 1) / height : 1.0f;
			movieTextureST.x = uScale;
			movieTextureST.y = -vScale;
			movieTextureST.z = 0.0f;
			movieTextureST.w = vScale;

			uScale = (dispWidth != chromaWidth * 2) ? (float)(dispWidth / 2 - 1) / (chromaWidth * 2) * 2 : 1.0f;
			vScale = (dispHeight != chromaHeight * 2) ? (float)(dispHeight / 2 - 1) / (chromaHeight * 2) * 2 : 1.0f;
			movieChromaTextureST.x = uScale;
			movieChromaTextureST.y = -vScale;
			movieChromaTextureST.z = 0.0f;
			movieChromaTextureST.w = vScale;

			if (hasAlpha) {
				uScale = (dispWidth != alphaWidth) ? (float)(dispWidth - 1) / alphaWidth : 1.0f;
				vScale = (dispHeight != alphaHeight) ? (float)(dispHeight - 1) / alphaHeight : 1.0f;
				movieAlphaTextureST.x = uScale;
				movieAlphaTextureST.y = -vScale;
				movieAlphaTextureST.z = 0.0f;
				movieAlphaTextureST.w = vScale;
			}
		}


		public override void UpdateTextures()
		{
			int numTextures = hasAlpha ? 4 : 3;
			nativePixels[0] = System.IntPtr.Zero;

			// update pinned pointers with ptr from native code
			hasTextureUpdated |= CRIWARE98BCB8A4(playerID, numTextures, nativePixels);

			// copy native data to texture pixels
			if (hasTextureUpdated && nativePixels[0] != System.IntPtr.Zero) {
				textures[currentTextureSet][0].LoadRawTextureData(nativePixels[0], width * height);
				textures[currentTextureSet][1].LoadRawTextureData(nativePixels[1], chromaWidth * chromaHeight);
				textures[currentTextureSet][2].LoadRawTextureData(nativePixels[2], chromaWidth * chromaHeight);

				textures[currentTextureSet][0].Apply();
				textures[currentTextureSet][1].Apply();
				textures[currentTextureSet][2].Apply();

				if (hasAlpha) {
					textures[currentTextureSet][3].LoadRawTextureData(nativePixels[3], alphaWidth * alphaHeight);
					textures[currentTextureSet][3].Apply();
				}

				isStoppingForSeek = false;
			}
		}

		private static void CalculateTextureSize(ref int w, ref int h, int videoWidth, int videoHeight, CodecType type, bool isChroma)
		{
			if (type == CodecType.SofdecPrime) {
				var alignment = CriManaPlugin.GetPrimeBufferAlignmentSize();
				if (!isChroma) {
					w = CeilingWith(Ceiling16(videoWidth), (int)alignment);
					h = CeilingWith(videoHeight, 8);
				} else {
					w = CeilingWith(Ceiling16(videoWidth) / 2, (int)alignment);
					h = CeilingWith(videoHeight, 8) / 2;
				}
			} else if (type == CodecType.VP9) {
				if (!isChroma) {
					w = CeilingWith(CeilingWith(videoWidth, 2), 8);
					h = CeilingWith(videoHeight, 2);
				} else {
					w = CeilingWith(CeilingWith(videoWidth, 2) / 2, 8);
					h = CeilingWith(videoHeight, 2) / 2;
				}
			} else if (type == CodecType.H264) {
				if (!isChroma) {
					w = videoWidth;
					h = videoHeight;
				} else {
					w = CeilingWith(videoWidth / 2, 8);
					h = CeilingWith(videoHeight, 2) / 2;
				}
			}
		}
	}
}

} //namespace CriWare