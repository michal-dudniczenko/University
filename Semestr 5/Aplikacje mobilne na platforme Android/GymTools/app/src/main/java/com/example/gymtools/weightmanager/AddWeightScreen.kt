package com.example.gymtools.weightmanager

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
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.gymtools.R
import com.example.gymtools.common.CustomFloatPicker
import com.example.gymtools.common.CustomFloatingButton
import com.example.gymtools.common.CustomStringPicker
import com.example.gymtools.common.Heading
import com.example.gymtools.common.adaptiveHeight
import com.example.gymtools.common.adaptiveWidth
import com.example.gymtools.common.formatDateFromLong
import com.example.gymtools.common.isValidDate
import com.example.gymtools.common.lightGreen
import com.example.gymtools.common.lightRed
import kotlinx.coroutines.launch

@Composable
fun AddWeightScreen(
    navController: NavHostController,
    viewModel: WeightManagerViewModel,
    modifier: Modifier = Modifier
) {
    var selectedDate by remember { mutableStateOf(formatDateFromLong(System.currentTimeMillis())) }
    var selectedWeight by remember { mutableFloatStateOf(0f) }

    val coroutineScope = rememberCoroutineScope()

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
                        coroutineScope.launch {
                            viewModel.addWeight(date = date.time, weight = selectedWeight)
                            navController.navigate("Weight history")
                        }
                    }
                }
            },
            modifier = Modifier
                .align(Alignment.BottomEnd)
                .offset(x = adaptiveWidth(-32).dp, y = adaptiveWidth(-32).dp)
        )
        CustomFloatingButton(
            icon = R.drawable.cancel,
            description = "Cancel button",
            color = lightRed,
            onClick = { navController.navigate("Weight history") },
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
            Heading("Add weight")
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
                    label = "Date",
                    imageId = R.drawable.calendar,
                )
                Spacer(modifier = Modifier.height(adaptiveHeight(40).dp))
                CustomFloatPicker(
                    label = "Weight",
                    imageId = R.drawable.scale_white,
                    onValueChange = { selectedWeight = it }
                )
                Spacer(modifier = Modifier.height(adaptiveHeight(200).dp))
            }
        }
    }
}