package com.example.befit

import androidx.compose.animation.EnterTransition
import androidx.compose.animation.ExitTransition
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.example.befit.common.BottomNavbar
import com.example.befit.common.InitScreenDimensions
import com.example.befit.common.NAVBAR_HEIGHT
import com.example.befit.common.TOP_LEVEL_SCREENS
import com.example.befit.common.adaptiveHeight
import com.example.befit.common.adaptiveWidth
import com.example.befit.common.currentTopLevelScreen
import com.example.befit.common.topLevelStartScreen
import com.example.befit.health.HealthNavigation
import com.example.befit.health.weightmanager.WeightManagerViewModel
import com.example.befit.settings.SettingsNavigation
import com.example.befit.stopwatches.StopwatchesScreen
import com.example.befit.stopwatches.StopwatchesViewModel
import com.example.befit.trainingprograms.TrainingProgramsNavigation
import com.example.befit.trainingprograms.TrainingProgramsViewModel

@Composable
fun App(
    stopwatchesViewModel: StopwatchesViewModel,
    weightManagerViewModel: WeightManagerViewModel,
    trainingProgramsViewModel: TrainingProgramsViewModel,
    modifier: Modifier = Modifier
) {
    InitScreenDimensions()

    Box(
        modifier = modifier
    ) {
        val navController = rememberNavController()
        NavHost(
            navController = navController,
            startDestination = TOP_LEVEL_SCREENS[topLevelStartScreen],
            enterTransition = { EnterTransition.None },
            exitTransition = { ExitTransition.None },
            modifier = Modifier
                .padding(bottom = adaptiveHeight(NAVBAR_HEIGHT).dp)
                .fillMaxSize()
        ) {
            composable(route = TOP_LEVEL_SCREENS[0]) {
                currentTopLevelScreen.intValue = 0
                TrainingProgramsNavigation(
                    viewModel = trainingProgramsViewModel
                )
            }
            composable(route = TOP_LEVEL_SCREENS[1]) {
                currentTopLevelScreen.intValue = 1
                StopwatchesScreen(
                    viewModel = stopwatchesViewModel
                )
            }
            composable(route = TOP_LEVEL_SCREENS[2]) {
                currentTopLevelScreen.intValue = 2
                HealthNavigation(
                    weightManagerViewModel = weightManagerViewModel
                )
            }
            composable(route = TOP_LEVEL_SCREENS[3]) {
                currentTopLevelScreen.intValue = 3
                SettingsNavigation()
            }
        }

        BottomNavbar(
            navController = navController,
            modifier = Modifier
                .align(Alignment.BottomCenter)
                .height(adaptiveWidth(NAVBAR_HEIGHT).dp)
        )
    }

}