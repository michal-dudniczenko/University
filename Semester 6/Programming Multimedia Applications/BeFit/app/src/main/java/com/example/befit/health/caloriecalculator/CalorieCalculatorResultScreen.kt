package com.example.befit.health.caloriecalculator

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.padding
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.befit.R
import com.example.befit.common.CalorieCalculatorRoutes
import com.example.befit.common.CustomFloatingButton
import com.example.befit.common.Heading
import com.example.befit.common.adaptiveWidth
import com.example.befit.common.bright
import com.example.befit.common.darkBackground

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
            icon = R.drawable.back,
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
            Heading("Result")
            Column(
                modifier = Modifier
                    .fillMaxWidth(0.95f)
                    .fillMaxHeight()
                    .padding(bottom = 125.dp)
            ) {
                ResultRow(
                    text = "Your BMR is:",
                    kcal = result?.bmr,
                    backgroundColor = darkBackground,
                    fontColor = bright
                )
                ResultRow(
                    text = "Maintain weight:",
                    kcal = result?.maintain,
                    backgroundColor = Color(72, 196, 184, 255)
                )
                ResultRow(
                    text = "Build muscle:",
                    kcal = result?.gainWeight,
                    backgroundColor = Color(134, 87, 222, 255)
                )
                ResultRow(
                    text = "Slow weight loss:",
                    bottomText = "0.25 kg/week",
                    kcal = result?.slowWeightLoss,
                    backgroundColor = Color(96, 197, 77, 255)
                )
                ResultRow(
                    text = "Weight loss:",
                    bottomText = "0.5 kg/week",
                    kcal = result?.weightLoss,
                    backgroundColor = Color(239, 82, 41, 255)
                )
                ResultRow(
                    text = "Fast weight loss:",
                    bottomText = "1 kg/week",
                    kcal = result?.fastWeightLoss,
                    backgroundColor = Color(224, 53, 53, 255)
                )

            }
        }
    }
}