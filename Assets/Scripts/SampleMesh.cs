using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SampleMesh : MonoBehaviour
{
	private const float pointSize = 0.05f;

	public int circleSides = 20;

	public float outerRadius = 1f;

	public float innerRadius = 0.4f;

	private MeshFilter meshFilter;

	private List<Vector3> points = new List<Vector3>();

	private void Awake()
	{
		meshFilter = GetComponent<MeshFilter>();
	}

	private void Update()
	{
		points.Clear();
		_003CUpdate_003Eg__DrawCircle_007C7_0(innerRadius);
		_003CUpdate_003Eg__DrawCircle_007C7_0(outerRadius);
		int[] array = new int[circleSides * 2 * 3];
		for (int i = 0; i < circleSides; i++)
		{
			int num = circleSides;
			array[i * 6] = i;
			array[i * 6 + 2] = num + i;
			array[i * 6 + 1] = (num + i + 1) % circleSides + circleSides;
			array[i * 6 + 3] = i;
			array[i * 6 + 5] = (num + i + 1) % circleSides + circleSides;
			array[i * 6 + 4] = (i + 1) % circleSides;
		}
		string text = "";
		int[] array2 = array;
		for (int j = 0; j < array2.Length; j++)
		{
			int num2 = array2[j];
			text = text + num2.ToString() + ",";
		}
		if (Time.frameCount == 5)
		{
			UnityEngine.Debug.Log(text);
		}
		Mesh mesh = new Mesh();
		mesh.SetVertices(points);
		mesh.SetIndices(array, MeshTopology.Triangles, 0);
		List<Vector2> list = new List<Vector2>();
		for (int k = 0; k < points.Count; k++)
		{
			if (k < points.Count / 2)
			{
				Vector2 item = new Vector2(0.5f, 0f);
				list.Add(item);
			}
			else
			{
				Vector2 item2 = new Vector2(0.5f, 1f);
				list.Add(item2);
			}
		}
		mesh.uv = list.ToArray();
		meshFilter.mesh = mesh;
	}

	[CompilerGenerated]
	private void _003CUpdate_003Eg__DrawCircle_007C7_0(float radius)
	{
		for (int i = 0; i < circleSides; i++)
		{
			float f = (float)i * 1f / (float)circleSides * (MathF.PI * 2f);
			Vector2 v = new Vector2(Mathf.Cos(f) * radius, Mathf.Sin(f) * radius);
			points.Add(v);
		}
	}
}
