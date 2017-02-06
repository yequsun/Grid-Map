using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[Serializable]
public class Grid
{
    public static Node[,] grid = new Node[120, 160];
    private ArrayList centers = new ArrayList();
    public int s_x = 0;
    public int s_y = 0;
    public int g_x = 0;
    public int g_y = 0;
    public long[] ucs = new long[3];
    public long[] astar = new long[3];
    public long[] astar2 = new long[3];
    public long[] astar3 = new long[3];
    public long[] astar4 = new long[3];
    public long[] astar5 = new long[3];
    public long nodesExpanded = 0;
    public long maxFringeSize = 0;
    private System.Random rand = new System.Random();
    private PriorityQueue fringe = new PriorityQueue();

    [Serializable]
    private class PriorityQueue
    {
        List<Node> Values = new List<Node>();
        List<double> Priorities = new List<double>();
        public int NumItems
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
        public Node parent = null;
        //[NonSerialized]
        private List<Node> neighbors = new List<Node>();
        

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
            file.WriteLine(s_x+" "+s_y);
            file.WriteLine(g_x+" "+g_y);
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

    public bool Search(bool isUniform, double w = 1)
    {
        nodesExpanded = 0;
        Node s = grid[s_x, s_y];
        s.isOptimal = true;
        s.parent = null;
        s.g = 0;
        if (isUniform)
            s.h = w * Math.Sqrt(Math.Pow(s.x - g_x, 2) + Math.Pow(s.y - g_y, 2));
        else s.h = 0;
        s.f = s.g + s.h;
        fringe.Enqueue(s, s.g);
        maxFringeSize = 1;
        HashSet<int> closed = new HashSet<int>();
        double p;
        while (fringe.NumItems != 0)
        {
            fringe.Dequeue(out s, out p);
            nodesExpanded++;
            if (s.x == g_x && s.y == g_y)
            {
                s.isOptimal = true;
                while (s.parent != null)
                {
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
                        if (isUniform)
                            n.h = w * Math.Sqrt(Math.Pow(n.x - g_x, 2) + Math.Pow(n.y - g_y, 2));
                        else n.h = 0;
                        n.f = n.g + n.h;
                        n.parent = s;
                        if (fringe.Contains(n))
                        {
                            fringe.Remove(n);
                        }
                        fringe.Enqueue(n, n.f);
                        maxFringeSize = Math.Max(fringe.NumItems, maxFringeSize);
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
        if (sRiver && nRiver)
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
        int length = 0;
        bool intersect = false;
        bool boudary = false;
        System.Random rand = new System.Random();
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
            for(int i = 0; i < 20; i++)
            {
                if (CheckBoundary(x,y))
                {
                    boudary = true;
                    break;
                }

                if (CheckIntersect(x,y))
                {
                    intersect = true;
                    break;
                }

                Node cur = grid[x, y];
                
                if(cur.GetCell() == '1')
                {
                    cur.SetCell('A');
                }

                if(cur.GetCell() == '2')
                {
                    cur.SetCell('B');
                }

                length++;

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

        if (length < 100)
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
        if(grid[x,y].GetCell() == 'A' || grid[x, y].GetCell() == 'B' || grid[x, y].GetCell() == 'a' || grid[x, y].GetCell() == 'b')
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
        if(x<0 || y<0 || x>119 || y > 159)
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
        System.Random rand = new System.Random();
        if (rand.Next(0, 100) <= 30)
        {
            return d;
        }
        else
        {
            d += rand.Next(1, 4);
            if (d >= 4)
            {
                d -= 4;
            }
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

            double a2 = Math.Pow((goalX - startX), 2);
            double b2 = Math.Pow((goalY - startY), 2);
            distance = Math.Sqrt(a2 + b2);
            Console.WriteLine("distance: " + distance);

        } while (distance < 100);

        this.s_x = startX;
        this.s_y = startY;
        this.g_x = goalX;
        this.g_y = goalY;
    }

    public bool TestSearch()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        int tests = 1;
        /*
        for (int i = 0; i < tests; i++)
        {
            if(!Search(false)) return false;
        }
        watch.Stop();
        ucs[0] = watch.ElapsedTicks / tests;
        ucs[1] = nodesExpanded;
        ucs[2] = maxFringeSize;
        Console.WriteLine("Uniform Cost:");
        Console.WriteLine("Time: "+ucs[0]+" Expansion: "+ucs[1]+" Space: "+ucs[2]);
        */
        watch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < tests; i++)
        {
            Search(true);
        }
        watch.Stop();
        astar[0] = watch.ElapsedTicks / tests;
        astar[1] = nodesExpanded;
        astar[2] = maxFringeSize;
        Console.WriteLine("A*:");
        Console.WriteLine("Time: "+astar[0]+" Expansion: "+astar[1]+" Space: "+astar[2]);
        /*
        watch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < tests; i++)
        {
            Search(true, 2);
        }
        watch.Stop();
        astar2[0] = watch.ElapsedTicks / tests;
        astar2[1] = nodesExpanded;
        astar2[2] = maxFringeSize;
        Console.WriteLine("A* (2):");
        Console.WriteLine("Time: "+astar2[0]+" Expansion: "+astar2[1]+" Space: "+astar2[2]);

        watch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < tests; i++)
        {
            Search(true, 3);
        }
        watch.Stop();
        astar3[0] = watch.ElapsedTicks / tests;
        astar3[1] = nodesExpanded;
        astar3[2] = maxFringeSize;
        Console.WriteLine("A* (3):");
        Console.WriteLine("Time: "+astar3[0]+" Expansion: "+astar3[1]+" Space: "+astar3[2]);
        
        watch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < tests; i++)
        {
            Search(true, 4);
        }
        watch.Stop();
        astar4[0] = watch.ElapsedTicks / tests;
        astar4[1] = nodesExpanded;
        astar4[2] = maxFringeSize;
        Console.WriteLine("A* (4):");
        Console.WriteLine("Time: "+astar4[0]+" Expansion: "+astar4[1]+" Space: "+astar4[2]);
        watch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < tests; i++)
        {
            Search(true, 5);
        }
        watch.Stop();
        */
        astar5[0] = watch.ElapsedTicks / tests;
        astar5[1] = nodesExpanded;
        astar5[2] = maxFringeSize;
        Console.WriteLine("A* (5):");
        Console.WriteLine("Time: "+astar5[0]+" Expansion: "+astar5[1]+" Space: "+astar5[2]);
        return true;
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

    public Node GetNode(int i, int j)
    {
        return grid[i, j];
    }

   /* public static int Main(string[] args)
    {
        Grid g = new Grid();;
        string path;
        string curPath;
        for (int j=0; j<5; j++){
            path = "map"+j+"_";
            for(int i=0;i<10;i++){
                if(!g.TestSearch()) {
                    Console.WriteLine("Search failed");
                    continue;
                }
                Console.WriteLine();
                curPath = path+i+".bin";
                WriteToBinaryFile(curPath,g);
                g.SetStartAndGoal();
            }
            g = new Grid();
        }
        return 0;
    }
    */
}