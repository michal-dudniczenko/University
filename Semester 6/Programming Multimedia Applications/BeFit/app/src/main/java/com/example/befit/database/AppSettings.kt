package com.example.befit.database

import androidx.room.Dao
import androidx.room.Entity
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.PrimaryKey
import androidx.room.Query
import androidx.room.Update
import com.example.befit.constants.AppThemes
import com.example.befit.constants.Languages

@Entity(tableName = "appSettings")
data class AppSettings(

    @PrimaryKey(autoGenerate = false)
    val id: Int = 1,

    val language: String = Languages.DEFAULT,
    val theme: String = AppThemes.DARK,
    val playVideosMuted: Boolean = true
)

@Dao
interface AppSettingsDao {

    @Insert(onConflict = OnConflictStrategy.IGNORE)
    suspend fun insert(appSettings: AppSettings)

    @Update
    suspend fun update(appSettings: AppSettings)

    @Query("SELECT * FROM appSettings")
    suspend fun getAll(): List<AppSettings>

}