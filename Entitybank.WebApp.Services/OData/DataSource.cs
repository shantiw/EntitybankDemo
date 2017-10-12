using System.Collections.Generic;
using XData.Data.OData;

namespace XData.Data.OData
{
    public abstract class DataSource
    {
        public string Entity { get; internal set; }
    }

    public class CountDataSource : DataSource
    {
        public string Filter { get; internal set; }
        public IReadOnlyDictionary<string, object> Parameters { get; internal set; } = new Dictionary<string, object>();
    }

    public class DefaultGetterDataSource : DataSource
    {
        public string Select { get; internal set; }
    }

    public abstract class SomeDataSource : DataSource
    {
        public string Select { get; internal set; }
        public string Filter { get; internal set; }
        public string Orderby { get; internal set; }

        public IReadOnlyDictionary<string, object> Parameters { get; internal set; } = new Dictionary<string, object>();

        public Expand[] Expands { get; internal set; }
    }

    public class CollectionDataSource : SomeDataSource
    {
    }

    public class PagingDataSource : SomeDataSource
    {
        public long PageSize { get; internal set; }
        public long PageIndex { get; internal set; }

        public long Skip { get => PageIndex * PageSize; }
        public long Top { get => PageSize; }

    }




}
