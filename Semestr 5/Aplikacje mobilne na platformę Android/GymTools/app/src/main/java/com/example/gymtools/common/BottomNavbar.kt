package com.example.gymtools.common

import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.painterResource
import androidx.navigation.NavHostController

@Composable
fun BottomNavbar(
    navController: NavHostController,
    modifier: Modifier = Modifier
) {
    Row(
        modifier = modifier
            .fillMaxWidth()
            .background(darkBackground)
    ) {
        for (i in SCREENS.indices) {
            BottomNavbarIcon(
                screenNumber = i,
                navController = navController,
                modifier = Modifier.weight(1f)
            )
        }
    }
}


@Composable
fun BottomNavbarIcon(
    screenNumber: Int,
    navController: NavHostController,
    modifier: Modifier = Modifier
) {
    var currentScreen by currentScreen

    Column(
        horizontalAlignment = Alignment.CenterHorizontally,
        verticalArrangement = Arrangement.Center,
        modifier = modifier
            .fillMaxSize()
            .background(if (currentScreen == screenNumber) bright else darkBackground)
            .clickable {
                currentScreen = screenNumber
                navController.navigate(SCREENS[screenNumber])
            }
    ) {
        Box(
            modifier = Modifier
                .fillMaxHeight(0.6f)
        ) {
            Image(
                painter = painterResource(id = if (currentScreen == screenNumber) SCREEN_ICON_FILE_NAME_BLACK[screenNumber] else SCREEN_ICON_FILE_NAME_WHITE[screenNumber]),
                contentDescription = "${SCREENS[screenNumber]} screen icon"
            )
        }
    }
}