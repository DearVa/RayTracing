using System.Drawing;

namespace RayTracing {
	enum Surface {
		COLOR,
		EMISSION,
		DIFFUSE,
		SPEC
	}

	class Sphere {
		public Vector3 center;
		public float radius;
		public Color4 color;
		public Surface surface;
		/// <summary>
		/// 反射率
		/// </summary>
		public float refl;
		/// <summary>
		/// 折射率
		/// </summary>
		public float refr;

		public Sphere(Vector3 center, float radius, Color color, Surface surface, float refl, float refr, float emis) {
			this.center = center;
			this.radius = radius;
			this.color = (Color4)color;
			this.surface = surface;
			this.refl = refl;
			this.refr = refr;
			if (surface == Surface.EMISSION) {
				this.color.L = emis;  // 发光强度
			}
		}

		public float Intersect(Ray ray) {
			Vector3 l = center - ray.origin;
			float tca = l * ray.dir;
			float d2 = l * l - tca * tca;
			float radius2 = radius * radius;
			if (d2 > radius2) {
				return -1f;
			}
			float thc = Mathf.Sqrt(radius2 - d2);
			if (tca - thc < 0) {
				return tca + thc;
			}
			return tca - thc;
		}
	}
}
