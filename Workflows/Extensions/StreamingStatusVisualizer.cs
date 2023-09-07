using Bonsai.Design;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class StreamingStatusVisualizer : DialogTypeVisualizer
{
    bool? value;
    Panel lightPanel;
    Pen lightPen;

    public override void Show(object value)
    {
        this.value = (bool)value;
        lightPanel.Invalidate();
    }

    public override void Load(IServiceProvider provider)
    {
        lightPen = new Pen(Brushes.Black, 4);
        lightPanel = new LightPanel();
        lightPanel.Dock = DockStyle.Fill;
        lightPanel.Resize += delegate { lightPanel.Invalidate(); };
        lightPanel.Paint += (sender, e) =>
        {
            var brush = value.HasValue
                ? value.GetValueOrDefault() ? Brushes.Green : Brushes.Red
                : Brushes.Gray;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var rectangle = lightPanel.ClientRectangle;
            rectangle.Inflate((int)-lightPen.Width, (int)-lightPen.Width);
            e.Graphics.FillEllipse(brush, rectangle);
            e.Graphics.DrawEllipse(lightPen, rectangle);
        };

        var visualizerService = (IDialogTypeVisualizerService)provider.GetService(typeof(IDialogTypeVisualizerService));
        if (visualizerService != null)
        {
            visualizerService.AddControl(lightPanel);
        }
    }

    public override void Unload()
    {
        lightPanel.Dispose();
        lightPanel = null;
    }

    class LightPanel : Panel
    {
        public LightPanel()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}
