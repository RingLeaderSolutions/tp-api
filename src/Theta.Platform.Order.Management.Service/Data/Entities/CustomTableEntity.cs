using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theta.Platform.Order.Management.Service.Data.Entities
{
    // Storing decimals and enums 
    // https://stackoverflow.com/questions/11071899/storing-decimal-data-type-in-azure-tables
    public class CustomTableEntity : TableEntity
    {
        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);

            foreach (var thisProperty in
                GetType().GetProperties().Where(thisProperty =>
                    thisProperty.GetType() != typeof(string) &&
                    properties.ContainsKey(thisProperty.Name) &&
                    properties[thisProperty.Name].PropertyType == EdmType.String))
            {
                var parse = thisProperty.PropertyType.GetMethods().SingleOrDefault(m =>
                    m.Name == "Parse" &&
                    m.GetParameters().Length == 1 &&
                    m.GetParameters()[0].ParameterType == typeof(string));

                var value = parse != null ?
                    parse.Invoke(thisProperty, new object[] { properties[thisProperty.Name].StringValue }) :
                    ConvertType(properties, thisProperty);

                thisProperty.SetValue(this, value);
            }
        }

        private static object ConvertType(IDictionary<string, EntityProperty> properties, System.Reflection.PropertyInfo thisProperty)
        {
            if (thisProperty.PropertyType.IsEnum)
                return Enum.Parse(thisProperty.PropertyType, properties[thisProperty.Name].StringValue, true);

            return Convert.ChangeType(properties[thisProperty.Name].PropertyAsObject, thisProperty.PropertyType);
        }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var properties = base.WriteEntity(operationContext);

            foreach (var thisProperty in
                GetType().GetProperties().Where(thisProperty =>
                    !properties.ContainsKey(thisProperty.Name) &&
                    typeof(TableEntity).GetProperties().All(p => p.Name != thisProperty.Name)))
            {
                var value = thisProperty.GetValue(this);
                if (value != null)
                {
                    properties.Add(thisProperty.Name, new EntityProperty(value.ToString()));
                }
            }

            return properties;
        }
    }
}
