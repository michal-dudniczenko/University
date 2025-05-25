package com.example.befit.trainingprograms

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
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.befit.R
import com.example.befit.common.CustomFloatingButton
import com.example.befit.common.CustomText
import com.example.befit.common.Heading
import com.example.befit.common.TrainingProgramsRoutes
import com.example.befit.common.adaptiveWidth
import com.example.befit.common.bigFontSize
import com.example.befit.common.bright
import com.example.befit.common.editColor
import com.example.befit.common.mediumGreen

@Composable
fun TrainingDaysListScreen(
    programId: Int,
    viewModel: TrainingProgramsViewModel,
    navController: NavHostController,
    modifier: Modifier = Modifier
) {
    var isEditMode by viewModel.isEditMode

    val programs by viewModel.programs
    val trainingDays by viewModel.trainingDays

    val program = programs.find { it.id == programId }
    val trainingDaysFromProgram = trainingDays.filter { it.programId == programId }

    Box(
        modifier = modifier
            .fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = R.drawable.back,
            description = "Back button",
            onClick = {
                navController.navigate(TrainingProgramsRoutes.PROGRAMS_LIST)
                isEditMode = false
            },
            modifier = Modifier
                .align(Alignment.BottomStart)
                .offset(x = adaptiveWidth(32).dp, y = adaptiveWidth(-32).dp)
        )
        if (trainingDaysFromProgram.isEmpty()) {
            CustomFloatingButton(
                icon = R.drawable.add,
                description = "Add button",
                onClick = { navController.navigate(TrainingProgramsRoutes.ADD_TRAINING_DAY(programId)) },
                modifier = Modifier
                    .align(Alignment.BottomEnd)
                    .offset(x = adaptiveWidth(-32).dp, y = adaptiveWidth(-32).dp)
            )
        } else {
            CustomFloatingButton(
                icon = if (isEditMode) R.drawable.edit_white else R.drawable.edit,
                color = if (isEditMode) editColor else bright,
                description = "Edit button",
                onClick = { isEditMode = !isEditMode },
                modifier = Modifier
                    .align(Alignment.BottomEnd)
                    .offset(x = adaptiveWidth(-32).dp, y = adaptiveWidth(-32).dp)
            )
        }
        if (program == null) {
            Box(
                modifier = Modifier.fillMaxSize()
            ) {
                CircularProgressIndicator(
                    color = bright,
                    modifier = Modifier.align(Alignment.Center)
                )
                return
            }
        }
        Column(
            horizontalAlignment = Alignment.CenterHorizontally,
            modifier = modifier
                .fillMaxWidth(0.8f)
                .fillMaxHeight(0.9f)
                .align(Alignment.Center)
        ) {
            Heading(program?.name ?: "")
            if (trainingDaysFromProgram.isEmpty()) {
                Box(
                    modifier = Modifier
                        .fillMaxSize()
                ) {
                    CustomText(
                        text = "Nothing here yet!",
                        fontSize = bigFontSize,
                        modifier = Modifier
                            .align(Alignment.Center)
                            .offset(y = adaptiveWidth(-75).dp)
                    )
                }
            } else {
                Column(
                    horizontalAlignment = Alignment.CenterHorizontally,
                    modifier = Modifier
                        .fillMaxWidth(0.95f)
                        .fillMaxHeight()
                        .verticalScroll(rememberScrollState())
                ) {
                    if (isEditMode) {
                        Box(
                            contentAlignment = Alignment.Center,
                            modifier = modifier
                                .fillMaxWidth()
                                .clip(RoundedCornerShape(adaptiveWidth(16).dp))
                                .background(color = mediumGreen)
                                .clickable {
                                    navController.navigate(TrainingProgramsRoutes.ADD_TRAINING_DAY(programId))
                                }
                        ) {
                            CustomText(
                                text = "Add training day",
                                modifier = Modifier.padding(adaptiveWidth(16).dp)
                            )
                        }
                        Spacer(modifier = Modifier.height(adaptiveWidth(32).dp))
                    }
                    for (trainingDay in trainingDaysFromProgram) {
                        TrainingDayRow(
                            trainingDayId = trainingDay.id,
                            trainingDayName = trainingDay.name,
                            viewModel = viewModel,
                            navController= navController
                        )
                    }
                }
            }
        }
    }
}