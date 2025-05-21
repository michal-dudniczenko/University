package com.example.befit.database

import androidx.room.Dao
import androidx.room.Delete
import androidx.room.Entity
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.PrimaryKey
import androidx.room.Query
import androidx.room.Update

@Entity(tableName = "stopwatches")
data class Stopwatch(
    
    @PrimaryKey(autoGenerate = true)
    val id: Int = 0,

    val name: String = "",
)

@Dao
interface StopwatchDao {

    @Insert(onConflict = OnConflictStrategy.IGNORE)
    suspend fun insert(stopwatch: Stopwatch)

    @Update
    suspend fun update(stopwatch: Stopwatch)

    @Delete
    suspend fun delete(stopwatch: Stopwatch)

    @Query("SELECT * FROM stopwatches")
    suspend fun getAll(): List<Stopwatch>

    @Query("SELECT * FROM stopwatches WHERE id = :id")
    suspend fun getById(id: Int): Stopwatch?

}