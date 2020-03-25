using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace MediaRecommender.Data.MoviesInternalDb
{
    public class DapperConfigurationHelpers
    {
        #region Private Methods

        internal static string GetColumnAttributeValue(PropertyInfo propertyInfo)
            => propertyInfo.GetCustomAttributes(false).OfType<ColumnAttribute>().FirstOrDefault()?.Name;

        #endregion
    }
}
