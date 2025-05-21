package com.example.befit.database

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
    tableName = "trainingDaysExercisesSets",
    foreignKeys = [
        ForeignKey(
            entity = TrainingDayExercise::class, // The referenced entity
            parentColumns = ["id"], // Column in the referenced table
            childColumns = ["trainingDayExerciseId"], // Column in this table
            onDelete = ForeignKey.CASCADE // Optional: Define cascading behavior
        ),
    ],
    indices = [Index(value = ["trainingDayExerciseId"])]
)
data class TrainingDayExerciseSet(

    @PrimaryKey(autoGenerate = true)
    val id: Int = 0,

    val trainingDayExerciseId: Int = -1,

    val reps: Int? = null,

)

@Dao
interface TrainingDayExerciseSetDao {

    @Insert(onConflict = OnConflictStrategy.IGNORE)
    suspend fun insert(trainingDayExerciseSet: TrainingDayExerciseSet)

    @Update
    suspend fun update(trainingDayExerciseSet: TrainingDayExerciseSet)

    @Delete
    suspend fun delete(trainingDayExerciseSet: TrainingDayExerciseSet)

    @Query("SELECT * FROM trainingDaysExercisesSets")
    suspend fun getAll(): List<TrainingDayExerciseSet>

    @Query("SELECT * FROM trainingDaysExercisesSets WHERE id = :id")
    suspend fun getById(id: Int): TrainingDayExerciseSet?

}