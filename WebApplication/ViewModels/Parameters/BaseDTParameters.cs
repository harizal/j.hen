﻿using static WApp.Utlis.Enums;

namespace WApp.ViewModels.Parameters
{
    public class BaseDTParameters
    {
        public int Draw { get; set; }

        public DTColumn[] Columns { get; set; }

        public DTOrder[] Order { get; set; }

        public int Start { get; set; }

        public int Length { get; set; }

        public DTSearch Search { get; set; }

        public string SortOrder
        {
            get
            {
                if (Columns == null || Order == null || Order.Length == 0)
                {
                    return null;
                }

                return Columns[Order[0].Column].Data + ((Order[0].Dir == DTOrderDir.DESC) ? (" " + Order[0].Dir) : string.Empty);
            }
        }

        public IEnumerable<string> AdditionalValues { get; set; } = new List<string>();
    }

    public class DTColumn
    {
        public string Data { get; set; }

        public string Name { get; set; }

        public bool Searchable { get; set; }

        public bool Orderable { get; set; }

        public DTSearch Search { get; set; }
    }

    public class DTSearch
    {
        public string Value { get; set; }

        public bool Regex { get; set; }
    }

    public class DTOrder
    {
        public int Column { get; set; }

        public DTOrderDir Dir { get; set; }
    }
}
