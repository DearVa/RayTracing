using System;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace RayTracing {
	class Scene {
		public bool stop;
		public bool save;

		const float fov = 90f;
		const int taskNum = 6;

		readonly Ray[,] rays;
		readonly Bitmap bitmap;
		readonly Graphics g;

		readonly static Texture t1 = new Texture("./block.jpg");
		readonly static Material m1 = new Material(Surface.EMISSION, Color.White, 1f);
		readonly static Material m2 = new Material(Surface.EMISSION, Color.Red, 1f);
		readonly static Material m3 = new Material(Surface.DIFFUSE, Color.LightCyan, 0f);
		readonly static Material m4 = new Material(Surface.DIFFUSE, Color.LightYellow, 0f);
		readonly static Material m5 = new Material(Surface.DIFFUSE, Color.Pink, 0f);
		readonly static Material m6 = new Material(Surface.DIFFUSE, Color.LimeGreen, 0f);

		readonly static Material m7 = new Material(Surface.SPECULAR, Color.Blue, 0f);
		readonly static Material m8 = new Material(Surface.SPECULAR, Color.OrangeRed, 0f);
		readonly static Material m9 = new Material(Surface.DIFFUSE, t1, 1f, 1f);

		readonly static Material m0 = new Material(Surface.EMISSION, Color.White, 5f);

		public static Mesh scene;

		//public static Color4 AmbientColor = new Color4(255f, 255f, 255f, 0.3f);
		public static Color4 AmbientColor = Color4.zero;

		private readonly int width, height;
		private readonly Rectangle rect;
		private BitmapData bitmapData;
		private unsafe byte* ptr;

		public Scene(int width, int height, Graphics g) {
			this.width = width;
			this.height = height;
			this.g = g;
			rect = new Rectangle(0, 0, width, height);
			bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
			rays = new Ray[width, height];

			float nearPlane = width / 2 / Mathf.Tan(fov / 2f);
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					rays[x, y] = new Ray(Vector3.zero, new Vector3(x - width / 2, y - height / 2 + 30f, nearPlane));
				}
			}

			scene = Mesh.LoadFromObj(@".\Mesh\Scene.obj");
			// Ceiling, Emission
			scene.TriangleGroups[0].Material = m1;
			scene.TriangleGroups[0].ReflRatio = 0.8f;
			scene.TriangleGroups[0].RefrRatio = 0.0f;

			// Cylinder, Glass
			scene.TriangleGroups[1].Material = m8;
			scene.TriangleGroups[1].ReflRatio = 1.0f;
			scene.TriangleGroups[1].RefrRatio = 1.5f;

			// Rubby, Diffuse
			scene.TriangleGroups[2].Material = m9;
			scene.TriangleGroups[2].ReflRatio = 0.5f;
			scene.TriangleGroups[2].RefrRatio = 0.0f;

			// Cube, Specular
			scene.TriangleGroups[3].Material = m7;
			scene.TriangleGroups[3].ReflRatio = 0.9f;
			scene.TriangleGroups[3].RefrRatio = 0.0f;

			// Room, Diffuse
			scene.TriangleGroups[4].Material = m9;
			scene.TriangleGroups[4].ReflRatio = 0.8f;
			scene.TriangleGroups[4].RefrRatio = 0.0f;

			for (int i = 5; i < scene.TriangleGroups.Count; i++) {
				scene.TriangleGroups[i].Material = m4;
				scene.TriangleGroups[i].ReflRatio = 0.9f;
				scene.TriangleGroups[i].RefrRatio = 0.0f;
			}

			//scene.TriangleGroups[0].Material = m7;
			//scene.TriangleGroups[0].ReflRatio = 0.9f;
			//scene.TriangleGroups[0].RefrRatio = 0.0f;

			//scene.TriangleGroups[1].Material = m7;
			//scene.TriangleGroups[1].ReflRatio = 0.9f;
			//scene.TriangleGroups[1].RefrRatio = 0.0f;

			//scene.TriangleGroups[2].Material = m1;
			//scene.TriangleGroups[2].ReflRatio = 0.9f;
			//scene.TriangleGroups[2].RefrRatio = 0.0f;
		}

		private delegate void Action();
		private readonly Font font = new Font(new FontFamily("Consolas"), 10f);

		public void Run() {
			int frame = 0;
			int[] rows = new int[height];
			for (int y = 0; y < height; y++) {
				rows[y] = y;
			}
			Stopwatch sw = new Stopwatch();
			sw.Start();
			while (true) {
				if (stop) {
					break;
				}
				long time = sw.ElapsedMilliseconds;
				bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
				unsafe {
					ptr = (byte*)bitmapData.Scan0.ToPointer();
				}
				//for (int y = 0; y < height; y++) {
				//	InternalRun(y);
				//}
				//Parallel.For(0, height, (y) => InternalRun(y));  // 并行加速
				Parallel.ForEach(rows, (y) => InternalRun(y));  // 并行加速，ForEach效率貌似会更高
				bitmap.UnlockBits(bitmapData);
				if (save) {
					bitmap.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/save.png");
					save = false;
				}
				g.DrawImage(bitmap, 0, 0);
				g.DrawString($"Frame Count: {++frame}", font, Brushes.Black, 10, 10);
				g.DrawString($"FPS: {1000d / (sw.ElapsedMilliseconds - time):F}", font, Brushes.Black, 10, 20);
				Console.WriteLine(frame);
			}
			Environment.Exit(0);
		}

		private void InternalRun(int y) {
			Vector3.InitRand(y);
			int dy = y * bitmapData.Stride;
			unsafe {
				for (int x = 0; x < width; x++) {
					Color color = (Color)rays[x, y].Render();
					int dx = x * 3 + dy;
					ptr[dx] = color.B;
					ptr[dx + 1] = color.G;
					ptr[dx + 2] = color.R;
				}
			}
		}
	}
}
