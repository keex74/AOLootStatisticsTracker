using AOSharp.Core.UI;
using Microsoft.Data.Sqlite;

namespace LootStatisticsTracker;

internal class SqliteWriter
{
    private SqliteConnection? currentConnection;

    public void SetPath(string path)
    {
        try
        {
            this.currentConnection?.Close();
            this.currentConnection?.Dispose();
            this.currentConnection = null;

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var bldr = new SqliteConnectionStringBuilder
            {
                DataSource = path,
            };
            this.currentConnection = new SqliteConnection(bldr.ConnectionString);
            this.currentConnection.Open();
            this.InitializeDB();
        }
        catch (Exception ex)
        {
            this.currentConnection?.Dispose();
            this.currentConnection = null;
            Chat.WriteLine($"Error opening the database: {ex}", AOSharp.Common.GameData.ChatColor.Red);
            Chat.WriteLine($"Filename: {path}", AOSharp.Common.GameData.ChatColor.Orange);
        }

    }

    public bool InsertContainer(LootInfo info)
    {
        var db = this.currentConnection;
        if (db == null)
        {
            return false;
        }

        if (this.ContainerAlreadyOpened(db, info))
        {
            return false;
        }

        using var ta = db.BeginTransaction();
        try
        {
            using var cmd = db.CreateCommand();
            cmd.CommandText = @"INSERT INTO Containers (instance, timeLooted, name, pfInstance, pfName, pfIsDungeon, locX, locY, locZ) 
VALUES(@instance, @timeLooted, @name, @pfInstance, @pfName, @pfIsDungeon, @locX, @locY, @locZ);";
            cmd.Parameters.AddWithValue("instance", info.Instance);
            cmd.Parameters.AddWithValue("timeLooted", info.RecordedOnUnix);
            cmd.Parameters.AddWithValue("name", info.Name);
            cmd.Parameters.AddWithValue("pfInstance", info.Playfield.PlayfieldID);
            cmd.Parameters.AddWithValue("pfName", info.Playfield.PlayfieldName);
            cmd.Parameters.AddWithValue("pfIsDungeon", info.Playfield.IsDungeon);
            cmd.Parameters.AddWithValue("locX", info.Position.X);
            cmd.Parameters.AddWithValue("locY", info.Position.Y);
            cmd.Parameters.AddWithValue("locZ", info.Position.Z);
            cmd.ExecuteNonQuery();
            using var cmd2 = db.CreateCommand();
            cmd2.CommandText = @"SELECT last_insert_rowid();";
            var lastrow = (long)cmd2.ExecuteScalar()!;
            InsertContainerItems(db, info, lastrow);
            InsertStats(db, info, lastrow, "ContainerStats", "fk_container");
            ta.Commit();
            return true;
        }
        catch (Exception)
        {
            ta.Rollback();
            return false;
        }
    }

    public bool InsertCorpse(LootInfo info, bool legacy)
    {
        var db = this.currentConnection;
        if (db == null)
        {
            return false;
        }

        if (this.CorpseAlreadyOpened(db, info))
        {
            return false;
        }

        using var ta = db.BeginTransaction();
        try
        {
            using var cmd = db.CreateCommand();
            cmd.CommandText = @"INSERT INTO Corpses (instance, timeLooted, corpseName, pfInstance, pfName, pfIsDungeon, locX, locY, locZ, sourceId, sourceName, sourceLevel,sourceProfession,sourceBreed,sourceGender,areCorpseStats) 
VALUES(@instance, @timeLooted, @corpseName, @pfInstance, @pfName, @pfIsDungeon, @locX, @locY, @locZ, @sourceId, @sourceName, @sourceLevel, @sourceProfession, @sourceBreed, @sourceGender, @areCorpseStats);";
            cmd.Parameters.AddWithValue("instance", info.Instance);
            cmd.Parameters.AddWithValue("timeLooted", info.RecordedOnUnix);
            cmd.Parameters.AddWithValue("corpseName", info.Name);
            cmd.Parameters.AddWithValue("pfInstance", info.Playfield.PlayfieldID);
            cmd.Parameters.AddWithValue("pfName", info.Playfield.PlayfieldName);
            cmd.Parameters.AddWithValue("pfIsDungeon", info.Playfield.IsDungeon);
            cmd.Parameters.AddWithValue("locX", info.Position.X);
            cmd.Parameters.AddWithValue("locY", info.Position.Y);
            cmd.Parameters.AddWithValue("locZ", info.Position.Z);
            cmd.Parameters.AddWithValue("sourceId", info.CorpseInstance);
            cmd.Parameters.AddWithValue("sourceName", info.SourceName);
            cmd.Parameters.AddWithValue("sourceLevel", info.Level);
            cmd.Parameters.AddWithValue("sourceProfession", info.Profession);
            cmd.Parameters.AddWithValue("sourceBreed", info.Breed);
            cmd.Parameters.AddWithValue("sourceGender", info.Gender);
            cmd.Parameters.AddWithValue("areCorpseStats", legacy ? 1 : 0);
            cmd.ExecuteNonQuery();
            using var cmd2 = db.CreateCommand();
            cmd2.CommandText = @"SELECT last_insert_rowid();";
            var lastrow = (long)cmd2.ExecuteScalar()!;
            InsertCorpseItems(db, info, lastrow);
            InsertBuffs(db, info, lastrow);
            InsertStats(db, info, lastrow, "Stats", "fk_corpse");
            ta.Commit();
            return true;
        }
        catch (Exception)
        {
            ta.Rollback();
            return false;
        }
    }

