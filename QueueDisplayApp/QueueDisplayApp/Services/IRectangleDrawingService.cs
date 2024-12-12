using System.Windows.Controls;

namespace QueueDisplayApp.Services;

public interface IRectangleDrawingService
{
    void DrawRectangles(Canvas canvas, IEnumerable<Element> elements);
}
