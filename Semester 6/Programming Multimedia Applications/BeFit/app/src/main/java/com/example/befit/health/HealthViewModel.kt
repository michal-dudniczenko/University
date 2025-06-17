package com.example.befit.health

import androidx.compose.runtime.State
import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.befit.constants.HealthRoutes
import com.example.befit.database.UserData
import com.example.befit.database.UserDataDao
import kotlinx.coroutines.flow.MutableSharedFlow
import kotlinx.coroutines.flow.asSharedFlow
import kotlinx.coroutines.launch



open class HealthViewModel(
    val userDataDao: UserDataDao
) : ViewModel() {
    private val _navigationEvent = MutableSharedFlow<String>()
    val navigationEvent = _navigationEvent.asSharedFlow()

    fun navigateToStart() {
        viewModelScope.launch {
            _navigationEvent.emit(HealthRoutes.START)
        }
    }

    private val _currentRoute = mutableStateOf(HealthRoutes.START)
    val currentRoute: State<String> = _currentRoute

    fun updateCurrentRoute(route: String) {
        if (route != _currentRoute.value) {
            _currentRoute.value = route
        }
    }

    private val _userData = mutableStateOf<UserData?>(null)
    val userData: State<UserData?> = _userData

    init {
        initialize()
    }

    protected open fun initialize() {
        viewModelScope.launch {
            val data = userDataDao.getAll()
            if (!data.isEmpty()) {
                _userData.value = data.first()
            }
        }
    }

    fun updateUserData(
        isMale: Boolean? = null,
        age: Int? = null,
        height: Int? = null,
        weight: Float? = null,
        activityLevel: Int? = null
    ) {
        val newUserData = UserData(
            id = 1,
            isMale = isMale ?: userData.value?.isMale ?: true,
            age = age ?: userData.value?.age ?: 0,
            height = height ?: userData.value?.height ?: 0,
            weight = weight ?: userData.value?.weight ?: 0f,
            activityLevel = activityLevel ?: userData.value?.activityLevel ?: -1,
        )
        _userData.value = newUserData
        viewModelScope.launch {
            userDataDao.update(newUserData)
        }

    }

    open fun updateUserData(
        userData: UserData
    ) {
        viewModelScope.launch {
            if (_userData.value == null) {
                userDataDao.insert(userData)
            } else {
                userDataDao.update(userData)
            }
        }
        _userData.value = userData
    }

    fun calculateBmi(
        height: Int,
        weight: Float
    ): Float {
        if (height <= 0 || weight <= 0) {
            return 0f
        }

        val newUserData = UserData(
            id = 1,
            isMale = userData.value?.isMale ?: true,
            age = userData.value?.age ?: 0,
            height = height,
            weight = weight,
            activityLevel = userData.value?.activityLevel ?: -1,
        )
        updateUserData(newUserData)

        return weight / ((height / 100f) * (height / 100f))
    }

    fun calculateWaterIntake(
        weight: Float
    ): Float {
        if (weight <= 0) {
            return 0f
        }

        val newUserData = UserData(
            id = 1,
            isMale = userData.value?.isMale ?: true,
            age = userData.value?.age ?: 0,
            height = userData.value?.height ?: 0,
            weight = weight,
            activityLevel = userData.value?.activityLevel ?: -1,
        )
        updateUserData(newUserData)

        return weight * 35 / 1000
    }
}