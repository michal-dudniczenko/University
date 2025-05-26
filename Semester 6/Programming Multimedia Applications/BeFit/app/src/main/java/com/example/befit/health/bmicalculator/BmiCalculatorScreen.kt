package com.example.befit.health.bmicalculator

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableFloatStateOf
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.befit.common.CustomFloatPicker
import com.example.befit.common.CustomFloatingButton
import com.example.befit.common.CustomIntPicker
import com.example.befit.common.CustomText
import com.example.befit.common.Heading
import com.example.befit.constants.HealthRoutes
import com.example.befit.constants.Strings
import com.example.befit.constants.Themes
import com.example.befit.constants.adaptiveWidth
import com.example.befit.health.HealthViewModel

@Composable
fun BmiCalculatorScreen(
    healthViewModel: HealthViewModel,
    navController: NavHostController,
    modifier: Modifier = Modifier
) {
    val userData by healthViewModel.userData

    var selectedHeight by remember { mutableIntStateOf(userData?.height ?: 0) }
    var selectedWeight by remember { mutableFloatStateOf(userData?.weight ?: 0f) }

    var bmi by remember { mutableFloatStateOf(0f) }

    Box(
        modifier = modifier
            .fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = Themes.BACK_ON_SECONDARY,
            description = "Back button",
            onClick = { navController.navigate(HealthRoutes.TOOLS_LIST) },
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
            Heading(Strings.BMI_CALCULATOR)
            Column(
                horizontalAlignment = Alignment.CenterHorizontally,
                modifier = Modifier
                    .fillMaxWidth(0.7f)
                    .padding(top = 50.dp)
            ) {
                CustomIntPicker(
                    selectedValue = selectedHeight,
                    onValueChange = { selectedHeight = it },
                    label = Strings.HEIGHT
                )
                Spacer(Modifier.height(24.dp))
                CustomFloatPicker(
                    selectedValue = selectedWeight,
                    onValueChange = { selectedWeight = it },
                    label = Strings.WEIGHT_BODY
                )
                Spacer(Modifier.height(24.dp))
                Box(
                    modifier = Modifier
                        .fillMaxWidth(0.8f)
                        .clip(RoundedCornerShape(adaptiveWidth(32).dp))
                        .background(color = Themes.SECONDARY)
                        .clickable {
                            if (selectedWeight > 0
                                && selectedHeight > 0
                            ) {
                                bmi = healthViewModel.calculateBmi(
                                    height = selectedHeight,
                                    weight = selectedWeight,
                                )
                            }
                        }
                        .padding(16.dp)
                ) {
                    CustomText(
                        text = Strings.CALCULATE,
                        color = Themes.ON_SECONDARY,
                        modifier = Modifier
                            .align(Alignment.Center)
                    )
                }
                Spacer(Modifier.height(40.dp))
            }
            if (bmi > 0) {
                BmiResult(bmi)
            }
        }
    }
}