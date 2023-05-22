using UnityEngine;

public static class Constants {
    public static Color ColorForColor(Colors color) 
    {
        switch (color)
        {
            case Colors.blue: return Color.blue;
            case Colors.red: return Color.red;
            case Colors.green: return Color.green;
            case Colors.orange: return new Color(1, 0.5f, 0);
            case Colors.yellow: return Color.yellow;
            case Colors.purple: return Color.magenta;
            case Colors.none: return Color.clear;
        }

        return Color.blue;
    } 
}
