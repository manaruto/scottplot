namespace ScottPlot.Plottables;

/// <summary>
/// Holds a collection of individually styled bars
/// </summary>
public class BarPlot : IPlottable, IHasLegendText, IRenderLast
{
    [Obsolete("use LegendText")]
    public string Label { get => LegendText; set => LegendText = value; }
    public string LegendText { get; set; } = string.Empty;

    public bool IsVisible { get; set; } = true;
    public IAxes Axes { get; set; } = new Axes();

    public List<Bar> Bars { get; } // TODO: bar plot data source?

    public bool LabelsOnTop { set => Bars.ForEach(x => x.LabelOnTop = value); }

    public LabelStyle ValueLabelStyle { get; set; } = new()
    {
        Alignment = Alignment.LowerCenter,
    };

    /// <summary>
    /// Text displayed above each bar, typically containing string representation of the value.
    /// This label is displayed below the bar for negative bars.
    /// </summary>
    public string ValueLabel
    {
        get => ValueLabelStyle.Text;
        set => ValueLabelStyle.Text = value;
    }

    /// <summary>
    /// Apply a fill color to all bars
    /// </summary>
    public Color Color
    {
        set
        {
            foreach (Bar bar in Bars)
            {
                bar.FillColor = value;
            }
        }
    }

    /// <summary>
    /// Define orientation for all bars
    /// </summary>
    public bool Horizontal
    {
        set
        {
            foreach (Bar bar in Bars)
            {
                bar.Orientation = value
                    ? Orientation.Horizontal
                    : Orientation.Vertical;
            }
        }
    }

    public BarPlot(List<Bar> bars)
    {
        Bars = bars;
    }

    public IEnumerable<LegendItem> LegendItems
    {
        get
        {
            if (Bars.Count == 0)
            {
                return LegendItem.None;
            }

            LegendItem item = new()
            {
                Plottable = this,
                LabelText = LegendText,
                FillColor = Bars.First().FillColor,
            };

            return LegendItem.Single(item);
        }
    }

    public AxisLimits GetAxisLimits()
    {
        ExpandingAxisLimits limits = new();

        foreach (Bar bar in Bars)
        {
            limits.Expand(bar.AxisLimits);
        }

        return limits.AxisLimits;
    }

    public virtual void Render(RenderPack rp)
    {
        using SKPaint paint = new();

        foreach (Bar bar in Bars)
        {
            bar.RenderBody(rp, Axes, paint);
            if (!bar.LabelOnTop)
            {
                ValueLabelStyle.Text = bar.Label;
                bar.RenderText(rp, Axes, paint, ValueLabelStyle);
            }
        }
    }

    public virtual void RenderLast(RenderPack rp)
    {
        using SKPaint paint = new();

        foreach (Bar bar in Bars)
        {
            if (bar.LabelOnTop)
            {
                ValueLabelStyle.Text = bar.Label;
                bar.RenderText(rp, Axes, paint, ValueLabelStyle);
            }
        }
    }
}
