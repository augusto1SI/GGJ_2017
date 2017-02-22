﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class UnitAI : Unit {

	//STATE MACHINE
	public enum UnitAIState
	{
		Inert,
		Alive,
		Awake,
		Follow,
		Dead,
		WaitToBeAwokenByElder,
		Morphing,
		Spit,
		MAX
	};

	public UnitAIState m_State;

	public GlobalShit.WaveType m_LastReceivedWave;


	protected static LayerMask worldMask = -1;


	// Use this for initialization
	public override void Start () {
		base.Start ();
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}

}
