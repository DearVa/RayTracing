using System;

public struct Mathf {
	public static float Sin(float f) {
		return (float)Math.Sin((double)f);
	}

	public static float Cos(float f) {
		return (float)Math.Cos((double)f);
	}

	public static float Tan(float f) {
		return (float)Math.Tan((double)f);
	}

	public static float Asin(float f) {
		return (float)Math.Asin((double)f);
	}

	public static float Acos(float f) {
		return (float)Math.Acos((double)f);
	}

	public static float Atan(float f) {
		return (float)Math.Atan((double)f);
	}

	public static float Atan2(float y, float x) {
		return (float)Math.Atan2((double)y, (double)x);
	}

	public static float Sqrt(float f) {
		return (float)Math.Sqrt((double)f);
	}

	public static float Abs(float f) {
		return Math.Abs(f);
	}

	public static int Abs(int value) {
		return Math.Abs(value);
	}

	public static float Min(float a, float b) {
		return (a < b) ? a : b;
	}

	public static float Min(params float[] values) {
		int num = values.Length;
		bool flag = num == 0;
		float result;
		if (flag) {
			result = 0f;
		} else {
			float num2 = values[0];
			for (int i = 1; i < num; i++) {
				bool flag2 = values[i] < num2;
				if (flag2) {
					num2 = values[i];
				}
			}
			result = num2;
		}
		return result;
	}

	public static int Min(int a, int b) {
		return (a < b) ? a : b;
	}

	public static int Min(params int[] values) {
		int num = values.Length;
		bool flag = num == 0;
		int result;
		if (flag) {
			result = 0;
		} else {
			int num2 = values[0];
			for (int i = 1; i < num; i++) {
				bool flag2 = values[i] < num2;
				if (flag2) {
					num2 = values[i];
				}
			}
			result = num2;
		}
		return result;
	}

	public static float Max(float a, float b) {
		return (a > b) ? a : b;
	}

	public static float Max(params float[] values) {
		int num = values.Length;
		bool flag = num == 0;
		float result;
		if (flag) {
			result = 0f;
		} else {
			float num2 = values[0];
			for (int i = 1; i < num; i++) {
				bool flag2 = values[i] > num2;
				if (flag2) {
					num2 = values[i];
				}
			}
			result = num2;
		}
		return result;
	}

	public static int Max(int a, int b) {
		return (a > b) ? a : b;
	}

	public static int Max(params int[] values) {
		int num = values.Length;
		bool flag = num == 0;
		int result;
		if (flag) {
			result = 0;
		} else {
			int num2 = values[0];
			for (int i = 1; i < num; i++) {
				bool flag2 = values[i] > num2;
				if (flag2) {
					num2 = values[i];
				}
			}
			result = num2;
		}
		return result;
	}

	public static float Pow(float f, float p) {
		return (float)Math.Pow((double)f, (double)p);
	}

	public static float Exp(float power) {
		return (float)Math.Exp((double)power);
	}

	public static float Log(float f, float p) {
		return (float)Math.Log((double)f, (double)p);
	}

	public static float Log(float f) {
		return (float)Math.Log((double)f);
	}

	public static float Log10(float f) {
		return (float)Math.Log10((double)f);
	}

	public static float Ceil(float f) {
		return (float)Math.Ceiling((double)f);
	}

	public static float Floor(float f) {
		return (float)Math.Floor((double)f);
	}

	public static float Round(float f) {
		return (float)Math.Round((double)f);
	}

	public static int CeilToInt(float f) {
		return (int)Math.Ceiling((double)f);
	}

	public static int FloorToInt(float f) {
		return (int)Math.Floor((double)f);
	}

	public static int RoundToInt(float f) {
		return (int)Math.Round((double)f);
	}

	public static float Sign(float f) {
		return (f >= 0f) ? 1f : -1f;
	}

	public static float Clamp(float value, float min, float max) {
		bool flag = value < min;
		if (flag) {
			value = min;
		} else {
			bool flag2 = value > max;
			if (flag2) {
				value = max;
			}
		}
		return value;
	}

	public static int Clamp(int value, int min, int max) {
		bool flag = value < min;
		if (flag) {
			value = min;
		} else {
			bool flag2 = value > max;
			if (flag2) {
				value = max;
			}
		}
		return value;
	}

