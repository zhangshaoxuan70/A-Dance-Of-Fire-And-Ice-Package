using UnityEngine;

public static class SuperGameFeelEffectsExtentions
{
	public static void Shake(this Camera cam)
	{
		Screenshake component = cam.transform.GetComponent<Screenshake>();
		if (component != null)
		{
			component.Shake();
			return;
		}
		UnityEngine.Debug.Log("Camera doesn't have Screenshake component, adding one!");
		cam.transform.gameObject.AddComponent<Screenshake>().Shake();
	}

	public static void Shake(this Camera cam, float multi)
	{
		Screenshake component = cam.transform.GetComponent<Screenshake>();
		if (component != null)
		{
			component.Shake(multi);
			return;
		}
		UnityEngine.Debug.Log("Camera doesn't have Screenshake component, adding one!");
		cam.transform.gameObject.AddComponent<Screenshake>().Shake(multi);
	}

	public static void Kick(this Camera cam, Vector3 dir)
	{
		Kickback component = cam.transform.GetComponent<Kickback>();
		if (component != null)
		{
			component.Kick(dir);
			return;
		}
		UnityEngine.Debug.Log("Camera doesn't have Kickback component, adding one!");
		cam.transform.gameObject.AddComponent<Kickback>().Kick(dir);
	}

	public static void Kick(this Camera cam, Vector3 dir, float multi)
	{
		Kickback component = cam.transform.GetComponent<Kickback>();
		if (component != null)
		{
			component.Kick(dir, multi);
			return;
		}
		UnityEngine.Debug.Log("Camera doesn't have Kickback component, adding one!");
		cam.transform.gameObject.AddComponent<Kickback>().Kick(dir, multi);
	}

	public static void Stop(this Camera cam)
	{
		Hitstop component = cam.transform.GetComponent<Hitstop>();
		if (component != null)
		{
			component.Stop();
			return;
		}
		UnityEngine.Debug.Log("Camera doesn't have Hitstop component, adding one!");
		cam.transform.gameObject.AddComponent<Hitstop>().Stop();
	}

	public static void Stop(this Camera cam, float multi)
	{
		Hitstop component = cam.transform.GetComponent<Hitstop>();
		if (component != null)
		{
			component.Stop(multi);
			return;
		}
		UnityEngine.Debug.Log("Camera doesn't have Hitstop component, adding one!");
		cam.transform.gameObject.AddComponent<Hitstop>().Stop(multi);
	}
}
