using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator;
using Microsoft.Extensions.DependencyInjection;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Shard.Common;

public abstract class ShardSqlMigration : IMigration
{
    public void GetUpExpressions(IMigrationContext context)
    {
        var sqlStatement = GetUpSql(context.ServiceProvider);

        var currentSchema = context.ServiceProvider.GetRequiredService<BucketMigrationContext>().CurrentDbSchema;
        if (!context.QuerySchema.SchemaExists(currentSchema))
        {
            context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = $"create schema {currentSchema};" });
        }

        context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = $"SET search_path TO {currentSchema};" });
        context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = sqlStatement });
    }

    public void GetDownExpressions(IMigrationContext context)
    {
        var sqlStatement = GetDownSql(context.ServiceProvider);

        var currentSchema = context.ServiceProvider.GetRequiredService<BucketMigrationContext>().CurrentDbSchema;

        context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = $"SET search_path TO {currentSchema};" });
        context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = sqlStatement });
    }

    protected abstract string GetUpSql(IServiceProvider services);
    protected abstract string GetDownSql(IServiceProvider services);

    object IMigration.ApplicationContext => throw new NotSupportedException();
    string IMigration.ConnectionString => throw new NotSupportedException();
}