    private void InsertCorpseItems(SqliteConnection db, LootInfo info, long corpseRowId)
    {
        using var cmd = db.CreateCommand();
        cmd.CommandText = @"INSERT INTO LootItems (fk_corpse,lowId,highId,name,ql,charges) 
VALUES(@fk_corpse,@lowId,@highId,@name,@ql,@charges);";
        cmd.Parameters.AddWithValue("fk_corpse", corpseRowId);
        cmd.Parameters.AddWithValue("lowId", 0);
        cmd.Parameters.AddWithValue("highId", 0);
        cmd.Parameters.AddWithValue("name", string.Empty);
        cmd.Parameters.AddWithValue("ql", 0);
        cmd.Parameters.AddWithValue("charges", 0);
        foreach (var b in info.Items)
        {
            cmd.Parameters["lowId"].Value = b.LowId;
            cmd.Parameters["highId"].Value = b.HighId;
            cmd.Parameters["name"].Value = b.Name;
            cmd.Parameters["ql"].Value = b.QL;
            cmd.Parameters["charges"].Value = b.Charges;
            cmd.ExecuteNonQuery();
        }
    }

    private void InsertContainerItems(SqliteConnection db, LootInfo info, long containerRowId)
    {
        using var cmd = db.CreateCommand();
        cmd.CommandText = @"INSERT INTO ContainerItems (fk_container,lowId,highId,name,ql,charges) 
VALUES(@fk_container,@lowId,@highId,@name,@ql,@charges);";
        cmd.Parameters.AddWithValue("fk_container", containerRowId);
        cmd.Parameters.AddWithValue("lowId", 0);
        cmd.Parameters.AddWithValue("highId", 0);
        cmd.Parameters.AddWithValue("name", string.Empty);
        cmd.Parameters.AddWithValue("ql", 0);
        cmd.Parameters.AddWithValue("charges", 0);
        foreach (var b in info.Items)
        {
            cmd.Parameters["lowId"].Value = b.LowId;
            cmd.Parameters["highId"].Value = b.HighId;
            cmd.Parameters["name"].Value = b.Name;
            cmd.Parameters["ql"].Value = b.QL;
            cmd.Parameters["charges"].Value = b.Charges;
            cmd.ExecuteNonQuery();
        }
    }

    private void InsertBuffs(SqliteConnection db, LootInfo info, long corpseid)
    {
        using var cmd = db.CreateCommand();
        cmd.CommandText = @"INSERT INTO Buffs (fk_corpse,buffId,name,timeLeft) 
VALUES(@fk_corpse,@buffId,@name,@timeLeft);";
        cmd.Parameters.AddWithValue("fk_corpse", corpseid);
        cmd.Parameters.AddWithValue("buffId", 0);
        cmd.Parameters.AddWithValue("name", string.Empty);
        cmd.Parameters.AddWithValue("timeLeft", 0);
        foreach (var b in info.MobBuffs)
        {
            cmd.Parameters["buffId"].Value = b.Id;
            cmd.Parameters["name"].Value = b.Name;
            cmd.Parameters["timeLeft"].Value = b.TimeLeft;
            cmd.ExecuteNonQuery();
        }
    }

