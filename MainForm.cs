using System;
using System.Threading;
using System.Windows.Forms;

namespace RayTracing {
	public partial class MainForm : Form {
		Scene scene;

		public MainForm() {
			InitializeComponent();
		}

		private void MainForm_Shown(object sender, EventArgs e) {
			scene = new Scene(Width, Height, CreateGraphics());
			Thread thread = new Thread(scene.Run) { IsBackground = true };
			thread.Start();
		}

		private void Save_Click(object sender, EventArgs e) {
			scene.save = true;
		}

		private void Exit_Click(object sender, EventArgs e) {
			Environment.Exit(0);  // 彻底退出程序
		}
	}
}
