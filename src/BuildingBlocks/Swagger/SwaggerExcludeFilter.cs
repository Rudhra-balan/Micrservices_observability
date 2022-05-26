using System;
using System.Linq;
using System.Reflection;

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BuildingBlocks.Swagger
{
    public class SwaggerExcludeFilter : ISchemaFilter
    {
        #region ISchemaFilter Members


        public void Apply(OpenApiSchema schema, SchemaFilterContext schemaFilterContext)
        {

            if (schema?.Properties == null || schemaFilterContext == null) return;
            var excludedProperties = schemaFilterContext.Type.GetProperties()
                .Where(t => t.GetCustomAttribute(typeof(SwaggerExcludeAttribute), true) != null);
            foreach (var excludedProperty in excludedProperties)
            {
                if (schema.Properties.ContainsKey(excludedProperty.Name.ToCamelCase()))
                    schema.Properties.Remove(excludedProperty.Name.ToCamelCase());
            }
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SwaggerExcludeAttribute : Attribute { }
}
