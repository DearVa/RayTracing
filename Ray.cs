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
		private Sphere interSphere;
		private readonly Sphere fromSphere;
		private readonly int reflectNum = 10;
		private Vector3 interPos, normal;
		private Ray reflRay = null, refrRay = null;
		private float fresnel = 0f;
		private bool first = true;

		public Ray(Vector3 origin, Vector3 dir) {
			this.origin = origin;
			this.dir = dir.normalized;
		}

		public Ray(Vector3 origin, Vector3 dir, int reflectNum, Sphere fromSphere) {
			this.origin = origin;
			this.dir = dir.normalized;
			this.reflectNum = reflectNum;
			this.fromSphere = fromSphere;
		}

		public Color4 Render() {
			//if (Vector3.rand.NextDouble() < color.L / 2) {
			//	return color;
			//}
			if (interSphere == null) {
				for (int i = 0; i < Scene.spheres.Length; i++) {
					var sphere = Scene.spheres[i];
					if (sphere == fromSphere) {
						continue;
					}
					float t = sphere.Intersect(this);
					if (t > 0.01f && t < inertT) {
						inertT = t;
						interSphere = sphere;
					}
				}
			}
			if (interSphere == null) {
				return Color4.zero;
			}
			if (first) {
				interPos = origin + dir * inertT;  // 交点坐标
				normal = (interPos - interSphere.center).normalized;  // 法线，从内向外
			}
			interColor = interSphere.GetColor(interPos);
			if (interSphere.surface == Surface.EMISSION) {
				return interColor;
			}
			// 反射
			if (reflectNum > 0) {
				if (interSphere.surface == Surface.DIFFUSE) {  // 漫反射
					Ray diffRay = new Ray(interPos, Vector3.RandInUnitHemisphere(normal), reflectNum - 1, interSphere);  // 每次都不一样，局部变量
					reflColorSum += diffRay.Render();
					reflColorNum++;
					reflColor = reflColorSum / reflColorNum;
				} else if (interSphere.surface == Surface.SPEC) {  // 镜面反射
					if (reflRay == null) {
						reflRay = new Ray(interPos, Vector3.Reflect(dir, normal), reflectNum - 1, interSphere);
					}
					reflColor = reflRay.Render();
				}
			} else {
				return interColor;
			}
			if (interSphere.surface == Surface.SPEC && interSphere.refrRatio > 0f) {  // 折射
				if (refrRay == null && fresnel != 1f) {
					if (Vector3.Refract(dir, normal, interSphere.refrRatio, out Vector3 refrect)) {
						refrRay = new Ray(interPos, refrect, reflectNum - 1, interSphere);
						fresnel = 1f - Mathf.Abs(dir * normal);
					} else {
						fresnel = 1f;  // 全反射
					}
				}
				if (refrRay != null) {
					refrColor = refrRay.Render();
					color = interColor * (1 - interSphere.reflRatio) + reflColor * interSphere.reflRatio * fresnel + refrColor * interSphere.reflRatio * (1 - fresnel);
				} else {
					color = interColor * (1 - interSphere.reflRatio) + reflColor * interSphere.reflRatio;
				}
			} else {
				color = interColor * (1 - interSphere.reflRatio) + reflColor * interSphere.reflRatio;
			}
			first = false;
			return color;
		}
	}
}
