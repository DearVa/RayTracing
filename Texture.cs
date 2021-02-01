using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace RayTracing {
	class Texture : IDisposable {
		public readonly unsafe byte* ptr;
		public readonly int width, height;

		private readonly Bitmap bitmap;
		private readonly BitmapData bitmapData;
		private bool disposedValue;

		public Texture(string filePath) {
			bitmap = new Bitmap(filePath);
			width = bitmap.Width;
			height = bitmap.Height;
			bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
			unsafe {
				ptr = (byte*)bitmapData.Scan0.ToPointer();
			}
		}

		protected virtual void Dispose(bool disposing) {
			if (!disposedValue) {
				if (disposing) {
					bitmap.UnlockBits(bitmapData);
					bitmap.Dispose();
				}
				disposedValue = true;
			}
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
