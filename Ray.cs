namespace RayTracing {
	class Ray {
		public Vector3 origin, dir;
		private Color4 interColor = Color4.zero;
		private Color4 color = Color4.zero;
		private Color4 refrColor = Color4.zero;
		private Color4 reflColor = Color4.zero;
		private Color4 reflColorSum = Color4.zero;
		private int reflColorNum = 0;
		private float inertT = float.PositiveInfinity;
		private TriangleGroup interTG;
		private readonly int reflectNum = 10;
		private Vector3 interPos, normal;
		private Ray reflRay = null, refrRay = null;
		private float fresnel = 0f;
		private bool first = true;

		public Ray(Vector3 origin, Vector3 dir) {
			this.origin = origin;
			this.dir = dir.normalized;
		}

		public Ray(Vector3 origin, Vector3 dir, int reflectNum) {
			this.origin = origin;
			this.dir = dir.normalized;
			this.reflectNum = reflectNum;
		}

		public Color4 Render() {
			if (first) {
				inertT = Scene.scene.Intersect(this, ref interTG, ref normal, ref interColor);
				interPos = origin + dir * inertT;  // 交点坐标
				first = false;
			}
			if (interTG == null) {
				//return Color4.zero;
				return Scene.AmbientColor;
			}
			Material mat = interTG.Material;
			if (mat.Surface == Surface.EMISSION) {
				return interColor;
			}
			// 反射
			if (reflectNum > 0) {
				if (mat.Surface == Surface.DIFFUSE) {  // 漫反射
					Ray diffRay = new Ray(interPos, Vector3.RandInUnitHemisphere(normal), reflectNum - 1);  // 每次都不一样，局部变量
					reflColorSum += diffRay.Render();
					reflColorNum++;
					reflColor = reflColorSum / reflColorNum;
				} else if (mat.Surface == Surface.SPECULAR) {  // 镜面反射
					if (reflRay == null) {
						reflRay = new Ray(interPos, Vector3.Reflect(dir, normal), reflectNum - 1);
					}
					reflColor = reflRay.Render();
				}
			} else {
				return interColor;
			}
			if (mat.Surface == Surface.SPECULAR && mat.RefrRatio > 0f) {  // 折射
				if (refrRay == null && fresnel != 1f) {
					if (Vector3.Refract(dir, normal, mat.RefrRatio, out Vector3 refrect)) {
						refrRay = new Ray(interPos, refrect, reflectNum - 1);
						fresnel = 1f - Mathf.Abs(dir * normal);
					} else {
						fresnel = 1f;  // 全反射
					}
				}
				if (refrRay != null) {
					refrColor = refrRay.Render();
					color = interColor * (1 - mat.ReflRatio) + reflColor * mat.ReflRatio * fresnel + refrColor * mat.ReflRatio * (1 - fresnel);
				} else {
					color = interColor * (1 - mat.ReflRatio) + reflColor * mat.ReflRatio;
				}
			} else {
				color = interColor * (1 - mat.ReflRatio) + reflColor * mat.ReflRatio;
			}
			first = false;
			return color;
		}

		public override string ToString() {
			return $"Origin: {origin} Dir: {dir}";
		}
	}
}
