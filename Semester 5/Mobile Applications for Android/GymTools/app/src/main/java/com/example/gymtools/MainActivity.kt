package com.example.gymtools

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
import com.example.gymtools.common.appBackground
import com.example.gymtools.database.GymToolsDatabase
import com.example.gymtools.stopwatches.StopwatchesViewModel
import com.example.gymtools.trainingprograms.TrainingProgramsViewModel
import com.example.gymtools.weightmanager.WeightManagerViewModel

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge(
            statusBarStyle = SystemBarStyle.dark(appBackground.toArgb()),
            navigationBarStyle = SystemBarStyle.dark(Color.Black.toArgb())
        )

        val database = GymToolsDatabase.getDatabase(this)

        val stopwatchesViewModel = StopwatchesViewModel()
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
                GymToolsApp(
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