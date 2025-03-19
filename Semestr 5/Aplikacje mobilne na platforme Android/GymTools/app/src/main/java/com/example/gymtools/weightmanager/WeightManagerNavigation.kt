package com.example.gymtools.weightmanager

import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.runtime.Composable
import androidx.compose.runtime.remember
import androidx.compose.ui.Modifier
import androidx.navigation.NavType
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import androidx.navigation.navArgument

@Composable
fun WeightManagerNavigation(
    viewModel: WeightManagerViewModel,
    modifier: Modifier = Modifier
) {
    val navController = rememberNavController()

    val startDestination = remember(viewModel.currentRoute) { viewModel.currentRoute }

    NavHost(
        navController = navController,
        startDestination = startDestination,
        modifier = modifier
            .fillMaxSize()
    ) {
        composable(route = "Weight history") {
            viewModel.updateCurrentRoute("Weight history")
            WeightHistoryScreen(
                viewModel = viewModel,
                navController = navController
            )
        }
        composable(
            route = "Edit weight/{weightId}",
            arguments = listOf(navArgument("weightId") { type = NavType.IntType })
        ) { backStackEntry ->
            val weightId = backStackEntry.arguments?.getInt("weightId")
            weightId?.let {
                viewModel.updateCurrentRoute("Edit weight/${weightId}")
                EditWeightScreen(
                    weightId = it,
                    viewModel = viewModel,
                    navController = navController
                )
            }
        }
        composable(route = "Add weight") {
            viewModel.updateCurrentRoute("Add weight")
            AddWeightScreen(
                viewModel = viewModel,
                navController = navController
            )
        }
    }
}




