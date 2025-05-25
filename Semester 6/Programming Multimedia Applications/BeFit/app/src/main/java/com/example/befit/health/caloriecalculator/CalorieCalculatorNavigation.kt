package com.example.befit.health.caloriecalculator

import androidx.compose.animation.EnterTransition
import androidx.compose.animation.ExitTransition
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.ui.Modifier
import androidx.navigation.NavHostController
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.example.befit.constants.CalorieCalculatorRoutes
import com.example.befit.health.HealthViewModel

@Composable
fun CalorieCalculatorNavigation(
    viewModel: CalorieCalculatorViewModel,
    healthViewModel: HealthViewModel,
    topLevelNavController: NavHostController,
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
            composable(route = CalorieCalculatorRoutes.CALCULATOR) {
                viewModel.updateCurrentRoute(CalorieCalculatorRoutes.CALCULATOR)
                CalorieCalculatorScreen(
                    healthViewModel = healthViewModel,
                    viewModel = viewModel,
                    navController = navController,
                    topLevelNavController = topLevelNavController,
                )
            }
            composable(route = CalorieCalculatorRoutes.RESULT) {
                viewModel.updateCurrentRoute(CalorieCalculatorRoutes.RESULT)
                CalorieCalculatorResultScreen(
                    viewModel = viewModel,
                    navController = navController
                )
            }
        }
    }
}