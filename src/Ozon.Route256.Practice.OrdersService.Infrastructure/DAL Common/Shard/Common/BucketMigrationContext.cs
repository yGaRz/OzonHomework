namespace Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Shard.Common;

public class BucketMigrationContext
{
    private string _currentDbSchema = string.Empty;

    public void UpdateCurrentDbSchema(int bucketId)
    {
        _currentDbSchema = ShardsHelper.GetSchemaName(bucketId);
    }

    public string CurrentDbSchema
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_currentDbSchema))
                throw new InvalidOperationException("Current db schema has not been initialized");

            return _currentDbSchema;
        }
    }
}
