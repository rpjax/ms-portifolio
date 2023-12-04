using Microsoft.EntityFrameworkCore;
using ModularSystem.Core;

namespace ModularSystem.EntityFramework;

/// <summary>
/// Represents a base context for Entity Framework Core operations.
/// </summary>
public class EFCoreContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // check all entities contained in this context and map the array props to a table, such as: string[], int[], etc...
        //MapArrayProperties(modelBuilder);
    }

    private void MapArrayProperties(ModelBuilder modelBuilder)
    {
        // Obter todas as entidades contidas neste contexto
        var entities = modelBuilder.Model.GetEntityTypes().ToArray();

        foreach (var entity in entities)
        {
            var entityType = entity.ClrType;
            // Obter todas as propriedades da entidade
            var properties = entity.ClrType.GetProperties();

            foreach (var propertyInfo in properties)
            {
                var propertyType = propertyInfo.PropertyType;

                var isEnumerable = propertyType.IsArray
                    || propertyType.IsGenericType
                    && propertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>);

                // Verificar se a propriedade é uma coleção (IEnumerable<T>)
                if (!isEnumerable)
                {
                    continue;
                }

                // Obter o tipo de elemento da coleção
                var elementType = propertyType.IsArray
                      ? propertyType.GetElementType()
                      : propertyType.GetGenericArguments().First();

                if (elementType == null)
                {
                    throw new Exception();
                }
                if(elementType != typeof(string))
                {
                    continue;
                }

                var foreignKeyColumnName = $"{entityType.Name}Id";
                var elementTableTypeProps = new AnonymousPropertyDefinition[]
                {
                    new(nameof(EFModel.Id), typeof(long)),
                    new(foreignKeyColumnName, typeof(long)),
                    new("Value", elementType)
                };
                var elementTableType = TypeHelper.CreateAnonymousType(elementTableTypeProps);

                // Criar uma tabela para a coleção
                modelBuilder
                    .Entity(elementTableType)
                    .HasKey(nameof(EFModel.Id));

                //sets the fk prop
                modelBuilder
                   .Entity(elementTableType)
                   .Property(foreignKeyColumnName)
                   .HasColumnName(foreignKeyColumnName);// set the foreign key relationship.

                // sets the value prop
                modelBuilder
                    .Entity(elementTableType)
                    .Property("Value")
                    .HasColumnName("Value");

                // sets relationship
                modelBuilder
                    .Entity(entity.ClrType)
                    .HasMany(elementTableType)  
                    .WithOne()
                    .HasForeignKey(foreignKeyColumnName); // Substitua pelo nome desejado para a chave estrangeira
            }
        }
    }

}

