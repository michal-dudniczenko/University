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
import androidx.core.view.WindowCompat
import com.example.befit.common.appBackground
import com.example.befit.database.AppDatabase
import com.example.befit.health.weightmanager.WeightManagerViewModel
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

        val stopwatchesViewModel = StopwatchesViewModel(database.stopwatchDao())
        val weightManagerViewModel = WeightManagerViewModel(database.weightDao())
        val trainingProgramsViewModel = TrainingProgramsViewModel(
            database.exerciseDao(),
            database.programDao(),
            database.trainingDayDao(),
            database.trainingDayExerciseDao(),
            database.trainingDayExerciseSetDao()
        )

        setContent {
            Scaffold(
                modifier = Modifier
                    .fillMaxSize()
            ) { innerPadding ->
                App(
                    stopwatchesViewModel = stopwatchesViewModel,
                    weightManagerViewModel = weightManagerViewModel,
                    trainingProgramsViewModel = trainingProgramsViewModel,
                    modifier = Modifier
                        .padding(innerPadding)
                        .background(appBackground)
                )
            }
        }
    }
}