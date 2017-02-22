using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class H : MonoBehaviour {

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
        return Chebyshev(s_x, s_y, g_x, g_y)/4;
    }




}
