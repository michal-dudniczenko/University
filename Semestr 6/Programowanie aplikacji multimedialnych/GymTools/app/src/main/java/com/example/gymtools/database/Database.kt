package com.example.gymtools.database

import android.content.Context
import androidx.room.Database
import androidx.room.Room
import androidx.room.RoomDatabase

@Database(
    entities = [
        Weight::class,
        Exercise::class,
        Program::class,
        TrainingDay::class,
        TrainingDayExercise::class,
        TrainingDayExerciseSet::class
    ],
    version = 1,
    exportSchema = false
)
abstract class GymToolsDatabase : RoomDatabase() {

    abstract fun weightDao(): WeightDao
    abstract fun exerciseDao(): ExerciseDao
    abstract fun programDao(): ProgramDao
    abstract fun trainingDayDao(): TrainingDayDao
    abstract fun trainingDayExerciseDao(): TrainingDayExerciseDao
    abstract fun trainingDayExerciseSetDao(): TrainingDayExerciseSetDao

    companion object {
        @Volatile
        private var Instance: GymToolsDatabase? = null

        fun getDatabase(context: Context): GymToolsDatabase {
            // if the Instance is not null, return it, otherwise create a new database instance.
            return Instance ?: synchronized(this) {
                Room.databaseBuilder(context, GymToolsDatabase::class.java, "gymtools_database")
                    .build()
                    .also { Instance = it }
            }
        }
    }
}