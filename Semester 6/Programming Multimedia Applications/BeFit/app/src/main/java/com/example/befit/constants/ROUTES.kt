package com.example.befit.constants

import com.example.befit.R

data class ScreenInfo(
    val route: String,
    val iconWhite: Int,
    val iconBlack: Int,
)

object AppRoutes {
    const val TRAINING_PROGRAMS = "training programs"
    const val STOPWATCHES = "stopwatches"
    const val HEALTH = "health"
    const val SETTINGS = "settings"
    const val START = TRAINING_PROGRAMS

    val screens = listOf(
        ScreenInfo(TRAINING_PROGRAMS, R.drawable.dumbbell_white, R.drawable.dumbbell_black),
        ScreenInfo(STOPWATCHES, R.drawable.stopwatch_white, R.drawable.stopwatch_black),
        ScreenInfo(HEALTH, R.drawable.health_white, R.drawable.health_black),
        ScreenInfo(SETTINGS, R.drawable.settings_white, R.drawable.settings_black),
    )
}

object TrainingProgramsRoutes {
    const val PROGRAMS_LIST = "programs list"
    const val ADD_PROGRAM = "add program"
    const val EDIT_PROGRAM = "edit program/{id}"
    fun EDIT_PROGRAM(id: Int): String {
        return "edit program/${id}"
    }
    const val TRAINING_DAYS_LIST = "training days list/{id}"
    fun TRAINING_DAYS_LIST(id: Int): String {
        return "training days list/${id}"
    }
    const val ADD_TRAINING_DAY = "add training day/{programId}"
    fun ADD_TRAINING_DAY(programId: Int): String {
        return "add training day/${programId}"
    }
    const val EDIT_TRAINING_DAY = "edit training day/{id}"
    fun EDIT_TRAINING_DAY(id: Int): String {
        return "edit training day/${id}"
    }
    const val VIEW_TRAINING_DAY = "view training day/{id}"
    fun VIEW_TRAINING_DAY(id: Int): String {
        return "view training day/${id}"
    }
    const val ADD_EXERCISE_TO_DAY = "add exercise to day/{trainingDayId}"
    fun ADD_EXERCISE_TO_DAY(trainingDayId: Int): String {
        return "add exercise to day/${trainingDayId}"
    }
    const val EDIT_EXERCISE_FROM_DAY = "edit exercise from day/{id}"
    fun EDIT_EXERCISE_FROM_DAY(id: Int): String {
        return "edit exercise from day/${id}"
    }
    const val VIEW_EXERCISE_FROM_DAY = "view exercise from day/{id}"
    fun VIEW_EXERCISE_FROM_DAY(id: Int): String {
        return "view exercise from day/${id}"
    }
    const val SETTINGS = "settings"
    const val EDIT_EXERCISE_LIST = "edit exercise list"
    const val ADD_EXERCISE = "add exercise"
    const val EDIT_EXERCISE = "edit exercise/{id}"
    fun EDIT_EXERCISE(id: Int): String {
        return "edit exercise/${id}"
    }
    const val START = PROGRAMS_LIST
}

object StopwatchesRoutes {
    const val STOPWATCHES = "stopwatches"
    const val EDIT = "edit/{id}"
    fun EDIT(id: Int): String {
        return "edit/${id}"
    }
    const val START = STOPWATCHES
}

object HealthRoutes {
    val TOOLS_LIST = "Tools List"
    val DIET_PLANS = Strings.DIET_PLANS
    val WEIGHT_HISTORY = Strings.WEIGHT_HISTORY
    val CALORIE_CALCULATOR = Strings.CALORIE_CALCULATOR
    val BMI_CALCULATOR = Strings.BMI_CALCULATOR
    val WATER_INTAKE_CALCULATOR = Strings.WATER_INTAKE_CALCULATOR
    val START = TOOLS_LIST

    val screens = listOf(
        TOOLS_LIST,
        DIET_PLANS,
        WEIGHT_HISTORY,
        CALORIE_CALCULATOR,
        BMI_CALCULATOR,
        WATER_INTAKE_CALCULATOR
    )
}

object WeightHistoryRoutes {
    const val HISTORY = "history"
    const val ADD = "add"
    const val EDIT = "edit/{id}"
    fun EDIT(id: Int): String {
        return "edit/${id}"
    }
    const val START = HISTORY
}

object CalorieCalculatorRoutes {
    const val CALCULATOR = "calculator"
    const val RESULT = "result"
    const val START = CALCULATOR
}

object SettingsRoutes {
    const val SETTINGS_LIST = "settings list"
    const val START = SETTINGS_LIST
}