using System.Reflection;
using WApp.ViewModels.Parameters;
using WApp.ViewModels.Result;

namespace WApp.Helpers
{
    public static class DataTablePagedHelper
    {
        public static BaseDTResult<T> GetDatatablePaged<T>(IEnumerable<T> datas, BaseDTParameters parameters)
        {
            var totalDatas = datas.Count();
            // Apply sorting
            if (!string.IsNullOrEmpty(parameters.SortOrder) && parameters.SortOrder != "id")
            {
                var sortColumn = parameters.SortOrder.Split(' ')[0];
                var sortDirection = parameters.SortOrder.Contains("DESC") ? "Descending" : "Ascending";

                // Use reflection to dynamically order the data (case-insensitive)
                var propertyInfo = typeof(T).GetProperty(sortColumn, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    datas = sortDirection == "Ascending"
                        ? datas.OrderBy(x => propertyInfo.GetValue(x, null))
                        : datas.OrderByDescending(x => propertyInfo.GetValue(x, null));
                }
            }

            var results = datas.Skip((parameters.Start / parameters.Length) * parameters.Length).Take(parameters.Length);
            return new BaseDTResult<T>
            {
                Data = results,
                Draw = parameters.Draw,
                RecordsFiltered = totalDatas,
                RecordsTotal = totalDatas
            };
        }
    }
}
