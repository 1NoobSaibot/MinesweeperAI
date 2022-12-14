namespace Minesweeper
{
	partial class AIGameView
	{
		/// <summary>
		/// Обязательная переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором форм Windows

		/// <summary>
		/// Требуемый метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.gameView1 = new Minesweeper.GameView();
			this.gameLooper = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// gameView1
			// 
			this.gameView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gameView1.Location = new System.Drawing.Point(0, 0);
			this.gameView1.Name = "gameView1";
			this.gameView1.Size = new System.Drawing.Size(800, 500);
			this.gameView1.TabIndex = 0;
			// 
			// gameLooper
			// 
			this.gameLooper.Interval = 200;
			this.gameLooper.Tick += new System.EventHandler(this.gameLooper_Tick);
			// 
			// AIGameView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 500);
			this.Controls.Add(this.gameView1);
			this.Name = "AIGameView";
			this.Text = "Form1";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AIGameView_FormClosing);
			this.ResumeLayout(false);

		}

		#endregion

		private GameView gameView1;
		private System.Windows.Forms.Timer gameLooper;
	}
}

