package com.example.gymtools.database

import androidx.room.Dao
import androidx.room.Delete
import androidx.room.Entity
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.PrimaryKey
import androidx.room.Query
import androidx.room.Update

@Entity(tableName = "weights")
data class Weight(

    @PrimaryKey(autoGenerate = true)
    val id: Int = 0,

    val date: Long = 0,

    val weight: Float = 0f

)

@Dao
interface WeightDao {

    @Insert(onConflict = OnConflictStrategy.IGNORE)
    suspend fun insert(weight: Weight)

    @Update
    suspend fun update(weight: Weight)

    @Delete
    suspend fun delete(weight: Weight)

    @Query("SELECT * FROM weights")
    suspend fun getAll(): List<Weight>

    @Query("SELECT * FROM weights WHERE id = :id")
    suspend fun getById(id: Int): Weight?

}