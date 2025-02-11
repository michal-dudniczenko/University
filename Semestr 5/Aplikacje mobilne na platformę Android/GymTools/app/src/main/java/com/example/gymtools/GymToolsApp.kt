package com.example.gymtools

import androidx.compose.animation.ExperimentalAnimationApi
import androidx.compose.animation.core.tween
import androidx.compose.animation.fadeIn
import androidx.compose.animation.fadeOut
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
import com.example.gymtools.common.BottomNavbar
import com.example.gymtools.common.InitScreenDimensions
import com.example.gymtools.common.NAVBAR_HEIGHT
import com.example.gymtools.common.SCREENS
import com.example.gymtools.common.adaptiveHeight
import com.example.gymtools.common.adaptiveWidth
import com.example.gymtools.common.currentScreen
import com.example.gymtools.common.startScreen
import com.example.gymtools.stopwatches.StopwatchesScreen
import com.example.gymtools.stopwatches.StopwatchesViewModel
import com.example.gymtools.trainingprograms.TrainingProgramsNavigation
import com.example.gymtools.trainingprograms.TrainingProgramsViewModel
import com.example.gymtools.weightmanager.WeightManagerNavigation
import com.example.gymtools.weightmanager.WeightManagerViewModel
import com.google.accompanist.navigation.animation.AnimatedNavHost

@OptIn(ExperimentalAnimationApi::class)
@Composable
fun GymToolsApp(
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
        AnimatedNavHost(
            navController = navController,
            startDestination = SCREENS[startScreen],
            enterTransition = { fadeIn(animationSpec = tween(durationMillis = 500)) },  // Example transition
            exitTransition = { fadeOut(animationSpec = tween(durationMillis = 500)) },  // Example transition
            popEnterTransition = { fadeIn(animationSpec = tween(durationMillis = 500)) },
            popExitTransition = { fadeOut(animationSpec = tween(durationMillis = 500)) },
            modifier = Modifier
                .padding(bottom = adaptiveHeight(NAVBAR_HEIGHT).dp)
                .fillMaxSize()
        ) {
            composable(route = SCREENS[0]) {
                currentScreen.intValue = 0
                StopwatchesScreen(stopwatchesViewModel)
            }
            composable(route = SCREENS[1]) {
                currentScreen.intValue = 1
                TrainingProgramsNavigation(trainingProgramsViewModel)
            }
            composable(route = SCREENS[2]) {
                currentScreen.intValue = 2
                WeightManagerNavigation(weightManagerViewModel)
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