package com.example.befit.health.caloriecalculator

data class CalorieCalculatorResult(
    val bmr: Int,
    val maintain: Int,
    val gainWeight: Int,
    val slowWeightLoss: Int,
    val weightLoss: Int,
    val fastWeightLoss: Int
)
