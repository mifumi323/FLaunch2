using FLaunch2.Models;
using FLaunch2.Services;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaunch2.Repositories;

/// <summary>
/// Itemデータの永続化を管理するリポジトリ
/// </summary>
public class ItemRepository : IDisposable
{
    private readonly LiteDatabase _database;
    private readonly ILiteCollection<Item> _collection;
    private bool _disposed;

    public ItemRepository()
    {
        _database = new LiteDatabase($"Filename={DataPathProvider.DatabasePath};Connection=shared");
        _collection = _database.GetCollection<Item>("items");

        // スコアによるインデックスを作成（パフォーマンス向上のため）
        _collection.EnsureIndex(x => x.Score, false);
        _collection.EnsureIndex(x => x.LastExecuted, false);
    }

    /// <summary>
    /// すべてのItemを取得します
    /// </summary>
    public IEnumerable<Item> GetAll()
    {
        return _collection.FindAll();
    }

    /// <summary>
    /// スコアの降順でソートされたItemリストを取得します
    /// </summary>
    public IEnumerable<Item> GetAllOrderedByScore()
    {
        return _collection.Query()
            .OrderByDescending(x => x.Score)
            .ToList();
    }

    /// <summary>
    /// 指定されたIDのItemを取得します
    /// </summary>
    public Item? GetById(Guid id)
    {
        return _collection.FindById(id);
    }

    /// <summary>
    /// Itemを追加または更新します
    /// </summary>
    public void Upsert(Item item)
    {
        _collection.Upsert(item);
    }

    /// <summary>
    /// 複数のItemを一括で追加または更新します
    /// </summary>
    public void UpsertMany(IEnumerable<Item> items)
    {
        _collection.Upsert(items);
    }

    /// <summary>
    /// Itemを削除します
    /// </summary>
    public bool Delete(Guid id)
    {
        return _collection.Delete(id);
    }

    /// <summary>
    /// すべてのItemを削除します
    /// </summary>
    public int DeleteAll()
    {
        return _collection.DeleteAll();
    }

    /// <summary>
    /// 指定された条件に一致するItemを検索します
    /// </summary>
    public IEnumerable<Item> Find(Func<Item, bool> predicate)
    {
        return _collection.FindAll().Where(predicate);
    }

    ~ItemRepository()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // マネージドリソースの解放
                _database?.Dispose();
            }
            
            // アンマネージドリソースの解放（この場合は不要）
            
            _disposed = true;
        }
    }
}
