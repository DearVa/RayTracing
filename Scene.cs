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

			scene = Mesh.LoadFromObj(@".\Mesh\Glass.obj");
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
				Parallel.ForEach(rows, y => InternalRun(y));  // 并行加速，ForEach效率貌似会更高
				bitmap.UnlockBits(bitmapData);
				if (save) {
					bitmap.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/save.png");
					save = false;
				}
				g.DrawImage(bitmap, 0, 0);
				g.FillRectangle(Brushes.White, new Rectangle(10, 10, 130, 25));
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
					//Color color = rays[x, y].Render().LToColor();
					int dx = x * 3 + dy;
					ptr[dx] = color.B;
					ptr[dx + 1] = color.G;
					ptr[dx + 2] = color.R;
				}
			}
		}
	}
}
