package com.example.befit.health.caloriecalculator

import com.example.befit.constants.Strings

data class ActivityLevel(
    val name: String,
    val multiplier: Float,
    val level: Int
)

object ActivityLevels {
    fun getLevels(): List<ActivityLevel> {
        return listOf(
            ActivityLevel(
                name = Strings.LITTLE_OR_NO_EXERCISE,
                multiplier = 1.2f,
                level = 1
            ),
            ActivityLevel(
                name = Strings.LIGHT_EXERCISE,
                multiplier = 1.375f,
                level = 2
            ),
            ActivityLevel(
                name = Strings.MODERATE_EXERCISE,
                multiplier = 1.55f,
                level = 3
            ),
            ActivityLevel(
                name = Strings.INTENSE_EXERCISE,
                multiplier = 1.725f,
                level = 4
            ),
            ActivityLevel(
                name = Strings.VERY_HARD_EXERCISE,
                multiplier = 1.9f,
                level = 5
            )
        )
    }
}