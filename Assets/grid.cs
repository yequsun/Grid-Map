using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[Serializable]
public class Grid
{
    public static Node[,] grid = new Node[120, 160];
    private ArrayList centers = new ArrayList();
    private List<long[]> metrics = new List<long[]>();
    public int s_x = 0;
    public int s_y = 0;
    public int g_x = 0;
    public int g_y = 0;
    public long nodesExpanded = 0;
    public long maxFringeSize = 0;
    public double pathCost = 0;
    private Random rand = new Random();
    private PriorityQueue fringe = new PriorityQueue();
    private PriorityQueue[] open;
    private HashSet<Node>[] closed;
    private Node[] bp;
    public double w1 = 1;
    public double w2 = 1;

    public Node GetNode(int i, int j)
    {
        return grid[i, j];
    }
    [Serializable]
    private class PriorityQueue
    {
        List<Node> Values = new List<Node>();
        List<double> Priorities = new List<double>();
        public int Count
        {
            get
            {
                return Values.Count;
            }
        }

        public void Enqueue(Node new_value, double new_priority)
        {
            Values.Add(new_value);
            Priorities.Add(new_priority);
        }

        public void Dequeue(out Node top_value, out double top_priority)
        {
            int best_index = 0;
            double best_priority = Priorities[0];
            for (int i = 1; i < Priorities.Count; i++)
            {
                if (best_priority > Priorities[i])
                {
                    best_priority = Priorities[i];
                    best_index = i;
                }
            }

            top_value = Values[best_index];
            top_priority = best_priority;

            Values.RemoveAt(best_index);
            Priorities.RemoveAt(best_index);
        }

        public Node Top()
        {
            int best_index = 0;
            if (Priorities.Count > 0)
            {
                double best_priority = Priorities[0];
                for (int i = 1; i < Priorities.Count; i++)
                {
                    if (best_priority > Priorities[i])
                    {
                        best_priority = Priorities[i];
                        best_index = i;
                    }
                }
                return Values[best_index];
            }
            return null;
        }

        public double MinKey()
        {
            if (Priorities.Count > 0)
            {
                double best_priority = Priorities[0];
                for (int i = 1; i < Priorities.Count; i++)
                {
                    if (best_priority > Priorities[i])
                    {
                        best_priority = Priorities[i];
                    }
                }
                return best_priority;
            }
            return 0;
        }

        public void Remove(Node n)
        {
            int i = Values.IndexOf(n);
            Values.RemoveAt(i);
            Priorities.RemoveAt(i);
        }

        public bool Contains(Node n)
        {
            foreach (Node v in Values)
            {
                if (n.x == v.x && n.y == v.y) return true;
            }
            return false;
        }
    }

    [Serializable]
    public class Node
    {
        private char cell;
        public int x = -1;
        public int y = -1;
        public double f = -1;
        public double g = -1;
        public double h = -1;
        public bool isOptimal = false;
        private List<Node> neighbors = new List<Node>();
        public Node parent = null;
        public double[] g2 = new double[5];
        public Node[] bp = new Node[5];

        public Node(int x, int y)
        {
            cell = '1';
            this.x = x;
            this.y = y;
        }

        public Node(char c, int x, int y)
        {
            cell = c;
            this.x = x;
            this.y = y;
        }

        public int GetX()
        {
            return x;
        }

        public int GetY()
        {
            return y;
        }
        public void SetCell(char c)
        {
            cell = c;
        }

        public char GetCell()
        {
            return cell;
        }

        public void SetNeighbors()
        {
            for (int i = x - 1; i < x + 2; i++)
            {
                for (int j = y - 1; j < y + 2; j++)
                {
                    try
                    {
                        if (!(i == x && j == y))
                        {
                            neighbors.Add(grid[i, j]);
                        }
                    }
                    catch { }
                }
            }
        }

        public List<Node> GetNeighbors()
        {
            return neighbors;
        }
    }

