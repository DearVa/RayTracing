using System;
using System.IO;
using System.Collections.Generic;

namespace RayTracing {
	class Mesh {
		public List<TriangleGroup> TriangleGroups = new List<TriangleGroup>();   // 面

		public float Intersect(Ray ray, ref TriangleGroup interTG, ref Vector3 normal, ref Color4 color) {
			float interT = float.PositiveInfinity;
			Vector3 nNormal = Vector3.zero;
			Color4 nColor = Color4.zero;
			foreach (var tg in TriangleGroups) {
				float t = tg.Intersect(ray, ref nNormal, ref nColor);
				if (t > 0.0001f && t < interT) {
					interTG = tg;
					interT = t;
					normal = nNormal;
					color = nColor;
				}
			}
			if (interT == float.PositiveInfinity) {
				return -1;
			}
			return interT;
		}

		public static Mesh LoadFromObj(string fileName) {
			using (var sr = new StreamReader(new FileStream(fileName, FileMode.Open))) {
				Mesh mesh = new Mesh();
				List<Vector3> vs = new List<Vector3>();    // 顶点
				List<Vector3> ns = new List<Vector3>();     // 法线
				List<Vector2> ts = new List<Vector2>();   // 纹理坐标
				TriangleGroup triangleGroup = null;
				int i = 0;
				while (!sr.EndOfStream) {
					string line = sr.ReadLine();
					i++;
					try {
						if (line.StartsWith("#")) {
							continue;
						}
						if (line.StartsWith("vn")) {
							ReadLine(ns, line);
						} else if (line.StartsWith("vt")) {
							ReadLine(ts, line);
						} else if (line.StartsWith("v")) {
							ReadLine(vs, line);
						} else if (line.StartsWith("o")) {
							string[] data = line.Split(' ');
							if (data.Length != 2) {
								throw new Exception("OBJ格式错误");
							}
							if (triangleGroup != null) {
								triangleGroup.GenerateAABB();
								mesh.TriangleGroups.Add(triangleGroup);
							}
							triangleGroup = new TriangleGroup(data[1]);
						} else if (line.StartsWith("f")) {
							Triangle triangle = ReadLine(vs, ns, ts, line);
							triangleGroup.Triangles.Add(triangle);
						}
					} catch (Exception e) {
						throw new Exception($"第{i}行", e);
					}
				}
				if (triangleGroup == null) {
					throw new Exception("OBJ格式错误，没有物体");
				}
				triangleGroup.GenerateAABB();
				mesh.TriangleGroups.Add(triangleGroup);
				return mesh;
			}
		}

		private static void ReadLine(List<Vector3> l, string line) {
			string[] data = line.Split(' ');
			if (data.Length != 4) {
				throw new Exception("OBJ格式错误");
			}
			float x = float.Parse(data[1]);
			float y = float.Parse(data[2]);
			float z = float.Parse(data[3]);
			l.Add(new Vector3(x, y, z));
		}

		private static void ReadLine(List<Vector2> l, string line) {
			string[] data = line.Split(' ');
			if (data.Length != 4 && data.Length != 3) {
				throw new Exception("OBJ格式错误");
			}
			float x = float.Parse(data[1]);
			float y = float.Parse(data[2]);
			l.Add(new Vector3(x, y));
		}

		private static Triangle ReadLine(List<Vector3> vs, List<Vector3> ns, List<Vector2> ts, string line) {
			string[] data = line.Split(' ');
			if (data.Length != 4) {
				throw new Exception("OBJ格式错误，目前不支持读取非三角面");
			}
			int[] v = new int[3];
			int[] n = new int[3];
			int[] t = new int[3];
			bool hasNormal = false;
			for (int i = 0; i < 3; i++) {
				string[] tuple = data[i + 1].Split('/');
				if (tuple.Length == 3) {
					hasNormal = true;
				} else if (tuple.Length != 2) {
					throw new Exception("OBJ格式错误");
				}
				v[i] = int.Parse(tuple[0]) - 1;
				t[i] = int.Parse(tuple[1]) - 1;
				if (hasNormal) {
					n[i] = int.Parse(tuple[2]) - 1;
				}
			}
			if (hasNormal) {
				return new Triangle(
					new Vector3[] { vs[v[0]], vs[v[1]], vs[v[2]] },
					new Vector3[] { ns[n[0]], ns[n[1]], ns[n[2]] },
					new Vector2[] { ts[t[0]], ts[t[1]], ts[t[2]] }
					);
			} else {
				return new Triangle(
					new Vector3[] { vs[v[0]], vs[v[1]], vs[v[2]] },
					new Vector2[] { ts[t[0]], ts[t[1]], ts[t[2]] }
					);
			}
		}
	}

	class TriangleGroup {
		public List<Triangle> Triangles = new List<Triangle>();
		public string Name;
		public Material Material;
		/// <summary>
		/// 反射率
		/// </summary>
		public float ReflRatio;
		/// <summary>
		/// 折射率
		/// </summary>
		public float RefrRatio;

		private AABB aabb;

		public TriangleGroup() { }

