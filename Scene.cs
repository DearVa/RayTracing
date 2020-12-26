using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace RayTracing {
	class Scene {
		public bool running = true;
		public bool save;

		const float fov = 90f;
		const int taskNum = 6;

		readonly Ray[,] rays;
		readonly Bitmap bitmap;
		readonly Graphics g;

		readonly static Bitmap test = new Bitmap("./block.jpg");

		public static readonly Sphere[] spheres = new Sphere[] {
			new Sphere(new Vector3(1e5f - 60f, 40.8f, 81.6f), 1e5f, Color.LightBlue, Surface.EMISSION, 0.5f, 0f, 1f),  // left
			new Sphere(new Vector3(-1e5f + 60f, 40.8f, 81.6f), 1e5f, Color.Red, Surface.EMISSION, 0.5f, 0f, 1f),  // right
			new Sphere(new Vector3(50f, 40.8f, 1e5f - 50f), 1e5f, Color.LightCyan, Surface.DIFFUSE, 0.5f, 0f, 0f),  // back
			new Sphere(new Vector3(50f, 40.8f, -1e5f + 70f), 1e5f, Color.LightYellow, Surface.DIFFUSE, 0.5f, 0f, 0f),  // front
			new Sphere(new Vector3(0, 1e5f - 50f, 81.6f), 1e5f, Color.Pink, Surface.DIFFUSE, 0.5f, 0f, 0f),  // top
			new Sphere(new Vector3(0, -1e5f + 33f, 81.6f), 1e5f, Color.LimeGreen, Surface.DIFFUSE, 0.5f, 0f, 0f),  // bottom

			new Sphere(new Vector3(-35f, 16.5f, 50f), 16.5f, Color.Blue, Surface.SPEC, 0.9f, 0f, 0f),  // mirr
			new Sphere(new Vector3(0f, 16.5f, 58f), 16.5f, Color.Pink, Surface.SPEC, 1f, 1.33f, 0f),  // glass
			new Sphere(new Vector3(35f, 16.5f, 50f), 16.5f, test, 1f, 1f, Surface.DIFFUSE, 0.5f, 0f, 0f),  // glass
			
			new Sphere(new Vector3(0f, -40f, 81.6f), 15f, Color.White, Surface.EMISSION, 0f, 0f, 2f),  // light
		};

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
		}

		private delegate void Action();
		private readonly Font font = new Font(new FontFamily("Consolas"), 10f);

		public void Run() {
			int frame = 0;
			Task[] tasks = new Task[taskNum];
			while (true) {
				if (!running) {
					return;
				}
				bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
				unsafe {
					ptr = (byte*)bitmapData.Scan0.ToPointer();
				}
				for (int i = 0; i < taskNum; i++) {
					int j = i;
					tasks[i] = Task.Factory.StartNew(() => InternalRun(j));
				}
				Task.WaitAll(tasks);
				bitmap.UnlockBits(bitmapData);
				if (save) {
					bitmap.Save(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/save.png");
					save = false;
				}
				g.DrawImage(bitmap, 0, 0);
				g.DrawString($"{++frame}", font, Brushes.Black, 10, 10);
			}
		}

		private void InternalRun(int i) {
			Vector3.InitRand(i);
			unsafe {
				for (int y = 0; y < height; y++) {
					int dy = y * bitmapData.Stride;
					for (int x = i; x < width; x += taskNum) {
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
}
