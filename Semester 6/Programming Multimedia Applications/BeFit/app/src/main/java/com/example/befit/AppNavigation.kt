package com.example.befit

import androidx.compose.animation.EnterTransition
import androidx.compose.animation.ExitTransition
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.ui.Modifier
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.example.befit.common.AppRoutes
import com.example.befit.common.BottomNavbar
import com.example.befit.health.HealthNavigation
import com.example.befit.health.HealthViewModel
import com.example.befit.health.caloriecalculator.CalorieCalculatorViewModel
import com.example.befit.health.weighthistory.WeightHistoryViewModel
import com.example.befit.settings.SettingsNavigation
import com.example.befit.settings.SettingsViewModel
import com.example.befit.stopwatches.StopwatchesNavigation
import com.example.befit.stopwatches.StopwatchesViewModel
import com.example.befit.trainingprograms.TrainingProgramsNavigation
import com.example.befit.trainingprograms.TrainingProgramsViewModel

@Composable
fun AppNavigation(
    viewModel: AppViewModel,
    trainingProgramsViewModel: TrainingProgramsViewModel,
    stopwatchesViewModel: StopwatchesViewModel,
    healthViewModel: HealthViewModel,
    weightHistoryViewModel: WeightHistoryViewModel,
    calorieCalculatorViewModel: CalorieCalculatorViewModel,
    settingsViewModel: SettingsViewModel,
    modifier: Modifier = Modifier
) {
    val navController = rememberNavController()
    val currentRoute by viewModel.currentRoute

    Column(
        modifier = modifier
            .fillMaxSize()
    ) {
        NavHost(
            navController = navController,
            startDestination = currentRoute,
            enterTransition = { EnterTransition.None },
            exitTransition = { ExitTransition.None },
            modifier = Modifier
                .weight(1f)
                .fillMaxWidth()
        ) {
            composable(route = AppRoutes.TRAINING_PROGRAMS) {
                viewModel.updateCurrentRoute(AppRoutes.TRAINING_PROGRAMS)
                TrainingProgramsNavigation(
                    viewModel = trainingProgramsViewModel
                )
            }
            composable(route = AppRoutes.STOPWATCHES) {
                viewModel.updateCurrentRoute(AppRoutes.STOPWATCHES)
                StopwatchesNavigation(
                    viewModel = stopwatchesViewModel
                )
            }
            composable(route = AppRoutes.HEALTH) {
                viewModel.updateCurrentRoute(AppRoutes.HEALTH)
                HealthNavigation(
                    viewModel = healthViewModel,
                    weightHistoryViewModel = weightHistoryViewModel,
                    calorieCalculatorViewModel = calorieCalculatorViewModel
                )
            }
            composable(route = AppRoutes.SETTINGS) {
                viewModel.updateCurrentRoute(AppRoutes.SETTINGS)
                SettingsNavigation(
                    viewModel = settingsViewModel
                )
            }
        }

        BottomNavbar(
            navController = navController,
            trainingProgramsViewModel = trainingProgramsViewModel,
            stopwatchesViewModel = stopwatchesViewModel,
            healthViewModel = healthViewModel,
            currentRoute = currentRoute
        )
    }

}