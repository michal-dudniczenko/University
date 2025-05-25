package com.example.befit.health.caloriecalculator

data class ActivityLevel(
    val name: String,
    val multiplier: Float,
    val level: Int
)

object ActivityLevels {
    val levels = listOf(
        ActivityLevel(
            name = "Little or no exercise",
            multiplier = 1.2f,
            level = 1
        ),
        ActivityLevel(
            name = "Light exercise 1–3 days/week",
            multiplier = 1.375f,
            level = 2
        ),
        ActivityLevel(
            name = "Moderate exercise 3–5 days/week",
            multiplier = 1.55f,
            level = 3
        ),
        ActivityLevel(
            name = "Intense exercise 6–7 days/week",
            multiplier = 1.725f,
            level = 4
        ),
        ActivityLevel(
            name = "Very hard daily exercise or physical job",
            multiplier = 1.9f,
            level = 5
        ),
    )
}