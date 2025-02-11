package com.example.gymtools.database

import androidx.room.Dao
import androidx.room.Delete
import androidx.room.Entity
import androidx.room.ForeignKey
import androidx.room.Index
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.PrimaryKey
import androidx.room.Query
import androidx.room.Update

@Entity(
    tableName = "trainingDays",
    foreignKeys = [
        ForeignKey(
            entity = Program::class, // The referenced entity
            parentColumns = ["id"], // Column in the referenced table
            childColumns = ["programId"], // Column in this table
            onDelete = ForeignKey.CASCADE // Optional: Define cascading behavior
        )
    ],
    indices = [Index(value = ["programId"])]
)
data class TrainingDay(

    @PrimaryKey(autoGenerate = true)
    val id: Int = 0,

    val programId: Int = -1,

    val name: String = "",

)

@Dao
interface TrainingDayDao {

    @Insert(onConflict = OnConflictStrategy.IGNORE)
    suspend fun insert(trainingDay: TrainingDay)

    @Update
    suspend fun update(trainingDay: TrainingDay)

    @Delete
    suspend fun delete(trainingDay: TrainingDay)

    @Query("SELECT * FROM trainingDays")
    suspend fun getAll(): List<TrainingDay>

    @Query("SELECT * FROM trainingDays WHERE id = :id")
    suspend fun getById(id: Int): TrainingDay?

}