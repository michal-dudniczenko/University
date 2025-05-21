package com.example.befit.health

import androidx.compose.animation.EnterTransition
import androidx.compose.animation.ExitTransition
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.example.befit.common.HEALTH_SCREENS
import com.example.befit.common.currentHealthScreen
import com.example.befit.common.healthStartScreen
import com.example.befit.common.topLevelStartScreen
import com.example.befit.health.bmicalculator.BmiCalculatorScreen
import com.example.befit.health.caloriecalculator.CalorieCalculatorScreen
import com.example.befit.health.dietplans.DietPlansScreen
import com.example.befit.health.hydration.HydrationCalculatorScreen
import com.example.befit.health.proteincalculator.ProteinCalculatorScreen
import com.example.befit.health.weightmanager.WeightManagerNavigation
import com.example.befit.health.weightmanager.WeightManagerViewModel

@Composable
fun HealthNavigation(
    weightManagerViewModel: WeightManagerViewModel,
    modifier: Modifier = Modifier
) {
    Box(
        modifier = modifier
    ) {
        val navController = rememberNavController()
        NavHost(
            navController = navController,
            startDestination = HEALTH_SCREENS[healthStartScreen],
            enterTransition = { EnterTransition.None },
            exitTransition = { ExitTransition.None },
            modifier = Modifier
                .fillMaxSize()
        ) {
            composable(route = HEALTH_SCREENS[0]) {
                currentHealthScreen.intValue = 0
                HealthToolsListScreen(navController)
            }
            composable(route = HEALTH_SCREENS[1]) {
                currentHealthScreen.intValue = 1
                DietPlansScreen(navController)
            }
            composable(route = HEALTH_SCREENS[2]) {
                currentHealthScreen.intValue = 2
                WeightManagerNavigation(
                    topLevelNavController = navController,
                    viewModel = weightManagerViewModel
                )
            }
            composable(route = HEALTH_SCREENS[3]) {
                currentHealthScreen.intValue = 3
                CalorieCalculatorScreen(navController)
            }
            composable(route = HEALTH_SCREENS[4]) {
                currentHealthScreen.intValue = 4
                ProteinCalculatorScreen(navController)
            }
            composable(route = HEALTH_SCREENS[5]) {
                currentHealthScreen.intValue = 5
                BmiCalculatorScreen(navController)
            }
            composable(route = HEALTH_SCREENS[6]) {
                currentHealthScreen.intValue = 6
                HydrationCalculatorScreen(navController)
            }
        }
    }
}