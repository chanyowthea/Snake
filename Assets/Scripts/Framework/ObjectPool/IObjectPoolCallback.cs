public interface IObjectPoolCallback
{
    void OnAllocated();
    void OnCollected();
}
