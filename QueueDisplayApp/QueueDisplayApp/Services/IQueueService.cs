namespace QueueDisplayApp.Services;

public interface IQueueService
{
    void AddElement(ElementStatus elementStatus);

    void RemoveElement();

    IEnumerable<Element> GetElements();
}
