﻿/****************************************************************************
 *
 * Copyright (c) 2014 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

#define CRI_UNITY_EDITOR_PREVIEW

using UnityEngine;
using UnityEditor;
using System.IO;

namespace CriWare {

[CustomEditor(typeof(CriAtomSource))]
public class CriAtomSourceEditor : CriAtomSourceBaseEditor {
	#region Variables
#if CRI_UNITY_EDITOR_PREVIEW
	public CriAtom atomComponent;
	private CriWare.Editor.CriAtomEditorUtilities.PreviewPlayer previewPlayer;
	private CriAtomExAcb previewAcb = null;
	private string lastCuesheet = "";
#endif
	#endregion

	#region Functions

	protected override void OnEnable()
	{
		base.OnEnable();

#if CRI_UNITY_EDITOR_PREVIEW
		/* シーンからCriAtomコンポーネントを見つけ出す */
#if UNITY_2023_1_OR_NEWER
		atomComponent = (CriAtom)FindAnyObjectByType(typeof(CriAtom));
#else
		atomComponent = (CriAtom)FindObjectOfType(typeof(CriAtom));
#endif

		previewPlayer = new CriWare.Editor.CriAtomEditorUtilities.PreviewPlayer();
		
		if (EditorApplication.isPlayingOrWillChangePlaymode == false && atomComponent != null) {
			if (string.IsNullOrEmpty(atomComponent.acfFile) == false) {
				CriAtomEx.RegisterAcf(null, Path.Combine(CriWare.Common.streamingAssetsPath, atomComponent.acfFile));
				if (string.IsNullOrEmpty(atomComponent.dspBusSetting) == false) {
					CriAtomEx.DetachDspBusSetting();
					CriAtomEx.AttachDspBusSetting(atomComponent.dspBusSetting);
				}
			}
		}
#endif
	}

	private void OnDisable() {
#if CRI_UNITY_EDITOR_PREVIEW
		if (previewAcb != null) {
			previewAcb.Dispose();
			previewAcb = null;
		}
		lastCuesheet = "";
		if (previewPlayer != null) {
			previewPlayer.Dispose();
			previewPlayer = null;
		}
#endif
	}

	protected override void InspectorCueReferenceGUI()
	{
		(source as CriAtomSource).cueSheet = EditorGUILayout.TextField("Cue Sheet", (source as CriAtomSource).cueSheet);
		(source as CriAtomSource).cueName = EditorGUILayout.TextField("Cue Name", (source as CriAtomSource).cueName);
	}

	protected override void InspectorPreviewGUI()
	{
#if CRI_UNITY_EDITOR_PREVIEW
		GUI.enabled = false;
		atomComponent = (CriAtom)EditorGUILayout.ObjectField("CriAtom Object", atomComponent, typeof(CriAtom), true);
		GUI.enabled = (atomComponent != null);
		GUILayout.BeginHorizontal();
		{
			EditorGUILayout.LabelField("Preview", GUILayout.MaxWidth(EditorGUIUtility.labelWidth - 5));
			if (GUILayout.Button("Play", GUILayout.MaxWidth(60))) {
				this.StartPreviewPlayer();
			}
			if (GUILayout.Button("Stop", GUILayout.MaxWidth(60))) {
				this.previewPlayer.Stop();
			}
		}
		GUILayout.EndHorizontal();
		GUI.enabled = true;
#endif
	}

#if CRI_UNITY_EDITOR_PREVIEW
	/* プレビュ用：音声データ設定・再生関数 */
	private void StartPreviewPlayer() {
		if (previewPlayer == null) {
			return;
		}

		if (lastCuesheet != (source as CriAtomSource).cueSheet) {
			if (previewAcb != null) {
				previewAcb.Dispose();
				previewAcb = null;
			}
			foreach (var cuesheet in atomComponent.cueSheets) {
				if (cuesheet.name == (source as CriAtomSource).cueSheet) {
					previewAcb = CriWare.Editor.CriAtomEditorUtilities.LoadAcbFile(null, cuesheet.acbFile, cuesheet.awbFile);
					lastCuesheet = cuesheet.name;
				}
			}
		}
		if (previewAcb != null) {
			previewPlayer.player.SetVolume(this.source.volume);
			previewPlayer.player.SetPitch(this.source.pitch);
			previewPlayer.player.Loop(this.source.loop);
			previewPlayer.Play(previewAcb, (source as CriAtomSource).cueName);
		} else {
			Debug.LogWarning("[CRIWARE] Specified cue sheet could not be found");
		}
	}
#endif

	#endregion
}

} //namespace CriWare

/* end of file */
