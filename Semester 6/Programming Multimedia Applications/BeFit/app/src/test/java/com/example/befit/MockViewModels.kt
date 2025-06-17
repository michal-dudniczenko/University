package com.example.befit

import com.example.befit.database.UserData
import com.example.befit.database.UserDataDao
import com.example.befit.health.HealthViewModel

class TestHealthViewModel(userDataDao: UserDataDao) : HealthViewModel(userDataDao) {
    override fun updateUserData(newUserData: UserData) {
        // do nothing
    }

    override fun initialize() {
        // do nothing
    }
}