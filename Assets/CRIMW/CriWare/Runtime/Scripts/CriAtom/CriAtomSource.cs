﻿/****************************************************************************
 *
 * Copyright (c) 2011 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

using UnityEngine;
using System;
using System.Collections;
using System.Security.AccessControl;

/**
 * \addtogroup CRIATOM_UNITY_COMPONENT
 * @{
 */

namespace CriWare {

/**
 * <summary>A component that plays sound.</summary>
 * <remarks>
 * <para header='Description'>Used by attaching to any GameObject.<br/>
 * If the Cue to be played is set to do 3D Positioning, 3D playback is performed.
 * In this case, it is necessary to attach CriWare.CriAtomListener to the camera or main character
 * since the localization is calculated with the position of the GameObject to which CriWare.CriAtomListener is attached.<br/>
 * Public variables are basically set on UnityEditor.</para>
 * </remarks>
 */
[AddComponentMenu("CRIWARE/CRI Atom Source")]
public class CriAtomSource : CriAtomSourceBase
{
	#region Variables
	[SerializeField]
	private string _cueName = "";
	[SerializeField]
	private string _cueSheet = "";
	#endregion

	#region Properties
	/**
	 * <summary>Sets/gets the name of the Cue to be played.</summary>
	 * <remarks>
	 * <para header='Description'>When you call the CriAtomSource::Play() function or
	 * when you play sound at the start of execution by setting the CriAtomSource::playOnStart property,
	 * the Cue set by this property is played.</para>
	 * </remarks>
	 */
	public string cueName {
		get {return this._cueName;}
		set {this._cueName = value;}
	}

	/**
	 * <summary>Sets/gets the Cue Sheet name.</summary>
	 * <remarks>
	 * <para header='Description'>The Cue specified in the CriAtomSource::Play function or CriWare.CriAtomSource.cueName property
	 * is searched from the Cue Sheet set in this property.</para>
	 * </remarks>
	 */
	public string cueSheet {
		get {return this._cueSheet;}
		set {this._cueSheet = value;}
	}
	#endregion

	#region Functions

	/**
	 * <summary>Starts playing the Cue that is set.</summary>
	 * <returns>Playback ID</returns>
	 * <remarks>
	 * <para header='Description'>The Cue to be played must be set in advance to the
	 * CriWare.CriAtomSource.cueName property.</para>
	 * </remarks>
	 */
	public override CriAtomExPlayback Play()
	{
		return this.Play(this.cueName);
        }
        private CriAtomExAcb custom_acb;
        public void Init()
        {
            if (custom_acb != null) custom_acb.Dispose();
            custom_acb = CriAtom.GetAcb(this.cueSheet);
            this.player.SetCue(custom_acb, cueName);

            if (this.hasValidPosition == false)
            {
                this.SetInitialSourcePosition();
                this.hasValidPosition = true;
            }

#if !UNITY_EDITOR && UNITY_ANDROID
        this.player.SetSoundRendererType(CriAtomEx.androidDefaultSoundRendererType);
#endif
        }
        public void PlayDirectly()
        {
            //if (this.status == Status.Stop)
            //	this.player.Loop(this._loop);

            this.player.Play();
        }
        protected override CriAtomExAcb GetAcb()
        {
            CriAtomExAcb acb = null;
            if (!String.IsNullOrEmpty(this.cueSheet))
            {
                acb = CriAtom.GetAcb(this.cueSheet);
            }
            return acb;
        }

        /**
         * <summary>Starts playing the Cue that is set.</summary>
         * <remarks>
         * <para header='Description'>You must set the CriAtomSource::playOnStart, CriWare.CriAtomSource.cueName
         * properties in advance.</para>
         * </remarks>
         */
        protected override void PlayOnStart()
	{
		if (this.playOnStart && !String.IsNullOrEmpty(this.cueName)) {
			StartCoroutine(PlayAsync(this.cueName));
		}
	}

	/**
	 * <summary>Asynchronously starts playing the Cue with the specified Cue name.</summary>
	 * <param name='cueName'>Cue name</param>
	 * <returns>Coroutine</returns>
	 * <remarks>
	 * <para header='Description'>It is executed asynchronously by using the coroutine function of Unity.
	 * Call this function by specifying it as an argument of MonoBehaviour::StartCoroutine.</para>
	 * </remarks>
	 */
	private IEnumerator PlayAsync(string cueName)
	{
		CriAtomExAcb acb = null;
		while (acb == null && !String.IsNullOrEmpty(this.cueSheet)) {
			acb = CriAtom.GetAcb(this.cueSheet);
			if (acb == null) {
				yield return null;
			}
		}
		this.player.SetCue(acb, cueName);
		InternalPlayCue();
	}

	#endregion
}

} //namespace CriWare
/** @} */
/* end of file */
