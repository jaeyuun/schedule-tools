namespace ScheduleTools.Core.Collections
{
    public sealed class Paginator<T>
    {
        private readonly List<T> _items = new List<T>();

        public int Count => _items.Count;

        public void Reset(IEnumerable<T>? items)
        {
            _items.Clear();

            if (items != null)
            {
                _items.AddRange(items);
            }
        }

        public List<T> GetPageItems(int pageIndex, int pageSize)
        {
            var result = new List<T>();

            if (_items.Count == 0 || pageSize <= 0)
            {
                return result;
            }

            var normalizedIndex = NormalizePageIndex(pageIndex, pageSize);
            var start = normalizedIndex * pageSize;
            var end = Math.Min(start + pageSize, _items.Count);

            for (var i = start; i < end; i++)
            {
                result.Add(_items[i]);
            }

            return result;
        }

        public int GetPageCount(int pageSize)
        {
            if (_items.Count == 0 || pageSize <= 0)
            {
                return 1;
            }

            return (_items.Count + pageSize - 1) / pageSize;
        }

        public int NormalizePageIndex(int pageIndex, int pageSize)
        {
            var pageCount = GetPageCount(pageSize);
            return Math.Clamp(pageIndex, 0, pageCount - 1);
        }
    }
}