    public Grid()
    {
        for (int i = 0; i < 120; i++)
        {
            for (int j = 0; j < 160; j++)
            {
                grid[i, j] = new Node(i, j);
            }
        }

        for (int i = 0; i < 120; i++)
        {
            for (int j = 0; j < 160; j++)
            {
                grid[i, j].SetNeighbors();
            }
        }

        SetHardCells();
        SetHighways();
        SetBlockedCells();
        SetStartAndGoal();
    }

    public Grid(string path)
    {
        string[] lines = System.IO.File.ReadAllLines(path);
        s_x = Convert.ToInt32(lines[0].Substring(0, lines[0].IndexOf(" ") + 1));
        s_y = Convert.ToInt32(lines[0].Substring(lines[0].IndexOf(" ") + 1));
        g_x = Convert.ToInt32(lines[1].Substring(0, lines[1].IndexOf(" ") + 1));
        g_y = Convert.ToInt32(lines[1].Substring(lines[1].IndexOf(" ") + 1));
        for (int i = 2; i < 10; i++)
        {
            centers.Add(lines[i]);
        }

        for (int i = 10; i < 130; i++)
        {
            for (int j = 0; j < 320; j += 2)
            {
                grid[i - 10, j / 2] = new Node(lines[i][j], i - 10, j / 2);
            }
        }

        for (int i = 10; i < 130; i++)
        {
            for (int j = 0; j < 320; j += 2)
            {
                grid[i - 10, j / 2].SetNeighbors();
            }
        }
    }

    private void SetHardCells()
    {
        int row = 0, col = 0;
        while (centers.Count < 8)
        {
            row = rand.Next(0, 120);
            col = rand.Next(0, 160);
            string pair = row.ToString() + " " + col.ToString();
            if (centers.Contains(pair)) continue;
            centers.Add(pair);
            for (int i = row - 15; i <= row + 15; i++)
            {
                if (i >= 120) break;
                if (i < 0) i = 0;
                for (int j = col - 15; j <= col + 15; j++)
                {
                    if (j >= 160) break;
                    if (j < 0) j = 0;
                    if (rand.Next(0, 2) == 1)
                    {
                        grid[i, j].SetCell('2');
                    }
                }
            }
        }
    }

    private void SetBlockedCells()
    {
        int i = 0, row = 0, col = 0;
        while (i < 3840)
        {
            row = rand.Next(0, 120);
            col = rand.Next(0, 160);
            if (grid[row, col].GetCell() == '1' || grid[row, col].GetCell() == '2')
            {
                grid[row, col].SetCell('0');
                i++;
            }
        }
    }

