using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


public class PerlinNoise
{
	private static readonly float FmSqrt2 = Mathf.Sqrt(2);

	private const int B = 0x100;

	private const int BM = 0xFF;

	private const int N = 0x1000;

	private readonly float _mu;

	private readonly int _octaves;

	private readonly int[] _p = new int[B];

	private readonly Vector2[] _g2 = new Vector2[B];

	private readonly float[] _m = new float[B];

	private readonly Random _random;

	public PerlinNoise(float mu, int octaves, int seed)
	{
		_mu = mu;

		_octaves = octaves;

		_random = new Random(seed);
	}

	private static float Curve(float t)
	{
		return t * t * (3.0f - 2.0f * t);
	}

	private static float Dot(float rx, float ry, Vector2 q)
	{
		return rx * q[0] + ry * q[1];
	}

	private int Random()
	{
		return _random.Next(32767);
	}

	private void Normalize(Vector2 v)
	{
		var s = Mathf.Sqrt(v[0] * v[0] + v[1] * v[1]);

		v[0] /= s;
		v[1] /= s;
	}

	private float RandomFloat()
	{
		return 2.0F * (float)_random.NextDouble() - 1.0F;
	}

	public void Initialize()
	{
		for (var i = 0; i < _g2.Length; ++i)
		{
			_g2[i] = new Vector2(0, 0);
		}

		for (var i = 0; i < B; i++)
		{
			_g2[i][0] = RandomFloat();
			_g2[i][1] = RandomFloat();

			Normalize(_g2[i]);
		}

		for (var i = 0; i < B; i++)
		{
			_p[i] = i;
		}

		for (var i = B - 1; i > 0; i--)
		{
			var i1 = i;
			var i2 = Random() % (i + 1);

			var t = _p[i];
			_p[i1] = _p[i2];
			_p[i2] = t;
		}

		var s = 1.0F;

		for (var i = 0; i < B; i++)
		{
			_m[i] = s;

			s /= _mu;
		}
	}

	private float GetNoise(Vector2 vec)
	{
		Vector2 q;

		var t0 = vec[0] + N;

		var bx0 = (int)t0 & BM;
		var bx1 = (bx0 + 1) & BM;

		var rx0 = t0 - (int)t0;
		var rx1 = rx0 - 1.0F;

		var t1 = vec[1] + N;

		var by0 = (int)t1 & BM;
		var by1 = (by0 + 1) & BM;

		var ry0 = t1 - (int)t1;
		var ry1 = ry0 - 1.0F;

		var b00 = _p[(_p[bx0] + by0) & BM];
		var b10 = _p[(_p[bx1] + by0) & BM];
		var b01 = _p[(_p[bx0] + by1) & BM];
		var b11 = _p[(_p[bx1] + by1) & BM];

		var sx = Curve(rx0);

		float u;
		float v;

		q = _g2[b00];
		u = _m[b00] * Dot(rx0, ry0, q);
		q = _g2[b10];
		v = _m[b10] * Dot(rx1, ry0, q);

		var a = Mathf.Lerp(u, v, sx);

		q = _g2[b01];
		u = _m[b01] * Dot(rx0, ry1, q);
		q = _g2[b11];
		v = _m[b11] * Dot(rx1, ry1, q);

		var b = Mathf.Lerp(u, v, sx);

		var sy = Curve(ry0);

		return Mathf.Lerp(a, b, sy);
	}

	public float GetNoises(Vector2 position)
	{
		var result = 0.0F;
		var scale = 1.0F;

		for (var i = 0; i < _octaves; i++)
		{
			scale *= 0.5F;

			result += GetNoise(position) * scale;

			position[0] *= 2.0F;
			position[1] *= 2.0F;
		}

		return FmSqrt2 * result / (1.0F - scale);
	}
}