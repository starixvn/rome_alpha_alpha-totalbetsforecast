namespace Alpha.TotalBetsForecast.StaticData
{
    using System.IO;
    using Alpha.StaticData;

    public class ExcelDynamicRepositoryBase : ExcelDynamicRepository
    {
        public override string GetExcelDirectory()
        {
            var rootPath = (Path.GetDirectoryName(typeof(ExcelDynamicRepositoryBase).Assembly.GetName().CodeBase).Substring(6));
            var excelPath = "ExcelData";

            return $"{rootPath}\\{excelPath}";
        }
    }
}
