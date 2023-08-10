using UnityEngine;

public static class SliderGradient
{
    public static Color GetColorDecremental(float value)
    {
        Gradient g = new();

        GradientColorKey[] gck = new GradientColorKey[3];
        gck[0].color = Color.red;
        gck[0].time = 0.0f;
        gck[1].color = new(1f, 0.5f, 0f, 1f); // Orange
        gck[1].time = 0.5f;
        gck[2].color = Color.green;
        gck[2].time = 1.0f;

        GradientAlphaKey[] gak = new GradientAlphaKey[0];

        g.SetKeys(gck, gak);

        return g.Evaluate(value);
    }

    public static Color GetColorIncremental(float value)
    {
        Gradient g = new();

        GradientColorKey[] gck = new GradientColorKey[3];
        gck[0].color = Color.red;
        gck[0].time = 1.0f;
        gck[1].color = new(1f, 0.5f, 0f, 1f); // Orange
        gck[1].time = 0.5f;
        gck[2].color = Color.green;
        gck[2].time = 0.0f;

        GradientAlphaKey[] gak = new GradientAlphaKey[0];

        g.SetKeys(gck, gak);

        return g.Evaluate(value);
    }
}
