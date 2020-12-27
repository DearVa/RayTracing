using System;
using System.Drawing;

namespace RayTracing {
	struct Vector3 {
		[ThreadStatic]
		public static Random rand;
		public float x, y, z;

		public static void InitRand(int offset) {
			rand = new Random((int)DateTime.Now.Ticks + offset);
		}

		public Vector3(float x, float y, float z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector3(float x, float y) {
			this.x = x;
			this.y = y;
			z = 0f;
		}

		public Vector3(Color color) {
			x = color.R;
			y = color.G;
			z = color.B;
		}

		public static Vector3 Scale(Vector3 a, Vector3 b) {
			return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		public void Scale(Vector3 scale) {
			x *= scale.x;
			y *= scale.y;
			z *= scale.z;
		}

		public static Vector3 Cross(Vector3 lhs, Vector3 rhs) {
			return new Vector3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
		}

		public override int GetHashCode() {
			return x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2;
		}

		public override bool Equals(object other) {
			bool flag = !(other is Vector3);
			return !flag && Equals((Vector3)other);
		}

		public bool Equals(Vector3 other) {
			return x == other.x && y == other.y && z == other.z;
		}

		public static Vector3 Reflect(Vector3 inDirection, Vector3 inNormal) {
			float num = -2f * inNormal * inDirection;
			return new Vector3(num * inNormal.x + inDirection.x, num * inNormal.y + inDirection.y, num * inNormal.z + inDirection.z);
		}

		public static bool Refract(Vector3 inDirection, Vector3 inNormal, float refrRatio, out Vector3 refrDir) {
			float cos = inNormal * inDirection;
			float sin2 = 1f - cos * cos;
			float sinr2 = refrRatio * refrRatio * sin2;
			float cosr2 = 1f - sinr2;
			if (cosr2 > 0f) {
				refrDir = (inDirection - inNormal * cos) * refrRatio - inNormal * Mathf.Sqrt(cosr2);
				return true;
			}
			refrDir = zero;
			return false;
		}

		public static Vector3 RandInUnitSphere() {
			float x = (float)(rand.NextDouble() * 2d - 1d);
			float y = (float)(rand.NextDouble() * 2d - 1d);
			float z = (float)(rand.NextDouble() * 2d - 1d);
			return new Vector3(x, y, z).normalized;
		}

		public static Vector3 RandInUnitHemisphere(Vector3 normal) {
			float r1 = (float)(2 * Mathf.PI * rand.NextDouble());
			float r2 = (float)rand.NextDouble(), r2s = Mathf.Sqrt(r2);
			Vector3 u;
			if (Mathf.Abs(normal.x) > 0.1f) {
				u = Cross(up, normal).normalized;
			} else {
				u = Cross(one, normal).normalized;
			}
			Vector3 v = Cross(normal, u);
			return (u * Mathf.Cos(r1) * r2s + v * Mathf.Sin(r1) * r2s + normal * Mathf.Sqrt(1 - r2)).normalized;
		}

		public static Vector3 Normalize(Vector3 value) {
			float num = Magnitude(value);
			bool flag = num > 1E-05f;
			Vector3 result;
			if (flag) {
				result = value / num;
			} else {
				result = zero;
			}
			return result;
		}

		public void Normalize() {
			float num = Magnitude(this);
			bool flag = num > 1E-05f;
			if (flag) {
				this /= num;
			} else {
				this = zero;
			}
		}

		public Vector3 normalized {
			get {
				return Normalize(this);
			}
		}

		public static float Dot(Vector3 lhs, Vector3 rhs) {
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
		}

		public static Vector3 Project(Vector3 vector, Vector3 onNormal) {
			float num = Dot(onNormal, onNormal);
			bool flag = num < Mathf.Epsilon;
			Vector3 result;
			if (flag) {
				result = zero;
			} else {
				float num2 = Dot(vector, onNormal);
				result = new Vector3(onNormal.x * num2 / num, onNormal.y * num2 / num, onNormal.z * num2 / num);
			}
			return result;
		}

		public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal) {
			float num = Dot(planeNormal, planeNormal);
			bool flag = num < Mathf.Epsilon;
			Vector3 result;
			if (flag) {
				result = vector;
			} else {
				float num2 = Dot(vector, planeNormal);
				result = new Vector3(vector.x - planeNormal.x * num2 / num, vector.y - planeNormal.y * num2 / num, vector.z - planeNormal.z * num2 / num);
			}
			return result;
		}

		public static float Angle(Vector3 from, Vector3 to) {
			float num = (float)Math.Sqrt((double)(from.sqrMagnitude * to.sqrMagnitude));
			bool flag = num < 1E-15f;
			float result;
			if (flag) {
				result = 0f;
			} else {
				float num2 = Mathf.Clamp(Dot(from, to) / num, -1f, 1f);
				result = (float)Math.Acos((double)num2) * 57.29578f;
			}
			return result;
		}

		public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis) {
			float num = Angle(from, to);
			float num2 = from.y * to.z - from.z * to.y;
			float num3 = from.z * to.x - from.x * to.z;
			float num4 = from.x * to.y - from.y * to.x;
			float num5 = Mathf.Sign(axis.x * num2 + axis.y * num3 + axis.z * num4);
			return num * num5;
		}

