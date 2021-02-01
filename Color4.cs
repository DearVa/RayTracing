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

		//public static explicit operator Color(Color4 c4) {
		//	float max = Mathf.Max(c4.R, c4.G, c4.B);
		//	float min = Mathf.Min(c4.R, c4.G, c4.B);
		//	float S = (max - min) / max;
		//	float H;
		//	if (c4.R == max) {
		//		H = (c4.G - c4.B) / (max - min) * 60;
		//	} else if (c4.G == max) {
		//		H = 120 + (c4.B - c4.R) / (max - min) * 60;
		//	} else {
		//		H = 240 + (c4.R - c4.G) / (max - min) * 60;
		//	}
		//	if (H < 0) {
		//		H += 360;
		//	}
		//	float V = Mathf.Min(c4.L, 1f);
		//	float R, G, B;
		//	if (S == 0) {
		//		R = G = B = V;
		//	} else {
		//		H /= 60;
		//		int i = (int)H;
		//		float f = H - i;
		//		float a = V * (1 - S);
		//		float b = V * (1 - S * f);
		//		float c = V * (1 - S * (1 - f));
		//		if (i == 0) {
		//			R = V; G = c; B = a;
		//		} else if (i == 1) {
		//			R = b; G = V; B = a;
		//		} else if (i == 2) {
		//			R = a; G = V; B = c;
		//		} else if (i == 3) {
		//			R = a; G = b; B = V;
		//		} else if (i == 4) {
		//			R = c; G = a; B = V;
		//		} else {
		//			R = V; G = a; B = b;
		//		}
		//	}
		//	return Color.FromArgb((int)(255 * R), (int)(255 * G), (int)(255 * B));
		//}

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

		public override string ToString() {
			return $"R: {R} G: {G} B: {B} L: {L}";
		}
	}
}
