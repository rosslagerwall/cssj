/*
    Copyright (C) 2011 Ross Lagerwall <rosslagerwall@gmail.com>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.IO;
using System.Collections.Generic;

public class SplitJoin {

	public static void split_parts(string filename, int parts) {
		FileStream inFile = new FileStream(filename, FileMode.Open, FileAccess.Read);
		long size = inFile.Length / parts;
		long rem = inFile.Length % parts;
		byte[] buf = new byte[4096];

		for (int i = 0; i < parts; i++) {
			long remaining = i < rem ? size + 1 : size;
			FileStream outFile = new FileStream(filename + "." + Convert.ToString(i), FileMode.CreateNew);

			int n;
			while ((n = inFile.Read(buf, 0, (int)Math.Min(remaining, 4096))) > 0) {
				outFile.Write(buf, 0, n);
				remaining -= n;
			}
			
		}
	}

	public static void split_size(string filename, long size) {
		FileStream inFile = new FileStream(filename, FileMode.Open, FileAccess.Read);
		long remaining = inFile.Length;
		int i = 0;
		byte[] buf = new byte[4096];
		while (remaining > 0) {

			FileStream outFile = new FileStream(filename + "." + Convert.ToString(i), FileMode.CreateNew);

			long rem = Math.Min(size, remaining);

			int n;
			while ((n = inFile.Read(buf, 0, (int)Math.Min(rem, 4096))) > 0) {
				outFile.Write(buf, 0, n);
				rem -= n;
				remaining -= n;
			}

			i++;
		}
	}

	public static void join(string filename) {
		FileStream outFile = new FileStream(filename, FileMode.CreateNew);
		byte[] buf = new byte[4096];

		int i = 0;
		while (File.Exists(filename + "." + Convert.ToString(i))) {
			FileStream inFile = new FileStream(filename + "." + Convert.ToString(i), FileMode.Open, FileAccess.Read);

			int n;
			while ((n = inFile.Read(buf, 0, 4096)) > 0) {
				outFile.Write(buf, 0, n);
			}

			i++;
		}
	}

	private static void helpSplit() {
		Console.WriteLine("usage: SplitJoin split [-h] [-s] [-p] <filename>\n" +
				"arguments:\n" +
				"[-h]       show help for this command\n" +
				"[-s]       specify the size for each part to be be (in bytes)\n" +
				"[-p]       specify the number of parts to split <filename> into\n" +
				"<filename> specify the filename to split");
		Environment.Exit(64);
	}

	private static void parseSplit(string[] args) {
		LinkedList<string> argL = new LinkedList<string>(args);
		argL.RemoveFirst();
		string filename = "";
		long size = -1;
		int parts = -1;

		while (argL.Count > 0) {
			string i = argL.First.Value;
			argL.RemoveFirst();
			if (i == "-s" || i == "--size") {
				if (argL.Count == 0)
					helpSplit();
				string j = argL.First.Value;
				argL.RemoveFirst();
				if (!j.StartsWith("-"))
					size = Convert.ToInt64(j);
			} else if (i.StartsWith("--size=")) {
				size = Convert.ToInt64(i.Substring(7));
			} else if (i == "-p" || i == "--parts") {
				if (argL.Count == 0)
					helpSplit();
				string j = argL.First.Value;
				argL.RemoveFirst();
				if (!j.StartsWith("-"))
					parts = Convert.ToInt32(j);
			} else if (i.StartsWith("--parts=")) {
				parts = Convert.ToInt32(i.Substring(8));
			} else if (i == "-h" || i == "--help") {
				helpSplit();
			} else {
				filename = i;
			}
		}

		if (filename == "") {
			helpSplit();
		}
		if ((size == -1 && parts == -1) || (size != -1 && parts != -1)) {
			helpSplit();
		} else if (size == -1) {
			split_parts(filename, parts);
		} else {
			split_size(filename, size);
		}
	}

	private static void helpJoin() {
		Console.WriteLine("usage: SplitJoin join [-h] <filename>\n" +
				"arguments:\n" +
				"[-h]       show help for this command\n" +
				"<filename> specify the filename to join (e.g. if the" +
				" files are \"reg.avi.0\", \"reg.avi.1\", ... then <filename> is" +
				" specified as \"reg.avi\"");
		Environment.Exit(64);
	}

	private static void parseJoin(string[] args) {
		LinkedList<string> argL = new LinkedList<string>(args);
		argL.RemoveFirst();
		string filename = "";

		while (argL.Count > 0) {
			string i = argL.First.Value;
			argL.RemoveFirst();
			if (i == "-h" || i == "--help") {
				helpJoin();
			} else {
				filename = i;
			}
		}
		if (filename == "") {
			helpJoin();
		}
		join(filename);
	}


	public static void Main(string[] args) {
		if (args.Length == 0 || args[0] == "-h" || args[0] == "--help") {
			Console.WriteLine("usage: SplitJoin [-h] split|join ...");
		} else if (args[0] == "split") {
			parseSplit(args);
		} else if (args[0] == "join") {
			parseJoin(args);
		} else {
			Console.WriteLine("usage: SplitJoin [-h] split|join ...");
		}
	}
}
