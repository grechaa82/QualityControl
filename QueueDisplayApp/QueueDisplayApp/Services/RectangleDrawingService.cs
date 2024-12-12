using Microsoft.Extensions.Logging;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace QueueDisplayApp.Services;

public class RectangleDrawingService : IRectangleDrawingService
{
    private readonly ILogger<RectangleDrawingService> _logger;

    public RectangleDrawingService(ILogger<RectangleDrawingService> logger)
    {
        _logger = logger;
    }

    public void DrawRectangles(Canvas canvas, IEnumerable<Element> elements)
    {
        canvas.Children.Clear();

        var reversedElements = elements.Reverse().ToList();

        double xPos = 10;
        foreach (var element in reversedElements)
        {
            var rectangle = new Rectangle
            {
                Fill = element.Status switch
                {
                    ElementStatus.Suitable => Brushes.Green,
                    ElementStatus.Marriage => Brushes.Yellow,
                    _ => Brushes.Gray
                },
                Width = element.Width,
                Height = element.Height
            };

            Canvas.SetLeft(rectangle, xPos);
            Canvas.SetTop(rectangle, 10);
            canvas.Children.Add(rectangle);

            xPos += element.Width + 10;
        }
    }
}

