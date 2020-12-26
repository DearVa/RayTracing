using System.Drawing;
using System.Drawing.Imaging;

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
		public Surface surface;
		public unsafe byte* ptr;
		/// <summary>
		/// 反射率
		/// </summary>
		public float refl;
		/// <summary>
		/// 折射率
		/// </summary>
		public float refr;

		private float tillX, tillY;
		private readonly int width, height;
		private readonly BitmapData bitmapData;
		private Color4 color;

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

		public Sphere(Vector3 center, float radius, Bitmap tex, float tillX, float tillY, Surface surface, float refl, float refr, float emis) {
			this.center = center;
			this.radius = radius;
			width = tex.Width;
			height = tex.Height;
			this.tillX = tillX;
			this.tillY = tillY;
			bitmapData = tex.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
			unsafe {
				ptr = (byte*)bitmapData.Scan0.ToPointer();
			}
			this.surface = surface;
			this.refl = refl;
			this.refr = refr;
			if (surface == Surface.EMISSION) {
				color.L = emis;  // 发光强度
			}
		}

		public Color4 GetColor(Vector3 pos) {
			if (bitmapData == null) {
				return color;
			} else {
				float u = Mathf.Atan((pos.z - center.z) / (pos.x - center.x)) / 2f / Mathf.PI;  // 球面投影求贴图uv
				float v = Mathf.Asin((pos.y - center.y) / radius) / Mathf.PI + 0.5f;
				int x = (int)Mathf.Abs(u * width * tillX) % width;
				int y = (int)Mathf.Abs(v * height * tillY) % height;
				int d = (y * width + x) * 3;
				unsafe {
					return new Color4(ptr[d + 2], ptr[d + 1], ptr[d], color.L);
				}
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
