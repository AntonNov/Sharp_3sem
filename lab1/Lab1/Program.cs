using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Net.Security;

namespace Sem3Lab1
{
	public enum EUsageType
	{
		goforward,
		goback,
		cut,
		copy,
		paste,
		create,
		delete,
		rename,
	}

	class FileExplorer
	{
		private DirectoryInfo directory = null;
		private FileInfo file = null;

		private FileSystemInfo storedFsi = null;
		private bool fsiIsCopied = false;
		private bool fsiIsFile = false;

		private DriveInfo[] drives = null;
		private DirectoryInfo[] subDirs = null;
		private FileInfo[] subFiles = null;

		public event Action Changed;
		public string buffer;

		public FileSystemWatcher watcher = null;

		public FileExplorer()
		{
			watcher = new FileSystemWatcher();
		}

		~FileExplorer()
		{
			watcher.Dispose();
		}

		public string[] GetInfo()
		{
			string[] result = new string[2];
			if (file != null)
			{
				result[0] = "File's information";
				result[1] = file.FullName;
			}
			else if (directory != null)
			{
				result[0] = "Directory's content";
				result[1] = directory.FullName;
			}
			else
			{
				result[0] = "List of drives";
				result[1] = "";
			}
			if (storedFsi != null)
			{
				result[0] += (fsiIsCopied) ? $" ({storedFsi.Name} is copied)" : $" ({storedFsi.Name} is cutted)";
			}
			return result;
		}

		public string[] GetContent()
		{
			watcher.EnableRaisingEvents = false;
			string[] result = null;
			if (file != null)
			{
				result = new string[3];
				result[0] = "Extension:       " + file.Extension;
				result[1] = "Creation date:   " + file.CreationTime.ToString();
				result[2] = "Last edit time:  " + file.LastWriteTime.ToString();

			}
			else if (directory != null)
			{
				subDirs = directory.GetDirectories();
				int dNum = subDirs.Length;
				subFiles = directory.GetFiles();
				int fNum = subFiles.Length;
				result = new string[dNum + fNum];
				for (int i = 0; i < dNum; i++)
				{
					result[i] = subDirs[i].Name + Path.DirectorySeparatorChar;
				}
				for (int i = dNum; i < dNum + fNum; i++)
				{
					result[i] = subFiles[i - dNum].Name;
				}
				watcher.Path = directory.FullName;
				watcher.EnableRaisingEvents = true;
			}
			else
			{
				drives = DriveInfo.GetDrives();
				int num = drives.Length;
				subDirs = new DirectoryInfo[num];
				result = new string[num];
				for (int i = 0; i < num; i++)
				{
					subDirs[i] = drives[i].RootDirectory;
					result[i] = drives[i].Name;
				}
			}
			return result;
		}

		public void UseContentElement(int index, EUsageType usageType)
		{
			switch (usageType)
			{
				case EUsageType.goforward:
					if (file == null)
					{
						if (index < subDirs.Length)
						{
							directory = subDirs[index];
						}
						else
						{
							file = subFiles[index - subDirs.Length];
						}
						Changed?.Invoke();
					}
					break;
				case EUsageType.goback:
					if (file != null)
					{
						file = null;
					}
					else if (directory != null)
					{
						directory = directory.Parent;
					}
					Changed?.Invoke();
					break;
				case EUsageType.create:
					if (file == null & !string.IsNullOrWhiteSpace(buffer))
					{
						if (buffer[buffer.Length - 1] == Path.DirectorySeparatorChar)
						{
							Directory.CreateDirectory(Path.Combine(directory.FullName, buffer));
						}
						else
						{
							File.Create(Path.Combine(directory.FullName, buffer)).Close();
						}
						Changed?.Invoke();
					}
					break;
				case EUsageType.delete:
					if (file == null)
					{
						if (index < subDirs.Length)
						{
							subDirs[index].Delete(true);
						}
						else
						{
							subFiles[index - subDirs.Length].Delete();
						}
						Changed?.Invoke();
					}
					break;
				case EUsageType.rename:
					if (file == null)
					{
						if (index >= subDirs.Length)
						{
							FileInfo temp = subFiles[index - subDirs.Length];
							temp.CopyTo(Path.Combine(Path.GetDirectoryName(temp.FullName), buffer));

							Changed?.Invoke();
						}
					}
					break;
				case EUsageType.cut:
					if (file == null)
					{
						if (index < subDirs.Length)
						{
							storedFsi = subDirs[index];
							fsiIsCopied = false;
							fsiIsFile = false;
						}
						else
						{
							storedFsi = subFiles[index - subDirs.Length];
							fsiIsCopied = false;
							fsiIsFile = true;
						}
						Changed?.Invoke();
					}
					break;
				case EUsageType.copy:
					if (file == null)
					{
						if (index < subDirs.Length)
						{
							storedFsi = subDirs[index];
							fsiIsCopied = true;
							fsiIsFile = false;
						}
						else
						{
							storedFsi = subFiles[index - subDirs.Length];
							fsiIsCopied = true;
							fsiIsFile = true;
						}
						Changed?.Invoke();
					}
					break;
				case EUsageType.paste:
					if (file == null & storedFsi != null)
					{
						if (fsiIsFile)
						{
							((FileInfo)storedFsi).CopyTo(Path.Combine(directory.FullName, ((FileInfo)storedFsi).Name), true);
							if (!fsiIsCopied)
							{
								((FileInfo)storedFsi).Delete();
								storedFsi = null;
							}
						}
						else
						{
							string oldDir = storedFsi.FullName;
							string newDir = Path.Combine(directory.FullName, ((DirectoryInfo)storedFsi).Name);
							Directory.CreateDirectory(newDir);
							DirectoryInfo[] copiedDirs = ((DirectoryInfo)storedFsi).GetDirectories("*", SearchOption.AllDirectories);
							foreach (DirectoryInfo di in copiedDirs)
							{
								Directory.CreateDirectory(di.FullName.Replace(oldDir, newDir));
							}
							FileInfo[] copiedFiles = ((DirectoryInfo)storedFsi).GetFiles("*", SearchOption.AllDirectories);
							foreach (FileInfo fi in copiedFiles)
							{
								fi.CopyTo(fi.FullName.Replace(oldDir, newDir), true);
							}
							if (!fsiIsCopied)
							{
								((DirectoryInfo)storedFsi).Delete(true);
								storedFsi = null;
							}
						}
						Changed?.Invoke();
					}
					break;
			}
		}
	}

