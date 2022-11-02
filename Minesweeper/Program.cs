using System;
using System.Threading;
using System.Windows.Forms;

namespace Minesweeper
{
	internal static class Program
	{
		/// <summary>
		/// Главная точка входа для приложения.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			MultiFormContext ctx = new MultiFormContext(new HumanGameView(), new AIGameView());
			Application.Run(ctx);
		}


		public class MultiFormContext : ApplicationContext
		{
			private int openForms;
			public MultiFormContext(params Form[] forms)
			{
				openForms = forms.Length;

				foreach (var form in forms)
				{
					form.FormClosed += (s, args) =>
					{
						//When we have closed the last of the "starting" forms, 
						//end the program.
						if (Interlocked.Decrement(ref openForms) == 0)
							ExitThread();
					};

					form.Show();
				}
			}
		}
	}
}
