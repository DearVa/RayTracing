using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System;

namespace RayTracing {
	enum Surface {
		EMISSION,
		DIFFUSE,
		SPECULAR
	}

	class Material {
		public Surface Surface;
		public float TillX, TillY;
		public Color4 Color;
		public Texture Texture;
		public bool Textured { get; private set; }
		/// <summary>
		/// 反射率
		/// </summary>
		public float ReflRatio;
		/// <summary>
		/// 折射率
		/// </summary>
		public float RefrRatio;

		public Material(Surface surface, Color color, float emission, float reflRatio, float refrRatio) {
			Surface = surface;
			Color = (Color4)color;
			Color.L = emission;
			ReflRatio = reflRatio;
			RefrRatio = refrRatio;
			Textured = false;
		}

		public Material(Surface surface, Color4 color, float reflRatio, float refrRatio) {
			Surface = surface;
			Color = color;
			ReflRatio = reflRatio;
			RefrRatio = refrRatio;
			Textured = false;
		}

		public Material(Surface surface, Texture texture, float tillX, float tillY, float reflRatio, float refrRatio) {
			Surface = surface;
			Texture = texture;
			TillX = tillX;
			TillY = tillY;
			ReflRatio = reflRatio;
			RefrRatio = refrRatio;
			Textured = true;
		}

		public Color4 GetColor(float u, float v) {
			u = Mathf.Max(Mathf.Min(u, 1f), 0f);
			v = Mathf.Max(Mathf.Min(v, 1f), 0f);
			int x = (int)(u * Texture.width * TillX) % Texture.width;
			int y = (int)(v * Texture.height * TillY) % Texture.height;
			int d = (y * Texture.width + x) * 3;
			unsafe {
				return new Color4(Texture.ptr[d + 2], Texture.ptr[d + 1], Texture.ptr[d], Color.L);
			}
		}

		public Color4 GetColor(Vector2 uv) {
			return GetColor(uv.x, uv.y);
		}

		public static Dictionary<string, Material> LoadFromMtl(string fileName) {
			using (var sr = new StreamReader(new FileStream(fileName, FileMode.Open))) {
				int i = 0;
				Dictionary<string, Material> ms = new Dictionary<string, Material>();
				Material material = null;
				string name = null;
				Surface surface = Surface.DIFFUSE;
				Color color = new Color();
				float emis = 0f, refl = 0.8f, refr = 0f;
				Texture texture = null;
				float tillX = 1f, tillY = 1f;
				while (!sr.EndOfStream) {
					var line = sr.ReadLine();
					i++;
					if (line.StartsWith("newmtl")) {
						var data = line.Split(' ');
						if (data.Length != 2) {
							throw new Exception($"第{i}行，MTL文件错误");
						}
						if (name != null) {
							if (texture != null) {
								material = new Material(surface, texture, tillX, tillY, refl, refr);
							} else {
								material = new Material(surface, color, emis, refl, refr);
							}
							surface = Surface.DIFFUSE;
							color = new Color();
							emis = 0f; refl = 0.8f; refr = 0f;
							texture = null;
							tillX = 1f; tillY = 1f;
							ms.Add(name, material);
							name = null;
						}
						name = data[1];
					} else if (line.StartsWith("Kd")) {
						var data = line.Split(' ');
						if (data.Length != 4) {
							throw new Exception($"第{i}行，MTL文件错误");
						}
						float r = float.Parse(data[1]);
						float g = float.Parse(data[2]);
						float b = float.Parse(data[3]);
						color = System.Drawing.Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
					} else if (line.StartsWith("Ke")) {  // 用1/Ke的R代表亮度
						var data = line.Split(' ');
						if (data.Length != 4) {
							throw new Exception($"第{i}行，MTL文件错误");
						}
						surface = Surface.EMISSION;
						emis = float.Parse(data[1]);
					} else if (line.StartsWith("Ni")) {
						var data = line.Split(' ');
						if (data.Length != 2) {
							throw new Exception($"第{i}行，MTL文件错误");
						}
						surface = Surface.SPECULAR;
						refr = float.Parse(data[1]);
						refl = 1f;  // 默认反射为1
					} else if (line.StartsWith("Ns")) {  // 暂时用100-粗糙度代表反射强度
						var data = line.Split(' ');
						if (data.Length != 2) {
							throw new Exception($"第{i}行，MTL文件错误");
						}
						surface = Surface.SPECULAR;
						refl = 1f - int.Parse(data[1]) / 100f;
					} else if (line.StartsWith("map_Kd")) {
						var data = line.Split(' ');
						if (data.Length == 1) {
							throw new Exception($"第{i}行，MTL文件错误");
						}
						for (int j = 1; j < data.Length; j++) {
							if (data[j] == "-s") {
								if (data.Length < j + 4) {
									throw new Exception($"第{i}行，MTL文件错误");
								}
								tillX = 1 / float.Parse(data[j + 1]);
								tillY = 1 / float.Parse(data[j + 2]);
							}
						}
						texture = new Texture(Path.Combine(Path.GetDirectoryName(fileName), data[data.Length - 1]));
					}
				}
				if (name != null) {
					if (texture != null) {
						material = new Material(surface, texture, tillX, tillY, refl, refr);
					} else {
						material = new Material(surface, color, emis, refl, refr);
					}
					ms.Add(name, material);
				} else {
					throw new Exception("MTL文件错误，无材质");
				}
				return ms;
			}
		}
	}
}
