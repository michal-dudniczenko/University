package com.example.befit.constants

data class ScreenInfo(
    val route: String,
    val iconPrimary: Int,
    val iconOnPrimary: Int,
)

data class ScreenNameRoute(
    val route: String,
    val name: String
)

object AppRoutes {
    const val TRAINING_PROGRAMS = "training programs"
    const val STOPWATCHES = "stopwatches"
    const val HEALTH = "health"
    const val SETTINGS = "settings"
    const val START = TRAINING_PROGRAMS

    fun getScreens(): List<ScreenInfo> {
        return listOf(
            ScreenInfo(TRAINING_PROGRAMS, Themes.DUMBBELL_PRIMARY, Themes.DUMBBELL_ON_PRIMARY),
            ScreenInfo(STOPWATCHES, Themes.STOPWATCH_PRIMARY, Themes.STOPWATCH_ON_PRIMARY),
            ScreenInfo(HEALTH, Themes.HEALTH_PRIMARY, Themes.HEALTH_ON_PRIMARY),
            ScreenInfo(SETTINGS, Themes.SETTINGS_PRIMARY, Themes.SETTINGS_ON_PRIMARY),
        )
    }
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
    const val EXERCISE_VIDEO = "exercise video/{exerciseId}/{trainingDayExerciseId}"
    fun EXERCISE_VIDEO(exerciseId: Int, trainingDayExerciseId: Int): String {
        return "exercise video/${exerciseId}/${trainingDayExerciseId}"
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
    const val TOOLS_LIST = "Tools List"
    const val DIET_PLANS = "Diet Plans"
    const val WEIGHT_HISTORY = "Weight History"
    const val CALORIE_CALCULATOR = "Calorie Calculator"
    const val BMI_CALCULATOR = "BMI Calculator"
    const val WATER_INTAKE_CALCULATOR = "Water Intake Calculator"
    const val START = TOOLS_LIST

    fun getScreens(): List<ScreenNameRoute> {
        return listOf(
            ScreenNameRoute(route = TOOLS_LIST, name = TOOLS_LIST),
            ScreenNameRoute(route = DIET_PLANS, name = Strings.DIET_PLANS),
            ScreenNameRoute(route = WEIGHT_HISTORY, name = Strings.WEIGHT_HISTORY),
            ScreenNameRoute(route = CALORIE_CALCULATOR, name = Strings.CALORIE_CALCULATOR),
            ScreenNameRoute(route = BMI_CALCULATOR, name = Strings.BMI_CALCULATOR),
            ScreenNameRoute(route = WATER_INTAKE_CALCULATOR, name = Strings.WATER_INTAKE_CALCULATOR)
        )
    }
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