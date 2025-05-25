package com.example.befit.settings

import androidx.compose.animation.EnterTransition
import androidx.compose.animation.ExitTransition
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.ui.Modifier
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.example.befit.constants.SettingsRoutes

@Composable
fun SettingsNavigation(
    viewModel: SettingsViewModel,
    modifier: Modifier = Modifier
) {
    val navController = rememberNavController()
    val currentRoute by viewModel.currentRoute

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
            composable(route = SettingsRoutes.SETTINGS_LIST) {
                viewModel.updateCurrentRoute(SettingsRoutes.SETTINGS_LIST)
                SettingsListScreen(viewModel)
            }
        }
    }
}