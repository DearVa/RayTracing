using System;
using System.Globalization;

namespace RayTracing {
	public struct Vector2 : IEquatable<Vector2> {
		public float this[int index] {
			get {
				float result;
				if (index != 0) {
					if (index != 1) {
						throw new IndexOutOfRangeException("Invalid Vector2 index!");
					}
					result = this.y;
				} else {
					result = this.x;
				}
				return result;
			}
			set {
				if (index != 0) {
					if (index != 1) {
						throw new IndexOutOfRangeException("Invalid Vector2 index!");
					}
					this.y = value;
				} else {
					this.x = value;
				}
			}
		}

		public Vector2(float x, float y) {
			this.x = x;
			this.y = y;
		}

		public void Set(float newX, float newY) {
			this.x = newX;
			this.y = newY;
		}

		public static Vector2 Lerp(Vector2 a, Vector2 b, float t) {
			t = Mathf.Clamp01(t);
			return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
		}

		public static Vector2 LerpUnclamped(Vector2 a, Vector2 b, float t) {
			return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
		}

		public static Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDistanceDelta) {
			float num = target.x - current.x;
			float num2 = target.y - current.y;
			float num3 = num * num + num2 * num2;
			bool flag = num3 == 0f || (maxDistanceDelta >= 0f && num3 <= maxDistanceDelta * maxDistanceDelta);
			Vector2 result;
			if (flag) {
				result = target;
			} else {
				float num4 = (float)Math.Sqrt((double)num3);
				result = new Vector2(current.x + num / num4 * maxDistanceDelta, current.y + num2 / num4 * maxDistanceDelta);
			}
			return result;
		}

		public static Vector2 Scale(Vector2 a, Vector2 b) {
			return new Vector2(a.x * b.x, a.y * b.y);
		}

		public void Scale(Vector2 scale) {
			this.x *= scale.x;
			this.y *= scale.y;
		}

		public void Normalize() {
			float magnitude = this.magnitude;
			bool flag = magnitude > 1E-05f;
			if (flag) {
				this /= magnitude;
			} else {
				this = Vector2.zero;
			}
		}

		public Vector2 normalized {
			get {
				Vector2 result = new Vector2(this.x, this.y);
				result.Normalize();
				return result;
			}
		}

		public override string ToString() {
			return string.Format("({0:F1}, {1:F1})", new object[]
			{
				this.x,
				this.y
			});
		}

		public string ToString(string format) {
			return string.Format("({0}, {1})", new object[]
			{
				this.x.ToString(format, CultureInfo.InvariantCulture.NumberFormat),
				this.y.ToString(format, CultureInfo.InvariantCulture.NumberFormat)
			});
		}

		public override int GetHashCode() {
			return this.x.GetHashCode() ^ this.y.GetHashCode() << 2;
		}

		public override bool Equals(object other) {
			bool flag = !(other is Vector2);
			return !flag && this.Equals((Vector2)other);
		}

		public bool Equals(Vector2 other) {
			return this.x == other.x && this.y == other.y;
		}

		public static Vector2 Reflect(Vector2 inDirection, Vector2 inNormal) {
			float num = -2f * Vector2.Dot(inNormal, inDirection);
			return new Vector2(num * inNormal.x + inDirection.x, num * inNormal.y + inDirection.y);
		}

		public static Vector2 Perpendicular(Vector2 inDirection) {
			return new Vector2(-inDirection.y, inDirection.x);
		}

		public static float Dot(Vector2 lhs, Vector2 rhs) {
			return lhs.x * rhs.x + lhs.y * rhs.y;
		}

		public float magnitude {
			get {
				return (float)Math.Sqrt((double)(this.x * this.x + this.y * this.y));
			}
		}

		public float sqrMagnitude {
			get {
				return this.x * this.x + this.y * this.y;
			}
		}

		public static float Angle(Vector2 from, Vector2 to) {
			float num = (float)Math.Sqrt((double)(from.sqrMagnitude * to.sqrMagnitude));
			bool flag = num < 1E-15f;
			float result;
			if (flag) {
				result = 0f;
			} else {
				float num2 = Mathf.Clamp(Vector2.Dot(from, to) / num, -1f, 1f);
				result = (float)Math.Acos((double)num2) * 57.29578f;
			}
			return result;
		}

