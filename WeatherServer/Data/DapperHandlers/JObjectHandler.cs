using System.Data;
using Dapper;
using Newtonsoft.Json.Linq;

namespace WeatherServer.Data.DapperHandlers;

public class JObjectHandler: SqlMapper.TypeHandler<JObject>
{
    public override JObject Parse(object value)
    {
        var json = (string)value;
        return JObject.Parse(json);
    }

    public override void SetValue(IDbDataParameter parameter, JObject value)
    {
        parameter.Value = value?.ToString(Newtonsoft.Json.Formatting.None);
    }

    public static JObjectHandler Instance { get; } = new();
}