	public static float Clamp01(float value) {
		bool flag = value < 0f;
		float result;
		if (flag) {
			result = 0f;
		} else {
			bool flag2 = value > 1f;
			if (flag2) {
				result = 1f;
			} else {
				result = value;
			}
		}
		return result;
	}

	public static float Lerp(float a, float b, float t) {
		return a + (b - a) * Mathf.Clamp01(t);
	}

	public static float LerpUnclamped(float a, float b, float t) {
		return a + (b - a) * t;
	}

	public static float LerpAngle(float a, float b, float t) {
		float num = Mathf.Repeat(b - a, 360f);
		bool flag = num > 180f;
		if (flag) {
			num -= 360f;
		}
		return a + num * Mathf.Clamp01(t);
	}

	public static float MoveTowards(float current, float target, float maxDelta) {
		bool flag = Mathf.Abs(target - current) <= maxDelta;
		float result;
		if (flag) {
			result = target;
		} else {
			result = current + Mathf.Sign(target - current) * maxDelta;
		}
		return result;
	}

	public static float MoveTowardsAngle(float current, float target, float maxDelta) {
		float num = Mathf.DeltaAngle(current, target);
		bool flag = -maxDelta < num && num < maxDelta;
		float result;
		if (flag) {
			result = target;
		} else {
			target = current + num;
			result = Mathf.MoveTowards(current, target, maxDelta);
		}
		return result;
	}

	public static float SmoothStep(float from, float to, float t) {
		t = Mathf.Clamp01(t);
		t = -2f * t * t * t + 3f * t * t;
		return to * t + from * (1f - t);
	}

	public static float Gamma(float value, float absmax, float gamma) {
		bool flag = false;
		bool flag2 = value < 0f;
		if (flag2) {
			flag = true;
		}
		float num = Mathf.Abs(value);
		bool flag3 = num > absmax;
		float result;
		if (flag3) {
			result = (flag ? (-num) : num);
		} else {
			float num2 = Mathf.Pow(num / absmax, gamma) * absmax;
			result = (flag ? (-num2) : num2);
		}
		return result;
	}

	public static bool Approximately(float a, float b) {
		return Mathf.Abs(b - a) < Mathf.Max(1E-06f * Mathf.Max(Mathf.Abs(a), Mathf.Abs(b)), Mathf.Epsilon * 8f);
	}

	internal static float Magnitude(int x1, int y1, float x2, float y2) {
		return Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
	}

	public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime) {
		smoothTime = Mathf.Max(0.0001f, smoothTime);
		float num = 2f / smoothTime;
		float num2 = num * deltaTime;
		float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
		float num4 = current - target;
		float num5 = target;
		float num6 = maxSpeed * smoothTime;
		num4 = Mathf.Clamp(num4, -num6, num6);
		target = current - num4;
		float num7 = (currentVelocity + num * num4) * deltaTime;
		currentVelocity = (currentVelocity - num * num7) * num3;
		float num8 = target + (num4 + num7) * num3;
		bool flag = num5 - current > 0f == num8 > num5;
		if (flag) {
			num8 = num5;
			currentVelocity = (num8 - num5) / deltaTime;
		}
		return num8;
	}

	public static float Repeat(float t, float length) {
		return Mathf.Clamp(t - Mathf.Floor(t / length) * length, 0f, length);
	}

	public static float PingPong(float t, float length) {
		t = Mathf.Repeat(t, length * 2f);
		return length - Mathf.Abs(t - length);
	}

	public static float InverseLerp(float a, float b, float value) {
		bool flag = a != b;
		float result;
		if (flag) {
			result = Mathf.Clamp01((value - a) / (b - a));
		} else {
			result = 0f;
		}
		return result;
	}

	public static float DeltaAngle(float current, float target) {
		float num = Mathf.Repeat(target - current, 360f);
		bool flag = num > 180f;
		if (flag) {
			num -= 360f;
		}
		return num;
	}

	internal static long RandomToLong(Random r) {
		byte[] array = new byte[8];
		r.NextBytes(array);
		return (long)(BitConverter.ToUInt64(array, 0) & 9223372036854775807UL);
	}

	public const float PI = 3.14159274f;
	public const float Infinity = float.PositiveInfinity;
	public const float NegativeInfinity = float.NegativeInfinity;
	public const float Deg2Rad = 0.0174532924f;
	public const float Rad2Deg = 57.29578f;
	public static float Epsilon = 2.718281828459f;
}