	class Program
	{
		static FileExplorer explorer;
		static string[] lastInfo;
		static string[] lastContent;
		static int select;
		static bool isWorking = false;

		static void Start()
		{
			explorer = new FileExplorer();
			Refresh();
			explorer.Changed += Refresh;
			isWorking = true;
		}

		static void Refresh()
		{
			try
			{
				lastInfo = explorer.GetInfo();
				lastContent = explorer.GetContent();
			}
			catch (Exception ex)
			{
				Console.Clear();
				ShowErrorMessage(ex);
				Console.ReadKey(true);
				if (isWorking)
				{
					isWorking = false;
					Start();
				}
				else
				{
					throw;
				}
			}
			select = 0;
			ShowInfoAndContent();
		}

		static void ShowInfoAndContent()
		{
			Console.Clear();
			Console.WriteLine("===========================");
			Console.WriteLine(lastInfo[0]);
			Console.WriteLine(lastInfo[1]);
			Console.WriteLine("===========================");
			for (int i = 0; i < lastContent.Length; i++)
			{
				if (select == i)
				{
					Console.Write(">>");
				}
				Console.WriteLine('\t' + lastContent[i]);
			}
		}

		static void ShowErrorMessage(Exception ex)
		{
			Console.Clear();
			Console.WriteLine(ex.ToString());
		}

		static void DoAction()
		{
			ConsoleKeyInfo key = Console.ReadKey(true);
			switch (key.Key)
			{
				case ConsoleKey.DownArrow:
					select++;
					if (select >= lastContent.Length)
					{
						select = 0;
					}
					ShowInfoAndContent();
					break;
				case ConsoleKey.UpArrow:
					select--;
					if (select < 0)
					{
						select = lastContent.Length - 1;
					}
					ShowInfoAndContent();
					break;
				case ConsoleKey.RightArrow:
				case ConsoleKey.Enter:
					explorer.UseContentElement(select, EUsageType.goforward);
					break;
				case ConsoleKey.LeftArrow:
				case ConsoleKey.Escape:
					explorer.UseContentElement(select, EUsageType.goback);
					break;
				case ConsoleKey.N:
					Console.WriteLine("Enter name of new file or directory:");
					explorer.buffer = Console.ReadLine();
					explorer.UseContentElement(select, EUsageType.create);
					break;
				case ConsoleKey.Delete:
					explorer.UseContentElement(select, EUsageType.delete);
					break;
				case ConsoleKey.X:
					explorer.UseContentElement(select, EUsageType.cut);
					break;
				case ConsoleKey.C:
					explorer.UseContentElement(select, EUsageType.copy);
					break;
				case ConsoleKey.V:
					explorer.UseContentElement(select, EUsageType.paste);
					break;
				case ConsoleKey.R:
					explorer.UseContentElement(select, EUsageType.rename);
					break;
				default:
					return;
			}
		}

		static void Main(string[] args)
		{
			Start();
			while (isWorking)
			{
				try
				{
					DoAction();
				}
				catch (Exception ex)
				{
					ShowErrorMessage(ex);
					Console.ReadKey(true);
					ShowInfoAndContent();
				}
			}
		}
	}
}
