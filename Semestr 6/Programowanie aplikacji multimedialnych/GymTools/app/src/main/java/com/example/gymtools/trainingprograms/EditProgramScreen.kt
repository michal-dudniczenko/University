package com.example.gymtools.trainingprograms

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
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.gymtools.R
import com.example.gymtools.common.CustomFloatingButton
import com.example.gymtools.common.CustomStringPicker
import com.example.gymtools.common.CustomText
import com.example.gymtools.common.Heading
import com.example.gymtools.common.adaptiveHeight
import com.example.gymtools.common.adaptiveWidth
import com.example.gymtools.common.lightGreen
import com.example.gymtools.common.lightRed
import kotlinx.coroutines.launch

@Composable
fun EditProgramScreen(
    programId: Int,
    navController: NavHostController,
    viewModel: TrainingProgramsViewModel,
    modifier: Modifier = Modifier
) {

    val program = viewModel.getProgramById(id = programId)

    var selectedName by remember { mutableStateOf(program?.name ?: "") }

    val coroutineScope = rememberCoroutineScope()

    var isEditMode by viewModel.isEditMode

    Box(
        modifier = modifier.fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = R.drawable.check,
            description = "Confirm button",
            color = lightGreen,
            onClick = {
                if (selectedName.isNotEmpty()) {
                    coroutineScope.launch {
                        viewModel.updateProgram(id = programId, name = selectedName)
                        navController.navigate("Training days list/$programId")
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
            onClick = { navController.navigate("Training days list/$programId") },
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
            Heading("Edit program")
            Column(
                horizontalAlignment = Alignment.CenterHorizontally,
                verticalArrangement = Arrangement.Center,
                modifier = modifier
                    .fillMaxWidth(0.95f)
                    .fillMaxHeight()
            ) {
                CustomStringPicker(
                    selectedValue = selectedName,
                    label = "Program name",
                    onValueChange = { selectedName = it }
                )
                Spacer(modifier = Modifier.height(adaptiveWidth(40).dp))
                Box(
                    contentAlignment = Alignment.Center,
                    modifier = modifier
                        .fillMaxWidth()
                        .background(
                            color = lightRed,
                            shape = RoundedCornerShape(adaptiveWidth(16).dp)
                        )
                        .clickable(
                            onClick = {
                                coroutineScope.launch {
                                    isEditMode = false
                                    viewModel.deleteProgram(id = programId)
                                    navController.navigate("Programs list")
                                }
                            }
                        )
                ) {
                    CustomText(
                        text = "Delete program",
                        color = Color.White,
                        modifier = Modifier.padding(adaptiveWidth(16).dp)
                    )
                }
                Spacer(modifier = Modifier.height(adaptiveHeight(200).dp))
            }
        }
    }
}