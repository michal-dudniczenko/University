package com.example.befit.database

import androidx.room.Dao
import androidx.room.Entity
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.PrimaryKey
import androidx.room.Query
import androidx.room.Update

@Entity(tableName = "userData")
data class UserData(

    @PrimaryKey(autoGenerate = false)
    val id: Int = 1,

    val sex: String = "",
    val age: Int = 0,
    val height: Int = 0,
    val weight: Float = 0f,
    val activityLevel: Int = -1
)

@Dao
interface UserDataDao {

    @Insert(onConflict = OnConflictStrategy.IGNORE)
    suspend fun insert(userData: UserData)

    @Update
    suspend fun update(userData: UserData)

    @Query("SELECT * FROM userData")
    suspend fun getAll(): List<UserData>

}