package com.example.befit.health.caloriecalculator

import androidx.compose.runtime.State
import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.befit.common.CalorieCalculatorRoutes
import com.example.befit.common.StopwatchesRoutes
import com.example.befit.health.HealthViewModel
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableSharedFlow
import kotlinx.coroutines.flow.asSharedFlow
import kotlinx.coroutines.launch
import kotlin.math.round

class CalorieCalculatorViewModel(
    private val healthViewModel: HealthViewModel
) : ViewModel() {

    private val _navigationEvent = MutableSharedFlow<String>()
    val navigationEvent = _navigationEvent.asSharedFlow()

    fun navigateToStart() {
        viewModelScope.launch {
            _navigationEvent.emit(CalorieCalculatorRoutes.START)
        }
    }

    private val _currentRoute = mutableStateOf(CalorieCalculatorRoutes.START)
    val currentRoute: State<String> = _currentRoute

    fun updateCurrentRoute(route: String) {
        if (route != _currentRoute.value) {
            _currentRoute.value = route
        }
    }

    private val _calculateResult = mutableStateOf<CalorieCalculatorResult?>(null)
    val calculateResult: State<CalorieCalculatorResult?> = _calculateResult

    fun calculateCalories(
        age: Int,
        height: Int,
        weight: Float,
        sex: String,
        activityLevel: ActivityLevel
    ) {
        val sexOffset = if (sex == "Male") 5 else -161
        val bmr = round(10 * weight + 6.25 * height - 5 * age + sexOffset).toInt()
        val maintain = round(bmr * activityLevel.multiplier).toInt()

        _calculateResult.value = CalorieCalculatorResult(
            bmr = bmr,
            maintain = maintain,
            gainWeight = maintain + 300,
            slowWeightLoss = maintain - 250,
            weightLoss = maintain - 500,
            fastWeightLoss = maintain - 1000
        )

        healthViewModel.updateUserData(
            age = age,
            height = height,
            weight = weight,
            sex = sex,
            activityLevel = activityLevel.level
        )
    }

    fun clearResult() {
        viewModelScope.launch {
            delay(200L)
            _calculateResult.value = null
        }
    }
}