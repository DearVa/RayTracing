
namespace RayTracing {
	partial class MainForm {
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要修改
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent() {
			this.save = new System.Windows.Forms.Button();
			this.exit = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// save
			// 
			this.save.Location = new System.Drawing.Point(1251, 784);
			this.save.Name = "save";
			this.save.Size = new System.Drawing.Size(75, 23);
			this.save.TabIndex = 0;
			this.save.Text = "保存到桌面";
			this.save.UseVisualStyleBackColor = true;
			this.save.Click += new System.EventHandler(this.Save_Click);
			// 
			// exit
			// 
			this.exit.ForeColor = System.Drawing.Color.Red;
			this.exit.Location = new System.Drawing.Point(1332, 784);
			this.exit.Name = "exit";
			this.exit.Size = new System.Drawing.Size(75, 23);
			this.exit.TabIndex = 1;
			this.exit.Text = "退出";
			this.exit.UseVisualStyleBackColor = true;
			this.exit.Click += new System.EventHandler(this.Exit_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1419, 819);
			this.ControlBox = false;
			this.Controls.Add(this.exit);
			this.Controls.Add(this.save);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "MainForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "RayTracing";
			this.Shown += new System.EventHandler(this.MainForm_Shown);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button save;
		private System.Windows.Forms.Button exit;
	}
}

