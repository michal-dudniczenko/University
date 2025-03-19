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
    tableName = "trainingDaysExercises",
    foreignKeys = [
        ForeignKey(
            entity = TrainingDay::class, // The referenced entity
            parentColumns = ["id"], // Column in the referenced table
            childColumns = ["trainingDayId"], // Column in this table
            onDelete = ForeignKey.CASCADE // Optional: Define cascading behavior
        ),
        ForeignKey(
            entity = Exercise::class, // The referenced entity
            parentColumns = ["id"], // Column in the referenced table
            childColumns = ["exerciseId"], // Column in this table
            onDelete = ForeignKey.CASCADE // Optional: Define cascading behavior
        )
    ],
    indices = [Index(value = ["trainingDayId"]), Index(value = ["exerciseId"])]
)
data class TrainingDayExercise(

    @PrimaryKey(autoGenerate = true)
    val id: Int = 0,

    val trainingDayId: Int = -1,

    val exerciseId: Int = -1,

    val restTime: String? = null,

    val weight: Float? = null

)

@Dao
interface TrainingDayExerciseDao {

    @Insert(onConflict = OnConflictStrategy.IGNORE)
    suspend fun insert(trainingDayExercise: TrainingDayExercise)

    @Update
    suspend fun update(trainingDayExercise: TrainingDayExercise)

    @Delete
    suspend fun delete(trainingDayExercise: TrainingDayExercise)

    @Query("SELECT * FROM trainingDaysExercises")
    suspend fun getAll(): List<TrainingDayExercise>

    @Query("SELECT * FROM trainingDaysExercises WHERE id = :id")
    suspend fun getById(id: Int): TrainingDayExercise?

}