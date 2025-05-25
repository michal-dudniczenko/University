package com.example.befit.common

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
import androidx.compose.foundation.layout.height
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.befit.health.HealthViewModel
import com.example.befit.stopwatches.StopwatchesViewModel
import com.example.befit.trainingprograms.TrainingProgramsViewModel

@Composable
fun BottomNavbar(
    navController: NavHostController,
    currentRoute: String?,
    trainingProgramsViewModel: TrainingProgramsViewModel,
    stopwatchesViewModel: StopwatchesViewModel,
    healthViewModel: HealthViewModel,
    modifier: Modifier = Modifier
) {
    Row(
        modifier = modifier
            .fillMaxWidth()
            .height(NAVBAR_HEIGHT.dp)
    ) {
        for (screen in AppRoutes.screens) {
            val isSelected = currentRoute == screen.route
            BottomNavbarIcon(
                isSelected = isSelected,
                iconId = if (isSelected) screen.iconBlack else screen.iconWhite,
                onClick = {
                    if (currentRoute == AppRoutes.TRAINING_PROGRAMS) {
                        trainingProgramsViewModel.navigateToStart()
                    } else if (currentRoute == AppRoutes.STOPWATCHES) {
                        stopwatchesViewModel.navigateToStart()
                    }
                    else if (currentRoute == AppRoutes.HEALTH) {
                        healthViewModel.navigateToStart()
                    }
                    navController.navigate(screen.route)

                },
                modifier = Modifier.weight(1f)
            )
        }
    }
}


@Composable
fun BottomNavbarIcon(
    isSelected: Boolean,
    iconId: Int,
    onClick: () -> Unit,
    modifier: Modifier = Modifier
) {
    Column(
        horizontalAlignment = Alignment.CenterHorizontally,
        verticalArrangement = Arrangement.Center,
        modifier = modifier
            .fillMaxSize()
            .background(if (isSelected) bright else darkBackground)
            .clickable(onClick = onClick)
    ) {
        Box(
            modifier = Modifier
                .fillMaxHeight(0.6f)
        ) {
            Image(
                painter = painterResource(id = iconId),
                contentDescription = "Bottom navbar button icon"
            )
        }
    }
}