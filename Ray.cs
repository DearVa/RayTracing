﻿namespace RayTracing {
	class Ray {
		public Vector3 origin, dir;
		private Color4 color = Color4.zero;
		private Color4 refrColor = Color4.zero;
		private Color4 reflColor = Color4.zero;
		private int depth = 0;
		private float inertT = float.PositiveInfinity;
		private Sphere interSphere;
		private readonly Sphere fromSphere;
		private readonly int reflectNum;
		private Vector3 interPos, normal;
		private Ray ray = null, refrRay = null;
		private float fresnel = 0f;
		private bool first = true;

		public Ray(Vector3 origin, Vector3 dir) {
			this.origin = origin;
			this.dir = dir.normalized;
			reflectNum = 5;
		}

		public Ray(Vector3 origin, Vector3 dir, int reflectNum, Sphere fromSphere) {
			this.origin = origin;
			this.dir = dir.normalized;
			this.reflectNum = reflectNum;
			this.fromSphere = fromSphere;
		}

		public Color4 Render() {
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
			if (interSphere.surface == Surface.EMISSION) {
				return interSphere.color;
			}
			if (first) {
				interPos = origin + dir * inertT;
				normal = (interPos - interSphere.center).normalized;
			}
			// 反射
			if (reflectNum > 0) {
				Vector3 reflect = Vector3.Reflect(dir, normal);
				if (interSphere.surface == Surface.DIFFUSE) {  // 漫反射
					ray = new Ray(interPos, reflect + Vector3.RandInUnitSphere(), reflectNum - 1, interSphere);  // 此处应当是一个垂直于法线的半球，但我用了一个球体
					float r = 1f / ++depth;
					reflColor = reflColor * (1 - r) + ray.Render() * r;
				} else if (interSphere.surface == Surface.SPEC) {  // 镜面反射
					if (ray == null) {
						ray = new Ray(interPos, reflect, reflectNum - 1, interSphere);
					}
					reflColor = ray.Render();
				}
			} else {
				return interSphere.color;
			}
			if (interSphere.surface == Surface.SPEC && interSphere.refr > 0f) {  // 折射
				if (refrRay == null && fresnel != 1f) {
					float sinb = dir * normal.normalized;
					if (sinb < 0 || sinb * interSphere.refr > 1) {  // 从外部射入或者不发生全反射，此处不知道公式对不对
						Vector3 refract = Vector3.Refract(dir, normal, interSphere.refr).normalized;
						refrRay = new Ray(interPos, refract, reflectNum - 1, interSphere);
						fresnel = Mathf.Abs(sinb);  // 这个变量其实不是菲涅尔
					} else {
						fresnel = 1f;
					}
				}
				if (refrRay != null) {
					float r = 1f / ++depth;
					refrColor = refrColor * (1 - r) + refrRay.Render() * r;
					color = interSphere.color * (1 - interSphere.refl) + reflColor * interSphere.refl * fresnel + refrColor * interSphere.refl * (1 - fresnel);
				} else {
					color = interSphere.color * (1 - interSphere.refl) + reflColor * interSphere.refl;
				}
			} else {
				color = interSphere.color * (1 - interSphere.refl) + reflColor * interSphere.refl;
			}
			first = false;
			return color;
		}
	}
}
