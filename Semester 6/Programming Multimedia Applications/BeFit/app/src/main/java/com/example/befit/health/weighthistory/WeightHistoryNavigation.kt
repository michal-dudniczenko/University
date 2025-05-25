package com.example.befit.health.weighthistory

import androidx.compose.animation.EnterTransition
import androidx.compose.animation.ExitTransition
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.remember
import androidx.compose.ui.Modifier
import androidx.navigation.NavHostController
import androidx.navigation.NavType
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import androidx.navigation.navArgument
import com.example.befit.common.WeightHistoryRoutes

@Composable
fun WeightHistoryNavigation(
    topLevelNavController: NavHostController,
    viewModel: WeightHistoryViewModel,
    modifier: Modifier = Modifier
) {
    val navController = rememberNavController()
    val currentRoute by viewModel.currentRoute

    LaunchedEffect(Unit) {
        viewModel.navigationEvent.collect { route ->
            navController.navigate(route)
        }
    }

    NavHost(
        navController = navController,
        startDestination = currentRoute,
        enterTransition = { EnterTransition.None },
        exitTransition = { ExitTransition.None },
        modifier = modifier
            .fillMaxSize()
    ) {
        composable(route = WeightHistoryRoutes.HISTORY) {
            viewModel.updateCurrentRoute(WeightHistoryRoutes.HISTORY)
            WeightHistoryScreen(
                viewModel = viewModel,
                navController = navController,
                topLevelNavController = topLevelNavController
            )
        }
        composable(
            route = WeightHistoryRoutes.EDIT,
            arguments = listOf(navArgument("id") { type = NavType.IntType })
        ) { backStackEntry ->
            val weightId = backStackEntry.arguments?.getInt("id")
            weightId?.let {
                viewModel.updateCurrentRoute(WeightHistoryRoutes.EDIT(weightId))
                EditWeightScreen(
                    weightId = it,
                    viewModel = viewModel,
                    navController = navController
                )
            }
        }
        composable(route = WeightHistoryRoutes.ADD) {
            viewModel.updateCurrentRoute(WeightHistoryRoutes.ADD)
            AddWeightScreen(
                viewModel = viewModel,
                navController = navController
            )
        }
    }
}




