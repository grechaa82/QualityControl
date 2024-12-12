using Microsoft.Extensions.Logging;

namespace QueueDisplayApp.Services;

public class QueueService : IQueueService
{

    private const int MaxSize = 5;
    private readonly Queue<Element> queueElements = new Queue<Element>();
    private readonly ILogger<QueueService> _logger;

    public QueueService(ILogger<QueueService> logger)
    {
        _logger = logger;
    }

    public void AddElement(ElementStatus elementStatus)
    {
        if (queueElements.Count < MaxSize)
        {
            Element rectangle = new Element
            {
                Status = elementStatus
            };

            queueElements.Enqueue(rectangle);
        }
    }

    public void RemoveElement()
    {
        if (queueElements.Count > 0)
        {
            queueElements.Dequeue();
        }
    }

    public IEnumerable<Element> GetElements()
    {
        return queueElements.ToArray();
    }
}