using System.Drawing;
using System.Drawing.Imaging;

namespace RayTracing {
	class Sphere {
		public Vector3 center;
		public float radius;
		public Material material;
		/// <summary>
		/// 反射率
		/// </summary>
		public float reflRatio;
		/// <summary>
		/// 折射率
		/// </summary>
		public float refrRatio;

		public Sphere(Vector3 center, float radius, Material material, float reflRatio, float refrRatio) {
			this.center = center;
			this.radius = radius;
			this.material = material;
			this.reflRatio = reflRatio;
			this.refrRatio = refrRatio;
		}

		public Color4 GetColor(Vector3 pos) {
			if (material.Textured) {
				float u = Mathf.Atan((pos.z - center.z) / (pos.x - center.x)) / Mathf.PI + 0.5f;  // 球面投影求贴图uv
				float v = Mathf.Asin((pos.y - center.y) / radius) / Mathf.PI + 0.5f;
				if (float.IsNaN(u) || float.IsNaN(v)) {
					return Color4.zero;
				}
				return material.GetColor(u, v);
			} else {
				return material.Color;
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