    public void OutputGrid(string path)
    {
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@path))
        {
            file.WriteLine(s_x + " " + s_y);
            file.WriteLine(g_x + " " + g_y);
            foreach (string s in centers)
            {
                file.WriteLine(s);
            }
            for (int i = 0; i < 120; i++)
            {
                for (int j = 0; j < 160; j++)
                {
                    file.Write(grid[i, j].GetCell() + " ");
                }
                file.WriteLine();
            }
        }
    }

    public bool Search(Func<int, int, int, int, double> h, double w = 1)
    {
        nodesExpanded = 0;
        Node s = grid[s_x, s_y];
        s.isOptimal = true;
        s.parent = null;
        s.g = 0;
        s.h = w * h(s.x, s.y, g_x, g_y);
        s.f = s.g + s.h;
        fringe.Enqueue(s, s.g);
        maxFringeSize = 1;
        HashSet<int> closed = new HashSet<int>();
        double p;
        while (fringe.Count != 0)
        {
            fringe.Dequeue(out s, out p);
            nodesExpanded++;
            if (s.x == g_x && s.y == g_y)
            {
                s.isOptimal = true;
                pathCost = 0;
                while (s.parent != null)
                {
                    pathCost += Cost(s, s.parent);
                    s = s.parent;
                    s.isOptimal = true;
                }
                fringe = new PriorityQueue();
                return true;
            }
            closed.Add(s.GetHashCode());
            foreach (Node n in s.GetNeighbors())
            {
                if (!closed.Contains(n.GetHashCode()))
                {
                    if (!fringe.Contains(n))
                    {
                        n.g = Double.PositiveInfinity;
                    }
                    double c = Cost(s, n);
                    if (s.g + c < n.g)
                    {
                        n.g = s.g + c;
                        n.h = w * h(s.x, s.y, g_x, g_y); ;
                        n.f = n.g + n.h;
                        n.parent = s;
                        if (fringe.Contains(n))
                        {
                            fringe.Remove(n);
                        }
                        fringe.Enqueue(n, n.f);
                        maxFringeSize = Math.Max(fringe.Count, maxFringeSize);
                    }
                }
            }
        }
        return false;
    }

    private void MapCell(int c, out int cell, out bool h)
    {
        if (c == 'a')
        {
            cell = '1';
            h = true;
        }
        else if (c == 'b')
        {
            cell = '2';
            h = true;
        }
        else
        {
            cell = c;
            h = false;
        }
    }

    private double Cost(Node s, Node n)
    {
        int sCell, nCell;
        bool sRiver, nRiver;
        MapCell(s.GetCell(), out sCell, out sRiver);
        MapCell(n.GetCell(), out nCell, out nRiver);
        if (nCell == '0')
            return Double.PositiveInfinity;

        double c = 1.0;
        if (Math.Abs(s.x - n.x) == 1 && Math.Abs(s.y - n.y) == 1)
            c = Math.Sqrt(2.0);
        if (Math.Abs(sCell - nCell) == 1)
            c *= 1.5;
        else if (sCell == '2')
            c *= 2.0;
        if (sRiver && nRiver && (s.x == n.x || s.y == n.y))
            c /= 4.0;
        return c;
    }

    private void SetHighways()
    {
        //set 4 highways
        int i = 0;
        while (i < 4)
        {
            if (SetHighway())
            {
                i++;
            }
        }
        return;
    }

    private bool SetHighway()
    {
        int direction = 0; //initial direction and boudary
        int x = 0; // origin x coordinate
        int y = 0; // origin y coordinate
        int Cost = 0;
        bool intersect = false;
        bool boudary = false;
        direction = rand.Next(0, 4);//0 top-down 1 right-left 2 bottom-up 3 left-right
        switch (direction)
        {
            case 0:
                y = rand.Next(1, 159);
                x = 0;
                break;
            case 1:
                y = 159;
                x = rand.Next(1, 119);
                break;
            case 2:
                y = rand.Next(1, 159);
                x = 119;
                break;
            case 3:
                y = 0;
                x = rand.Next(1, 119);
                break;
        }

        while (!boudary && !intersect)
        {
            for (int i = 0; i < 20; i++)
            {
                if (CheckBoundary(x, y))
                {
                    boudary = true;
                    break;
                }

                if (CheckIntersect(x, y))
                {
                    intersect = true;
                    break;
                }
                Node cur = grid[x, y];

                if (cur.GetCell() == '1')
                {
                    cur.SetCell('A');
                }

                if (cur.GetCell() == '2')
                {
                    cur.SetCell('B');
                }

                Cost++;

                if (i != 19)
                {
                    switch (direction)
                    {
                        case 0:
                            x++;
                            break;
                        case 1:
                            y--;
                            break;
                        case 2:
                            x--;
                            break;
                        case 3:
                            y++;
                            break;
                    }
                }
            }
            //change direction
            if (boudary || intersect)
            {
                break;
            }
            direction = ChangeDirection(direction);
            //set next point
            switch (direction)
            {
                case 0:
                    x++;
                    break;
                case 1:
                    y--;
                    break;
                case 2:
                    x--;
                    break;
                case 3:
                    y++;
                    break;
            }
        }

        if (intersect)
        {
            EraseHighway();
            return false;
        }

        if (Cost < 100)
        {
            EraseHighway();
            return false;
        }
        else
        {
            FinalizeHighway();
            return true;
        }
    }

    private bool CheckIntersect(int x, int y)
    {
        if (grid[x, y].GetCell() == 'A' || grid[x, y].GetCell() == 'B' || grid[x, y].GetCell() == 'a' || grid[x, y].GetCell() == 'b')
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckBoundary(int x, int y)
    {
        if (x < 0 || y < 0 || x > 119 || y > 159)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void EraseHighway()
    {
        Node cur;
        for (int i = 0; i < 120; i++)
        {
            for (int j = 0; j < 160; j++)
            {
                cur = grid[i, j];
                if (cur.GetCell().Equals('A'))
                {
                    cur.SetCell('1');
                }
                if (cur.GetCell().Equals('B'))
                {
                    cur.SetCell('2');
                }
            }
        }
    }

    private void FinalizeHighway()
    {
        Node cur;
        for (int i = 0; i < 120; i++)
        {
            for (int j = 0; j < 160; j++)
            {
                cur = grid[i, j];
                if (cur.GetCell().Equals('A'))
                {
                    cur.SetCell('a');
                }
                if (cur.GetCell().Equals('B'))
                {
                    cur.SetCell('b');
                }
            }
        }
    }

    private int ChangeDirection(int d)
    {
        if (rand.Next(0, 100) <= 60)
        {
            return d;
        }
        else
        {
            if (rand.Next(0, 100) < 50)
            {
                d += 1;
            }
            else
            {
                d -= 1;
            }
            d = d % 4;
            return d;
        }
    }

    public void SetStartAndGoal()
    {
        int startRegion, goalRegion;
        startRegion = rand.Next(0, 4);
        goalRegion = rand.Next(0, 4);
        //0 top20 1 bottom20 2 leftmost20 3 rightmost20
        int startX = 0, startY = 0, goalX = 0, goalY = 0;
        double distance;
        Node start, goal;

        do
        {
            do
            {
                switch (startRegion)
                {
                    case 0:
                        startY = rand.Next(0, 160);
                        startX = rand.Next(0, 20);
                        break;
                    case 1:
                        startY = rand.Next(0, 160);
                        startX = rand.Next(99, 120);
                        break;
                    case 2:
                        startY = rand.Next(0, 20);
                        startX = rand.Next(0, 120);
                        break;
                    case 3:
                        startY = rand.Next(139, 160);
                        startX = rand.Next(0, 120);
                        break;
                }
                start = grid[startX, startY];

            } while (!start.GetCell().Equals('1') && !start.GetCell().Equals('2'));

            do
            {
                switch (goalRegion)
                {
                    case 0:
                        goalY = rand.Next(0, 160);
                        goalX = rand.Next(0, 20);
                        break;
                    case 1:
                        goalY = rand.Next(0, 160);
                        goalX = rand.Next(99, 120);
                        break;
                    case 2:
                        goalY = rand.Next(0, 20);
                        goalX = rand.Next(0, 120);
                        break;
                    case 3:
                        goalY = rand.Next(139, 160);
                        goalX = rand.Next(0, 120);
                        break;
                }

                goal = grid[goalX, goalY];

            } while (!goal.GetCell().Equals('1') && !goal.GetCell().Equals('2'));

            distance = (goalX - startX) ^ 2 + (goalY - startY) ^ 2;
            distance = Math.Sqrt(distance);

        } while (distance < 100);
        this.s_x = startX;
        this.s_y = startY;
        this.g_x = goalX;
        this.g_y = goalY;
    }

    public static void WriteToBinaryFile(string filePath, Grid objectToWrite, bool append = false)
    {
        using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
        {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            binaryFormatter.Serialize(stream, objectToWrite);
        }
    }

    public static Grid ReadFromBinaryFile(string filePath)
    {
        using (Stream stream = File.Open(filePath, FileMode.Open))
        {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            return (Grid)binaryFormatter.Deserialize(stream);
        }
    }

    private static double NoHeuristic(int s_x, int s_y, int g_x, int g_y)
    {
        return 0;
    }
    private static double Euclidean(int s_x, int s_y, int g_x, int g_y)
    {
        return Math.Sqrt(Math.Pow(s_x - g_x, 2) + Math.Pow(s_y - g_y, 2));
    }
    private static double EuclideanBy4(int s_x, int s_y, int g_x, int g_y)
    {
        return Euclidean(s_x, s_y, g_x, g_y) / 4;
    }

    private static double Manhattan(int s_x, int s_y, int g_x, int g_y)
    {
        double dx = s_x - g_x;
        double dy = s_y - g_y;
        double dMan = Math.Abs(dx) + Math.Abs(dy);
        return dMan;
    }

    private static double ManhattanBy4(int s_x, int s_y, int g_x, int g_y)
    {
        return Manhattan(s_x, s_y, g_x, g_y) / 4;
    }

    private static double Chebyshev(int s_x, int s_y, int g_x, int g_y)
    {
        return Math.Max(Math.Abs(s_x - g_x), Math.Abs(s_y - g_y));
    }

    private static double ChebyshevBy4(int s_x, int s_y, int g_x, int g_y)
    {
        return Chebyshev(s_x, s_y, g_x, g_y) / 4;
    }
    private double NearestCenter(int x, int y, int dummy1, int dummy2)
    {
        double[] distances = new double[8];
        string[] pair;
        for (int i = 0; i < 8; i++)
        {
            string cur = this.centers[i].ToString();
            pair = cur.Split(' ');
            int x1 = Convert.ToInt32(pair[0]);
            int y1 = Convert.ToInt32(pair[1]);
            distances[i] = Euclidean(x, y, x1, y1);
        }
        return -distances.Min();
    }

    public static int Main(string[] args)
    {
        Grid g = new Grid();
        string path;
        string curPath;
        long[] m;
        long[,] avgs = new long[4, 4];
        for (int j = 0; j < 5; j++)
        {
            path = "maps/map" + j + "_";
            for (int i = 0; i < 1; i++)
            {
                // g = ReadFromBinaryFile(path+i+".bin");
                // for(int k=0; k<4; k++) {
                //     m = g.metrics[k];
                //     for(int l=0; l<4;l++) {
                //         avgs[k,l] += m[l];
                //     }
                // }
                g.w2 = 1.25;
                g.TestSearch();
                g.w1 = 2;
                g.TestSearch();
                g.w2 = 2;
                g.w1 = 1;
                g.TestSearch();
                g.w2 = 1;
                g.w1 = 3;
                g.TestSearch();

                curPath = path + i + ".bin";
                WriteToBinaryFile(curPath, g);
                g.SetStartAndGoal();
            }
            g = new Grid();
        }
        // for (int a=0;a<4;a++) {
        //     for(int b=0;b<4;b++) {
        //         Console.Write(avgs[a,b]/50 + " ");
        //     }
        // Console.WriteLine();
        // }
        return 0;
    }

    private void TestSearch()
    {
        long[] m = new long[4];
        var watch = System.Diagnostics.Stopwatch.StartNew();
        int tests = 10;
        for (int i = 0; i < tests; i++)
        {
            IntergratedSearch();
        }
        watch.Stop();
        m[0] = watch.ElapsedTicks / tests;
        m[1] = nodesExpanded;
        m[2] = (long)pathCost;
        m[3] = maxFringeSize;
        metrics.Add(m);
        foreach (long i in m)
        {
            Console.Write(i + " ");
        }
        Console.WriteLine();
    }

    public void SequentialSearch()
    {
        maxFringeSize = 0;
        open = new PriorityQueue[5];
        closed = new HashSet<Node>[5];
        Node s = grid[s_x, s_y];
        Node g = grid[g_x, g_y];
        s.bp = new Node[5];
        g.bp = new Node[5];
        for (int i = 0; i < 5; i++)
        {
            open[i] = new PriorityQueue();
            closed[i] = new HashSet<Node>();
            s.g2[i] = 0;
            g.g2[i] = Double.PositiveInfinity;
            open[i].Enqueue(s, Key(s, i));
        }
        double p;
        while (open[0].MinKey() < Double.PositiveInfinity)
        {
            for (int i = 1; i < 5; i++)
            {
                if (open[i].MinKey() <= w2 * open[0].MinKey())
                {
                    if (g.g2[i] < Double.PositiveInfinity)
                    {
                        pathCost = 0;
                        while (g.bp[i] != null)
                        {
                            g.isOptimal = true;
                            try
                            {
                                pathCost += Cost(g, g.bp[i]);
                            }
                            catch { }
                            g = g.bp[i];
                        }
                        g.isOptimal = true;
                        nodesExpanded = 0;
                        for (int j = 0; j < 5; j++)
                            nodesExpanded += closed[j].Count;
                        return;
                    }
                    else
                    {
                        s = open[i].Top();
                        ExpandState(s, i);
                        closed[i].Add(s);
                    }
                }
                else if (g.g2[0] < Double.PositiveInfinity && s.g2[0] <= open[0].MinKey())
                {
                    pathCost = 0;
                    while (g.bp[0] != null)
                    {
                        g.isOptimal = true;
                        try
                        {
                            pathCost += Cost(g, g.bp[0]);
                        }
                        catch { }
                        g = g.bp[0];
                    }
                    g.isOptimal = true;
                    nodesExpanded = 0;
                    for (int j = 0; j < 5; j++)
                        nodesExpanded += closed[j].Count;
                    return;
                }
                else
                {
                    s = open[0].Top();
                    ExpandState(s, 0);
                    closed[0].Add(s);
                }
            }
        }
        Console.WriteLine("Fail");
    }

    public double Key(Node n, int i)
    {
        //node n: node to work with
        //int i: h function index
        //double w1: weight w1
        Func<int, int, int, int, double> hf;
        switch (i)
        {
            case 0:
                hf = ManhattanBy4;
                break;
            case 1:
                hf = Euclidean;
                break;
            case 2:
                hf = Chebyshev;
                break;
            case 3:
                hf = NearestCenter;
                break;
            case 4:
                hf = Manhattan;
                break;
            default:
                hf = ManhattanBy4;
                break;
        }
        double h = hf(n.x, n.y, g_x, g_y);
        return n.g2[i] + w1 * h;
    }

    public void ExpandState(Node s, int i)
    {
        open[i].Remove(s);
        foreach (Node n in s.GetNeighbors())
        {
            if (!closed[i].Contains(n))
            {
                n.g2[i] = Double.PositiveInfinity;
                n.bp[i] = null;
            }
            if (n.g2[i] > s.g2[i] + Cost(s, n))
            {
                n.g2[i] = s.g2[i] + Cost(s, n);
                n.bp[i] = s;
                if (!closed[i].Contains(n))
                {
                    try
                    {
                        open[i].Remove(n);
                    }
                    catch { }
                    open[i].Enqueue(n, Key(n, i));
                }
            }
        }
    }

    public void IntergratedSearch()
    {
        Node s = grid[s_x, s_y];
        Node g = grid[g_x, g_y];
        open = new PriorityQueue[5];
        closed = new HashSet<Node>[2];
        s.bp = new Node[1];
        g.bp = new Node[1];
        //use node.parent as bp
        for (int i = 0; i < 5; i++)
        {
            open[i] = new PriorityQueue();
            s.g2[i] = 0;
            g.g2[i] = Double.PositiveInfinity;
            open[i].Enqueue(s, KeyI(s, i));
        }

        closed[0] = new HashSet<Node>();
        closed[1] = new HashSet<Node>();

        while (open[0].MinKey() < Double.PositiveInfinity)
        {
            for (int i = 1; i < 5; i++)
            {
                // Console.WriteLine(closed[0].Count + " " + closed[1].Count);
                if (open[i].MinKey() <= w2 * open[0].MinKey())
                {
                    if (g.g2[0] <= open[i].MinKey())
                    {
                        if (g.g2[0] < Double.PositiveInfinity)
                        {
                            pathCost = 0;
                            while (g.bp[0] != null)
                            {
                                g.isOptimal = true;
                                try
                                {
                                    pathCost += Cost(g, g.bp[0]);
                                }
                                catch { }
                                g = g.bp[0];
                            }
                            g.isOptimal = true;
                            nodesExpanded = 0;
                            for (int j = 0; j < 2; j++)
                            {
                                nodesExpanded += closed[j].Count;
                            }
                            return;
                        }
                    }
                    else
                    {
                        s = open[i].Top();
                        if (s == null) continue;
                        // Console.WriteLine(i + " " +open[i].Count);
                        ExpandStateI(s);
                        closed[1].Add(s);
                    }
                }
                else if (g.g2[0] < Double.PositiveInfinity && s.g2[0] <= open[0].MinKey())
                {
                    pathCost = 0;
                    while (g.bp[0] != null)
                    {
                        g.isOptimal = true;
                        try
                        {
                            pathCost += Cost(g, g.bp[0]);
                        }
                        catch { }
                        g = g.bp[0];
                    }
                    g.isOptimal = true;
                    nodesExpanded = 0;
                    for (int j = 0; j < 2; j++)
                        nodesExpanded += closed[j].Count;
                    return;
                }
                else
                {
                    s = open[0].Top();
                    if (s == null) continue;
                    // Console.WriteLine(i + " " +open[i].Count);
                    ExpandStateI(s);
                    closed[0].Add(s);
                }
            }
        }

    }
    public void ExpandStateI(Node s)
    {
        // Console.WriteLine(s==null);
        for (int i = 0; i < 5; i++)
            try
            {
                open[i].Remove(s);
            }
            catch { }
        long f = 0;
        for (int j = 0; j < 5; j++)
        {
            f += open[j].Count;
        }

        maxFringeSize = Math.Max(f, maxFringeSize);
        foreach (Node n in s.GetNeighbors())
        {
            if (!closed[0].Contains(n) && !closed[1].Contains(n))
            {
                n.g2[0] = Double.PositiveInfinity;
                n.bp[0] = null;
            }
            if (n.g2[0] > s.g2[0] + Cost(s, n))
            {
                n.g2[0] = s.g2[0] + Cost(s, n);
                n.bp[0] = s;
                if (!closed[0].Contains(n))
                {
                    try
                    {
                        open[0].Remove(n);
                    }
                    catch { }
                    open[0].Enqueue(n, KeyI(n, 0));
                    // Console.WriteLine(0 + " enqueue");
                    if (!closed[1].Contains(n))
                    {
                        for (int i = 1; i < 5; i++)
                        {
                            if (KeyI(n, i) <= w2 * KeyI(n, 0))
                            {
                                try
                                {
                                    open[i].Remove(n);
                                }
                                catch { }
                                // Console.WriteLine(i + " enqueue");
                                open[i].Enqueue(n, KeyI(n, i));
                            }
                        }
                    }
                }
            }
        }

    }

    public double KeyI(Node n, int i)
    {
        //node n: node to work with
        //int i: h function index
        //double w1: weight w1
        Func<int, int, int, int, double> hf;
        switch (i)
        {
            case 0:
                hf = ManhattanBy4;
                break;
            case 1:
                hf = Euclidean;
                break;
            case 2:
                hf = Chebyshev;
                break;
            case 3:
                hf = NearestCenter;
                hf = ChebyshevBy4;
                break;
            case 4:
                hf = Manhattan;
                break;
            default:
                hf = ManhattanBy4;
                break;
        }
        double h = hf(n.x, n.y, g_x, g_y);
        return n.g2[0] + w1 * h;
    }
}