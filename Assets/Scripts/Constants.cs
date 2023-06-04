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
            case Colors.purple: return new Color(0.5f,0.3f,0.9f);
            case Colors.cyan: return Color.cyan;
            case Colors.ocker: return new Color(0.6f, 0.8f, 0.3f);
            case Colors.magenta: return Color.magenta;
            case Colors.lightBlue: return new Color(0.3f, 0.45f, 0.9f);
            case Colors.darkGreen: return new Color(0f, 0.6f, 0.4f);
            case Colors.none: return Color.clear;
        }

        return Color.blue;
    } 
}
