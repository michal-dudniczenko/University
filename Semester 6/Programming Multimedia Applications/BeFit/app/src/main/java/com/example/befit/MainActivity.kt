package com.example.befit

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.SystemBarStyle
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.Scaffold
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.toArgb
import com.example.befit.constants.InitScreenDimensions
import com.example.befit.constants.appBackground
import com.example.befit.database.AppDatabase
import com.example.befit.health.HealthViewModel
import com.example.befit.health.caloriecalculator.CalorieCalculatorViewModel
import com.example.befit.health.weighthistory.WeightHistoryViewModel
import com.example.befit.settings.SettingsViewModel
import com.example.befit.stopwatches.StopwatchesViewModel
import com.example.befit.trainingprograms.TrainingProgramsViewModel

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge(
            statusBarStyle = SystemBarStyle.dark(appBackground.toArgb()),
            navigationBarStyle = SystemBarStyle.dark(Color.Black.toArgb())
        )

        val database = AppDatabase.getDatabase(this)

        val appViewModel = AppViewModel()
        val trainingProgramsViewModel = TrainingProgramsViewModel(
            database.exerciseDao(),
            database.programDao(),
            database.trainingDayDao(),
            database.trainingDayExerciseDao(),
            database.trainingDayExerciseSetDao()
        )
        val stopwatchesViewModel = StopwatchesViewModel(database.stopwatchDao())
        val healthViewModel = HealthViewModel(
            database.userDataDao()
        )
        val weightHistoryViewModel = WeightHistoryViewModel(database.weightDao())
        val calorieCalculatorViewModel = CalorieCalculatorViewModel(
            healthViewModel = healthViewModel
        )
        val settingsViewModel = SettingsViewModel()

        setContent {
            InitScreenDimensions()

            Scaffold(
                modifier = Modifier
                    .fillMaxSize()
            ) { innerPadding ->
                AppNavigation(
                    viewModel = appViewModel,
                    trainingProgramsViewModel = trainingProgramsViewModel,
                    stopwatchesViewModel = stopwatchesViewModel,
                    healthViewModel = healthViewModel,
                    weightHistoryViewModel = weightHistoryViewModel,
                    calorieCalculatorViewModel = calorieCalculatorViewModel,
                    settingsViewModel = settingsViewModel,
                    modifier = Modifier
                        .padding(innerPadding)
                        .background(appBackground)
                )
            }
        }
    }
}