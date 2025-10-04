// <copyright file="InsertResult.cs" company="PlaceholderCompany">
// Written by Keex in 2025.
// </copyright>

namespace LootStatisticsTracker;

/// <summary>
/// Defines results of inserted data into the database.
/// </summary>
internal enum InsertResult
{
    /// <summary>
    /// Insert was successful.
    /// </summary>
    OK,

    /// <summary>
    /// The database was not open.
    /// </summary>
    DbNotOpen,

    /// <summary>
    /// The data object was already inserted.
    /// </summary>
    AlreadyInserted,
}
