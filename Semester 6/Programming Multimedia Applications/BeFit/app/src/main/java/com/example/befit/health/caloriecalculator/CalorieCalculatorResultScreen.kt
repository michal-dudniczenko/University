package com.example.befit.health.caloriecalculator

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.befit.common.CustomFloatingButton
import com.example.befit.common.Heading
import com.example.befit.constants.CalorieCalculatorRoutes
import com.example.befit.constants.Strings
import com.example.befit.constants.Themes
import com.example.befit.constants.adaptiveWidth

@Composable
fun CalorieCalculatorResultScreen(
    viewModel: CalorieCalculatorViewModel,
    navController: NavHostController,
    modifier: Modifier = Modifier
) {
    val result by viewModel.calculateResult

    Box(
        modifier = modifier
            .fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = Themes.BACK_ON_SECONDARY,
            description = "Back button",
            onClick = {
                navController.navigate(CalorieCalculatorRoutes.CALCULATOR)
                viewModel.clearResult()
            },
            modifier = Modifier
                .align(Alignment.BottomStart)
                .offset(x = adaptiveWidth(32).dp, y = adaptiveWidth(-32).dp)
        )
        Column(
            horizontalAlignment = Alignment.CenterHorizontally,
            modifier = modifier
                .fillMaxWidth(0.8f)
                .fillMaxHeight(0.9f)
                .align(Alignment.Center)
        ) {
            Heading(Strings.RESULT)
            Column(
                modifier = Modifier
                    .fillMaxWidth(0.95f)
                    .fillMaxHeight()
                    .padding(bottom = 125.dp)
                    .verticalScroll(rememberScrollState())
            ) {
                ResultRow(
                    text = Strings.YOUR_BMR_IS,
                    kcal = result?.bmr,
                    backgroundColor = Themes.PRIMARY,
                    fontColor = Themes.ON_PRIMARY
                )
                ResultRow(
                    text = Strings.MAINTAIN_WEIGHT,
                    kcal = result?.maintain,
                    backgroundColor = Color(72, 196, 184, 255)
                )
                ResultRow(
                    text = Strings.BUILD_MUSCLE,
                    kcal = result?.gainWeight,
                    backgroundColor = Color(134, 87, 222, 255)
                )
                ResultRow(
                    text = Strings.SLOW_WEIGHT_LOSS,
                    bottomText = "0.25 kg/${Strings.WEEK}",
                    kcal = result?.slowWeightLoss,
                    backgroundColor = Color(96, 197, 77, 255)
                )
                ResultRow(
                    text = Strings.WEIGHT_LOSS,
                    bottomText = "0.5 kg/${Strings.WEEK}",
                    kcal = result?.weightLoss,
                    backgroundColor = Color(239, 82, 41, 255)
                )
                ResultRow(
                    text = Strings.FAST_WEIGHT_LOSS,
                    bottomText = "1 kg/${Strings.WEEK}",
                    kcal = result?.fastWeightLoss,
                    backgroundColor = Color(224, 53, 53, 255)
                )

            }
        }
    }
}