package com.example.befit.settings

import androidx.compose.animation.EnterTransition
import androidx.compose.animation.ExitTransition
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.example.befit.common.SETTINGS_SCREENS
import com.example.befit.common.currentSettingsScreen
import com.example.befit.common.settingsStartScreen

@Composable
fun SettingsNavigation(
    modifier: Modifier = Modifier
) {
    Box(
        modifier = modifier
    ) {
        val navController = rememberNavController()
        NavHost(
            navController = navController,
            startDestination = SETTINGS_SCREENS[settingsStartScreen],
            enterTransition = { EnterTransition.None },
            exitTransition = { ExitTransition.None },
            modifier = Modifier
                .fillMaxSize()
        ) {
            composable(route = SETTINGS_SCREENS[0]) {
                currentSettingsScreen.intValue = 0
                SettingsListScreen(navController)
            }
        }
    }
}