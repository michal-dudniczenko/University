package com.example.befit.database

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
        TrainingDayExerciseSet::class,
        Stopwatch::class,
        UserData::class,
        AppSettings::class
    ],
    version = 1,
    exportSchema = false
)
abstract class AppDatabase : RoomDatabase() {

    abstract fun weightDao(): WeightDao
    abstract fun exerciseDao(): ExerciseDao
    abstract fun programDao(): ProgramDao
    abstract fun trainingDayDao(): TrainingDayDao
    abstract fun trainingDayExerciseDao(): TrainingDayExerciseDao
    abstract fun trainingDayExerciseSetDao(): TrainingDayExerciseSetDao
    abstract fun stopwatchDao(): StopwatchDao
    abstract fun userDataDao(): UserDataDao
    abstract fun appSettingDao(): AppSettingsDao

    companion object {
        @Volatile
        private var Instance: AppDatabase? = null

        fun getDatabase(context: Context): AppDatabase {
            // if the Instance is not null, return it, otherwise create a new database instance.
            return Instance ?: synchronized(this) {
                Room.databaseBuilder(context, AppDatabase::class.java, "app_database")
                    .build()
                    .also { Instance = it }
            }
        }
    }
}