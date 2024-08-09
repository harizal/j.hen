using WApp.ViewModels.Parameters;
using WApp.ViewModels.Result;

namespace WApp.Helpers
{
    public static class DataTablePagedHelper
    {
        public static BaseDTResult<T> GetDatatablePaged<T>(IEnumerable<T> datas, BaseDTParameters parameters)
        {
            var totalDatas = datas.Count();
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
