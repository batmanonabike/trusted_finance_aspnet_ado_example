namespace TrustedTests.Helpers
{
    internal interface IIdTracker
    {
        void AddId(int id);
        void RemoveId(int id);
        IEnumerable<int> Ids { get; }
    }

    internal class IdTracker : IIdTracker
    {
        private readonly HashSet<int> _ids = [];
        public IEnumerable<int> Ids => [.. _ids];
        public void AddId(int id) => _ids.Add(id);
        public void RemoveId(int id) => _ids.Remove(id);
    }
}
