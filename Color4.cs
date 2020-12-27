using System.Drawing;

namespace RayTracing {
	struct Color4 {
		public float R, G, B, L;

		public Color4(float r, float g, float b) {
			R = r;
			G = g;
			B = b;
			L = 0;
		}

		public Color4(float r, float g, float b, float l) {
			R = r;
			G = g;
			B = b;
			L = l;
		}

		public static explicit operator Color4(Color c) {
			return new Color4(c.R, c.G, c.B, 0);
		}

		public static explicit operator Color(Color4 c) {
			float l = Mathf.Min(c.L, 1f);  // 先把亮度限制在1以内
			c.R *= l;
			c.G *= l;
			c.B *= l;
			int r, g, b;
			r = c.R > 255f ? 255 : (int)c.R;
			g = c.G > 255f ? 255 : (int)c.G;
			b = c.B > 255f ? 255 : (int)c.B;
			return Color.FromArgb(r, g, b);
		}

		public static Color4 operator +(Color4 a, Color4 b) {
			return new Color4(a.R + b.R, a.G + b.G, a.B + b.B, a.L + b.L);
		}

		public static Color4 operator *(Color4 c, float f) {
			return new Color4(c.R * f, c.G * f, c.B * f, c.L * f);
		}

		public static Color4 operator /(Color4 c, float f) {
			return new Color4(c.R / f, c.G / f, c.B / f, c.L / f);
		}

		public static Color4 zero = new Color4(0, 0, 0, 0);
	}
}
