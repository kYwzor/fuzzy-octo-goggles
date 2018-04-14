using System;
using System.IO;
using System.Text;

public class TestWriter
{
	static private string filename;

    static public void Start(String filename)
    {
		TestWriter.filename = filename;
		File.AppendAllText (filename, "foundPath,weight,visited,expanded,maxListSize,depth,extra\n");
    }

    static public void writeResultLine(bool foundPath, double weight, int visited, int expanded, ulong maxListSize, int depth, String extra)
    {
		string fs = foundPath.ToString ();
        string ws = weight.ToString();
        string vs = visited.ToString();
        string es = expanded.ToString();
        string ls = maxListSize.ToString();
		string ds = depth.ToString();

		string newLine = String.Format("{0},{1},{2},{3},{4},{5},{6}\n", fs, ws, vs, es, ls, ds, extra);
        Console.WriteLine("EXORS" + newLine);

		File.AppendAllText(filename, newLine);
    }

}
