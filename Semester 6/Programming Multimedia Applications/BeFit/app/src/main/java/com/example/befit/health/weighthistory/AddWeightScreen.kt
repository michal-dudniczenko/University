package com.example.befit.health.weighthistory

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.offset
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableFloatStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.befit.common.CustomFloatPicker
import com.example.befit.common.CustomFloatingButton
import com.example.befit.common.CustomStringPicker
import com.example.befit.common.Heading
import com.example.befit.common.formatDateFromLong
import com.example.befit.common.isValidDate
import com.example.befit.constants.Strings
import com.example.befit.constants.Themes
import com.example.befit.constants.WeightHistoryRoutes
import com.example.befit.constants.adaptiveHeight
import com.example.befit.constants.adaptiveWidth

@Composable
fun AddWeightScreen(
    navController: NavHostController,
    viewModel: WeightHistoryViewModel,
    modifier: Modifier = Modifier
) {
    var selectedDate by remember { mutableStateOf(formatDateFromLong(System.currentTimeMillis())) }
    var selectedWeight by remember { mutableFloatStateOf(0f) }

    Box(
        modifier = modifier.fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = Themes.CHECK_ON_ADD_CONFIRM,
            description = "Confirm button",
            color = Themes.ADD_CONFIRM_COLOR,
            onClick = {
                if (selectedWeight != 0f) {
                    val date = isValidDate(selectedDate)
                    if (date != null) {
                        viewModel.addWeight(date = date.time, weight = selectedWeight)
                        navController.navigate(WeightHistoryRoutes.HISTORY)
                    }
                }
            },
            modifier = Modifier
                .align(Alignment.BottomEnd)
                .offset(x = adaptiveWidth(-32).dp, y = adaptiveWidth(-32).dp)
        )
        CustomFloatingButton(
            icon = Themes.CANCEL_ON_DELETE_CANCEL,
            description = "Cancel button",
            color = Themes.DELETE_CANCEL_COLOR,
            onClick = { navController.navigate(WeightHistoryRoutes.HISTORY) },
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
            Heading(Strings.ADD_WEIGHT)
            Column(
                horizontalAlignment = Alignment.CenterHorizontally,
                verticalArrangement = Arrangement.Center,
                modifier = modifier
                    .fillMaxWidth(0.95f)
                    .fillMaxHeight()
            ) {
                CustomStringPicker(
                    selectedValue = selectedDate,
                    onValueChange = { selectedDate = it },
                    label = Strings.DATE,
                    imageId = Themes.CALENDAR_ON_PRIMARY,
                )
                Spacer(modifier = Modifier.height(adaptiveHeight(40).dp))
                CustomFloatPicker(
                    label = Strings.WEIGHT_BODY,
                    imageId = Themes.NUMBERS_ON_PRIMARY,
                    onValueChange = { selectedWeight = it }
                )
                Spacer(modifier = Modifier.height(adaptiveHeight(200).dp))
            }
        }
    }
}