    private void InsertStats(SqliteConnection db, LootInfo info, long parentRowId, string tableName, string fkName)
    {
        using var cmd = db.CreateCommand();
        cmd.CommandText = $@"INSERT INTO {tableName} ({fkName},statId,name,value) 
VALUES(@fk_parent,@statId,@name,@value);";
        cmd.Parameters.AddWithValue("fk_parent", parentRowId);
        cmd.Parameters.AddWithValue("statId", 0);
        cmd.Parameters.AddWithValue("name", string.Empty);
        cmd.Parameters.AddWithValue("value", 0);
        foreach (var b in info.Stats)
        {
            cmd.Parameters["statId"].Value = b.ID;
            cmd.Parameters["name"].Value = b.Name;
            cmd.Parameters["value"].Value = b.Value;
            cmd.ExecuteNonQuery();
        }
    }

    private bool CorpseAlreadyOpened(SqliteConnection db, LootInfo info)
    {
        using var cmdCheck = db.CreateCommand();
        cmdCheck.CommandText = $@"SELECT timeLooted FROM Corpses WHERE instance = @instance AND pfInstance = @pf AND corpseName = @name;";
        cmdCheck.Parameters.AddWithValue("instance", info.Instance);
        cmdCheck.Parameters.AddWithValue("pf", info.Playfield.PlayfieldID);
        cmdCheck.Parameters.AddWithValue("name", info.Name);
        using var rdr = cmdCheck.ExecuteReader();
        while (rdr.Read())
        {
            // Check corpse age
            var timeNow = info.RecordedOnUnix;
            var otherTime = rdr.GetInt64(1);
            var msElapsed = timeNow - otherTime;
            if (msElapsed > 30 * 60 * 1000)
            {
                // Older than 30 minutes
                continue;
            }

            return true;
        }

        return false;
    }

    private bool ContainerAlreadyOpened(SqliteConnection db, LootInfo info)
    {
        using var cmdCheck = db.CreateCommand();
        cmdCheck.CommandText = $@"SELECT timeLooted FROM Containers WHERE instance = @instance AND pfInstance = @pf AND name = @name;";
        cmdCheck.Parameters.AddWithValue("instance", info.Instance);
        cmdCheck.Parameters.AddWithValue("pf", info.Playfield.PlayfieldID);
        cmdCheck.Parameters.AddWithValue("name", info.Name);
        using var rdr = cmdCheck.ExecuteReader();
        if (rdr.Read())
        {
            return true;
        }

        return false;
    }

    private void InitializeDB()
    {
        var db = this.currentConnection;
        if (db == null)
        {
            return;
        }

        using var cmd = db.CreateCommand();
        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Corpses 
(instance NUMERIC, 
timeLooted NUMERIC, 
corpseName TEXT, 
pfInstance NUMERIC, 
pfName TEXT, 
pfIsDungeon NUMERIC, 
locX NUMERIC, 
locY NUMERIC, 
locZ NUMERIC,
sourceId NUMERIC,
sourceName TEXT, 
sourceLevel NUMERIC, 
sourceProfession NUMERIC, 
sourceBreed NUMERIC,
sourceGender NUMERIC,
areCorpseStats);";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS LootItems 
(fk_corpse NUMERIC NOT NULL,
lowId NUMERIC, 
highId NUMERIC, 
name TEXT, 
ql NUMERIC,
charges NUMERIC);";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Buffs 
(fk_corpse NUMERIC NOT NULL,
buffId NUMERIC, 
name TEXT,
timeLeft NUMERIC);";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Stats 
(fk_corpse NUMERIC NOT NULL,
statid NUMERIC, 
name TEXT,
value NUMERIC);";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Containers 
(instance NUMERIC,
timeLooted NUMERIC, 
name TEXT, 
pfInstance NUMERIC, 
pfName TEXT, 
pfIsDungeon NUMERIC, 
locX NUMERIC, 
locY NUMERIC, 
locZ NUMERIC);";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ContainerItems 
(fk_container NUMERIC NOT NULL,
lowId NUMERIC, 
highId NUMERIC, 
name TEXT, 
ql NUMERIC,
charges NUMERIC);";
        cmd.ExecuteNonQuery();

        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ContainerStats 
(fk_container NUMERIC NOT NULL,
statid NUMERIC, 
name TEXT,
value NUMERIC);";
        cmd.ExecuteNonQuery();
    }
}
