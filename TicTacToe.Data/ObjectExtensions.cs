namespace TicTacToe.Data
{
    using System.Linq;

    internal static class ObjectExtensions
    {
        internal static object GetId(this object obj)
        {
            var type = obj.GetType();
            var idProperty =
                type
                    .GetProperties()
                    .FirstOrDefault(p => p.CustomAttributes.Any(a => a.GetType().Name == "KeyAttribute"))
                ?? type.GetProperty("Id")
                ?? type.GetProperty(string.Format("{0}Id", type.Name));

            return idProperty.GetValue(obj);
        }
    }
}