		public TriangleGroup(string name) {
			Name = name;
		}

		public float Intersect(Ray ray, ref Vector3 normal, ref Color4 color) {
			if (aabb.Intersect(ray)) {
				float interT = float.PositiveInfinity;
				foreach (var tr in Triangles) {
					Vector3 e1 = tr.Vertices[1] - tr.Vertices[0];
					Vector3 e2 = tr.Vertices[2] - tr.Vertices[0];
					Vector3 P = Vector3.Cross(ray.dir, e2);
					float d = e1 * P;
					Vector3 T;
					if (d > 0) {
						T = ray.origin - tr.Vertices[0];
					} else {
						T = tr.Vertices[0] - ray.origin;
						d = -d;
					}
					if (d < 0.0001f) {
						continue;
					}
					float u = T * P / d;
					if (u < 0f || u > 1f) {
						continue;
					}
					Vector3 Q = Vector3.Cross(T, e1);
					float v = ray.dir * Q / d;
					if (v < 0f || u + v > 1f) {
						continue;
					}
					float t = e2 * Q / d;
					if (t > 0.0001 && t < interT) {
						interT = t;
						if (Material.Textured) {
							Vector2 uv1 = tr.Texcoods[1] - tr.Texcoods[0];
							Vector2 uv2 = tr.Texcoods[2] - tr.Texcoods[0];
							Vector2 uv;
							uv = uv1 * u + uv2 * v + tr.Texcoods[0];
							color = Material.GetColor(uv);
						} else {
							color = Material.color;
						}
						normal = tr.Normal;
						if (normal * ray.dir > 0) {
							normal = -normal;
						}
					}
				}
				if (interT == float.PositiveInfinity) {
					return -1;
				} else {
					return interT;
				}
			}
			return -1f;
		}

		public void GenerateAABB() {
			float xMax = float.NegativeInfinity, xMin = float.PositiveInfinity;
			float yMax = float.NegativeInfinity, yMin = float.PositiveInfinity;
			float zMax = float.NegativeInfinity, zMin = float.PositiveInfinity;
			foreach (var tr in Triangles) {
				foreach (var v in tr.Vertices) {
					xMax = Mathf.Max(xMax, v.x);
					yMax = Mathf.Max(yMax, v.y);
					zMax = Mathf.Max(zMax, v.z);
					xMin = Mathf.Min(xMin, v.x);
					yMin = Mathf.Min(yMin, v.y);
					zMin = Mathf.Min(zMin, v.z);
				}
			}
			aabb = new AABB(xMax, xMin, yMax, yMin, zMax, zMin);
		}

		public override string ToString() {
			return Name;
		}
	}

	struct AABB {
		public Vector3 Min, Max;

		public AABB(float xMax, float xMin, float yMax, float yMin, float zMax, float zMin) {
			float temp;
			if (xMax < xMin) {
				temp = xMin;
				xMin = xMax;
				xMax = temp;
			} else if (xMax == xMin) {
				xMax += 0.01f;
			}
			if (yMax < yMin) {
				temp = yMin;
				yMin = yMax;
				yMax = temp;
			} else if (yMax == yMin) {
				yMax += 0.01f;
			}
			if (zMax < zMin) {
				temp = zMin;
				zMin = zMax;
				zMax = temp;
			} else if (zMax == zMin) {
				zMax += 0.01f;
			}
			Min = new Vector3(xMin, yMin, zMin);
			Max = new Vector3(xMax, yMax, zMax);
		}

		public bool Intersect(Ray ray) {
			Vector3 v1 = Min - ray.origin;
			Vector3 t1 = new Vector3(v1.x / ray.dir.x, v1.y / ray.dir.y, v1.z / ray.dir.z);
			Vector3 v2 = Max - ray.origin;
			Vector3 t2 = new Vector3(v2.x / ray.dir.x, v2.y / ray.dir.y, v2.z / ray.dir.z);
			Vector3 tMin = Vector3.Min(t1, t2);
			Vector3 tMax = Vector3.Max(t1, t2);
			float near = Mathf.Max(tMin.x, tMin.y, tMin.z);
			float far = Mathf.Min(tMax.x, tMax.y, tMax.z);
			return near < far;
		}

		public override string ToString() {
			return $"Min: {Min} Max: {Max}";
		}
	}

	struct Triangle {
		public Vector3[] Vertices;
		public Vector3[] Normals;
		public Vector2[] Texcoods;
		public Vector3 Normal;

		public Triangle(Vector3[] vs, Vector2[] ts) {
			Vertices = vs;
			Texcoods = ts;
			Normal = Vector3.Cross(vs[1] - vs[0], vs[2] - vs[0]).normalized;
			Normals = new Vector3[] { Normal, Normal, Normal };
		}

		public Triangle(Vector3[] vs, Vector3[] ns, Vector2[] ts) {
			Vertices = vs;
			Normals = ns;
			Texcoods = ts;
			Normal = Vector3.Cross(vs[1] - vs[0], vs[2] - vs[0]).normalized;
			if (Normal * ns[0] < 0) {
				Normal = -Normal;
			}
		}
	}
}
