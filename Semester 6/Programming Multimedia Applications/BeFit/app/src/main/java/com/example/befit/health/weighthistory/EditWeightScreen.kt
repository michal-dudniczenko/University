package com.example.befit.health.weighthistory

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
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
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.befit.R
import com.example.befit.common.CustomFloatPicker
import com.example.befit.common.CustomFloatingButton
import com.example.befit.common.CustomStringPicker
import com.example.befit.common.CustomText
import com.example.befit.common.Heading
import com.example.befit.constants.WeightHistoryRoutes
import com.example.befit.constants.adaptiveHeight
import com.example.befit.constants.adaptiveWidth
import com.example.befit.common.formatDateFromLong
import com.example.befit.common.isValidDate
import com.example.befit.constants.Strings
import com.example.befit.constants.lightGreen
import com.example.befit.constants.lightRed

@Composable
fun EditWeightScreen(
    navController: NavHostController,
    weightId: Int,
    viewModel: WeightHistoryViewModel,
    modifier: Modifier = Modifier
) {
    val weight = viewModel.getWeightById(id = weightId)

    var selectedDate by remember { mutableStateOf(formatDateFromLong(weight?.date ?: System.currentTimeMillis())) }
    var selectedWeight by remember { mutableFloatStateOf(weight?.weight ?: 0f) }

    Box(
        modifier = modifier.fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = R.drawable.check,
            description = "Confirm button",
            color = lightGreen,
            onClick = {
                if (selectedWeight != 0f) {
                    val date = isValidDate(selectedDate)
                    if (date != null) {
                        viewModel.updateWeight(id = weightId, date = date.time, weight = selectedWeight)
                        navController.navigate(WeightHistoryRoutes.HISTORY)
                    }
                }
            },
            modifier = Modifier
                .align(Alignment.BottomEnd)
                .offset(x = adaptiveWidth(-32).dp, y = adaptiveWidth(-32).dp)
        )
        CustomFloatingButton(
            icon = R.drawable.back,
            description = "Back button",
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
            Heading(Strings.EDIT_WEIGHT)
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
                    imageId = R.drawable.calendar,
                )
                Spacer(modifier = Modifier.height(adaptiveWidth(40).dp))
                CustomFloatPicker(
                    selectedValue = selectedWeight,
                    label = Strings.WEIGHT_BODY,
                    imageId = R.drawable.scale_white,
                    onValueChange = { selectedWeight = it }
                )
                Spacer(modifier = Modifier.height(adaptiveWidth(40).dp))
                Box(
                    contentAlignment = Alignment.Center,
                    modifier = modifier
                        .fillMaxWidth()
                        .clip(RoundedCornerShape(adaptiveWidth(16).dp))
                        .background(color = lightRed)
                        .clickable(
                            onClick = {
                                viewModel.deleteWeight(id = weightId)
                                navController.navigate(WeightHistoryRoutes.HISTORY)
                            }
                        )
                ) {
                    CustomText(
                        text = Strings.DELETE_WEIGHT,
                        color = Color.White,
                        modifier = Modifier.padding(adaptiveWidth(16).dp)
                    )
                }
                Spacer(modifier = Modifier.height(adaptiveHeight(150).dp))
            }
        }
    }
}
