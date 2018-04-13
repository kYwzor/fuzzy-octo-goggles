using System;
using System.IO;
using System.Text;

public class TestWriter
{
    double lastWeight = -1;
    public TestWriter()
    {
    }

    public void writeResultLine(String filename, bool foundPath, double weight, int visited, int expanded, ulong maxListSize, String seed)
    {
        if(weight != lastWeight)
        {
            lastWeight = weight;
            //before your loop
            StringBuilder csv = new StringBuilder();

            //in your loop
            String fps = foundPath.ToString();
            String ws = weight.ToString();
            String vs = visited.ToString();
            String es = expanded.ToString();
            String mls = maxListSize.ToString();
            String ss = seed.ToString();
            //Suggestion made by KyleMit
            String newLine = string.Format("{0},{1},{2},{3},{4},{5}", fps, ws, vs, es, mls, ss);
            Console.WriteLine("EXORS" + newLine);
            csv.AppendLine(newLine);

            //after your loop
            File.AppendAllText(filename, csv.ToString());
        }
    }

}
