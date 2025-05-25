package com.example.befit.health

import androidx.compose.animation.EnterTransition
import androidx.compose.animation.ExitTransition
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.ui.Modifier
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.example.befit.common.HealthRoutes
import com.example.befit.health.bmicalculator.BmiCalculatorScreen
import com.example.befit.health.caloriecalculator.CalorieCalculatorNavigation
import com.example.befit.health.caloriecalculator.CalorieCalculatorViewModel
import com.example.befit.health.dietplans.DietPlansScreen
import com.example.befit.health.waterintakecalculator.WaterIntakeCalculatorScreen
import com.example.befit.health.weighthistory.WeightHistoryNavigation
import com.example.befit.health.weighthistory.WeightHistoryViewModel

@Composable
fun HealthNavigation(
    viewModel: HealthViewModel,
    weightHistoryViewModel: WeightHistoryViewModel,
    calorieCalculatorViewModel: CalorieCalculatorViewModel,
    modifier: Modifier = Modifier
) {
    val navController = rememberNavController()
    val currentRoute by viewModel.currentRoute

    LaunchedEffect(Unit) {
        viewModel.navigationEvent.collect { route ->
            navController.navigate(route)
            calorieCalculatorViewModel.navigateToStart()
            weightHistoryViewModel.navigateToStart()
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
            composable(route = HealthRoutes.TOOLS_LIST) {
                viewModel.updateCurrentRoute(HealthRoutes.TOOLS_LIST)
                HealthToolsListScreen(navController)
            }
            composable(route = HealthRoutes.DIET_PLANS) {
                viewModel.updateCurrentRoute(HealthRoutes.DIET_PLANS)
                DietPlansScreen(navController)
            }
            composable(route = HealthRoutes.WEIGHT_HISTORY) {
                viewModel.updateCurrentRoute(HealthRoutes.WEIGHT_HISTORY)
                WeightHistoryNavigation(
                    topLevelNavController = navController,
                    viewModel = weightHistoryViewModel
                )
            }
            composable(route = HealthRoutes.CALORIE_CALCULATOR) {
                viewModel.updateCurrentRoute(HealthRoutes.CALORIE_CALCULATOR)
                CalorieCalculatorNavigation(
                    viewModel = calorieCalculatorViewModel,
                    healthViewModel = viewModel,
                    topLevelNavController = navController
                )
            }
            composable(route = HealthRoutes.BMI_CALCULATOR) {
                viewModel.updateCurrentRoute(HealthRoutes.BMI_CALCULATOR)
                BmiCalculatorScreen(
                    healthViewModel = viewModel,
                    navController = navController
                )
            }
            composable(route = HealthRoutes.WATER_INTAKE_CALCULATOR) {
                viewModel.updateCurrentRoute(HealthRoutes.WATER_INTAKE_CALCULATOR)
                WaterIntakeCalculatorScreen(
                    healthViewModel = viewModel,
                    navController = navController
                )
            }
        }
    }
}