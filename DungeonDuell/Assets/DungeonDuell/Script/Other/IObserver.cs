namespace dungeonduell
{
    public interface IObserver
    {
        public void SubscribeToEvents();
        public void UnsubscribeToAllEvents();
    }
}