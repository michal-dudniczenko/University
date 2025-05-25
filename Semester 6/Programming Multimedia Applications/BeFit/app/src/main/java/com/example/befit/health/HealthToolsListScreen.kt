package com.example.befit.health

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.navigation.NavHostController
import com.example.befit.common.FunctionalityRow
import com.example.befit.common.Heading
import com.example.befit.constants.HealthRoutes
import com.example.befit.constants.Strings

@Composable
fun HealthToolsListScreen(
    navController: NavHostController,
    modifier: Modifier = Modifier
) {
    Box (
        modifier = modifier
            .fillMaxSize()
    ) {
        Column(
            horizontalAlignment = Alignment.CenterHorizontally,
            modifier = modifier
                .fillMaxWidth(0.8f)
                .fillMaxHeight(0.9f)
                .align(Alignment.Center)
        ) {
            Heading(Strings.HEALTH)
            Column(
                horizontalAlignment = Alignment.CenterHorizontally,
                modifier = Modifier
                    .fillMaxWidth(0.95f)
                    .fillMaxHeight()
                    .verticalScroll(rememberScrollState())
            ) {
                for (i in 1 until HealthRoutes.screens.size) {
                    FunctionalityRow(
                        text = HealthRoutes.screens[i],
                        onClick = {
                            navController.navigate(HealthRoutes.screens[i])
                        }
                    )
                }
            }
        }
    }
}