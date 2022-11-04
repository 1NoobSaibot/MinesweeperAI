using Newtonsoft.Json;
using System.IO;

namespace Minesweeper
{
	public class Storage<T> where T : class
	{
		private readonly string fileName;
		private T _data;

		public T Data => _data;


		public Storage(string fileName)
		{
			this.fileName = fileName;
			_Load();
		}


		public void Save(T data)
		{
			string json = JsonConvert.SerializeObject(data);
			_data = data;
			_RewriteFile(json);
		}

		private void _RewriteFile(string json)
		{
			using (StreamWriter writer = new StreamWriter(fileName, false))
			{
				writer.Write(json);
			}
		}


		private void _Load()
		{
			if (!File.Exists(fileName))
			{
				return;
			}

			using (StreamReader reader = new StreamReader(fileName))
			{
				string json = reader.ReadToEnd();
				_data = JsonConvert.DeserializeObject<T>(json);
			}
		}
	}
}
