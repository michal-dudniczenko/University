package com.example.befit.health.caloriecalculator

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
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.befit.common.CustomActivityLevelPicker
import com.example.befit.common.CustomFloatPicker
import com.example.befit.common.CustomFloatingButton
import com.example.befit.common.CustomIntPicker
import com.example.befit.common.CustomSexPicker
import com.example.befit.common.CustomText
import com.example.befit.common.Heading
import com.example.befit.constants.CalorieCalculatorRoutes
import com.example.befit.constants.HealthRoutes
import com.example.befit.constants.Strings
import com.example.befit.constants.Themes
import com.example.befit.constants.adaptiveWidth
import com.example.befit.health.HealthViewModel

@Composable
fun CalorieCalculatorScreen(
    viewModel: CalorieCalculatorViewModel,
    healthViewModel: HealthViewModel,
    navController: NavHostController,
    topLevelNavController: NavHostController,
    modifier: Modifier = Modifier
) {
    val userData by healthViewModel.userData

    var selectedAge by remember { mutableIntStateOf(userData?.age ?: 0) }
    var selectedHeight by remember { mutableIntStateOf(userData?.height ?: 0) }
    var selectedWeight by remember { mutableFloatStateOf(userData?.weight ?: 0f) }
    var isMale by remember { mutableStateOf<Boolean?>(userData?.isMale) }
    var selectedActivityLevel by remember { mutableIntStateOf(userData?.activityLevel ?: 0) }

    Box(
        modifier = modifier
            .fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = Themes.BACK_ON_SECONDARY,
            description = "Back button",
            onClick = { topLevelNavController.navigate(HealthRoutes.TOOLS_LIST) },
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
            Heading(Strings.CALORIE_CALCULATOR)
            Column(
                horizontalAlignment = Alignment.CenterHorizontally,
                modifier = Modifier
                    .fillMaxWidth(0.7f)
                    .fillMaxHeight()
                    .padding(bottom = 107.dp)
            ) {
                CustomIntPicker(
                    selectedValue = selectedAge,
                    onValueChange = { selectedAge = it },
                    label = Strings.AGE
                )
                Spacer(Modifier.height(24.dp))
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
                CustomSexPicker(
                    selectedValue = if (isMale == null) null else if (isMale!!) Strings.MALE else Strings.FEMALE,
                    onValueSelected = { isMale = it }
                )
                Spacer(Modifier.height(24.dp))
                CustomActivityLevelPicker(
                    selectedValue = if (selectedActivityLevel > 0) ActivityLevels.levels[selectedActivityLevel-1] else null,
                    onValueSelected = { selectedActivityLevel = it }
                )
                Spacer(Modifier.height(32.dp))
                Box(
                    modifier = Modifier
                        .fillMaxWidth(0.8f)
                        .clip(RoundedCornerShape(adaptiveWidth(32).dp))
                        .background(color = Themes.SECONDARY)
                        .clickable {
                            if (selectedAge > 0
                                && selectedHeight > 0
                                && selectedWeight > 0
                                && isMale != null
                                && selectedActivityLevel > 0
                            ) {
                                viewModel.calculateCalories(
                                    age = selectedAge,
                                    height = selectedHeight,
                                    weight = selectedWeight,
                                    isMale = isMale!!,
                                    activityLevel = ActivityLevels.levels[selectedActivityLevel-1]
                                )
                                navController.navigate(CalorieCalculatorRoutes.RESULT)
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
            }
        }
    }
}
