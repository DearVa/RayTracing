using System.Drawing;

namespace RayTracing {
	enum Surface {
		EMISSION,
		DIFFUSE,
		SPECULAR
	}

	class Material {
		public Surface surface;
		public float tillX, tillY;
		public Color4 color;
		public Texture texture;
		public bool Textured { get; private set; }

		public Material(Surface surface, Color color, float emission) {
			this.surface = surface;
			this.color = (Color4)color;
			this.color.L = emission;
			Textured = false;
		}

		public Material(Surface surface, Color4 color) {
			this.surface = surface;
			this.color = color;
			Textured = false;
		}

		public Material(Surface surface, Texture texture, float tillX, float tillY) {
			this.surface = surface;
			this.texture = texture;
			this.tillX = tillX;
			this.tillY = tillY;
			Textured = true;
		}

		public Color4 GetColor(float u, float v) {
			u = Mathf.Max(Mathf.Min(u, 1f), 0f);
			v = Mathf.Max(Mathf.Min(v, 1f), 0f);
			int x = (int)(u * texture.width * tillX) % texture.width;
			int y = (int)(v * texture.height * tillY) % texture.height;
			int d = (y * texture.width + x) * 3;
			unsafe {
				return new Color4(texture.ptr[d + 2], texture.ptr[d + 1], texture.ptr[d], color.L);
			}
		}

		public Color4 GetColor(Vector2 uv) {
			return GetColor(uv.x, uv.y);
		}
	}
}
