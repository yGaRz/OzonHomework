﻿using System.Data;
using System.Data.Common;
using Npgsql;

namespace Ozon.Route256.Practice.OrdersService.DAL.Shard.Common;

public class ShardNpgsqlCommand: DbCommand
{
    private readonly NpgsqlCommand _npgsqlCommand;

    public ShardNpgsqlCommand(NpgsqlCommand npgsqlCommand, int bucketId)
    {
        _npgsqlCommand = npgsqlCommand;
        BucketId       = bucketId;
    }

    public int BucketId { get; }

    public override string CommandText
    {
        get => _npgsqlCommand.CommandText;
        set
        {
            var command = value.Replace(ShardsHelper.BucketPlaceholder, ShardsHelper.GetSchemaName(BucketId));
            _npgsqlCommand.CommandText = command;
        }
    }
    
    public override void Cancel() => _npgsqlCommand.Cancel();

    public override int ExecuteNonQuery() => _npgsqlCommand.ExecuteNonQuery();

    public override object? ExecuteScalar() => _npgsqlCommand.ExecuteScalar();

    public override void Prepare() => _npgsqlCommand.Prepare();

    public override int CommandTimeout
    {
        get => _npgsqlCommand.CommandTimeout;
        set => _npgsqlCommand.CommandTimeout = value;
    }

    public override CommandType CommandType
    {
        get => _npgsqlCommand.CommandType;
        set => _npgsqlCommand.CommandType = value;
    }

    public override UpdateRowSource UpdatedRowSource
    {
        get => _npgsqlCommand.UpdatedRowSource;
        set => _npgsqlCommand.UpdatedRowSource = value;
    }

    protected override DbConnection? DbConnection
    {
        get => _npgsqlCommand.Connection;
        set => _npgsqlCommand.Connection = value as NpgsqlConnection;
    }

    protected override DbParameterCollection DbParameterCollection => _npgsqlCommand.Parameters;

    protected override DbTransaction? DbTransaction
    {
        get => _npgsqlCommand.Transaction;
        set => _npgsqlCommand.Transaction = value as NpgsqlTransaction;
    }

    public override bool DesignTimeVisible
    {
        get => _npgsqlCommand.DesignTimeVisible;
        set => _npgsqlCommand.DesignTimeVisible = value;
    }

    protected override DbParameter CreateDbParameter() => _npgsqlCommand.CreateParameter();

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) =>
        _npgsqlCommand.ExecuteReader(behavior);

    public override ValueTask DisposeAsync()
    {
        return _npgsqlCommand.DisposeAsync();
    }
}