		public static float Distance(Vector3 a, Vector3 b) {
			float num = a.x - b.x;
			float num2 = a.y - b.y;
			float num3 = a.z - b.z;
			return (float)Math.Sqrt((double)(num * num + num2 * num2 + num3 * num3));
		}

		public static Vector3 ClampMagnitude(Vector3 vector, float maxLength) {
			float sqrMagnitude = vector.sqrMagnitude;
			bool flag = sqrMagnitude > maxLength * maxLength;
			Vector3 result;
			if (flag) {
				float num = (float)Math.Sqrt((double)sqrMagnitude);
				float num2 = vector.x / num;
				float num3 = vector.y / num;
				float num4 = vector.z / num;
				result = new Vector3(num2 * maxLength, num3 * maxLength, num4 * maxLength);
			} else {
				result = vector;
			}
			return result;
		}

		public static float Magnitude(Vector3 vector) {
			return (float)Math.Sqrt((double)(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z));
		}

		public float magnitude {
			get {
				return (float)Math.Sqrt((double)(x * x + y * y + z * z));
			}
		}

		public static float SqrMagnitude(Vector3 vector) {
			return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
		}

		public float sqrMagnitude {
			get {
				return x * x + y * y + z * z;
			}
		}

		public static Vector3 Min(Vector3 lhs, Vector3 rhs) {
			return new Vector3(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z));
		}

		public static Vector3 Max(Vector3 lhs, Vector3 rhs) {
			return new Vector3(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z));
		}

		public static Vector3 zero {
			get {
				return zeroVector;
			}
		}

		public static Vector3 one {
			get {
				return oneVector;
			}
		}

		public static Vector3 forward {
			get {
				return forwardVector;
			}
		}

		public static Vector3 back {
			get {
				return backVector;
			}
		}

		public static Vector3 up {
			get {
				return upVector;
			}
		}

		public static Vector3 down {
			get {
				return downVector;
			}
		}

		public static Vector3 left {
			get {
				return leftVector;
			}
		}

		public static Vector3 right {
			get {
				return rightVector;
			}
		}

		public static Vector3 positiveInfinity {
			get {
				return positiveInfinityVector;
			}
		}

		public static Vector3 negativeInfinity {
			get {
				return negativeInfinityVector;
			}
		}

		public static Vector3 operator +(Vector3 a, Vector3 b) {
			return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		public static Vector3 operator -(Vector3 a, Vector3 b) {
			return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public static Vector3 operator -(Vector3 a) {
			return new Vector3(-a.x, -a.y, -a.z);
		}

		public static Vector3 operator *(Vector3 a, float d) {
			return new Vector3(a.x * d, a.y * d, a.z * d);
		}

		public static float operator *(Vector3 a, Vector3 b) {
			return Dot(a, b);
		}

		public static Vector3 operator *(float d, Vector3 a) {
			return new Vector3(a.x * d, a.y * d, a.z * d);
		}

		public static Vector3 operator /(Vector3 a, float d) {
			return new Vector3(a.x / d, a.y / d, a.z / d);
		}

		public static bool operator ==(Vector3 lhs, Vector3 rhs) {
			float num = lhs.x - rhs.x;
			float num2 = lhs.y - rhs.y;
			float num3 = lhs.z - rhs.z;
			float num4 = num * num + num2 * num2 + num3 * num3;
			return num4 < 9.99999944E-11f;
		}

		public static bool operator !=(Vector3 lhs, Vector3 rhs) {
			return !(lhs == rhs);
		}

		public override string ToString() {
			return string.Format("({0:F1}, {1:F1}, {2:F1})", new object[] { x, y, z });
		}

		public const float kEpsilon = 1E-05f;
		public const float kEpsilonNormalSqrt = 1E-15f;
		private static readonly Vector3 zeroVector = new Vector3(0f, 0f, 0f);
		private static readonly Vector3 oneVector = new Vector3(1f, 1f, 1f);
		private static readonly Vector3 upVector = new Vector3(0f, 1f, 0f);
		private static readonly Vector3 downVector = new Vector3(0f, -1f, 0f);
		private static readonly Vector3 leftVector = new Vector3(-1f, 0f, 0f);
		private static readonly Vector3 rightVector = new Vector3(1f, 0f, 0f);
		private static readonly Vector3 forwardVector = new Vector3(0f, 0f, 1f);
		private static readonly Vector3 backVector = new Vector3(0f, 0f, -1f);
		private static readonly Vector3 positiveInfinityVector = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
		private static readonly Vector3 negativeInfinityVector = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

	}
}