		public static float SignedAngle(Vector2 from, Vector2 to) {
			float num = Vector2.Angle(from, to);
			float num2 = Mathf.Sign(from.x * to.y - from.y * to.x);
			return num * num2;
		}

		public static float Distance(Vector2 a, Vector2 b) {
			float num = a.x - b.x;
			float num2 = a.y - b.y;
			return (float)Math.Sqrt((double)(num * num + num2 * num2));
		}

		public static Vector2 ClampMagnitude(Vector2 vector, float maxLength) {
			float sqrMagnitude = vector.sqrMagnitude;
			bool flag = sqrMagnitude > maxLength * maxLength;
			Vector2 result;
			if (flag) {
				float num = (float)Math.Sqrt((double)sqrMagnitude);
				float num2 = vector.x / num;
				float num3 = vector.y / num;
				result = new Vector2(num2 * maxLength, num3 * maxLength);
			} else {
				result = vector;
			}
			return result;
		}

		public static float SqrMagnitude(Vector2 a) {
			return a.x * a.x + a.y * a.y;
		}

		public float SqrMagnitude() {
			return this.x * this.x + this.y * this.y;
		}

		public static Vector2 Min(Vector2 lhs, Vector2 rhs) {
			return new Vector2(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y));
		}

		public static Vector2 Max(Vector2 lhs, Vector2 rhs) {
			return new Vector2(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y));
		}

		public static Vector2 operator +(Vector2 a, Vector2 b) {
			return new Vector2(a.x + b.x, a.y + b.y);
		}

		public static Vector2 operator -(Vector2 a, Vector2 b) {
			return new Vector2(a.x - b.x, a.y - b.y);
		}

		public static Vector2 operator *(Vector2 a, Vector2 b) {
			return new Vector2(a.x * b.x, a.y * b.y);
		}

		public static Vector2 operator /(Vector2 a, Vector2 b) {
			return new Vector2(a.x / b.x, a.y / b.y);
		}

		public static Vector2 operator -(Vector2 a) {
			return new Vector2(-a.x, -a.y);
		}

		public static Vector2 operator *(Vector2 a, float d) {
			return new Vector2(a.x * d, a.y * d);
		}

		public static Vector2 operator *(float d, Vector2 a) {
			return new Vector2(a.x * d, a.y * d);
		}

		public static Vector2 operator /(Vector2 a, float d) {
			return new Vector2(a.x / d, a.y / d);
		}

		public static bool operator ==(Vector2 lhs, Vector2 rhs) {
			float num = lhs.x - rhs.x;
			float num2 = lhs.y - rhs.y;
			return num * num + num2 * num2 < 9.99999944E-11f;
		}

		public static bool operator !=(Vector2 lhs, Vector2 rhs) {
			return !(lhs == rhs);
		}

		public static implicit operator Vector2(Vector3 v) {
			return new Vector2(v.x, v.y);
		}

		public static implicit operator Vector3(Vector2 v) {
			return new Vector3(v.x, v.y, 0f);
		}

		public static Vector2 zero {
			get {
				return Vector2.zeroVector;
			}
		}

		public static Vector2 one {
			get {
				return Vector2.oneVector;
			}
		}

		public static Vector2 up {
			get {
				return Vector2.upVector;
			}
		}

		public static Vector2 down {
			get {
				return Vector2.downVector;
			}
		}

		public static Vector2 left {
			get {
				return Vector2.leftVector;
			}
		}

		public static Vector2 right {
			get {
				return Vector2.rightVector;
			}
		}

		public static Vector2 positiveInfinity {
			get {
				return Vector2.positiveInfinityVector;
			}
		}

		public static Vector2 negativeInfinity {
			get {
				return Vector2.negativeInfinityVector;
			}
		}

		public float x;

		public float y;

		private static readonly Vector2 zeroVector = new Vector2(0f, 0f);

		private static readonly Vector2 oneVector = new Vector2(1f, 1f);

		private static readonly Vector2 upVector = new Vector2(0f, 1f);

		private static readonly Vector2 downVector = new Vector2(0f, -1f);

		private static readonly Vector2 leftVector = new Vector2(-1f, 0f);

		private static readonly Vector2 rightVector = new Vector2(1f, 0f);

		private static readonly Vector2 positiveInfinityVector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

		private static readonly Vector2 negativeInfinityVector = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

		public const float kEpsilon = 1E-05f;

		public const float kEpsilonNormalSqrt = 1E-15f;
	}
}
