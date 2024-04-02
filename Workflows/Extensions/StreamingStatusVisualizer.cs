using Bonsai.Design;
using Bonsai.Expressions;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class StreamingStatusVisualizer : DialogTypeVisualizer
{
    static readonly StringFormat LabelFormat = new StringFormat(StringFormat.GenericDefault)
    {
        Alignment = StringAlignment.Center,
        LineAlignment = StringAlignment.Center,
        FormatFlags = StringFormatFlags.NoWrap,
        Trimming = StringTrimming.Character
    };

    bool? value;
    Panel lightPanel;
    Pen lightPen;
    Font labelFont;

    public override void Show(object value)
    {
        this.value = (bool)value;
        lightPanel.Invalidate();
    }

    public override void Load(IServiceProvider provider)
    {
        var context = (ITypeVisualizerContext)provider.GetService(typeof(ITypeVisualizerContext));
        var combinatorBuilder = ExpressionBuilder.GetVisualizerElement(context.Source).Builder;
        var streamingStatus = (StreamingStatus)ExpressionBuilder.GetWorkflowElement(combinatorBuilder);

        lightPen = new Pen(Brushes.Black, 4);
        lightPanel = new LightPanel();
        labelFont = new Font(lightPanel.Font, FontStyle.Bold);
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
            if (!string.IsNullOrEmpty(streamingStatus.Name))
            {
                e.Graphics.DrawString(
                    streamingStatus.Name,
                    labelFont,
                    Brushes.White,
                    rectangle,
                    LabelFormat);
            }
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
        lightPen.Dispose();
        labelFont.Dispose();
        lightPanel = null;
        lightPen = null;
        labelFont = null;
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
