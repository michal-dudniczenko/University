package com.example.befit.stopwatches

import androidx.compose.animation.EnterTransition
import androidx.compose.animation.ExitTransition
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.ui.Modifier
import androidx.navigation.NavType
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import androidx.navigation.navArgument
import com.example.befit.common.StopwatchesRoutes

@Composable
fun StopwatchesNavigation(
    viewModel: StopwatchesViewModel,
    modifier: Modifier = Modifier
) {
    val navController = rememberNavController()
    val currentRoute by viewModel.currentRoute

    LaunchedEffect(Unit) {
        viewModel.navigationEvent.collect { route ->
            navController.navigate(route)
        }
    }

    Box(
        modifier = modifier
    ) {
        NavHost(
            navController = navController,
            startDestination = currentRoute,
            enterTransition = { EnterTransition.None },
            exitTransition = { ExitTransition.None },
            modifier = Modifier
                .fillMaxSize()
        ) {
            composable(route = StopwatchesRoutes.STOPWATCHES) {
                viewModel.updateCurrentRoute(StopwatchesRoutes.STOPWATCHES)
                StopwatchesScreen(
                    viewModel = viewModel,
                    navController = navController
                )
            }
            composable(
                route = StopwatchesRoutes.EDIT,
                arguments = listOf(navArgument("id") { type = NavType.IntType })
            ) { backStackEntry ->
                val stopwatchId = backStackEntry.arguments?.getInt("id")
                stopwatchId?.let {
                    viewModel.updateCurrentRoute(StopwatchesRoutes.EDIT(stopwatchId))
                    EditStopwatchNameScreen(
                        stopwatchId = it,
                        viewModel = viewModel,
                        navController = navController
                    )
                }
            }
        }
    }
}