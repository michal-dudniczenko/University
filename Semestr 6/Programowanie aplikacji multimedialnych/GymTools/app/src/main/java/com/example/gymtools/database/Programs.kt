package com.example.gymtools.database

import androidx.room.Dao
import androidx.room.Delete
import androidx.room.Entity
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.PrimaryKey
import androidx.room.Query
import androidx.room.Update

@Entity(tableName = "programs")
data class Program(

    @PrimaryKey(autoGenerate = true)
    val id: Int = 0,

    val name: String = "",

)

@Dao
interface ProgramDao {

    @Insert(onConflict = OnConflictStrategy.IGNORE)
    suspend fun insert(program: Program)

    @Update
    suspend fun update(program: Program)

    @Delete
    suspend fun delete(program: Program)

    @Query("SELECT * FROM programs")
    suspend fun getAll(): List<Program>

    @Query("SELECT * FROM programs WHERE id = :id")
    suspend fun getById(id: Int): Program?

}