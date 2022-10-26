namespace Minesweeper
{
	partial class GameCell
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

		#region Код, автоматически созданный конструктором компонентов

		/// <summary> 
		/// Требуемый метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InitializeComponent()
		{
			this._button = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _button
			// 
			this._button.Dock = System.Windows.Forms.DockStyle.Fill;
			this._button.Font = new System.Drawing.Font("Courier New", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._button.Location = new System.Drawing.Point(0, 0);
			this._button.Name = "_button";
			this._button.Size = new System.Drawing.Size(50, 50);
			this._button.TabIndex = 0;
			this._button.Text = "1";
			this._button.UseVisualStyleBackColor = true;
			this._button.Click += new System.EventHandler(this._button_Click);
			// 
			// GameCell
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._button);
			this.Name = "GameCell";
			this.Size = new System.Drawing.Size(50, 50);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _button;
	}
}
