using System.Linq;
using UnityEngine;

public class scrFailBar : ADOBase
{
	public bool isactive = true;

	public bool activateBar;

	public float overloadCounter;

	private float overloadDamagePerMiss = 0.5f;

	private float overloadCooldown = 0.4f;

	public float multipressCounter;

	public float multipressCooldownResetCounter;

	private float multipressCooldown = 0.2f;

	private float multipressDamage = 0.35f;

	private float multipressResetLimit = 6f;

	private new scrConductor conductor;

	private new scrController controller;

	private void Start()
	{
		Timer.Add(delegate
		{
			activateBar = true;
		}, 1f);
		overloadCounter = 0f;
		multipressCounter = 0f;
		if (conductor == null)
		{
			conductor = GameObject.Find("Conductor").GetComponent<scrConductor>();
		}
		if (controller == null)
		{
			controller = GameObject.Find("Controller").GetComponent<scrController>();
		}
	}

	public void Rewind()
	{
		overloadCounter = 0f;
		multipressCounter = 0f;
		activateBar = false;
		overloadCooldown = 0.4f;
		Timer.Add(delegate
		{
			activateBar = true;
		}, 1f);
	}

	public void SetDamageVal(float val)
	{
		overloadDamagePerMiss = val;
	}

	public void SetCooldownVal(float val)
	{
		overloadCooldown = val;
	}

	private void Update()
	{
		if (isactive && activateBar)
		{
			if (DidFail())
			{
				controller.FailAction(overload: true, multipressCounter > 1f);
				overloadCounter = Mathf.Min(overloadCounter, 0.5f);
				multipressCounter = Mathf.Min(multipressCounter, 0.5f);
			}
			overloadCounter -= (float)((double)overloadCooldown * conductor.deltaSongPos / conductor.crotchet);
			overloadCounter = Mathf.Max(overloadCounter, 0f);
			multipressCounter -= (float)((double)multipressCooldown * conductor.deltaSongPos / conductor.crotchet);
			multipressCounter = Mathf.Max(multipressCounter, 0f);
			multipressCooldownResetCounter += (float)(1.0 * conductor.deltaSongPos / conductor.crotchet);
			if (multipressCooldownResetCounter > multipressResetLimit)
			{
				multipressCounter = 0f;
				multipressCooldownResetCounter = 0f;
			}
		}
	}

	public bool Damage(bool multipress = false)
	{
		if (!multipress)
		{
			if (!GCS.d_drumcontroller)
			{
				overloadCounter += overloadDamagePerMiss;
			}
			else
			{
				overloadCounter += overloadDamagePerMiss / 1.7f;
			}
		}
		else
		{
			multipressCounter += multipressDamage;
			multipressCooldownResetCounter = 0f;
		}
		return DidFail();
	}

	public bool DidFail(bool checkForDuplicateDeath = true)
	{
		if (checkForDuplicateDeath && scrController.deathStates.Contains(controller.state))
		{
			return false;
		}
		if ((overloadCounter > 1f || multipressCounter > 1f) && (ADOBase.isLevelEditor || (controller.gameworld && controller.percentComplete < 0.96f)))
		{
			return true;
		}
		return false;
	}

	public void Damage(float newval)
	{
		if (!GCS.d_drumcontroller)
		{
			overloadCounter += newval;
		}
		else
		{
			overloadCounter += newval / 1.7f;
		}
	}
}
